using R3;
using TMPro;
using UnityEngine;

public class SwitchBuildingPlanLoader : DataDisplay<SwitchBuildingPlanOption.SwitchBuildingData>
{
    [SerializeField] private TMP_Text _hoursDuration;
    [SerializeField] private TMP_Text _progress;
    
    private SwitchBuildingPlanOption.SwitchBuildingData _data;
    
    public override void Load(SwitchBuildingPlanOption.SwitchBuildingData data)
    {
        _data = data;
        _data.Progress.Subscribe(OnProgressUpdate).AddTo(this);
    }

    private void OnProgressUpdate(float progress)
    {
        var remainingHours = _data.BuildingHours - (int)Mathf.Ceil(_data.BuildingHours * progress);
        _hoursDuration.text = "Займет " + remainingHours + " часов";
        _progress.text = "Построено " + (int)Mathf.Round(progress * 100) + "%";
    }
}