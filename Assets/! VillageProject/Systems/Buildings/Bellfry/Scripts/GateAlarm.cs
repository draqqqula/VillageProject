using R3;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GateAlarm : MonoBehaviour
{
    public enum AlarmStatus
    {
        Quiet,
        SmallAlarm,
        BigAlarm
    }

    private ReactiveProperty<AlarmStatus> _alarmStatus = new ReactiveProperty<AlarmStatus>(AlarmStatus.Quiet);
    [SerializeField] private List<GateState> Gates;
    [SerializeField] private float _smallAlarmDuration;
    private Coroutine _smallAlarmCancellation;

    public ReadOnlyReactiveProperty<AlarmStatus> Alarm => _alarmStatus;

    private void Awake()
    {
        Gates = GameObject.FindObjectsByType<GateState>(FindObjectsSortMode.None).ToList();
    }

    private void OnEnable()
    {
        foreach (var gate in Gates)
        {
            gate.GetComponentInChildren<Health>().OnDamageDealt += HandleKnocked;
            gate.GetComponentInChildren<DeathEvent>().FiredEvent += HandleGateBroken;
        }
    }

    private void OnDisable()
    {
        foreach (var gate in Gates)
        {
            gate.GetComponentInChildren<Health>().OnDamageDealt -= HandleKnocked;
            gate.GetComponentInChildren<DeathEvent>().FiredEvent -= HandleGateBroken;
        }
    }

    private void HandleKnocked(float damage)
    {
        if (_alarmStatus.Value == AlarmStatus.BigAlarm)
        {
            return;
        }
        _alarmStatus.Value = AlarmStatus.SmallAlarm;
        CancelSmallAlarmCancellation();
        _smallAlarmCancellation = StartCoroutine(CancelSmallAlarm());
    }

    private void HandleGateBroken()
    {
        _alarmStatus.Value = AlarmStatus.BigAlarm;
        CancelSmallAlarmCancellation();
    }

    private IEnumerator CancelSmallAlarm()
    {
        yield return new WaitForSeconds(_smallAlarmDuration);
        _alarmStatus.Value = AlarmStatus.Quiet;
    }

    private void CancelSmallAlarmCancellation()
    {
        if (_smallAlarmCancellation != null)
        {
            StopCoroutine(_smallAlarmCancellation);
            _smallAlarmCancellation = null;
        }
    }
}
