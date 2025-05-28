using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class FixGatesDataLoader : DataDisplay<FixGatesMenuOption.FixGatesData>
{
    [SerializeField] private TMP_Text _status;
    public override void Load(FixGatesMenuOption.FixGatesData data)
    {
        if (data.IsFullHealth)
        {
            _status.text = "Починка не требуется";
        }
        else
        {
            _status.text = "Починить: " + data.PriceToFix.Required.First().Amount.ToString() + " золота";
        }
    }
}