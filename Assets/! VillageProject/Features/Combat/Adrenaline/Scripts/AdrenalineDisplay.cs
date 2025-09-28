using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AdrenalineDisplay : MonoBehaviour
{
    [Inject] private Adrenaline _adrenaline;
    [SerializeField] private Image _display;

    private void Awake()
    {
        _adrenaline.Value.Subscribe(UpdateDisplay);
    }

    private void UpdateDisplay(float value)
    {
        _display.fillAmount = value / _adrenaline.MaxValue;
    }
}
