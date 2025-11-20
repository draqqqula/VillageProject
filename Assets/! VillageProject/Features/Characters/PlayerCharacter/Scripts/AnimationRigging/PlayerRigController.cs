using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Zenject;

[RequireComponent(typeof(RigBuilder))]
public class PlayerRigController : MonoBehaviour
{
    [SerializeField] private RigBuilder _rigBuilder;
    [Inject(Id = "DeactivateWeaponRig")] private IAnimationWindowListener _deactivateWeaponRigWindow;

    private void Awake()
    {
        _deactivateWeaponRigWindow.OnEnter += OnWeaponRigEntered;
        _deactivateWeaponRigWindow.OnExit += OnWeaponRigExited;
    }

    private void OnDestroy()
    {
        _deactivateWeaponRigWindow.OnEnter -= OnWeaponRigEntered;
        _deactivateWeaponRigWindow.OnExit -= OnWeaponRigExited;
    }

    private void OnWeaponRigEntered() => DeactivateRig(0);
    private void OnWeaponRigExited() => ActivateRig(0);

    public void DeactivateRig(int index)
    {
        var rig = _rigBuilder.layers[index];
        rig.active = false;
    }

    public void ActivateRig(int index)
    {
        var rig = _rigBuilder.layers[index];
        rig.active = true;
    }
}
