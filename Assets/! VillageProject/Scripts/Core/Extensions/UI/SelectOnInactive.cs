using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SelectOnInactive : MonoBehaviour
{

    [SerializeField] private Selectable _selectable;
    private void OnDisable()
    {
        if (_selectable.isActiveAndEnabled)
        {
            _selectable.Select();
        }
    }
}