using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Render Setup", menuName = "Render Setup", order = 100)]
public class RenderSetup : ScriptableObject
{
    [Serializable]
    public class Group
    {
        [SerializeField] private RenderingLayerMask layerMask;
        [SerializeField] private List<Layer> _layers;
    }

    [Serializable]
    public class Layer
    {
        [field: SerializeField] public float _distanceFrom { get; set; }
        [field: SerializeField] public float _distanceTo { get; set; }
    }

    [SerializeField] private List<Layer> _group;
}