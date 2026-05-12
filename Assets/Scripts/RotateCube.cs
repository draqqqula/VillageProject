using UnityEngine;

public class RotateCube : MonoBehaviour
{
    [SerializeField] private Vector3 _delta;

    private void FixedUpdate()
    {
        transform.Rotate(_delta * Time.fixedDeltaTime);
    }
}
