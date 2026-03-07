using UnityEngine;

public class Villager : MonoBehaviour
{
    [SerializeField] private VillagerData _villagerData;
    [SerializeField] private ActivityType _currentActivity;
    public VillagerData VillagerData {get; private set;}

    private void Awake()
    {
        VillagerData = ScriptableObject.Instantiate(_villagerData);
    }
    
    public void ChangeActivity(ActivityType activity)
    {
        if (VillagerData.ActivityType == activity) return;
        
        VillagerData.ActivityType = activity;
        _currentActivity = VillagerData.ActivityType;
        Debug.Log($"Villager {gameObject.name} change to {activity}");
    }
}