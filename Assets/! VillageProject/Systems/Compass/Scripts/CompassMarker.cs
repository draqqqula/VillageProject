using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class CompassMarker : MonoBehaviour
{
    public UnityEvent<float> OnOpacityChanged;

    private float _opacity;
    [field: SerializeField] public float Angle { get; set; }
    [field: SerializeField] public float ClearZone { get; set; }
    [field: SerializeField] public float FadeZone { get; set; }
    public float ViewPort => FadeZone + ClearZone;
    public void SetOpacity(float opacity)
    {
        if (_opacity != opacity)
        {
            OnOpacityChanged?.Invoke(opacity);
            _opacity = opacity;
        }
    }
}