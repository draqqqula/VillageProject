using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Makes sure only one worker of type <see cref="T"/> is active at the time
/// </summary>
public abstract class WorkerSwitcher<T> : MonoBehaviour where T : WorkerBase
{

    private T _activeWorker;
    private bool _isNeutral;
    public UnityEvent OnNeutralEnter;
    public UnityEvent OnNeutralExit;
    [SerializeField] private List<T> _workers;

    public bool IsNeutral => _activeWorker == null;
    public T ActiveWorker
    {
        get
        {
            return _activeWorker;
        }
        protected set
        {
            if (!ReferenceEquals(_activeWorker, value))
            {
                if (_activeWorker == null)
                {
                    OnNeutralExit?.Invoke();
                }
                else
                {
                    if (value == null)
                    {
                        OnNeutralEnter?.Invoke();
                    }
                    else
                    {
                        HandleWorkCancelled(_activeWorker);
                    }
                }
                _activeWorker = value;
            }
        }
    }

    #region EditorLogic

    public void BindWorkers()
    {
        _workers = GetWorkers();
        SubscribeToEvents();
    }

    public abstract List<T> GetWorkers();

    private void Reset()
    {
        BindWorkers();
    }

    private void SubscribeToEvents()
    {
        foreach (var worker in _workers)
        {
            AddCall(worker, worker.WorkAssigned, new UnityAction<T>(HandleWorkAssigned));
            AddCall(worker, worker.WorkCompleted, new UnityAction<T>(HandleWorkCompleted));
            worker.enabled = false;
        }
    }

    private void AddCall(T worker, UnityEvent unityEvent, UnityAction<T> call)
    {
        //UnityEventTools.RemovePersistentListener(unityEvent, call);
        //UnityEventTools.AddObjectPersistentListener(unityEvent, call, worker);
    }

    #endregion

    public virtual void HandleWorkAssigned(T worker)
    {
        ActiveWorker = worker;
        worker.enabled = true;
    }

    public virtual void HandleWorkCompleted(T worker)
    {
        ActiveWorker = null;
        worker.enabled = false;
    }

    public virtual void HandleWorkCancelled(T worker)
    {
        worker.enabled = false;
    }
}