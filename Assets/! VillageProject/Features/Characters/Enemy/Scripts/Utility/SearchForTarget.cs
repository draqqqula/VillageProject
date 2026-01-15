using UnityEngine;
using R3;
using System;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

public class SearchForTarget : MonoBehaviour
{
    class PriorityComparer : IComparer<Target>
    {
        public int Compare(Target x, Target y)
        {
            return x.Priority - y.Priority;
        }
    }

    [SerializeField] private Target _mainTarget;
    private SortedSet<Target> _targets = new (new PriorityComparer());

    public event Action OnMainTargetChangedEvent;
    public UnityEvent OnMainTargetChanged;
    public UnityEvent<Target, float> OnDistanceToTargetUpdated;
    public Target MainTarget
    {
        get
        {
            return _mainTarget;
        }

        set
        {
            if (_mainTarget != value)
            {
                _mainTarget = value;
                OnMainTargetChanged?.Invoke();
                OnMainTargetChangedEvent?.Invoke();
            }
        }
    }

    public void Detect(Target target)
    {
        Debug.Log($"Detected target {target.gameObject.name}");
        if (!target.isActiveAndEnabled)
        {
            return;
        }
        target.ForgetByAll += Forget;
        if (_targets.Add(target))
        {
            ResetMainTarget();
        }
    }
    public void Forget(Target target)
    {
        target.ForgetByAll -= Forget;
        if (_targets.Remove(target)
            && MainTarget == target)
        {
            ResetMainTarget();
        }
    }

    private void ResetMainTarget()
    {
        MainTarget = _targets.FirstOrDefault();
    }
}
