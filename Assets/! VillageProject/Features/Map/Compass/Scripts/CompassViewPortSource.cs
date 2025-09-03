using R3.Triggers;
using System.Collections;
using UnityEngine;

public class CompassViewPortSource : MonoBehaviour
{
    [SerializeField] private CompassDisplay _compass;

    private void Update()
    {
        _compass.ViewPort = Camera.main.fieldOfView;
    }
}