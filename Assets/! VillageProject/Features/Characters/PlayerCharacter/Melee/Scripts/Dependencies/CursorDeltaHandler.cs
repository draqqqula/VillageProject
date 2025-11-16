using R3;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class CursorDeltaHandler : IDisposable
{
    [Inject(Id = "Raycast")] private Transform _origin;
    [Inject] private CoroutineHandler _coroutineHandler;
    private ReactiveProperty<Vector2> _delta;
    private ReactiveProperty<Vector2> _velocity;
    private Coroutine _coroutine;

    public CursorDeltaHandler()
    {
        _delta = new ReactiveProperty<Vector2>(Vector2.zero);
        _velocity = new ReactiveProperty<Vector2>(Vector2.zero);
    }

    public ReadOnlyReactiveProperty<Vector2> Delta => _delta;
    public ReadOnlyReactiveProperty<Vector2> Velocity => _velocity;

    public void Initialize()
    {
        _coroutine = _coroutineHandler.StartCoroutine(EvaluateDelta());
    }

    public void Dispose()
    {
        _coroutineHandler.StopCoroutine(_coroutine);
        //_signalBus.Fire(new FloatingCrosshairStateSignal(false));
        _delta.Dispose();
    }

    private IEnumerator EvaluateDelta()
    {
        var delayedPosition = _origin.eulerAngles;
        //_signalBus.Fire(new FloatingCrosshairStateSignal(true));
        Vector3 velocity = Vector3.zero;
        while (true)
        {
            var cachedAngles = _origin.eulerAngles;
            yield return new WaitForEndOfFrame();
            delayedPosition = MathExtensions.SmoothDampEulerProjected(delayedPosition, _origin.eulerAngles, ref velocity, 0.08f);
            _velocity.Value = delayedPosition.AxisAnglesBetweenEuler(_origin.eulerAngles);
            //_signalBus.Fire(new FloatingCrosshairPositionSignal(delayedPosition));
            _delta.Value = cachedAngles.AxisAnglesBetweenEuler(_origin.eulerAngles);
        }
    }
}