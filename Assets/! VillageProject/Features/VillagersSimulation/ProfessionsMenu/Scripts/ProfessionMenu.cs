using System;
using System.Linq;
using System.Text;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfessionMenu : MonoBehaviour
{
    [SerializeField] private ButtonWithTextInfo _mainButton;
    [SerializeField] private ProfessionMenuInfo[] _professionsInfo;
    
    private Transform _contentParent;
    [SerializeField] private RectTransform _popupPos;
    
    [SerializeField] private ProfessionPopup _choosingPopupPrefab;
    private ProfessionPopup _professionPopup;
    
    public ReadOnlyReactiveProperty<ProfessionType> CurrentProfession => _currentProfession;
    private ReactiveProperty<ProfessionType> _currentProfession = new ReactiveProperty<ProfessionType>();

    public void Init(ProfessionType currentProfession, Transform contentParent)
    {
        var professionInfo = _professionsInfo.FirstOrDefault(p => p.ProfessionType == currentProfession);
        var anotherProfessionsInfo = _professionsInfo.Where(p => p.ProfessionType != currentProfession).ToArray();
        
        _mainButton.Button.onClick.AddListener(OnMainButtonClicked);
        _mainButton.Text.text = professionInfo.RussianName + " v";
        _currentProfession.Value = professionInfo.ProfessionType;
        
        _contentParent = contentParent;
        _professionPopup = Instantiate(_choosingPopupPrefab, _contentParent);
        _professionPopup.transform.SetAsLastSibling();
        
        _professionPopup.Init(anotherProfessionsInfo, OnChoosingButtonClicked);
        _professionPopup.gameObject.SetActive(false);
    }

    private void OnMainButtonClicked()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(_mainButton.Text.text);
        stringBuilder.Remove(stringBuilder.Length - 1, 1);
        
        if (_professionPopup.gameObject.activeInHierarchy)
        {
            _professionPopup.gameObject.SetActive(false);
            stringBuilder.Append("v");
        }
        else
        {
            _professionPopup.gameObject.SetActive(true);
            _professionPopup.transform.position = _popupPos.position;
            stringBuilder.Append("^");
        }
        
        _mainButton.Text.text = stringBuilder.ToString();
    }

    private void OnChoosingButtonClicked(ButtonWithTextInfo button)
    {
        var professionInfo = _professionsInfo.FirstOrDefault(p => p.RussianName == button.Text.text);
        _professionPopup.gameObject.SetActive(false);
        SetProfession(professionInfo);
    }
    
    public void SetProfession(ProfessionType professionType)
    {
        var professionInfo = _professionsInfo.FirstOrDefault(p => p.ProfessionType == professionType);
        SetProfession(professionInfo);
    }

    private void SetProfession(ProfessionMenuInfo professionInfo)
    {
        if (CurrentProfession.CurrentValue == professionInfo.ProfessionType) return;
        
        ProfessionType? prevProfession = CurrentProfession.CurrentValue;
        _currentProfession.Value = professionInfo.ProfessionType;
        
        _mainButton.Text.text = professionInfo.RussianName + " v";
        UpdateChoosingButtons(prevProfession, professionInfo);
    }

    private void UpdateChoosingButtons(ProfessionType? addedProfession, ProfessionMenuInfo removeProfessionInfo)
    {
        var addProfessionInfo = _professionsInfo.FirstOrDefault(p => p.ProfessionType == addedProfession);
        var button = _professionPopup.ChoosingButtons.FirstOrDefault(b => b.Text.text == removeProfessionInfo.RussianName);
        button.Text.text = addProfessionInfo.RussianName;
    }

    private void OnDestroy()
    {
        _mainButton.Button.onClick.RemoveAllListeners();
    }
}

[Serializable]
public class ProfessionMenuInfo
{
    [field: SerializeField] public ProfessionType ProfessionType { get; private set; }
    [field: SerializeField] public string RussianName { get; private set; }
}

[Serializable]
public class ButtonWithTextInfo
{
    [field: SerializeField] public Button Button { get; private set; }
    [field: SerializeField] public TextMeshProUGUI Text { get; private set; }

    public ButtonWithTextInfo(Button button, TextMeshProUGUI text)
    {
        Button = button;
        Text = text;
    }
}