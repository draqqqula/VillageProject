using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfessionPopup : MonoBehaviour
{
    public ButtonWithTextInfo[] ChoosingButtons {get; private set;}
    [SerializeField] private Button _professionButtonPrefab;
    [SerializeField] private Transform _content;
    
    public void Init(ProfessionMenuInfo[] professionsInfo, Action<ButtonWithTextInfo> onButtonClick)
    {
        ChoosingButtons = new ButtonWithTextInfo[professionsInfo.Length];
        
        for(int i = 0; i < professionsInfo.Length; i++)
        {
            var button = Instantiate(_professionButtonPrefab, _content);
            var buttonInfo = new ButtonWithTextInfo(button, button.transform.GetChild(0).GetComponent<TextMeshProUGUI>());
            button.onClick.AddListener(() => onButtonClick(buttonInfo));

            buttonInfo.Text.text = professionsInfo[i].RussianName;
            ChoosingButtons[i] = buttonInfo;
        }
    }
    
    private void OnDestroy()
    {
        foreach (var button in ChoosingButtons)
        {
            button.Button.onClick.RemoveAllListeners();
        }
    }
}