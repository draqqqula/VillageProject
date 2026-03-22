using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class BuildingPlanner : MonoBehaviour
{
    private const int QueueLength = 3;
    
    [SerializeField] private List<BuildingPlan> _allBuildingPlans;
    private List<BuildingPlan> _completedBuildingPlans = new List<BuildingPlan>();
    public Queue<BuildingPlan> PriorityBuildingPlans { get; private set; }
    
    [Inject] private BuildingStorage _storage;
    [Inject] private DiContainer _container;

    public BuildingPlan GetCurrentPlan()
    {
        if (PriorityBuildingPlans == null || PriorityBuildingPlans.Count == 0) GeneratePriorityPlans(QueueLength);
        return PriorityBuildingPlans.Peek();
    }

    public bool TryCompleteCurrentPlan()
    {
        if (PriorityBuildingPlans == null || PriorityBuildingPlans.Count == 0)
        {
            Debug.LogError("Can't complete current plan! Queue is empty!");
            return false;
        }

        if (GetCurrentPlan().BuildingProgress < 1)
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
        var plan = PriorityBuildingPlans.Dequeue();
        Perform(plan);
        ShowPreview();
        
        _completedBuildingPlans.Add(plan);
    }
    
    private void Perform(BuildingPlan plan)
    {
        var slot = plan.BuildingAnchor.GetComponentInChildren<SingleInstance>();
        
        var building = _container.InstantiatePrefab(plan.BuildingPrefab, slot.transform);
        _container.InjectGameObject(building);
        slot.Substitute(building);
        _storage.Add(building.GetComponent<Building>());
    } 
    
    private void GeneratePriorityPlans(int count)
    {
        var remainingBuildingPlans = _allBuildingPlans.Except(_completedBuildingPlans).ToArray();
        PriorityBuildingPlans = new Queue<BuildingPlan>();
        
        if (remainingBuildingPlans.Length < count) count = remainingBuildingPlans.Length;
        while (PriorityBuildingPlans.Count < count)
        {
            var randomPlan = remainingBuildingPlans[Random.Range(0, remainingBuildingPlans.Length)];
            if (!PriorityBuildingPlans.Contains(randomPlan)) PriorityBuildingPlans.Enqueue(randomPlan);
        }
        ShowPreview();
    }
    
    private void ShowPreview()
    {
        var plan = GetCurrentPlan();

        if (plan.PreviewObject != null)
        {
            plan.PreviewObject.gameObject.SetActive(true);
        }
        else
        {
            var slot = plan.BuildingAnchor.GetComponentInChildren<SingleInstance>();
            plan.PreviewObject = Instantiate(plan.BuildingPreviewPrefab, slot.transform);
        }
    }
    
    private void HidePreview()
    {
        var plan = GetCurrentPlan();
        if (plan.PreviewObject != null)
        {
            plan.PreviewObject.gameObject.SetActive(false);
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
    [field: SerializeField] public float BuildingProgress {get; set;}
}
