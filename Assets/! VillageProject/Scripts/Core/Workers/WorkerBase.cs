using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class WorkerBase : MonoBehaviour
{
    public UnityEvent WorkCompleted;
    public UnityEvent WorkAssigned;

    protected virtual void SignalWorkCompleted()
    {
        WorkCompleted?.Invoke();
    }

    protected virtual void SignalWorkAssigned()
    {
        WorkAssigned?.Invoke();
    }
}
