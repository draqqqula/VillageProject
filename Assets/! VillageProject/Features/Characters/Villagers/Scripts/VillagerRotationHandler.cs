using System;
using System.Collections;
using UnityEngine;

public class VillagerRotationHandler : IDisposable
{
    private const float DefaultRotationSpeed = 120;
    private NavmeshMovementAgent _navmeshAgent;

    private Coroutine _coroutine;

    public VillagerRotationHandler(NavmeshMovementAgent navmeshAgent)
    {
        _navmeshAgent = navmeshAgent;
    }
    
    public void ActivateRotation(Quaternion targetRotation, float duration = -1, Action callback = null)
    {
        if (duration < 0) duration = DefaultRotationSpeed;
        
        if (_coroutine != null) _navmeshAgent.StopCoroutine(_coroutine);
        _coroutine = _navmeshAgent.StartCoroutine(RotateRoutine(targetRotation, duration, callback));
    }

    private IEnumerator RotateRoutine(Quaternion targetRotation, float speed, Action callback = null)
    {
        float angle = Quaternion.Angle(_navmeshAgent.transform.rotation, targetRotation);
        float duration = angle / speed;
        
        Quaternion startRotation = _navmeshAgent.transform.rotation;
        
        float progress = 0f;

        while (progress < duration)
        {
            progress += Time.deltaTime;
            float t = progress / duration;

            _navmeshAgent.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }
        
        _navmeshAgent.transform.rotation = targetRotation;
        callback?.Invoke();
    }

    public void DeactivateRotation()
    {
        if (_coroutine != null)
        {
            _navmeshAgent.StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    public void Dispose()
    {
        DeactivateRotation();
    }
}