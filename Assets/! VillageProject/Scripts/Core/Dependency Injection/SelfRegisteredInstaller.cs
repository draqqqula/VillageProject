using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SelfRegisteredInstaller : MonoInstaller
{
    [SerializeField] private List<GameObject> _toRegister;

    public override void InstallBindings()
    {
        foreach (var item in _toRegister)
        {
            var display = item.GetComponent<SelfRegistered>();
            if (display != null)
            {
                display.RegisterSelf(Container);
            }
        }
    }
}