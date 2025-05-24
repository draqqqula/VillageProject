using System.Collections;
using UnityEngine;
using R3;
using System;

public class BellRinger : MonoBehaviour
{
    [SerializeField] private GateAlarm _alarm;
    [SerializeField] private BellSound _bell;
    [SerializeField] private float _smallAlarmInterval;
    [SerializeField] private float _bigAlarmInterval;
    [SerializeField] private float _smallAlarmAmplitude;
    [SerializeField] private float _bigAlarmAmplitude;
    [SerializeField] private Vector3 _direction;
    private IDisposable _subscription;
    private Coroutine _ringing;

    private float Interval => _alarm.Alarm.CurrentValue == GateAlarm.AlarmStatus.SmallAlarm ? _smallAlarmInterval : _bigAlarmInterval;
    private float Amplitude => _alarm.Alarm.CurrentValue == GateAlarm.AlarmStatus.SmallAlarm ? _smallAlarmAmplitude : _bigAlarmAmplitude;
    private int Force => _alarm.Alarm.CurrentValue == GateAlarm.AlarmStatus.SmallAlarm ? 0 : 1;

    private void Reset()
    {
        _alarm = GetComponent<GateAlarm>();
        _bell = GetComponent<BellSound>();
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
                _ringing = StartCoroutine(BellMove());
            }
        }
    }

    private IEnumerator RingInTime(int force, float duration)
    {
        yield return new WaitForSeconds(duration);
        _bell.Ring(force);
    }

    private IEnumerator BellMove()
    {
        float t = 0;
        while (true)
        {
            if (_alarm.Alarm.CurrentValue == GateAlarm.AlarmStatus.Quiet)
            {
                _ringing = null;
                yield break;
            }
            t = 0;
            var force = Force;
            var intervalDuration = Interval;
            var amplitude = Amplitude;
            StartCoroutine(RingInTime(force, intervalDuration * 0.25f));
            StartCoroutine(RingInTime(force, intervalDuration * 0.75f));
            while (t < intervalDuration)
            {
                var a = Mathf.Sin(2 * (t / intervalDuration) * Mathf.PI) * amplitude;
                transform.eulerAngles = _direction * a;
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }
        }
    }
}