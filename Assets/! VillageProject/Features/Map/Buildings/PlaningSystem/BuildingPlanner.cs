using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class BuildingPlanner : MonoBehaviour
{
    private const int PlansLength = 3;
    
    [SerializeField] private List<BuildingPlan> _allBuildingPlans;
    private List<BuildingPlan> _completedBuildingPlans = new List<BuildingPlan>();
    public List<BuildingPlan> PriorityBuildingPlans { get; private set; }
    
    [Inject] private BuildingStorage _storage;
    [Inject] private DiContainer _container;
    
    public event Action<BuildingPlan> OnCurrentPlanChanged;

    private void Awake()
    {
        PriorityBuildingPlans = new List<BuildingPlan>();
        GeneratePriorityPlans(PlansLength);
    }
    
    public BuildingPlan GetCurrentPlan()
    {
        if (PriorityBuildingPlans.Count == 0) GeneratePriorityPlans(PlansLength);
        if (PriorityBuildingPlans.Count == 0) return null;
        
        return PriorityBuildingPlans[0];
    }

    public void ChangeCurrentPlan(BuildingPlan plan)
    {
        HidePreview();
        PriorityBuildingPlans.Remove(plan);
        PriorityBuildingPlans.Insert(0, plan);
        ShowPreview();
        
        OnCurrentPlanChanged?.Invoke(plan);
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
        HidePreview();
        
        var plan = PriorityBuildingPlans[0];
        PriorityBuildingPlans.RemoveAt(0);
        
        Perform(plan);
        plan.PreviewObject.Data.Plan.Value = null;
        _completedBuildingPlans.Add(plan);
        
        GeneratePriorityPlans(PlansLength, false);
        OnCurrentPlanChanged?.Invoke(GetCurrentPlan());
    }
    
    private void Perform(BuildingPlan plan)
    {
        var slot = plan.BuildingAnchor.GetComponentInChildren<SingleInstance>();
        
        var building = _container.InstantiatePrefab(plan.BuildingPrefab, slot.transform);
        _container.InjectGameObject(building);
        slot.Substitute(building);
        _storage.Add(building.GetComponent<Building>());
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
                PriorityBuildingPlans.Add(plan);
                InstantiatePreview(plan);
            }
        }
        ShowPreview();
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

    private void InstantiatePreview(BuildingPlan plan)
    {
        var slot = plan.BuildingAnchor.GetComponentInChildren<SingleInstance>();
        plan.PreviewObject = _container.InstantiatePrefabForComponent<Building>(plan.BuildingPreviewPrefab, slot.transform);
        plan.PreviewObject.Data.Plan.Value = plan;
        slot.Substitute(plan.PreviewObject.gameObject);
            
        var mapIcon = plan.PreviewObject.GetComponent<BuildPlanMapIcon>();
        mapIcon.Activate();
        plan.PreviewObject.DeactivateView();
    }
    
    private void ShowPreview()
    {
        var plan = GetCurrentPlan();

        if (plan.PreviewObject != null)
        {
            plan.PreviewObject.ActivateView();
        }
    }
    
    private void HidePreview()
    {
        var plan = GetCurrentPlan();
        if (plan.PreviewObject != null)
        {
            plan.PreviewObject.DeactivateView();
        }
    }
}

[System.Serializable]
public class BuildingPlan
{
    [field: SerializeField] public Anchor BuildingAnchor {get; private set;}
    [field: SerializeField] public Building BuildingPrefab {get; private set;}
    [field: SerializeField] public Building BuildingPreviewPrefab {get; private set;}
    public Building PreviewObject {get; set;}
    
    [field: SerializeField] public int HoursDuration {get; private set;}
    public ReactiveProperty<float> BuildingProgress => _buildingProgress;
    private ReactiveProperty<float> _buildingProgress = new ReactiveProperty<float>(0);
}
