using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SelectOnActive : MonoBehaviour
{
    [SerializeField] private Selectable _selectable;
    private void OnEnable()
    {
        if (_selectable.isActiveAndEnabled)
        {
            _selectable.Select();
        }
    }
}