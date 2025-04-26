using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class RaycastOriginDamageComponent : DamageComponentBase
{
    [SerializeField] private Transform _origin;
    [field: SerializeField] public float MaxDistance { get; private set; }
    public Ray Ray => new Ray(_origin.position, _origin.forward);
}