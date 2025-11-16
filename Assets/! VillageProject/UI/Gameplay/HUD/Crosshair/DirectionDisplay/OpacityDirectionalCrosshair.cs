using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OpacityDirectionalCrosshair : DirectionalCrosshair
{
    [SerializeField] private Image _image;
    [SerializeField, Range(0, 1)] private float _minValue;

    private float Window => 1 - _minValue;

    public override float Intensity 
    { 
        get
        {
            if (_image == null)
            {
                return 0f;
            }
            return (_image.color.a - _minValue) / Window;
        }
        set
        {
            if (_image == null)
            {
                return;
            }
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, _minValue + Window * value);
        }
    }
}