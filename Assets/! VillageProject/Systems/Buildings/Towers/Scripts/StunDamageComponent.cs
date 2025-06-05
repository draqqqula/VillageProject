using System;
using System.Collections;
using Unity.Behavior;
using UnityEngine;

[Serializable]
public class StunDamageComponent : DamageComponentBase
{
    [SerializeField] private Speed _speed;
    [SerializeField] private GameObject _vfx;
    [SerializeField] private Transform _vfxRoot;

    public void Apply(float duration)
    {
        _speed.Value.Value = 0.5f;
        GameObject.Instantiate(_vfx, _vfxRoot);
    }
}