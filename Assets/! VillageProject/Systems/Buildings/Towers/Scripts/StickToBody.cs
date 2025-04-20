using UnityEngine;

public class StickToBody : MonoBehaviour
{
    [SerializeField] private LayerMask _mask;
    [SerializeField] private float _raycastDistance;

    public void Attach()
    {
        if (Physics.Raycast(transform.position, transform.rotation.eulerAngles, out var hit, _raycastDistance, _mask))
        {
            transform.position = hit.point;
            transform.parent = hit.transform;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
