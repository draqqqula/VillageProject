using System;
using System.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Zenject;

public class BattleCryRangeView : MonoBehaviour
{
    [SerializeField] private float _rangeLifetime;
    [SerializeField] private Vector3 _scaleRatio = Vector3.one;
    
    [Header("Curves")]
    [SerializeField] private AnimationCurve _curveOppacityByTime = AnimationCurve.Linear(0, 1, 1, 0); 
    [SerializeField] private AnimationCurve _curveSizeByTime = AnimationCurve.Linear(0, 0, 1, 1); 
    [SerializeField] private bool _isAutoFixCurves;
    
    private MeshRenderer _meshRenderer;
    private Material _material;
    private Coroutine _coroutine;
    
    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
        _signalBus.Subscribe<BattleCrySignalInvoker.BattleCryStartedSignal>(ActivateView);
        
        _meshRenderer = GetComponent<MeshRenderer>();
        _material = new Material(_meshRenderer.material);
        _meshRenderer.material = _material;
        _meshRenderer.enabled = false;
    }

    private void OnDestroy()
    {
        _signalBus.Unsubscribe<BattleCrySignalInvoker.BattleCryStartedSignal>(ActivateView);
    }

    public void ActivateView()
    {
        if (_coroutine != null) StopCoroutine(_coroutine);
        _meshRenderer.enabled = true;
        _coroutine = StartCoroutine(RangeRoutine());
    }

    private IEnumerator RangeRoutine()
    {
        var progress = 0f;
        while (progress < _rangeLifetime)
        {
            progress += Time.deltaTime;
            
            var size = _scaleRatio * _curveSizeByTime.Evaluate(progress);
            var oppacity = _curveOppacityByTime.Evaluate(progress);

            transform.localScale = size;
            _material.SetFloat("_Alpha", oppacity);
            yield return null;
        }
        
        var endSize = _scaleRatio * _curveSizeByTime.Evaluate(progress);
        transform.localScale = endSize;
        var endOppacity = _curveOppacityByTime.Evaluate(progress);
        _material.SetFloat("_Alpha", endOppacity);
        
        _meshRenderer.enabled = false;
        _material.SetFloat("_Alpha", 1);
        _coroutine = null;
    }

    #if UNITY_EDITOR
    
    private void OnValidate()
    {
        if (!_isAutoFixCurves) return;
        UpdateCurves();
    }
    
    [ContextMenu("Update Curves")]
    public void UpdateCurves()
    {
        _curveSizeByTime.MoveKey(_curveSizeByTime.keys.Length - 1, new Keyframe(_rangeLifetime, Mathf.Max(transform.localScale.x, transform.localScale.z)));
        _curveOppacityByTime.MoveKey(_curveOppacityByTime.keys.Length - 1, new Keyframe(_rangeLifetime, _curveOppacityByTime.keys[_curveOppacityByTime.keys.Length - 1].value));
    }
    
    #endif
}