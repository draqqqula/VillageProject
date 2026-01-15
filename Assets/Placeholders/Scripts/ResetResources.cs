using System;
using System.Collections.Generic;
using UnityEngine;

public class ResetResources : MonoBehaviour
{
    [Serializable]
    public class ResourceAndValue
    {
        public ResourceVariable Variable;
        public uint Value;
    }

    [SerializeField] private List<ResourceAndValue> _resources;

    void Start()
    {
        foreach (var resource in _resources)
        {
            if (resource.Variable.Amount > resource.Value)
            {
                resource.Variable.Decrement(resource.Variable.Amount - resource.Value);
            }
            else if (resource.Variable.Amount < resource.Value)
            {
                resource.Variable.Increment(resource.Value - resource.Variable.Amount);
            }
        }
    }
}
