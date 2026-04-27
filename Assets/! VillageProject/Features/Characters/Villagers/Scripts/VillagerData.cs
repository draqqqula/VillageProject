using System;
using R3;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "VillagerData", menuName = "Villagers Simulation/New Villager Data")]
public class VillagerData : ScriptableObject
{
    [field: SerializeField] public string Key {get; private set;}
    [field: SerializeField] public Gender Gender {get; private set;}
    [field: SerializeField] public ActivityType? ActivityType {get; set;}
    [field: SerializeField] public Profession Profession {get; set;}

    public Health Health { get; private set; }
    
    [field: SerializeField] public Loyalty Loyalty {get; private set;}
    
    public bool IsOnHome { get; set; }
    public bool IsTalking { get; set; }
    public bool IsMoving => _navmeshAgent.GetVelocityPerSecond() > 0.1f;

    private NavmeshMovementAgent _navmeshAgent;
    public HomePoint HomePoint {get; set;}

    public void Init(NavmeshMovementAgent navmeshAgent, Health health)
    {
        _navmeshAgent = navmeshAgent;
        Health = health;
    }
}

public enum Gender {Male, Female}

[Serializable]
public class Profession
{
    public ProfessionType Type => ProfessionData.Type;
    [field: SerializeField] public ProfessionData ProfessionData {get; set;}
    [SerializeField] private float _startExperience = 0f;
    
    private ReactiveProperty<float> _experience;
    public ReactiveProperty<float> Experience
    {
        get
        {
            if (_experience == null) _experience = new ReactiveProperty<float>(_startExperience);
            return _experience;
        }
    }
}

[Serializable]
public class Loyalty
{
    [field: SerializeField] private float _startLoyalty;
    private ReactiveProperty<float> _property;
    public ReactiveProperty<float> Property
    {
        get
        {
            if (_property == null) _property = new ReactiveProperty<float>(_startLoyalty);
            return _property;
        }
    }
}