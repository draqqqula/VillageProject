using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PressOnActive : MonoBehaviour
{
    [SerializeField] private Button _button;
    private void OnEnable()
    {
        _button.onClick.Invoke();
    }
}