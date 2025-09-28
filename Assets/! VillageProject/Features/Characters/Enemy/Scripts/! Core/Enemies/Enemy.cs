using UnityEngine;

public abstract class Enemy : MonoBehaviour, IMarkedByIndicator
{
    [field: SerializeField] public Transform OriginPoint { get; private set; }
}