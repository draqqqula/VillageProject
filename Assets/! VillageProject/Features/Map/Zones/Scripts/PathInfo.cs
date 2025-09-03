using BezierSolution;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathInfo : MonoBehaviour
{
    [Serializable]
    public class PathWithDestination
    {
        public BezierSpline path;
        public GameObject destinationZone;
    }

    [field: SerializeField] private List<PathWithDestination> _destinations;
    private Dictionary<GameObject, BezierSpline> _destinationToPath;
    public IReadOnlyDictionary<GameObject, BezierSpline> DestinationToPath => _destinationToPath;
    private void Start()
    {
        _destinationToPath = _destinations.ToDictionary(it => it.destinationZone, it => it.path);
    }
}
