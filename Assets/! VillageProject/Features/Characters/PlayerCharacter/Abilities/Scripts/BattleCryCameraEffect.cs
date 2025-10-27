using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(CinemachineCamera))]
public class BattleCryCameraEffect : MonoBehaviour
{
    [Header("Durations")]
    [SerializeField] private float _zoomOutDuration = 1f;
    [SerializeField] private float _zoomInDuration = 0.3f;
    [SerializeField] private float _zoomToOriginalDuration = 0.5f;
    
    [Header("FOV")]
    [SerializeField] private float zoomOutFOV = 90f;
    [SerializeField] private float zoomInFOV = 40f;
    private float originalFOV;
    
    private CinemachineCamera _virtualCamera;
    private Coroutine _coroutine;
    private SignalBus _signalBus;

    [Inject]
    private void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
        _signalBus.Subscribe<BattleCrySignalInvoker.BattleCryStartedSignal>(PlayEffect);
        
        _virtualCamera = GetComponent<CinemachineCamera>();
        originalFOV = _virtualCamera.Lens.FieldOfView;
    }
    
    [ContextMenu("PlayEffect")]
    public void PlayEffect()
    {
        if (_coroutine != null) return;
        _coroutine = StartCoroutine(CryCameraRoutine());
    }
    
    private IEnumerator CryCameraRoutine()
    {
        yield return StartCoroutine(ChangeFOVRoutine(originalFOV, zoomOutFOV, _zoomOutDuration));
        yield return StartCoroutine(ChangeFOVRoutine(zoomOutFOV, zoomInFOV, _zoomInDuration));
        yield return StartCoroutine(ChangeFOVRoutine(zoomInFOV, originalFOV, _zoomToOriginalDuration));
        _coroutine = null;
    }
    
    private IEnumerator ChangeFOVRoutine(float fromFOV, float toFOV, float duration)
    {
        float progress = 0f;
        
        while (progress < duration)
        {
            progress += Time.deltaTime;
            float t = progress / duration;
            
            _virtualCamera.Lens.FieldOfView = Mathf.Lerp(fromFOV, toFOV, t);
            yield return null;
        }
        
        _virtualCamera.Lens.FieldOfView = toFOV;
    }

    private void OnDestroy()
    {
        _signalBus.Unsubscribe<BattleCrySignalInvoker.BattleCryStartedSignal>(PlayEffect);
    }
}