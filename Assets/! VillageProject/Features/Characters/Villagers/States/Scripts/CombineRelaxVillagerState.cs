using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public sealed class CombineRelaxVillagerState : RelaxVillagerState, IUpdatableState
{
    private const int ChangeActivityHours = 3;
    
    private GameObject _villagerObject;
    private GameTimer _gameTimer;

    private ChoosingStateConfig[] _statesConfigs;
    private RelaxVillagerState _curState;

    private int _lastTick = Int32.MaxValue;
    private bool _isChangingActivity;

    public CombineRelaxVillagerState(GameObject villagerObject, VillagerStateFactory factory, GameTimer gameTimer)
    {
        _villagerObject = villagerObject;
        _gameTimer = gameTimer;
        
        _statesConfigs = new ChoosingStateConfig[3];
        
        _statesConfigs[0] = new ChoosingStateConfig(2, factory.CreateRelaxInHomeVillagerState(), 0.3f);
        _statesConfigs[1] = new ChoosingStateConfig(4, factory.CreateTalkRelaxVillagerState(), 0.2f);
        _statesConfigs[2] = new ChoosingStateConfig(4, factory.CreateWalkInCenterState(), 0.5f);
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

        foreach (var state in _statesConfigs)
        {
            if (state.VillagerState == _curState) continue;
            sum += state.Chance;

            if (randomValue <= sum)
            {
                _curState = state.VillagerState;
                _lastTick = _gameTimer.CurrentTick + _gameTimer.ConvertHoursToTick(state.ChangeActivityHours);
                break;
            }
        }
        
        _curState.EnterState();
        _isChangingActivity = false;
    }

    private float GetTotalWeight()
    {
        var totalWeight = 0f;

        foreach (var state in _statesConfigs)
        {
            if (state.VillagerState == _curState) continue;
            totalWeight += state.Chance;
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

public class ChoosingStateConfig
{
    public int ChangeActivityHours { get; private set; }
    public RelaxVillagerState VillagerState {get; private set;}
    public float Chance {get; set;}

    public ChoosingStateConfig(int changeActivityHours, RelaxVillagerState villagerState, float chance)
    {
        ChangeActivityHours = changeActivityHours;
        VillagerState = villagerState;
        Chance = chance;
    }
}