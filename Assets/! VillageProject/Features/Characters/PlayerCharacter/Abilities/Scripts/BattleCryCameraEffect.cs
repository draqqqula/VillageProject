using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(CinemachineCamera))]
public class BattleCryCameraEffect : MonoBehaviour
{
    [SerializeField] private float _effectDuration;
    [SerializeField] private AnimationCurve _zoomCurve;
    private float _originalFOV;
    
    private CinemachineCamera _virtualCamera;
    private Coroutine _coroutine;
    private SignalBus _signalBus;

    [Inject]
    private void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
        _signalBus.Subscribe<BattleCrySignalInvoker.BattleCryStartedSignal>(PlayEffect);
        
        _virtualCamera = GetComponent<CinemachineCamera>();
        _originalFOV = _virtualCamera.Lens.FieldOfView;
    }
    
    [ContextMenu("PlayEffect")]
    public void PlayEffect()
    {
        if (_coroutine != null) return;
        _coroutine = StartCoroutine(ChangeFOVRoutine(_originalFOV, _effectDuration));
    }
    
    private IEnumerator ChangeFOVRoutine(float fromFOV, float duration)
    {
        float progress = 0f;
        
        while (progress < duration)
        {
            progress += Time.deltaTime;
            float t = progress / duration;
            
            _virtualCamera.Lens.FieldOfView = fromFOV * _zoomCurve.Evaluate(t);
            yield return null;
        }
        
        _virtualCamera.Lens.FieldOfView = fromFOV * _zoomCurve.Evaluate(duration);
        _coroutine = null;
    }

    private void OnDestroy()
    {
        _signalBus.Unsubscribe<BattleCrySignalInvoker.BattleCryStartedSignal>(PlayEffect);
    }
}