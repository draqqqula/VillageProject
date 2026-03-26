using UnityEngine;

public class ArcherWorkState : WorkVillagerState
{
    private VillagerMovementHandler _movementHandler;
    private NavmeshMovementAgent _navmeshAgent;

    private Building _archerTower;
    private BuildingStorage _storage;
    
    private bool _isInited = false;
    private bool _isOnTower = false;
    
    public ArcherWorkState(NavmeshMovementAgent navmeshAgent, Profession profession, BuildingStorage buildingStorage)
    {
        _navmeshAgent = navmeshAgent;
        _storage = buildingStorage;
        _movementHandler = new VillagerMovementHandler(navmeshAgent, navmeshAgent.transform.position);
        
        _isInited = true;
    }
    
    public override void EnterState()
    {
        if (!_isInited) return;
        
        _archerTower = _storage.Get(BuildingType.ArcherTower, BuildingData.State.Wait);
        if (_archerTower == null)
        {
            Debug.LogWarning($"{_archerTower} is not valid ArcherTower!");
            return;
        }
        
        _archerTower.SetReady();
        
        _movementHandler.SetTargetPos(_archerTower.Data.EnterPoint.position);
        _movementHandler.ActivateMovement(OnMovementEnded);
    }

    private void OnMovementEnded(WorkResult result)
    {
        if (result != WorkResult.Success) return;
        
        var archerPoint = (_archerTower.Data as ArcherTowerData).ArcherPoint;
        _navmeshAgent.enabled = false;
        _navmeshAgent.transform.position = archerPoint.position;
        _isOnTower = true;
    }

    public override void ExitState()
    {
        if (!_isInited) return;

        if (_isOnTower)
        {
            var point = _archerTower.Data.EnterPoint;
            _navmeshAgent.transform.position = point.position;
            _navmeshAgent.enabled = true;
        }
        
        _archerTower.SetWaiting();
        _movementHandler.DeactivateMovement();
    }

    public override void Dispose()
    {
        if (!_isInited) return;
        _movementHandler.Dispose();
    }
}