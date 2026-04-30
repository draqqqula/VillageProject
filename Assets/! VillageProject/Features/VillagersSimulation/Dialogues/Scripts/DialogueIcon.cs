using UnityEngine;

public class DialogueIcon : MonoBehaviour
{
    private Camera _camera;
    private Transform _villagerPoint;

    public void Init(Transform villagerPoint)
    {
        _camera = Camera.main;
        _villagerPoint = villagerPoint;
        
        HideView();
    }
    
    public void ShowView()
    {
        gameObject.SetActive(true);
    }

    public void HideView()
    {
        gameObject.SetActive(false);
    }

    
    private void LateUpdate()
    {
        transform.position = _villagerPoint.position;

        Vector3 direction = _camera.transform.position - transform.position;
        direction.y = 0;

        transform.rotation = Quaternion.LookRotation(-direction);
    }
}