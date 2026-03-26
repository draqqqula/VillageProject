using R3;
using UnityEngine;
using Zenject;

public class SwitchBuildingPlanOption : BuildingMenuItemBase<SwitchBuildingPlanOption.SwitchBuildingData>
{
    public class SwitchBuildingData
    {
        public int BuildingHours;
        public ReactiveProperty<float> Progress;

        public SwitchBuildingData(int buildingHours, ReactiveProperty<float> progress)
        {
            BuildingHours = buildingHours;
            Progress = progress;
        }
    }
    
    public override ReadOnlyReactiveProperty<SwitchBuildingData> Data => _data;
    public override ReadOnlyReactiveProperty<bool> Available => _available;
    
    private ReactiveProperty<SwitchBuildingData> _data = new ReactiveProperty<SwitchBuildingData>();
    private ReactiveProperty<bool> _available = new ReactiveProperty<bool>(false);
    private bool _isViewActivatedBeforeShowing = false;
    
    [field: SerializeField] public string Description { get; private set; }

    [Inject] private BuildingPlanner _buildingPlanner;
    private BuildingPlan _plan;
    
    public override void ShowPreview(GameObject ui)
    {
        if (_plan is NewBuildingPlan newBuildingPlan)
        {
            newBuildingPlan.PreviewObject.ActivateView();
        }
        
        var showDescription = ui.GetComponent<ShowDescription>();
        showDescription.enabled = true;
        showDescription.SetText(Description);
        showDescription.gameObject.GetComponentInParent<LayoutHelper>().Rebuild();
    }

    public override void HidePreview(GameObject ui)
    {
        if (_buildingPlanner.GetCurrentPlan() != _plan && _plan is NewBuildingPlan newBuildingPlan)
        {
            newBuildingPlan.PreviewObject.DeactivateView();
        }
        ui.GetComponent<ShowDescription>().enabled = false;
    }

    public override bool TryPerform()
    {
        if (_available.Value)
        {
            _buildingPlanner.ChangeCurrentPlan(_plan);
        }
        return false;
    }
    
    private void Awake()
    {
        GetComponent<Building>().Data.Plan.Skip(1).Subscribe(OnPlanSetted).AddTo(this);
        _buildingPlanner.OnCurrentPlanChanged += CheckAvailable;
    }

    private void OnPlanSetted(BuildingPlan plan)
    {
        _plan = plan;
        CheckAvailable(_buildingPlanner.GetCurrentPlan());

        if (_plan != null) _data.Value = new SwitchBuildingData(_plan.HoursDuration, _plan.BuildingProgress);
        else _data.Value = null;
    }

    private void CheckAvailable(BuildingPlan plan)
    {
        if (_plan == null || plan == _plan) _available.Value = false;
        else _available.Value = true;
    }

    private void OnDestroy()
    {
        _buildingPlanner.OnCurrentPlanChanged -= CheckAvailable;
    }
}