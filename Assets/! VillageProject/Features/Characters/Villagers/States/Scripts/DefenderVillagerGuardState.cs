using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class DefenderVillagerGuardState : GuardVillagerState, IUpdatableState
{
    private const float AttackDistance = 2;
    
    private SearchForTarget _searchForTarget;
    private NavmeshMovementAgent _navmeshAgent;
    private VillagerMovementHandler _movementHandler;
    private Vector3 _prevPos;
    
    private Animator _animator;
    
    private Coroutine _coroutine;
    
    public DefenderVillagerGuardState(NavmeshMovementAgent navmeshAgent, SearchForTarget searchForTarget, Animator animator)
    {
        _searchForTarget = searchForTarget;
        _animator = animator;
        
        _navmeshAgent = navmeshAgent;
        _movementHandler = new VillagerMovementHandler(navmeshAgent);
    }
    
    public override void EnterState()
    { 
        
    }

    private void ActivateMovement()
    {
        _prevPos = _searchForTarget.MainTarget.transform.position;
        _movementHandler.ActivateMovement(_prevPos);
    }
    
    public void Update()
    {
        if (_prevPos != _searchForTarget.MainTarget.transform.position) ActivateMovement();
        
        if (Vector3.Distance(_navmeshAgent.transform.position, _searchForTarget.MainTarget.transform.position) <= AttackDistance)
        {
            _animator.SetTrigger("Attack");
        }
    }

    public override async UniTask ExitState(CancellationToken token)
    {
        _movementHandler.DeactivateMovement();
        _animator.ResetTrigger("Attack");

        if (_coroutine != null)
        {
            _navmeshAgent.StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    public override void Dispose()
    {
        _movementHandler.Dispose();
    }
}