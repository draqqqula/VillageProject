using System;
using R3;
using UnityEngine;
using Zenject;

public sealed class BuildPlanMapIcon : MapIcon
{
    [SerializeField] private Building _building;
    
    private BuildPlanDisplay _planDisplay;
    [Inject] private BuildingPlanner _buildingPlanner;

    private bool _isActivated;

    public void Activate()
    {
        if (_isActivated) return;
        
        InstantiateIcon();
        _planDisplay = _instance.GetComponent<BuildPlanDisplay>();
        if (_building.Data.Plan.Value == _buildingPlanner.GetCurrentPlan()) _planDisplay.UnlockPlan();
        else _planDisplay.LockPlan();
        
        _building.Data.Plan.Value.BuildingProgress.Subscribe(UpdateBuildProgress).AddTo(this);
        _buildingPlanner.OnCurrentPlanChanged += OnCurrentPlanChanged;
        _isActivated = true;
    }
    
    private void UpdateBuildProgress(float progress)
    {
        if (progress >= 1f) OnPlanCompleted();
        else
        {
            _planDisplay.UpdateBuildProgress(progress);
        }
    }

    private void OnPlanCompleted()
    {
        if (!_isActivated) return;
        
        _buildingPlanner.OnCurrentPlanChanged -= OnCurrentPlanChanged;
        DestroyIcon();
        _isActivated = false;
    }

    private void OnCurrentPlanChanged(BuildingPlan newPlan)
    {
        if (_building.Data.Plan.Value != newPlan) _planDisplay.LockPlan();
        else _planDisplay.UnlockPlan();
    }
    
    protected override void OnEnable() { }
    
    protected override void OnDisable() { }

    private void OnDestroy()
    {
        if (_isActivated) OnPlanCompleted();
    }
}