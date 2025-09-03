using R3;
using R3.Triggers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassMarkerManager : MonoBehaviour
{
    [SerializeField] private List<CompassMarker> _markers;
    [SerializeField] private Transform _root;
    public IEnumerable<CompassMarker> All => _markers;
    public IDisposable Add(GameObject marker, out CompassMarker instance)
    {
        var go = Instantiate(marker, _root);
        instance = go.GetComponent<CompassMarker>();
        _markers.Add(instance);
        var cached = instance;
        return Disposable.Create(() => Remove(cached));
    }

    public void Remove(CompassMarker marker)
    {
        _markers.Remove(marker);
        Destroy(marker.gameObject);
    }
}