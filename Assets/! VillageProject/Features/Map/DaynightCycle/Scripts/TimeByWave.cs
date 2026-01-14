using R3;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class TimeByWave : MonoBehaviour
{
    [Inject] private WaveController _waveController;
    [SerializeField] private CycleFromTime _cycle;
    [SerializeField] private float _speed;
    [SerializeField] private float _t;
    [SerializeField] private int _day;

    void Start()
    {
        StartCoroutine(StartDelayed());
    }

    private IEnumerator StartDelayed()
    {
        yield return new WaitForEndOfFrame();
        _waveController.IsOnBreak.Subscribe(HandleBreak);
    }

    private void HandleBreak(bool value)
    {
        _day += 1;
        StopAllCoroutines();
        StartCoroutine(Cycle());
    }

    public IEnumerator Cycle()
    {
        while (Mathf.FloorToInt(_t) < _day)
        {
            _t = Mathf.Min(_t + _speed * Time.deltaTime, (float)_day);
            _cycle.SetTime(_t / 2);
            yield return new WaitForEndOfFrame();
        }
    }
}
