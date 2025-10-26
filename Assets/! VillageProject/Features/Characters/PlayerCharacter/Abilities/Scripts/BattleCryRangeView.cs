using System.Collections;
using UnityEngine;

public class BattleCryRangeView : MonoBehaviour
{
    [SerializeField] private Transform _plane;
    [SerializeField] private float _rangeLifetime;

    [SerializeField] private Vector3 _scaleRatio = Vector3.one;
    
    [SerializeField] private AnimationCurve _сurveOppacityByTime; 
    [SerializeField] private AnimationCurve _сurveSizeByTime; 
    
    private Material _material;

    private Coroutine _coroutine;

    private void Awake()
    {
        var meshRenderer = GetComponent<MeshRenderer>();
        _material = new Material(meshRenderer.material);
        meshRenderer.material = _material;
        _plane.gameObject.SetActive(false);
    }
    
    public void ActivateView()
    {
        if (_coroutine != null) StopCoroutine(_coroutine);
        _plane.gameObject.SetActive(true);
        _coroutine = StartCoroutine(RangeRoutine());
    }

    private IEnumerator RangeRoutine()
    {
        var progress = 0f;
        while (progress < _rangeLifetime)
        {
            progress += Time.fixedDeltaTime;
            
            var size = _scaleRatio * _сurveSizeByTime.Evaluate(progress);
            var oppacity = _сurveOppacityByTime.Evaluate(progress);

            _plane.transform.localScale = size;
            _material.SetFloat("_Alpha", oppacity);
            yield return null;
        }
        
        var endSize = _scaleRatio * _сurveSizeByTime.Evaluate(progress);
        _plane.transform.localScale = endSize;
        var endOppacity = _сurveOppacityByTime.Evaluate(progress);
        _material.SetFloat("_Alpha", endOppacity);
        
        _plane.gameObject.SetActive(false);
        _material.SetFloat("_Alpha", 1);
        _coroutine = null;
    }
}
