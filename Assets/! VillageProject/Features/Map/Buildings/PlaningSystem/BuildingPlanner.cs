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
    public List<BuildingPlan> PriorityBuildingPlans { get; private set; }
    
    private BuildingStorage _storage;
    private DiContainer _container;
    private WaveController _waveController;
    
    [SerializeField] private GameObject _baseBuildingPrefab;
    
    private bool _isInitialized;
    public event Action<BuildingPlan> OnCurrentPlanChanged;

    [Inject]
    private void Construct(DiContainer container, BuildingStorage storage, WaveController waveController)
    {
        PriorityBuildingPlans = new List<BuildingPlan>();
        
        _container = container;
        _storage = storage;
        _storage.OnBuildingBroken += AddBrokenBuildingToPlan;
        _waveController = waveController;
        _waveController.OnWaveRoadChanged += OnRoadChanged;
    }

    public void Init()
    {
        GeneratePriorityPlans(NewBuildingPlansLength, _waveController.GetWaveRoadIndexes());
        _isInitialized = true;
    }
    
    private void OnRoadChanged(string[] roadIndexes)
    {
        if (!_isInitialized) return;
        TryHidePreview();
        
        GeneratePriorityPlans(NewBuildingPlansLength, roadIndexes);
        SortPlansByRoad(roadIndexes);
        ChangeCurrentPlan(GetCurrentPlan());
    }
    
    public BuildingPlan GetCurrentPlan()
    {
        if (PriorityBuildingPlans.Count == 0) GeneratePriorityPlans(NewBuildingPlansLength, _waveController.GetWaveRoadIndexes());
        if (PriorityBuildingPlans.Count == 0) return null;
        
        return PriorityBuildingPlans[0];
    }

    public void ChangeCurrentPlan(BuildingPlan plan)
    {
        TryHidePreview();
        PriorityBuildingPlans.Remove(plan);
        PriorityBuildingPlans.Insert(0, plan);
        TryShowPreview();
        OnCurrentPlanChanged?.Invoke(plan);
    }

    private void AddBrokenBuildingToPlan(Building building)
    {
        if (PriorityBuildingPlans.Any(p => p is RepairingPlan repairingPlan && repairingPlan.BrokenBuilding == building))
        {
            return;
        }

        var newPlan = new RepairingPlan(building.Data.RepairingHours, building, building.Data.RoadIndex);
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

        GeneratePriorityPlans(NewBuildingPlansLength, _waveController.GetWaveRoadIndexes(), false);
        OnCurrentPlanChanged?.Invoke(GetCurrentPlan());
    }
    
    private void Perform(NewBuildingPlan plan)
    {
        plan.PreviewObject.Data.Plan.Value = null;
        var slot = plan.BuildingAnchor.GetComponentInChildren<SingleInstance>();
        
        var building = _container.InstantiatePrefabForComponent<Building>(plan.BuildingPrefab, slot.transform);
        building.Data.RoadIndex = plan.RoadIndex;
        building.CompleteBuilding();
        slot.Substitute(building.gameObject);
        _storage.Add(building);
    }

    private void Perform(RepairingPlan plan)
    {
        plan.BrokenBuilding.Data.Plan.Value = null;
        plan.BrokenBuilding.Health.Amount = plan.BrokenBuilding.Health.MaxHealth;
        if (plan.BrokenBuilding.TryGetComponent(out GateState gateState)) gateState.Fix();
    }
    
    private void GeneratePriorityPlans(int maxCounts, string[] roadIndexes, bool isClearNonStartedPlans = true)
    {
        if (isClearNonStartedPlans) ClearNonStartedPlans();
        
        int existingCounts = PriorityBuildingPlans.Count(p => p is NewBuildingPlan newBuildingPlan && roadIndexes.Contains(newBuildingPlan.RoadIndex));
        int neededCount = maxCounts - existingCounts;
        if (neededCount <= 0) return;
        
        var remainingBuildingPlans = GetRemainingBuildingPlans(roadIndexes);
        if (remainingBuildingPlans.Count <= 0)
        {
            Debug.LogWarning($"Can't generate plan! All plans in roads were completed!");
            return;
        }
        
        neededCount = Mathf.Min(neededCount, remainingBuildingPlans.Count);
        int totalCounts = PriorityBuildingPlans.Count + neededCount;
        while (PriorityBuildingPlans.Count < totalCounts)
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

    private void ClearNonStartedPlans()
    {
        if (PriorityBuildingPlans.Count == 0) return;
        
        var removingPlans = PriorityBuildingPlans.Where(p => p is NewBuildingPlan && p.BuildingProgress.CurrentValue == 0)
            .Select(p => p as NewBuildingPlan).ToArray();

        foreach (var removingPlan in removingPlans)
        {            
            DestroyPreview(removingPlan);
            PriorityBuildingPlans.Remove(removingPlan);
        }
    }

    private List<BuildingPlan> GetRemainingBuildingPlans(string[] roadIndexes)
    {
        var remainingBuildingPlans = _allBuildingPlans.Where(p => !PriorityBuildingPlans.Contains(p) && p.BuildingProgress.CurrentValue < 1
                                                                  && roadIndexes.Contains(p.RoadIndex)).Select(p => p as BuildingPlan).ToList();
        return remainingBuildingPlans;
        
        if (remainingBuildingPlans.Count <= 0) // Раскомментить, если нужно брать планы по всем дорогам, когда планы на текущих дорогах закончились
        {
            Debug.LogWarning($"Can't generate plan! All plans in road {roadIndexes} were completed!");

            remainingBuildingPlans = _allBuildingPlans.Where(p => !PriorityBuildingPlans.Contains(p) && p.BuildingProgress.CurrentValue < 1)
                .Select(p => p as BuildingPlan).ToList();
        }
        
        return remainingBuildingPlans;
    }

    private bool TryGeneratePriorityPlan(out BuildingPlan plan, List<BuildingPlan> remainingBuildingPlans)
    {
        plan = null;
        if (remainingBuildingPlans.Count <= 0)
        {
            Debug.LogWarning("Can't generate plan! All plans were completed!");
            return false;
        }

        var maxProgressPlan = remainingBuildingPlans.Max(p => p.BuildingProgress.Value);
        BuildingPlan randomPlan = null;

        if (maxProgressPlan > 0) randomPlan = remainingBuildingPlans.Find(p => p.BuildingProgress.Value >= maxProgressPlan);
        else randomPlan = remainingBuildingPlans[Random.Range(0, remainingBuildingPlans.Count)];

        if (!PriorityBuildingPlans.Contains(randomPlan))
        {
            remainingBuildingPlans.Remove(randomPlan);
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
            insertIndex = lastRepairIndex != -1 ? lastRepairIndex + 1 : 1;
        }
        else
        {
            insertIndex = PriorityBuildingPlans.Count;
        }
        
        return Mathf.Clamp(insertIndex, 0, PriorityBuildingPlans.Count);
    }

    private void SortPlansByRoad(string[] roadIndexes)
    {
        if (PriorityBuildingPlans.Count <= 0) return;
        
        var currentPlan = PriorityBuildingPlans[0];
        
        var repairingPlans = PriorityBuildingPlans.Skip(1).Where(p => p is RepairingPlan);
        repairingPlans = repairingPlans.OrderByDescending(p => GetPriority(p, roadIndexes)).ToList();
        
        var builds = PriorityBuildingPlans.Skip(1).Where(p => p is NewBuildingPlan);
        builds = builds.OrderByDescending(p => GetPriority(p, roadIndexes)).ToList();
        
        PriorityBuildingPlans = new List<BuildingPlan>();
        PriorityBuildingPlans.Add(currentPlan);
        PriorityBuildingPlans.AddRange(repairingPlans);
        PriorityBuildingPlans.AddRange(builds);
    }

    private int GetPriority(BuildingPlan buildingPlan, string[] roadIndexes)
    {
        return roadIndexes.Contains(buildingPlan.RoadIndex) ? 1 : 0;
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

    private void DestroyPreview(NewBuildingPlan plan)
    {
        var slot = plan.BuildingAnchor.GetComponentInChildren<SingleInstance>();
        var anchor = Instantiate(_baseBuildingPrefab,  slot.transform);
        _container.InjectGameObject(anchor);
        
        var mapIcon = plan.PreviewObject.GetComponent<BuildPlanMapIcon>();
        mapIcon.Deactivate();
        
        slot.Substitute(anchor);
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

    private void OnDestroy()
    {
        _storage.OnBuildingBroken -= AddBrokenBuildingToPlan;
        _waveController.OnWaveRoadChanged -= OnRoadChanged;
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
    
    [field: SerializeField] public string RoadIndex {get; protected set;}
}

[System.Serializable]
public class NewBuildingPlan : BuildingPlan
{
    [field: SerializeField] public Anchor BuildingAnchor {get; private set;}
    [field: SerializeField] public Building BuildingPrefab {get; private set;}
    [field: SerializeField] public Building BuildingPreviewPrefab {get; private set;}
    
    public Building PreviewObject {get; set;}
}

[System.Serializable]
public class RepairingPlan : BuildingPlan
{
    public Building BrokenBuilding {get; private set;}

    public RepairingPlan(int hours, Building brokenBuilding, string roadIndex)
    {
        HoursDuration = hours;
        BrokenBuilding = brokenBuilding;
        RoadIndex = roadIndex;
    }
}
