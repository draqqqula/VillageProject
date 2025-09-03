using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class FixGatesDataLoader : DataDisplay<FixGatesMenuOption.FixGatesData>
{
    [SerializeField] private TMP_Text _status;
    [SerializeField] private TMP_Text _price;
    public override void Load(FixGatesMenuOption.FixGatesData data)
    {
        if (data.IsFullHealth)
        {
            _price.transform.parent.gameObject.SetActive(false);
            _status.gameObject.SetActive(true);
            _status.text = "Починка не требуется";
        }
        else
        {
            _status.gameObject.SetActive(false);
            _price.transform.parent.gameObject.SetActive(true);
            _price.text = data.PriceToFix.Required.First().Amount.ToString();
        }
    }
}