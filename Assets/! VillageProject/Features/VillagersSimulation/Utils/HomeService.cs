using System;
using System.Linq;
using R3;
using UnityEngine;
using Random = UnityEngine.Random;

public class HomeService : MonoBehaviour
{
    [SerializeField] private HomePoint[] _homePoints;

    public HomePoint OccupyHouse()
    {
        var houses = _homePoints.Where(h => !h.IsBusy).ToArray();
        var house = houses[Random.Range(0, houses.Length)];
        
        house.IsBusy = true;
        return house;
    }
    
    private void OnValidate()
    {
        foreach (var h in _homePoints)
        {
            if (h.Point == null) continue;
            h.DoorPoint = h.Point.GetChild(0);
            h.RelaxPoint = h.Point.GetChild(1);
        }
    }
}

[Serializable]
public class HomePoint
{
    [field: SerializeField] public Transform Point {get; private set;}
    [field: SerializeField] public bool IsBusy { get; set; }
    
    public ReactiveProperty<bool> IsAttacked {get => _isAttacked; private set => _isAttacked = value;}
    private ReactiveProperty<bool> _isAttacked = new ReactiveProperty<bool>(false);
    
    public Transform DoorPoint { get; set; }
    public Transform RelaxPoint { get; set; }
}