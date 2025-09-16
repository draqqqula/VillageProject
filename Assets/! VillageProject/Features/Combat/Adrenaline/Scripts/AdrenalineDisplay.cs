using R3;
using UnityEngine;
using UnityEngine.UI;

public class AdrenalineDisplay : MonoBehaviour
{
    [SerializeField] private Adrenaline _stamina;
    [SerializeField] private Image _display;

    private void Awake()
    {
        _stamina.Value.Subscribe(UpdateDisplay);
    }

    private void UpdateDisplay(float value)
    {
        _display.fillAmount = value / _stamina.MaxValue;
    }
}
