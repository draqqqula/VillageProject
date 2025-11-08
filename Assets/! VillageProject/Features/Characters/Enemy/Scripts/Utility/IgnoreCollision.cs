using UnityEngine;

public class IgnoreCollision : MonoBehaviour
{
    [SerializeField] private Collider _colliderA;
    [SerializeField] private Collider _colliderB;

    public void Awake()
    {
        Physics.IgnoreCollision(_colliderA, _colliderB, true);
    }
}
