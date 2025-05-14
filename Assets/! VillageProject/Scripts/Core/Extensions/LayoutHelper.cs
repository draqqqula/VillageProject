using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class LayoutHelper : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    public void Rebuild()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
    }

    private void Reset()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        Rebuild();
    }
}