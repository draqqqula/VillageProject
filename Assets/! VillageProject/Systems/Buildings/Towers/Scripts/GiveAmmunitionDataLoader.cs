using R3;
using R3.Triggers;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UI;

public class GiveAmmunitionDataLoader : DataDisplay<GiveAmmunitionMenuOption.GiveAmmunitionData>
{
    private const string Amount = "amount";
    private const string Max = "max";

    [SerializeField] private GameObject _arrowsDisplayPrefab;
    [SerializeField] private Selectable _selectable;
    [SerializeField] private LocalizeStringEvent _buyText;
    [SerializeField] private LocalizeStringEvent _storageText;
    [SerializeField] private GameObject _noArrows;
    private Canvas _canvas;
    private GameObject _arrowsDisplayInstance;

    public override void Load(GiveAmmunitionMenuOption.GiveAmmunitionData data)
    {
        _canvas = GetComponentInParent<Canvas>();
        var variable = (IntVariable)_buyText.StringReference[Amount];
        variable.Value = (int)data.BuyAmount;
        _selectable.OnSelectAsObservable().Subscribe(HandleSelected).AddTo(this);
        _selectable.OnDeselectAsObservable().Subscribe(HandleDeselected).AddTo(this);
        _selectable.OnDisableAsObservable().Subscribe(HandleDisabled).AddTo(this);
        if (data.StorageResource == null)
        {
            _storageText.gameObject.SetActive(false);
            _noArrows.SetActive(true);
        }
        else
        {
            _storageText.gameObject.SetActive(true);
            _noArrows.SetActive(false);
            ((IntVariable)_storageText.StringReference[Amount]).Value = (int)data.StorageAmount;
            ((IntVariable)_storageText.StringReference[Max]).Value = (int)data.MaxAmount;
        }
    }


    private void HandleSelected(BaseEventData data)
    {
        if (_arrowsDisplayInstance != null)
        {
            return;
        }
        _arrowsDisplayInstance = Instantiate(_arrowsDisplayPrefab, _canvas.transform);
    }

    private void HandleDeselected(BaseEventData data)
    {
        if (_arrowsDisplayInstance == null)
        {
            return;
        }
        Destroy(_arrowsDisplayInstance);
    }

    private void HandleDisabled(Unit data)
    {
        if (_arrowsDisplayInstance == null)
        {
            return;
        }
        Destroy(_arrowsDisplayInstance);
    }
}