using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class BuyArrowsDataLoader : DataDisplay<BuyArrows.BuyArrowsData>
{
    [SerializeField] private ResourceVariable _resource;
    [SerializeField] private TMP_Text _Price;
    [SerializeField] private TMP_Text _Amount;
    [SerializeField] private TMP_Text _ResourceDisplay;

    public override void Load(BuyArrows.BuyArrowsData data)
    {
        _Amount.text = "Купить " + data.Amount.ToString() + " стрел";
        _Price.text = "Стоит " + data.Price.Required.First().Amount.ToString() + " золота";
    }

    private void Update()
    {
        _ResourceDisplay.text = "При себе " + _resource.Amount + "Стрел";
    }
}