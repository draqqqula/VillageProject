using TMPro;
using UnityEngine;

public class DialogueView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textLabel;
    private Camera _camera;

    private Transform _villagerPoint;

    public void Init(Transform villagerPoint)
    {
        _camera = Camera.main;
        _villagerPoint = villagerPoint;
        
        HideView();
    }
    
    public void ShowView(string text)
    {
        gameObject.SetActive(true);
        _textLabel.text = text;
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