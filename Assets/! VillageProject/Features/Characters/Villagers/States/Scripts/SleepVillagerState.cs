using UnityEngine;

public sealed class SleepVillagerState : VillagerState
{
    public override ActivityType ActivityType => ActivityType.Sleep;
    
    private NavmeshMovementAgent _navmeshAgent;
    private VillagerMovementHandler _movementHandler;

    public SleepVillagerState(NavmeshMovementAgent navmeshAgent, Transform homePoint)
    {
        _navmeshAgent = navmeshAgent;
        _movementHandler = new VillagerMovementHandler(navmeshAgent, homePoint.position);
    }
    
    public override void EnterState()
    {
        _movementHandler.ActivateMovement(OnMovementEnded);
    }

    private void OnMovementEnded(WorkResult result)
    {
        _navmeshAgent.gameObject.SetActive(false);
    }

    public override void ExitState()
    {
        _navmeshAgent.gameObject.SetActive(true);
        _movementHandler.DeactivateMovement();
    }

    public override void Dispose()
    {
        _movementHandler.Dispose();
    }
}