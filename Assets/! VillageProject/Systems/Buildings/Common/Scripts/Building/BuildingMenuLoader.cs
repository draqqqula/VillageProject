using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BuildingMenuLoader : MonoBehaviour
{
    [SerializeField] private SingleInstance _buildingRoot;
    [Inject] private BuildingMenuDisplay _display;

    public void Load()
    {
        _display.Load(_buildingRoot.Instance);
        _buildingRoot.InstanceChanged += HandleNewBuilding;
    }

    private void OnDisable()
    {
        _buildingRoot.InstanceChanged -= HandleNewBuilding;
        _display.Unload();
    }

    private void HandleNewBuilding()
    {
        Load();
    }
}