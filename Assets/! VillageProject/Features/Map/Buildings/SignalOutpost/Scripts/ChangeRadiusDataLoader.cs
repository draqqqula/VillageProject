using System.Collections;
using TMPro;
using UnityEngine;

public class ChangeRadiusDataLoader : DataDisplay<ChangeRadiusMenuItem.SignalTowerRangeData>
{
    [SerializeField] private TMP_Text _display;
    public override void Load(ChangeRadiusMenuItem.SignalTowerRangeData data)
    {
        _display.text = data.Name.GetLocalizedString();
    }
}