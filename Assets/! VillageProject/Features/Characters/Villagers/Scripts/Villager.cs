using UnityEngine;

public class Villager : MonoBehaviour
{
    [field:SerializeField] public VillagerData VillagerData {get; private set;}

    private void Awake()
    {
        VillagerData = new VillagerData();
    }
    
    public void ChangeActivity(ActivityType activity)
    {
        if (VillagerData.ActivityType == activity) return;
        
        VillagerData.ActivityType = activity;
        Debug.Log($"Villager {gameObject.name} change to {activity}");
    }
}