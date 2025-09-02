using UnityEngine;
using Zenject;
using R3;
using ObservableCollections;
using System.Collections.Generic;
using System;

public class ObjectiveDisplay : MonoBehaviour
{
    [Inject] private MatchObjective _objective;
    [SerializeField] private GameObject _prefab;

    void Awake()
    {
        _objective.Targets.ObserveAdd().Subscribe(it => CreateIcon(it.Value, it.Index));
        _objective.Targets.ObserveRemove().Subscribe(it => DestroyIcon(it.Index));
        _objective.Targets.ObserveMove().Subscribe(it => MoveIcon(it.OldIndex, it.NewIndex));
        _objective.Targets.ObserveReplace().Subscribe(it => UpdateIcon(it.Index, it.NewValue));
    }

    private TargetIcon CreateIcon(float health, int index)
    {
        var go = Instantiate(_prefab, transform);
        go.transform.SetSiblingIndex(index);
        var icon = go.GetComponent<TargetIcon>();
        icon.UpdateHealth(health);
        return icon;
    }

    private void DestroyIcon(int index)
    {
        Destroy(transform.GetChild(index).gameObject);
    }

    private void MoveIcon(int oldIndex, int newIndex)
    {
        transform.GetChild(oldIndex).SetSiblingIndex(newIndex);
    }

    private void UpdateIcon(int index, float value)
    {
        transform.GetChild(index).GetComponent<TargetIcon>().UpdateHealth(value);
    }
}
