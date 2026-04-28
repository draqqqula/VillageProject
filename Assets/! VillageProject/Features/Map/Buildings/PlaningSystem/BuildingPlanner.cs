using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class BuildingPlanner : MonoBehaviour
{
    private const int NewBuildingPlansLength = 3;
    
    [SerializeField] private List<NewBuildingPlan> _allBuildingPlans;
    private List<BuildingPlan> _completedBuildingPlans = new List<BuildingPlan>();
    public List<BuildingPlan> PriorityBuildingPlans { get; private set; }
    
    private BuildingStorage _storage;
    private DiContainer _container;

    private bool _isPlanPriorityForPlayer = false;
    
    public event Action<BuildingPlan> OnCurrentPlanChanged;

    [Inject]
    private void Construct(DiContainer container, BuildingStorage storage)
    {
        _container = container;
        _storage = storage;
        _storage.OnBuildingBroken += AddBrokenBuildingToPlan;
        
        PriorityBuildingPlans = new List<BuildingPlan>();
    }

    public void Init()
    {
        GeneratePriorityPlans(NewBuildingPlansLength);
    }
    
    public BuildingPlan GetCurrentPlan()
    {
        if (PriorityBuildingPlans.Count == 0) GeneratePriorityPlans(NewBuildingPlansLength);
        if (PriorityBuildingPlans.Count == 0) return null;
        
        return PriorityBuildingPlans[0];
    }

    public void ChangeCurrentPlan(BuildingPlan plan)
    {
        TryHidePreview();
        PriorityBuildingPlans.Remove(plan);
        PriorityBuildingPlans.Insert(0, plan);
        TryShowPreview();

        _isPlanPriorityForPlayer = PriorityBuildingPlans.Any(p => p is RepairingPlan);
        OnCurrentPlanChanged?.Invoke(plan);
    }

    private void AddBrokenBuildingToPlan(Building building)
    {
        if (PriorityBuildingPlans.Any(p => p is RepairingPlan repairingPlan && repairingPlan.BrokenBuilding == building))
        {
            return;
        }

        var newPlan = new RepairingPlan(building.Data.RepairingHours, building);
        building.Data.Plan.Value = newPlan;
            
        TryHidePreview();
        var insertIndex = GetInsertPlanIndex(newPlan);
        PriorityBuildingPlans.Insert(insertIndex, newPlan);
        
        var mapIcon = newPlan.BrokenBuilding.GetComponent<BuildPlanMapIcon>();
        mapIcon.Activate();
        
        if (insertIndex == 0) OnCurrentPlanChanged?.Invoke(newPlan);
        TryShowPreview();
    }
    
    public bool TryCompleteCurrentPlan()
    {
        if (PriorityBuildingPlans.Count == 0)
        {
            Debug.LogError("Can't complete current plan! List is empty!");
            return false;
        }

        if (GetCurrentPlan().BuildingProgress.Value < 1)
        {
            Debug.Log("Plan is not completed!");
            return false;
        }
        
        CompleteCurrentPlan();
        return true;
    }
    
    private void CompleteCurrentPlan()
    {
        TryHidePreview();
        
        var plan = PriorityBuildingPlans[0];
        PriorityBuildingPlans.RemoveAt(0);
        
        if (plan is NewBuildingPlan newBuildingPlan) Perform(newBuildingPlan);
        else if (plan is RepairingPlan repairingPlan) Perform(repairingPlan);
        
        _completedBuildingPlans.Add(plan);
        
        GeneratePriorityPlans(NewBuildingPlansLength, false);
        
        _isPlanPriorityForPlayer = false;
        OnCurrentPlanChanged?.Invoke(GetCurrentPlan());
    }
    
    private void Perform(NewBuildingPlan plan)
    {
        plan.PreviewObject.Data.Plan.Value = null;
        var slot = plan.BuildingAnchor.GetComponentInChildren<SingleInstance>();
        
        var building = _container.InstantiatePrefabForComponent<Building>(plan.BuildingPrefab, slot.transform);
        building.CompleteBuilding();
        slot.Substitute(building.gameObject);
        _storage.Add(building.GetComponent<Building>());
    }

    private void Perform(RepairingPlan plan)
    {
        plan.BrokenBuilding.Data.Plan.Value = null;
        plan.BrokenBuilding.Health.Amount = plan.BrokenBuilding.Health.MaxHealth;
        if (plan.BrokenBuilding.TryGetComponent(out GateState gateState)) gateState.Fix();
    }
    
    private void GeneratePriorityPlans(int maxCounts, bool isNewPlans = true)
    {
        if (_allBuildingPlans.Count <= 0)
        {
            Debug.LogWarning("Can't generate plan! All plans were completed!");
            return;
        }
        if (isNewPlans) PriorityBuildingPlans.Clear();
        
        var remainingBuildingPlans = _allBuildingPlans.Except(_completedBuildingPlans).ToArray();
        if (remainingBuildingPlans.Length < maxCounts) maxCounts = remainingBuildingPlans.Length;
        
        while (PriorityBuildingPlans.Count < maxCounts)
        {
            if (TryGeneratePriorityPlan(out var plan, remainingBuildingPlans))
            {
                var insertIndex = GetInsertPlanIndex(plan);
                PriorityBuildingPlans.Insert(insertIndex, plan);
                if (plan is NewBuildingPlan newBuildingPlan) InstantiatePreview(newBuildingPlan);
            }
        }
        TryShowPreview();
    }

    private bool TryGeneratePriorityPlan(out BuildingPlan plan, BuildingPlan[] remainingBuildingPlans)
    {
        plan = null;
        if (_allBuildingPlans.Count <= 0)
        {
            Debug.LogWarning("Can't generate plan! All plans were completed!");
            return false;
        }
        
        var randomPlan = remainingBuildingPlans[Random.Range(0, remainingBuildingPlans.Length)];
        if (!PriorityBuildingPlans.Contains(randomPlan))
        {
            plan = randomPlan;
            return true;
        }
        return false;
    }
    
    private int GetInsertPlanIndex(BuildingPlan newPlan)
    {
        int insertIndex = 0;

        var lastRepairIndex = PriorityBuildingPlans.FindLastIndex(p => p is RepairingPlan);

        if (newPlan is RepairingPlan)
        {
            insertIndex = lastRepairIndex != -1 ? lastRepairIndex + 1 : (_isPlanPriorityForPlayer ? 1 : 0);
        }
        else
        {
            insertIndex = PriorityBuildingPlans.Count;
        }
        
        return Mathf.Clamp(insertIndex, 0, PriorityBuildingPlans.Count);
    }

    private void InstantiatePreview(NewBuildingPlan plan)
    {
        var slot = plan.BuildingAnchor.GetComponentInChildren<SingleInstance>();
        plan.PreviewObject = _container.InstantiatePrefabForComponent<Building>(plan.BuildingPreviewPrefab, slot.transform);
        plan.PreviewObject.Data.Plan.Value = plan;
        plan.PreviewObject.StartBuilding();
        slot.Substitute(plan.PreviewObject.gameObject);
            
        var mapIcon = plan.PreviewObject.GetComponent<BuildPlanMapIcon>();
        mapIcon.Activate();
        plan.PreviewObject.DeactivateView();
    }
    
    private bool TryShowPreview()
    {
        var plan = GetCurrentPlan();

        if (plan is NewBuildingPlan newBuildingPlan && newBuildingPlan.PreviewObject != null)
        {
            newBuildingPlan.PreviewObject.ActivateView();
            return true;
        }
        return false;
    }
    
    private bool TryHidePreview()
    {
        var plan = GetCurrentPlan();
        if (plan is NewBuildingPlan newBuildingPlan && newBuildingPlan.PreviewObject != null)
        {
            newBuildingPlan.PreviewObject.DeactivateView();
            return true;
        }
        return false;
    }
}

