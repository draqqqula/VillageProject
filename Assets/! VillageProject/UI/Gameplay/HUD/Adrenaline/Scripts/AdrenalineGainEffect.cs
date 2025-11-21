using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AdrenalineGainEffect : MonoBehaviour
{
    private const float MinSize = 0.01f;

    [SerializeField] private float _smoothTime;
    [SerializeField] private float _sizeMultiplier = 1f;
    [SerializeField] private AnimationCurve _opacityBySize;
    [SerializeField] private Image _image;

    private float _size;
    private float _initialSize;
    private float _velocity;

    public void SetSize(float size)
    {
        _initialSize = size * _sizeMultiplier;
        _size = _initialSize;
    }

    private void Update()
    {
        transform.localScale = Vector3.one * _size;
        _size = Mathf.SmoothDamp(_size, 0, ref _velocity, _smoothTime);
        var t = _size / _initialSize;
        _image.color = _image.color.WithAlpha(_opacityBySize.Evaluate(t));
        if (_size < MinSize)
        {
            Destroy(gameObject);
        }
    }
}