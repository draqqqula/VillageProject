using System;
using R3;
using TMPro;
using UnityEngine;

public class SwitchBuildingPlanLoader : DataDisplay<SwitchBuildingPlanOption.SwitchBuildingData>
{
    [SerializeField] private TMP_Text _tittleText;
    [SerializeField] private TMP_Text _hoursDuration;
    [SerializeField] private TMP_Text _progress;
    
    private SwitchBuildingPlanOption.SwitchBuildingData _data;
    private IDisposable _subscription;
    
    public override void Load(SwitchBuildingPlanOption.SwitchBuildingData data)
    {
        _data = data;
        
        KillSubscription();
        if (_data == null) OnPlanEmpty();
        else _subscription = _data.Progress.Subscribe(OnProgressUpdate);
    }

    private void OnProgressUpdate(float progress)
    {
        _tittleText.text = "Построить проект";
        
        var remainingHours = _data.BuildingHours - (int)Mathf.Ceil(_data.BuildingHours * progress);
        _hoursDuration.text = "Займет " + remainingHours + " часов";
        _progress.text = "Построено " + (int)Mathf.Round(progress * 100) + "%";
    }

    private void OnPlanEmpty()
    {
        _tittleText.text = "Проект построен";
        _hoursDuration.text = "";
        _progress.text = "";
    }

    private void KillSubscription()
    {
        _subscription?.Dispose();
        _subscription = null;
    }

    private void OnDestroy()
    {
        KillSubscription();
    }
}