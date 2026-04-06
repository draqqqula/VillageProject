using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ArcherWorkState : WorkVillagerState
{
    private VillagerMovementHandler _movementHandler;
    private NavmeshMovementAgent _navmeshAgent;
    
    private SkinReferencesResolver _skinReferencesResolver;

    private Building _archerTower;
    private BuildingStorage _storage;
    
    private bool _isInited = false;
    private bool _isOnTower = false;
    
    public ArcherWorkState(NavmeshMovementAgent navmeshAgent, SkinReferencesResolver skinReferencesResolver, Profession profession,
        BuildingStorage buildingStorage)
    {
        _navmeshAgent = navmeshAgent;
        _storage = buildingStorage;
        _skinReferencesResolver = skinReferencesResolver;
        
        _movementHandler = new VillagerMovementHandler(navmeshAgent);
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
        
        Debug.Log("Found ArcherTower!");
        _archerTower.SetReady();
        Debug.Log("Start Movement!");
        _movementHandler.ActivateMovement(_archerTower.Data.EnterPoint.position, OnMovementEnded);
    }

    private void OnMovementEnded(WorkResult result)
    {
        if (result != WorkResult.Success) return;
        
        var archerPoint = (_archerTower.Data as ArcherTowerData).ArcherPoint;
        _navmeshAgent.enabled = false;
        _navmeshAgent.transform.position = archerPoint.position;
        _skinReferencesResolver.Animator.SetBool("Work", true);
        _isOnTower = true;
    }

    public override async UniTask ExitState(CancellationToken token)
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
        await _skinReferencesResolver.AnimatorHandler.TransitByBool("Work", false, token);
    }

    public override void Dispose()
    {
        if (!_isInited) return;
        _movementHandler.Dispose();
    }
}