using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;
using R3;
using UnityEngine.AI;

public sealed class CombineRelaxVillagerState : RelaxVillagerState, IUpdatableState
{
    private GameObject _villagerObject;
    private VillagerData _villagerData;
    
    private RelaxVillagerStateConfigs _configs;
    private RelaxVillagerState _curState;
    private SkipTimeController _skipTimeController;
    
    private GameTimer _gameTimer;
    
    private int _lastTick = Int32.MaxValue;
    private bool _isChangingActivity;
    private bool _isInited;

    public CombineRelaxVillagerState(GameObject villagerObject, VillagerData villagerData, RelaxVillagerStateConfigs configs,
        VillagerStateFactory factory, GameTimer gameTimer, SkipTimeController skipTimeController)
    {
        _skipTimeController = skipTimeController;
        
        _villagerObject = villagerObject;
        _villagerData = villagerData;
        _configs = configs;
        
        _gameTimer = gameTimer;

        foreach (var state in _configs.StatesConfigs)
        {
            if (state.StateName == "RelaxInHome") state.VillagerState = factory.CreateRelaxInHomeVillagerState();
            else if (state.StateName == "TalkRelax") state.VillagerState = factory.CreateTalkRelaxVillagerState();
            else if (state.StateName == "WalkInCenter") state.VillagerState = factory.CreateWalkInCenterState();
            else if (state.StateName == "VisitInterestingPoints") state.VillagerState = factory.CreateVisitInterestingPointsState();
        }
    }
    
    public override void EnterState()
    {
        _isInited = true;
        _gameTimer.OnTick += OnTick;
        _ = ChangeActivity(_villagerObject.GetCancellationTokenOnDestroy());
    }

    public override void EnterStateWithSkip()
    {
        EnterState();
    }
    
    private void OnTick(int currentTick)
    {
        if (!_isInited || _isChangingActivity) return;
        
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
        if (_curState != null)
        {
            if (!_skipTimeController.IsSkipping.CurrentValue)
            {
                await _curState.ExitState(token);
                await UniTask.WaitWhile(() => _villagerData.IsTalking, cancellationToken: token);
            }
            else
            {
                _curState.ExitStateWithSkip();
            }
            
            if (!_isInited) return;
        }

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
        
        Debug.Log($"{_villagerData.Key} start relaxing with state {_curState}!");
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
        _isInited = false;
        _gameTimer.OnTick -= OnTick;
        _lastTick = Int32.MaxValue;

        if (_curState != null)
        {
            if (!_skipTimeController.IsSkipping.CurrentValue) await _curState.ExitState(_villagerObject.GetCancellationTokenOnDestroy());
            else _curState.ExitStateWithSkip();
        }
    }

    public override void ExitStateWithSkip()
    {
        if (_isInited) _ = ExitState(_villagerObject.GetCancellationTokenOnDestroy());
    }

    public override void Dispose() { }
}