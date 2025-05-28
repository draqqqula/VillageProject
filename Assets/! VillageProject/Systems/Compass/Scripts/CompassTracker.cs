using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class CompassTracker : MonoBehaviour
{
    [Inject] private CompassMarkerManager _markerManager;
    [SerializeField] private GameObject _marker;
    private IDisposable _disposable;
    private CompassMarker _instance;
    public CompassMarker Marker => _instance;

    private void OnEnable()
    {
        _disposable?.Dispose();
        _disposable = _markerManager.Add(_marker, out _instance);
    }

    private void OnDisable()
    {
        _disposable?.Dispose();
    }
}