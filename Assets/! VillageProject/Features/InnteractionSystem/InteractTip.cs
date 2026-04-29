using TMPro;
using UnityEngine;

public class InteractTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Text;
    
    public void ShowTip(string interactAction, string key)
    {
        gameObject.SetActive(true);
        
        Text.text = $"Нажмите {key}, чтобы {interactAction}";
    }

    public void HideTip()
    {
        gameObject.SetActive(false);
    }
}