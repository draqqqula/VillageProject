using System.Collections;
using UnityEngine;
using R3;
using System;

public class BellRinger : MonoBehaviour
{
    [SerializeField] private GateAlarm _alarm;
    [SerializeField] private Bell _bell;
    [SerializeField] private float _smallAlarmInterval;
    [SerializeField] private float _bigAlarmInterval;
    private IDisposable _subscription;
    private Coroutine _ringing;

    private void Reset()
    {
        _alarm = GetComponent<GateAlarm>();
        _bell = GetComponent<Bell>();
    }

    private void OnEnable()
    {
        _subscription = _alarm.Alarm
            .Subscribe(HandleAlarm)
            .AddTo(this);
    }

    private void OnDisable()
    {
        _subscription.Dispose();
        _subscription = null;
    }

    private void HandleAlarm(GateAlarm.AlarmStatus status)
    {
        if (status != GateAlarm.AlarmStatus.Quiet)
        {
            if (_ringing == null)
            {
                StartCoroutine(RingWithInterval());
            }
        }
    }

    private IEnumerator RingWithInterval()
    {
        while (true)
        {
            if (_alarm.Alarm.CurrentValue == GateAlarm.AlarmStatus.Quiet)
            {
                _ringing = null;
                yield break;
            }
            var isSmall = _alarm.Alarm.CurrentValue == GateAlarm.AlarmStatus.SmallAlarm;

            _bell.Ring(isSmall ? 1 : 2);
            yield return new WaitForSeconds(isSmall ? _smallAlarmInterval : _bigAlarmInterval);
        }
    }
}