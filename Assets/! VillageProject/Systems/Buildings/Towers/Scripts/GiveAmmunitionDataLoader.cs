using System.Collections;
using TMPro;
using UnityEngine;

public class GiveAmmunitionDataLoader : DataDisplay<GiveAmmunitionMenuOption.GiveAmmunitionData>
{
    [SerializeField] private TMP_Text _buyText;
    [SerializeField] private TMP_Text _resourceText;
    [SerializeField] private TMP_Text _storageText;
    public override void Load(GiveAmmunitionMenuOption.GiveAmmunitionData data)
    {
        _buyText.text = $"Отдать {data.BuyAmount} {data.BuyResource.Name}";
        _resourceText.text = $"С собой {data.ResourceAmount} {data.BuyResource.Name}";
        if (data.StorageResource == null)
        {
            _storageText.text = "В башне пусто";
        }
        else
        {
            _storageText.text = $"В башне {data.StorageAmount}/{data.MaxAmount} {data.StorageResource.Name}";
        }
    }
}