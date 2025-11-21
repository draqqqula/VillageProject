using R3;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class CursorDeltaHandler : IDisposable
{
    [Inject] private SwingConfiguration _configuration;
    [Inject] private CoroutineHandler _coroutineHandler;
    private ReactiveProperty<Vector2> _velocity;
    private Coroutine _coroutine;

    public CursorDeltaHandler()
    {
        _velocity = new ReactiveProperty<Vector2>(Vector2.zero);
    }

    public ReadOnlyReactiveProperty<Vector2> Velocity => _velocity;

    public void Initialize()
    {
        _coroutine = _coroutineHandler.StartCoroutine(EvaluateDelta());
    }

    public void Dispose()
    {
        _coroutineHandler.StopCoroutine(_coroutine);
    }

    private IEnumerator EvaluateDelta()
    {
        var delayedPosition = Vector2.zero;
        var currentPosition = Vector2.zero;
        var velocity = Vector2.zero;
        while (true)
        {
            var delta = _configuration.LookInput.action.ReadValue<Vector2>() * _configuration.CursorSensitivity;
            currentPosition = currentPosition + delta;
            delayedPosition = Vector2.SmoothDamp(delayedPosition, currentPosition, ref velocity, _configuration.CursorSmoothTime);
            _velocity.Value = currentPosition - delayedPosition;
            yield return new WaitForEndOfFrame();
        }
    }
}