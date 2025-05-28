using TMPro;
using UnityEngine;

public class ShowDescription : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    private GameObject _instance;

    private void OnEnable()
    {
        _instance = Instantiate(_prefab, transform.parent);
    }

    private void OnDisable()
    {
        if ( _prefab != null )
        {
            Destroy(_instance);
        }
    }

    public void SetText(string text)
    {
        _instance.GetComponentInChildren<TMP_Text>().SetText(text);
    }
}
