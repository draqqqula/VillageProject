using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private void Start()
    {
        transform.LookAt(Camera.main.transform, Vector3.up);
    }

    void Update()
    {
        transform.LookAt(Camera.main.transform, Vector3.up);
    }
}
