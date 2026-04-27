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
    private IDisposable _progressSubscription;
    private IDisposable _hoursSubscription;
    
    public override void Load(SwitchBuildingPlanOption.SwitchBuildingData data)
    {
        _data = data;
        
        KillSubscription();
        if (_data == null) OnPlanEmpty();
        else
        {
            _progressSubscription = _data.Progress.Subscribe(OnProgressUpdate);
            _hoursSubscription = _data.BuildingHours.Subscribe(OnHoursUpdate);
        }
    }

    private void OnProgressUpdate(float progress)
    {
        OnViewChanged(progress, _data.BuildingHours.CurrentValue);
    }

    private void OnHoursUpdate(int hours)
    {
        OnViewChanged(_data.Progress.CurrentValue, hours);
    }

    private void OnViewChanged(float progress, int hours)
    {
        _tittleText.text = "Построить проект";
        
        var remainingHours = hours - (int)Mathf.Ceil(hours * progress);
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
        _progressSubscription?.Dispose();
        _progressSubscription = null;
        
        _hoursSubscription?.Dispose();
        _hoursSubscription = null;
    }

    private void OnDestroy()
    {
        KillSubscription();
    }
}