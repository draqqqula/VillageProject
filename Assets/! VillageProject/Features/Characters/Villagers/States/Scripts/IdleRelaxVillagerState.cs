using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class IdleRelaxVillagerState : RelaxVillagerState
{
    private Transform _target;
    private NavmeshMovementAgent _navmeshAgent;
    private VillagerTransformHandler _transformHandler;

    private SkinReferencesResolver _skinReferencesResolver;
    private bool _isSitPoint;
    private bool _isRelaxing;

    public IdleRelaxVillagerState(NavmeshMovementAgent navmeshAgent, SkinReferencesResolver skinReferencesResolver,
        Transform relaxPoint, bool isSitPoint)
    {
        _target = relaxPoint;
        _navmeshAgent = navmeshAgent;
        _transformHandler = new VillagerTransformHandler(navmeshAgent);

        _skinReferencesResolver = skinReferencesResolver;
        _isSitPoint = isSitPoint;
    }

    public override void EnterState()
    {
        _transformHandler.ActivateMovementWithRotation(_target, callback: OnPointReached);
    }

    private void OnPointReached()
    {
        _isRelaxing = true;
        if (_isSitPoint) _skinReferencesResolver.AnimatorHandler.SetBool("SitRelax", true);
        else _skinReferencesResolver.AnimatorHandler.SetBool("StandRelax", true);
    }

    public override async UniTask ExitState(CancellationToken token)
    {
        _transformHandler.DeactivateMovement();
        if (!_isRelaxing) return;

        if (_isSitPoint)
        {
            await _skinReferencesResolver.AnimatorHandler.TransitByBool("SitRelax", false, _navmeshAgent.GetCancellationTokenOnDestroy());
        }
        else
        {
            await _skinReferencesResolver.AnimatorHandler.TransitByBool("StandRelax", false, _navmeshAgent.GetCancellationTokenOnDestroy());
        }
    }

    public override void Dispose()
    {
        _transformHandler.Dispose();
    }
}