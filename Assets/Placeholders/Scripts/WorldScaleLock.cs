using UnityEngine;

[ExecuteAlways]
public class WorldScaleLock : MonoBehaviour
{
    [Tooltip("Желаемый scale в мировых координатах")]
    [SerializeField] public Vector3 worldScale = Vector3.one;

    void LateUpdate()
    {
        ApplyWorldScale();
    }

    void ApplyWorldScale()
    {
        if (transform.parent == null)
        {
            transform.localScale = worldScale;
            return;
        }

        Vector3 parentWorldScale = transform.parent.lossyScale;

        // защита от деления на 0
        parentWorldScale.x = Mathf.Abs(parentWorldScale.x) < 0.0001f ? 0.0001f : parentWorldScale.x;
        parentWorldScale.y = Mathf.Abs(parentWorldScale.y) < 0.0001f ? 0.0001f : parentWorldScale.y;
        parentWorldScale.z = Mathf.Abs(parentWorldScale.z) < 0.0001f ? 0.0001f : parentWorldScale.z;

        transform.localScale = new Vector3(
            worldScale.x / parentWorldScale.x,
            worldScale.y / parentWorldScale.y,
            worldScale.z / parentWorldScale.z
        );
    }
}