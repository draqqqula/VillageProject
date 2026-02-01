using R3;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementConfiguration : ScriptableObject
{
    [SerializeField] public float Sensitivity;
    [SerializeField] public bool Invert;
    [SerializeField] public InputActionReference LeftAttack;
    [SerializeField] public InputActionReference RightAttack;

    public ReactiveProperty<int> Updated = new ReactiveProperty<int>(0);
}
