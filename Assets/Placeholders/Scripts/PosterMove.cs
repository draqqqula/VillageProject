using UnityEngine;

public class PosterMove : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private AnimationCurve _curve;
    [SerializeField] private float _scale;
    [SerializeField] private float _offset;
    [SerializeField] private float _magnitude;

    public void Update()
    {
        _target.localPosition = new Vector3(0, _curve.Evaluate(_offset + Time.time * _scale) * _magnitude, 0);
    }
}
