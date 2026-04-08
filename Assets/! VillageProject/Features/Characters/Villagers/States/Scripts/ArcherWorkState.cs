using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ArcherWorkState : WorkVillagerState
{
    private VillagerTransformHandler _transformHandler;
    private NavmeshMovementAgent _navmeshAgent;
    
    private SkinReferencesResolver _skinReferencesResolver;
    private SearchForTarget _searchForTarget;
    private Collider _discoveryCollider;

    private Building _archerTower;
    private BuildingStorage _storage;
    
    private VillagerStateFactory _factory;
    private DefenderWorkState _patrulState;
    
    private bool _isInited = false;
    private bool _isOnTower = false;
    
    public ArcherWorkState(NavmeshMovementAgent navmeshAgent, SkinReferencesResolver skinReferencesResolver, Profession profession,
        BuildingStorage buildingStorage, SearchForTarget searchForTarget, Collider discoveryCollider, VillagerStateFactory factory)
    {
        _navmeshAgent = navmeshAgent;
        _storage = buildingStorage;
        _skinReferencesResolver = skinReferencesResolver;
        
        _searchForTarget = searchForTarget;
        _discoveryCollider = discoveryCollider;
        _factory = factory;
        
        _transformHandler = new VillagerTransformHandler(navmeshAgent);
        _isInited = true;
    }
    
    public override void EnterState()
    {
        if (!_isInited) return;
        
        _archerTower = _storage.Get(BuildingType.ArcherTower, BuildingData.State.Wait);

        if (_archerTower == null)
        {
            Debug.LogWarning($"{_archerTower} is not valid ArcherTower!");
            _patrulState = _factory.CreateDefenderWorkState();
            _patrulState.EnterState();
            return;
        }
     
        (_archerTower.Data as ArcherTowerData).DamageHandler.OnAnimInvoked += OnShootInvoked;
        Debug.Log("Found ArcherTower!");
        _archerTower.SetReady();
        Debug.Log("Start Movement!");
        _transformHandler.ActivateMovementWithRotation(_archerTower.Data.EnterPoint, callback: OnPointReached);
    }

    private void OnPointReached()
    {
        var archerPoint = (_archerTower.Data as ArcherTowerData).ArcherPoint;
        _navmeshAgent.enabled = false;
        _navmeshAgent.transform.position = archerPoint.position;
        
        _skinReferencesResolver.Animator.SetBool("Agressed", true);
        _skinReferencesResolver.Animator.SetBool("Work", true);
        _discoveryCollider.enabled = false;
        _isOnTower = true;
    }

    private void OnShootInvoked()
    {
        _transformHandler.ActivateRotation(_searchForTarget.MainTarget.transform.position);
        _skinReferencesResolver.Animator.SetTrigger("Attack");
    }

    public override async UniTask ExitState(CancellationToken token)
    {
        if (!_isInited) return;

        if (_patrulState != null)
        {
            await _patrulState.ExitState(token);
            _patrulState = null;
            return;
        }

        if (_isOnTower)
        {
            var point = _archerTower.Data.EnterPoint;
            _navmeshAgent.transform.position = point.position;
            _navmeshAgent.enabled = true;
            _discoveryCollider.enabled = true;
        }

        if (_archerTower != null)
        {
            (_archerTower.Data as ArcherTowerData).DamageHandler.OnAnimInvoked -= OnShootInvoked;
            _archerTower.SetWaiting();
            _archerTower = null;
        }
        
        _transformHandler.DeactivateMovement();
        
        _skinReferencesResolver.Animator.SetBool("Agressed", false);
        await _skinReferencesResolver.AnimatorHandler.TransitByBool("Work", false, token);
    }

    public override void Dispose()
    {
        if (!_isInited) return;
        
        if (_archerTower != null) (_archerTower.Data as ArcherTowerData).DamageHandler.OnAnimInvoked -= OnShootInvoked;
        _transformHandler.Dispose();
    }
}