using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class NewBuildingDataLoader : DataDisplay<NewBuildingOption.NewBuildingData>
{
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private TMP_Text _price;
    public override void Load(NewBuildingOption.NewBuildingData data)
    {
        _name.text = data.Name;
        _description.text = data.Description;
        _price.text = "Стоит " + data.Price.Required.First().Amount.ToString() + " золота";
    }
}