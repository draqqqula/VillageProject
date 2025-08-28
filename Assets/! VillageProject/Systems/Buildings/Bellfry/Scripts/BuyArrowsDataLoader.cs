using R3;
using R3.Triggers;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UI;

public class BuyArrowsDataLoader : DataDisplay<BuyArrows.BuyArrowsData>
{
    private const string Amount = "amount";

    [SerializeField] private GameObject _arrowsDisplayPrefab;
    [SerializeField] private Selectable _selectable;
    [SerializeField] private ResourceVariable _resource;
    [SerializeField] private TMP_Text _Price;
    [SerializeField] private LocalizeStringEvent _Amount;
    [SerializeField] private Image _Image;
    private Canvas _canvas;
    private GameObject _arrowsDisplayInstance;

    public override void Load(BuyArrows.BuyArrowsData data)
    {
        _canvas = GetComponentInParent<Canvas>(true);
        _Amount.StringReference.SetReference(data.Label.TableReference, data.Label.TableEntryReference);
        ((IntVariable)_Amount.StringReference[Amount]).Value = (int)data.Amount.Amount;
        _Price.text = data.Price.Required.First().Amount.ToString();
        _Image.sprite = data.Sprite;
        _resource = data.Amount.Resource;
        _selectable.OnSelectAsObservable().Subscribe(HandleSelected).AddTo(this);
        _selectable.OnDeselectAsObservable().Subscribe(HandleDeselected).AddTo(this);
        _selectable.OnDisableAsObservable().Subscribe(HandleDisabled).AddTo(this);
    }

    private void HandleSelected(BaseEventData data)
    {
        if (_arrowsDisplayInstance != null)
        {
            return;
        }
        _arrowsDisplayInstance = Instantiate(_arrowsDisplayPrefab, _canvas.transform);
        if (_arrowsDisplayInstance.TryGetComponent<VariableDisplay>(out var variableDisplay))
        {
            variableDisplay.Resource = _resource;
        }
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