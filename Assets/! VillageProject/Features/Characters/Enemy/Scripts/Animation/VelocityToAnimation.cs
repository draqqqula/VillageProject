using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using R3;
using UnityEngine;

public class VelocityToAnimation : MonoBehaviour
{
    [SerializeField] private MovementSwitcher _movementSwitcher;
    [SerializeField] private Animator _movementAnimator;
    [SerializeField, Range(0, 100)] private float _maxVelocity = 1f;
    [SerializeField] private string _variableName = "Velocity";

    public void SetReferencesResolver(ReactiveProperty<SkinReferencesResolver> skinReferencesResolver)
    {
        skinReferencesResolver.Subscribe(OnSkinChanged).AddTo(this);
    }

    private void OnSkinChanged(SkinReferencesResolver skinReferencesResolver)
    {
        _movementAnimator = skinReferencesResolver.Animator;
    }

    private void Update()
    {
        if (_movementAnimator == null) return;
        
        float veclocity = 0;
        if (_movementSwitcher.ActiveWorker != null)
        {
            veclocity = _movementSwitcher.ActiveWorker.GetVelocityPerSecond();
        }
        _movementAnimator.SetFloat(_variableName, Mathf.Clamp01(veclocity / _maxVelocity));
    }
}
