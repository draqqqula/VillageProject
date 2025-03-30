using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class MovementAgentBase : MonoBehaviour
{
    public UnityEvent DestinationReached;
    public UnityEvent PathAssigned;
    public abstract float GetProgress();
    public abstract void HandleCancellation();
}

public abstract class MovementAgentBase<TPath> : MovementAgentBase
{
    protected abstract bool TryTakePath(TPath path);

    public bool TryTakePathWithCallback(TPath path)
    {
        if (TryTakePath(path))
        {
            PathAssigned?.Invoke();
            return true;
        }
        return false;
    }
}