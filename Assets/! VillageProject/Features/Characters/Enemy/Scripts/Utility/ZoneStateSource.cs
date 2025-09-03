using System.Linq;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public class ZoneStateSource : MonoBehaviour
{
    [SerializeField] private BehaviorGraphAgent _behaviorGraphAgent;
    [SerializeField] private ZoneTracker _zoneTracker;
    [SerializeField] private string _zoneName;
    private GameObject _targetZone;
    private BlackboardVariable<bool> _variable;

    private void Reset()
    {
        _behaviorGraphAgent = GetComponentInParent<BehaviorGraphAgent>();
        _zoneTracker = GetComponentInParent<ZoneTracker>();
    }

    private void Awake()
    {
        _targetZone = GameObject.FindGameObjectsWithTag("zone").FirstOrDefault(it => it.name == _zoneName);
        _behaviorGraphAgent.GetVariable("In" + _zoneName + "Zone", out _variable);
    }

    private void OnEnable()
    {
        HandleNewActiveZone(_zoneTracker.ActiveZone);
        _zoneTracker.OnNewActiveZone.AddListener(HandleNewActiveZone);
    }

    private void OnDisable()
    {
        _zoneTracker.OnNewActiveZone.RemoveListener(HandleNewActiveZone);
    }

    private void HandleNewActiveZone(GameObject activeZone)
    {
        if (activeZone == _targetZone)
        {
            _variable.Value = true;
        }
        else
        {
            _variable.Value = false;
        }
    }
}
