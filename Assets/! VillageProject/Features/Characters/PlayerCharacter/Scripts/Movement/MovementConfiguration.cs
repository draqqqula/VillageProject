using R3;
using UnityEngine;

public class MovementConfiguration : ScriptableObject
{
    [SerializeField] public float Sensitivity;
    [SerializeField] public bool Invert;

    public ReactiveProperty<int> Updated = new ReactiveProperty<int>(0);
}
