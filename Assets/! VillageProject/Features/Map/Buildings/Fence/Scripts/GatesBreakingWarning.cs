using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GatesBreakingWarning : MonoBehaviour
{
    private bool _active;

    public UnityEvent Started;
    public UnityEvent Stopped;

    public void Renew(float duration)
    {
        StopAllCoroutines();
        if (!_active)
        {
            _active = true;
            Started?.Invoke();
        }
        StartCoroutine(StopInTime(duration));
    }

    private IEnumerator StopInTime(float time)
    {
        yield return new WaitForSeconds(time);
        _active = false;
        Stopped?.Invoke();
    }
}
