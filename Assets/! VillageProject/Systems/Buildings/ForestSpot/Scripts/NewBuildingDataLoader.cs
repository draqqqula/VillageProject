using System.Collections;
using TMPro;
using UnityEngine;

public class NewBuildingDataLoader : DataDisplay<NewBuildingOption.NewBuildingData>
{
    public override void Load(NewBuildingOption.NewBuildingData data)
    {
        gameObject.GetComponentInChildren<TMP_Text>().text = data.Name;
    }
}