using System;
using R3;
using UnityEngine;

public class Building : MonoBehaviour
{
    [field: SerializeReference, SubclassSelector] public BuildingData Data { get; private set; }
    [field: SerializeField] private GameObject _view;
    
    [field: SerializeField] public Health Health { get; private set; }
    public event Action<Building> OnBroken;

    public bool IsViewActivated => _view.activeSelf;

    private void Awake()
    {
        if (Health != null) Health.AmountReactive.Skip(1).Subscribe(OnHealthChanged).AddTo(this);
    }

    private void OnHealthChanged(float value)
    {
        if (value <= 0)
        {
            Data.CurrentState = BuildingData.State.Broken;
            OnBroken?.Invoke(this);
        }
    }
    
    public void StartBuilding()
    {
        if (Data.CurrentState == BuildingData.State.Broken) return;
        Data.CurrentState = BuildingData.State.Build;
    }
    
    public void CompleteBuilding()
    {
        if (Data.Type == BuildingType.ArcherTower) Data.CurrentState = BuildingData.State.Wait;
        else SetReady();
    }

    public void SetReserved()
    {
        Data.CurrentState = BuildingData.State.Reserved;
    }

    public void SetReady()
    {
        Data.CurrentState = BuildingData.State.Ready;
    }

    public void SetWaiting()
    {
        Data.CurrentState = BuildingData.State.Wait;
    }
    
    public void ActivateView()
    {
        if (IsViewActivated) return;
        _view.SetActive(true);
    }
    
    public void DeactivateView()
    {
        if (!IsViewActivated) return;
        _view.SetActive(false);
    }
}