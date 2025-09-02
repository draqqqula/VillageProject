using R3;
using UnityEngine;
using UnityEngine.UI;

public class TargetIcon : MonoBehaviour
{
    [SerializeField] private Image _image;
    
    public void UpdateHealth(float value)
    {
        _image.fillAmount = value;
    }
}