[System.Serializable]
public abstract class BuildingPlan
{
    [SerializeField] private int _hoursDuration;

    public int HoursDuration
    {
        get => DurationProperty.CurrentValue;
        set
        {
            if (_hoursDuration == 0) return;
            
            if (_durationProperty == null) 
            {
                _durationProperty = new ReactiveProperty<int>(_hoursDuration);
                return;
            }
            
            _durationProperty.Value = value;
            _hoursDuration = value;
        }
    }

    private ReactiveProperty<int> _durationProperty;
    public ReadOnlyReactiveProperty<int> DurationProperty
    {
        get
        {
            if (_durationProperty == null) _durationProperty = new ReactiveProperty<int>(_hoursDuration);
            return _durationProperty;
        }
    }
    
    private int _defaultHoursDuration = -1;
    public int DefaultHoursDuration
    {
        get
        {
            if (_defaultHoursDuration == -1) _defaultHoursDuration = HoursDuration;
            return _defaultHoursDuration;
        }
    }

    private ReactiveProperty<float> _buildingProgress = new ReactiveProperty<float>(0);
    public ReactiveProperty<float> BuildingProgress => _buildingProgress;


    public BuildingPlan(int hours)
    {
        HoursDuration = hours;
    }
}

[System.Serializable]
public class NewBuildingPlan : BuildingPlan
{
    [field: SerializeField] public Anchor BuildingAnchor {get; private set;}
    [field: SerializeField] public Building BuildingPrefab {get; private set;}
    [field: SerializeField] public Building BuildingPreviewPrefab {get; private set;}
    public Building PreviewObject {get; set;}

    public NewBuildingPlan() : this(0)
    {
        
    }
    
    public NewBuildingPlan(int hours) : base(hours)
    {
        
    }
}

[System.Serializable]
public class RepairingPlan : BuildingPlan
{
    public Building BrokenBuilding {get; private set;}

    public RepairingPlan(int hours, Building brokenBuilding) : base(hours)
    {
        BrokenBuilding = brokenBuilding;
    }
}
