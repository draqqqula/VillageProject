using BezierSolution;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Get Local Path", story: "Chose [Path] from [Tracker] current zone leading to [TargetZone]", category: "Action/Find", id: "e07402ee3bbae82c926addb0a5f18a2a")]
public partial class GetLocalPathAction : Action
{
    [SerializeReference] public BlackboardVariable<BezierSpline> Path;
    [SerializeReference] public BlackboardVariable<ZoneTracker> Tracker;
    [SerializeReference] public BlackboardVariable<GameObject> TargetZone;

    protected override Status OnStart()
    {
        var info = Tracker.Value.ActiveZone.GetComponent<PathInfo>();
        if (info != null && info.DestinationToPath.TryGetValue(TargetZone, out var path))
        {
            Path.Value = path;
            return Status.Success;
        }
        return Status.Failure;
    }
}

