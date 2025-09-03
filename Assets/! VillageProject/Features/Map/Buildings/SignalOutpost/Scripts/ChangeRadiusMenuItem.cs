using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using static ChangeRadiusMenuItem;
using R3;

public class ChangeRadiusMenuItem : BuildingMenuItemBase<SignalTowerRangeData>
{
    [Serializable]
    public class Range
    {
        [SerializeField] public float Scale;
        [SerializeField] public LocalizedString Name;
    }

    public class SignalTowerRangeData
    {
        public LocalizedString Name;
    }

    private ReactiveProperty<bool> _available = new ReactiveProperty<bool>(true);
    private ReactiveProperty<SignalTowerRangeData> _data = new ReactiveProperty<SignalTowerRangeData>();

    [SerializeField] private List<Range> _ranges;
    [SerializeField] private Transform _range;
    [SerializeField] private GameObject _rangeDisplay;
    [SerializeField] private int _selected;

    public override ReadOnlyReactiveProperty<SignalTowerRangeData> Data => _data;

    public override ReadOnlyReactiveProperty<bool> Available => _available;

    private void Awake()
    {
        SetRadius();
    }

    public override void ShowPreview(GameObject ui)
    {
        _rangeDisplay.SetActive(true);
    }

    public override void HidePreview(GameObject ui)
    {
        _rangeDisplay.SetActive(false);
    }

    public override bool TryPerform()
    {
        if (_selected >= _ranges.Count - 1)
        {
            _selected = 0;
        }
        else
        {
            _selected += 1;
        }
        SetRadius();
        return true;
    }

    private void SetRadius()
    {
        var range = _ranges[_selected];
        _range.localScale = Vector3.one * range.Scale;
        _data.Value = new SignalTowerRangeData()
        {
            Name = range.Name
        };
    }
}
