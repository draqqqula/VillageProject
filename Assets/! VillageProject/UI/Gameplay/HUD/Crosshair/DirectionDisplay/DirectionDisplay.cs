using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DirectionDisplay : MonoBehaviour
{
    [SerializeField] private DirectionalCrosshair[] Crosshairs;

    private DirectionalCrosshair _active;
    private AttackDirection _direction;
    public AttackDirection Direction
    {
        get
        {
            return _direction;
        }
        set
        {
            _direction = value;
            UpdateCrosshair(value);
        }
    }

    public void SetIntensity(float value)
    {
        if (_active == null)
        {
            return;
        }
        _active.Intensity = value;
    }

    private void UpdateCrosshair(AttackDirection direction)
    {
        var newActive = Crosshairs[(int)direction];
        if (_active != null && ReferenceEquals(_active, newActive))
        {
            return;
        }
        _active?.gameObject.SetActive(false);
        newActive.gameObject.SetActive(true);
        _active = newActive;
    }
}