using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using R3;
using Zenject;
using R3.Triggers;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System.Linq;
using System.Reflection.Emit;

public class BuildingMenuDisplay : MonoBehaviour
{
    private const string DefaultLabel = "None";

    [SerializeField] private GameObject _menuItemPrefab;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private ExpandableLayout _optionList;
    [SerializeField] private IServiceProvider _serviceProvider;
    DiContainer _container;
    private List<IDisposable> _subscribtions = new List<IDisposable>();
    public void Load(GameObject building)
    {
        Unload();

        var info = building.GetComponent<BuildingInfo>();
        Subscribe(info.OnDisableAsObservable(), value => HandleBuildingDisabled(info));
        _text.text = info.Name.GetLocalizedString();

        HandleOptions(info.Options);

        SelectFirstOption();
    }

    private void HandleOptions(IEnumerable<BuildingMenuItemBase> options)
    {
        foreach (var option in options)
        {
            var ui = option.GetUI(_optionList.transform);
            var item = _optionList.Add(ui);
            item.SetActive(option.Enabled.CurrentValue);
            var button = item.GetComponent<Button>();
            Subscribe(button.OnSelectAsObservable(), value => option.ShowPreview());
            Subscribe(button.OnDeselectAsObservable(), value => option.HidePreview());
            Subscribe(button.OnDisableAsObservable(), value => option.HidePreview());
            Subscribe(button.OnDestroyAsObservable(), value => option.HidePreview());
            Subscribe(button.OnClickAsObservable(), value => option.TryPerform());
            Subscribe(option.Enabled, value => HandleOptionEnabled(value, item));
        }
        _optionList.UpdateLayout();
    }

    private void SelectFirstOption()
    {
        if (_optionList.Items.Count > 0)
        {
            _optionList.Items[0].AddComponent<AutoSelect>();
        }
    }

    public void Unload()
    {
        _text.text = DefaultLabel;

        foreach (var item in _optionList.Items)
        {
            Destroy(item);
        }
        ClearSubsrciptions();
    }

    private void ClearSubsrciptions()
    {
        foreach (var subscription in _subscribtions)
        {
            subscription.Dispose();
        }
        _subscribtions.Clear();
    }

    private void Subscribe<T>(Observable<T> observable, Action<T> action)
    {
        _subscribtions.Add(observable.Subscribe(action));
    }

    private void HandleOptionEnabled(bool value, GameObject window)
    {
        window.SetActive(value);
    }

    private void HandleBuildingDisabled(BuildingInfo building)
    {
        Unload();
    }
}