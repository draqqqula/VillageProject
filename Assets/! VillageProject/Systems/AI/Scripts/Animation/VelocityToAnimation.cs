using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class VelocityToAnimation : MonoBehaviour
{
    [SerializeField] private MovementSwitcher _movementSwitcher;
    [SerializeField] private Animator _movementAnimator;
    [SerializeField, Range(0, 100)] private float _maxVelocity = 1f;
    [SerializeField] private string _variableName = "Velocity";

    private void Update()
    {
        float veclocity = 0;
        if (_movementSwitcher.ActiveWorker != null)
        {
            veclocity = _movementSwitcher.ActiveWorker.GetVelocity();
        }
        _movementAnimator.SetFloat(_variableName, Mathf.Clamp01(veclocity / _maxVelocity));
    }
}
