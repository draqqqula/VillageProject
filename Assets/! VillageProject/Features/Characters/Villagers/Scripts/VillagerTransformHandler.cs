using System;
using UnityEngine;

public class VillagerTransformHandler : IDisposable
{
    private VillagerMovementHandler _movementHandler;
    private VillagerRotationHandler _rotationHandler;
    
    public VillagerTransformHandler(NavmeshMovementAgent navmeshAgent)
    {
        _movementHandler = new VillagerMovementHandler(navmeshAgent);
        _rotationHandler = new VillagerRotationHandler(navmeshAgent);
    }
    
    public void ActivateMovement(Vector3 targetPos, Action<WorkResult> callback = null)
    {
        _movementHandler.ActivateMovement(targetPos, callback);
    }
    
    public void ActivateMovementWithPosInCircle(Transform transformCenter, float maxRadius, float minRadius = 0, 
        Action<WorkResult> callback = null)
    {
        _movementHandler.ActivateMovementWithPosInCircle(transformCenter, maxRadius, minRadius, callback);
    }

    public void DeactivateMovement()
    {
        _movementHandler.DeactivateMovement();
    }
    
    public void ActivateMovementWithRotation(Transform target, float rotationDuration = -1, Action callback = null)
    {
        ActivateMovementWithRotation(target.position, target.rotation, rotationDuration, callback);
    }
    
    public void ActivateMovementWithRotation(Vector3 targetPos, Quaternion targetRotation, float rotationDuration = -1, Action callback = null)
    {
        _movementHandler.ActivateMovement(targetPos, (workResult) =>
        {
            if (workResult == WorkResult.Success) ActivateRotation(targetRotation, rotationDuration, callback);
        });
    }
    
    public void ActivateRotation(Quaternion targetRotation, float duration = -1, Action callback = null)
    {
        _rotationHandler.ActivateRotation(targetRotation, duration, callback);
    }
    
    public void DeactivateRotation()
    {
        _rotationHandler.DeactivateRotation();
    }

    public void Dispose()
    {
        _movementHandler.Dispose();
        _rotationHandler.Dispose();
    }
}