using System;
using System.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using Zenject;

[RequireComponent(typeof(MeshRenderer))]
public class RangeView : MonoBehaviour
{
    [SerializeField] private float _rangeLifetime;
    [SerializeField] private Vector3 _scaleRatio = Vector3.one;

    [Header("Curves")]
    [FormerlySerializedAs("_curveOppacityByTime")]
    [SerializeField] private AnimationCurve _curveOppacityByNormalizedTime = AnimationCurve.Linear(0, 1, 1, 0); 
    [FormerlySerializedAs("_curveSizeByTime")] 
    [SerializeField] private AnimationCurve _curveSizeByNormilizedTime = AnimationCurve.Linear(0, 0, 1, 1); 
    [SerializeField] private bool _isAutoFixLifetime;
    
    private MeshRenderer _meshRenderer;
    private Material _material;
    private Coroutine _coroutine;
    
    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _material = new Material(_meshRenderer.material);
        _meshRenderer.material = _material;
        _meshRenderer.enabled = false;
        
        ActivateView();
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
            
            var size = _scaleRatio * _curveSizeByNormilizedTime.Evaluate(progress / _rangeLifetime);
            var oppacity = _curveOppacityByNormalizedTime.Evaluate(progress / _rangeLifetime);

            transform.localScale = size;
            _material.SetFloat("_Alpha", oppacity);
            yield return null;
        }
        
        var endSize = _scaleRatio * _curveSizeByNormilizedTime.Evaluate(1);
        transform.localScale = endSize;
        var endOppacity = _curveOppacityByNormalizedTime.Evaluate(1);
        _material.SetFloat("_Alpha", endOppacity);
        
        _meshRenderer.enabled = false;
        _material.SetFloat("_Alpha", 1);
        _coroutine = null;
    }

    #if UNITY_EDITOR
    
    private void OnValidate()
    {
        if (!_isAutoFixLifetime) return;
        
        _rangeLifetime = Mathf.Max(_curveOppacityByNormalizedTime[_curveOppacityByNormalizedTime.length - 1].time,
            _curveSizeByNormilizedTime[_curveSizeByNormilizedTime.length - 1].time);
    }
    
    [ContextMenu("Update Curves")]
    public void UpdateCurves()
    {
        _curveSizeByNormilizedTime.MoveKey(_curveSizeByNormilizedTime.keys.Length - 1,
            new Keyframe(_curveSizeByNormilizedTime[_curveSizeByNormilizedTime.length - 1].time, Mathf.Max(transform.localScale.x, transform.localScale.z)));
    }
    #endif
}