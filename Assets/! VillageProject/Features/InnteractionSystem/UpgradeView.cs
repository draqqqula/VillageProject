using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UpgradeView : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _upgradeTitle;
    [SerializeField] protected TextMeshProUGUI _upgradeDescription;
    
    [SerializeField] protected GameObject _defaultPriceObj;
    [SerializeField] protected GameObject _pricesPanel;
    [SerializeField] protected TextMeshProUGUI _defaultPriceText;
    [SerializeField] protected TextMeshProUGUI _priceText;
    
    [SerializeField] private Image _buttonImage;
    [SerializeField] private Sprite _lockedSprite;
    
    [field:SerializeField] public Button UpgradeButton { get; private set; }
    
    public virtual void UpdateView(UpgradeConfig config)
    {
        if (config.Price != config.DefaultPrice)
        {
            _defaultPriceObj.gameObject.SetActive(true);
            _defaultPriceText.text = config.DefaultPrice.ToString();
        }
        else
        {
            _defaultPriceObj.gameObject.SetActive(false);
        }
        _priceText.text = config.Price.ToString();
    }

    public void SetEndView()
    {
        _buttonImage.sprite = _lockedSprite;
        _upgradeDescription.text = "Максимальный уровень";
        
        _pricesPanel.gameObject.SetActive(false);
    }
}