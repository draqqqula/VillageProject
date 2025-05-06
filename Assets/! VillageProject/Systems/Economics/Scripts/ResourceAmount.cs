using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class ResourceAmount
{
    public ResourceAmount(ResourceVariable resource, uint amount)
    {
        Resource = resource;
        Amount = amount;
    }

    [field: SerializeField] public ResourceVariable Resource { get; private set; }
    [field: SerializeField] public uint Amount { get; private set; }
}