using System.Threading;
using Cysharp.Threading.Tasks;

public sealed class VisitInterestingPointsState : RelaxVillagerState
{
    private NavmeshMovementAgent _navmeshAgent;
    private SkinReferencesResolver _skinReferencesResolver;
    private VillagerData _villagerData;
    
    private InterestingPointsService _interestingPointsService;
    private VillagerTransformHandler _transformHandler;
    
    private WalkInCenterVillagerState _walkInCenterState;
    private InterestingPoint _currentPoint;
    
    private SkipTimeController _skipTimeController;
    
    public VisitInterestingPointsState(VillagerData villagerData, NavmeshMovementAgent navmeshAgent, SkinReferencesResolver skinReferencesResolver,
        InterestingPointsService interestingPointsService, SkipTimeController skipTimeController, VillagerStateFactory factory)
    {
        _villagerData = villagerData;
        _navmeshAgent = navmeshAgent;
        _skinReferencesResolver = skinReferencesResolver;
        
        _interestingPointsService = interestingPointsService;
        _transformHandler = new VillagerTransformHandler(navmeshAgent);
        
        _walkInCenterState = factory.CreateWalkInCenterState();
        _skipTimeController = skipTimeController;
    }
    
    public override void EnterState()
    {
        var point = _interestingPointsService.GetRandomInterestingPoint();

        if (point == null)
        {
            _walkInCenterState.EnterState();
            return;
        }
        
        _currentPoint = point;
        _currentPoint.IsBusy = true;
        _villagerData.IsCanTalk = _currentPoint.IsCanTalkOnPoint;
        
        _transformHandler.ActivateMovementWithRotation(point.Point, callback: OnPointReached);
    }

    public override void EnterStateWithSkip()
    {
        EnterState();
    }

    private void OnPointReached()
    {
        _skinReferencesResolver.AnimatorHandler.SetBool(_currentPoint.AnimationTrigger, true);
    }

    public override async UniTask ExitState(CancellationToken token)
    {
        if (_currentPoint == null)
        {
            if (!_skipTimeController.IsSkipping.CurrentValue)
                await _walkInCenterState.ExitState(token);
            else
                _walkInCenterState.ExitStateWithSkip();
            return;
        }
        
        if (_currentPoint.IsBusy)
        {
            if (!_skipTimeController.IsSkipping.CurrentValue)
                await _skinReferencesResolver.AnimatorHandler.TransitByBool(_currentPoint.AnimationTrigger, false, token);
            else
                _skinReferencesResolver.AnimatorHandler.SetBool(_currentPoint.AnimationTrigger, false);
            
            _currentPoint.IsBusy = false;
            _currentPoint = null;
            _villagerData.IsCanTalk = true;
        }
        
        _transformHandler.DeactivateMovement();
    }

    public override void ExitStateWithSkip()
    {
        _ = ExitState(_navmeshAgent.GetCancellationTokenOnDestroy());
    }

    public override void Dispose()
    {
        _transformHandler.Dispose();
        _walkInCenterState.Dispose();
        _walkInCenterState = null;
        _currentPoint = null;
    }
}