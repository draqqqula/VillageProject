using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class UpgradeMenu : MonoBehaviour
{
    [SerializeField] private UpgradeWay[] _upgrades;
    
    [SerializeField] private string _inputActionMapName;
    [SerializeField] private InputActionAsset _asset;

    [SerializeField] private AnimationCurve _priceForLoyaltyCurve;
    [Inject] DiContainer _container;

    private void Awake()
    {
        foreach (var way in _upgrades)
        {
            foreach (var upgrade in way.Upgrades)
            {
                _container.Inject(upgrade.Upgrade);
            }
            
            way.View.UpgradeButton.onClick.AddListener(() => OnUpgradeChoosen(way));
        }
    }

    public void Activate(Villager villager)
    {
        gameObject.SetActive(true);
        var multiplier = _priceForLoyaltyCurve.Evaluate(villager.VillagerData.Loyalty.Property.CurrentValue);

        foreach (var way in _upgrades)
        {
            if (way.CurrentLevel >= way.Upgrades.Length) continue;
            
            foreach (var upgrade in way.Upgrades)
            {
                upgrade.Price = (int)Mathf.Ceil(upgrade.DefaultPrice * multiplier);
            }
            way.View.UpdateView(way.Upgrades[way.CurrentLevel]);
        }
    }

    private void OnUpgradeChoosen(UpgradeWay way)
    {
        var config = way.Upgrades[way.CurrentLevel];

        if (config.PriceRef.Value.TryPay())
        {
            config.Upgrade.Apply();
            way.CurrentLevel++;
        
            if (way.CurrentLevel < way.Upgrades.Length)
            {
                var nextUpgrade = way.Upgrades[way.CurrentLevel];
                way.View.UpdateView(nextUpgrade);
            }
            else way.View.SetEndView();
        }
    }

    private void OnEnable()
    {
        Time.timeScale = 0f;
        _asset.FindActionMap(_inputActionMapName).Disable();
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
        _asset.FindActionMap(_inputActionMapName).Enable();
    }
}

[Serializable]
public class UpgradeWay
{
    [field:SerializeField] public UpgradeConfig[] Upgrades {get; set;}
    [field:SerializeField] public UpgradeView View {get; set;}
    
    private int _currentLevel = 0;
    public int CurrentLevel { get => _currentLevel; set => _currentLevel = value; }
}

[Serializable]
public class UpgradeConfig
{
    [field: SerializeField] public int Level { get; private set; }
    [field: SerializeField] public PriceReference PriceRef { get; private set; }

    private int _price = -1;

    public int Price
    {
        get
        {
            if (_price == -1) _price = (int)PriceRef.Value.Required.ToList()[0].Amount;
            return _price;
        }
        set => _price = value;
    }
   
    private int _defaultPrice = -1;
    public int DefaultPrice 
    {
        get
        {
            if (_defaultPrice == -1) _defaultPrice = Price;
            return _defaultPrice;
        }
    }

    [SerializeReference, SubclassSelector] private Upgrade _upgrade;
    public Upgrade Upgrade => _upgrade;
}