using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class MovementSwitcher : WorkerSwitcher<MovementWorkerBase>
{
    public override List<MovementWorkerBase> GetWorkers()
    {
        var list = new List<MovementWorkerBase>();
        GetComponents(list);
        return list;
    }

    public override void HandleWorkCancelled(MovementWorkerBase agent)
    {
        base.HandleWorkCancelled(agent);
        agent.HandleCancellation();
    }
}
