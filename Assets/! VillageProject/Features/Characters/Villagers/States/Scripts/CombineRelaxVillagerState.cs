using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;
using R3;

public sealed class CombineRelaxVillagerState : RelaxVillagerState, IUpdatableState
{
    private GameObject _villagerObject;
    private VillagerData _villagerData;
    
    private RelaxVillagerStateConfigs _configs;
    private RelaxVillagerState _curState;
    
    private GameTimer _gameTimer;
    
    private int _lastTick = Int32.MaxValue;
    private bool _isChangingActivity;

    public CombineRelaxVillagerState(GameObject villagerObject, VillagerData villagerData, RelaxVillagerStateConfigs configs,
        VillagerStateFactory factory, GameTimer gameTimer)
    {
        _villagerObject = villagerObject;
        _villagerData = villagerData;
        _configs = configs;
        
        _gameTimer = gameTimer;

        foreach (var state in _configs.StatesConfigs)
        {
            if (state.StateName == "RelaxInHome") state.VillagerState = factory.CreateRelaxInHomeVillagerState();
            else if (state.StateName == "TalkRelax") state.VillagerState = factory.CreateTalkRelaxVillagerState();
            else if (state.StateName == "WalkInCenter") state.VillagerState = factory.CreateWalkInCenterState();
        }
    }
    
    public override void EnterState()
    {
        _gameTimer.OnTick += OnTick;
        _ = ChangeActivity(_villagerObject.GetCancellationTokenOnDestroy());
    }
    
    private void OnTick(int currentTick)
    {
        if (_isChangingActivity) return;
        
        if (currentTick >= _lastTick)
        {
            _ = ChangeActivity(_villagerObject.GetCancellationTokenOnDestroy());
        }
    }
    
    public void Update()
    {
        if (_curState is IUpdatableState updatableState) updatableState.Update();
    }

    private async UniTask ChangeActivity(CancellationToken token)
    {
        _isChangingActivity = true;
        if (_curState != null) await _curState.ExitState(token);

        var totalWeight = GetTotalWeight();
        var randomValue = Random.Range(0f, totalWeight);
        float sum = 0;

        foreach (var state in _configs.StatesConfigs)
        {
            if (state.VillagerState == _curState) continue;
            
            var weight = state.Weight * state.WeightByLoyalty.Evaluate(_villagerData.Loyalty.Property.CurrentValue);
            sum += weight;

            if (randomValue <= sum)
            {
                var activityHours = (int)Mathf.Ceil(state.ActivityHours * state.ActivityHoursByLoyalty.Evaluate(_villagerData.Loyalty.Property.CurrentValue));
                _curState = state.VillagerState;
                _lastTick = _gameTimer.CurrentTick + _gameTimer.ConvertHoursToTick(activityHours);
                break;
            }
        }
        
        _curState.EnterState();
        _isChangingActivity = false;
    }

    private float GetTotalWeight()
    {
        var totalWeight = 0f;

        foreach (var state in _configs.StatesConfigs)
        {
            if (state.VillagerState == _curState) continue;
            var weight = state.Weight * state.WeightByLoyalty.Evaluate(_villagerData.Loyalty.Property.CurrentValue);
            totalWeight += weight;
        }
        
        return totalWeight;
    }

    public override async UniTask ExitState(CancellationToken token)
    {
        _gameTimer.OnTick -= OnTick;
        _lastTick = Int32.MaxValue;

        if (_curState != null)
        {
            await _curState.ExitState(_villagerObject.GetCancellationTokenOnDestroy());
        }
    }

    public override void Dispose() { }
}

