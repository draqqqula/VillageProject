using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static NewBuildingOption;

public class NewBuildingOption : BuildingMenuItemBase<NewBuildingData>
{
    public class NewBuildingData
    {
        public string Name;
        public Price Price;

        public NewBuildingData(string name, Price price)
        {
            Name = name;
            Price = price;
        }
    }

    [Inject] private DiContainer _container;
    private ReactiveProperty<NewBuildingData> _data = new ReactiveProperty<NewBuildingData>();
    private SingleInstance _slot;
    private GameObject _previewObject;
    [field: SerializeField] public GameObject BuildingPrefab { get; private set; }
    [field: SerializeField] public GameObject PreviewPrefab { get; private set; }
    [field: SerializeField] public PriceReference Price { get; private set; }
    [field: SerializeField] public string Description { get; private set; }

    public override ReadOnlyReactiveProperty<NewBuildingData> Data => _data;

    public override ReadOnlyReactiveProperty<bool> Available => Price.Available;

    public override bool TryPerform()
    {
        if (Price.Value.TryPay())
        {
            var building = _container.InstantiatePrefab(BuildingPrefab, _slot.transform);
            _container.InjectGameObject(building);
            _slot.Substitute(building);
            return true;
        }
        return false;
    }

    public override void ShowPreview(GameObject ui)
    {
        var parent = _slot.transform;
        _previewObject = Instantiate(PreviewPrefab, parent);
        var showDescription = ui.GetComponent<ShowDescription>();
        showDescription.enabled = true;
        showDescription.SetText(Description);
        showDescription.gameObject.GetComponentInParent<LayoutHelper>().Rebuild();
    }
    public override void HidePreview(GameObject ui)
    {
        if (_previewObject != null)
        {
            Destroy(_previewObject);
        }
        ui.GetComponent<ShowDescription>().enabled = false;
    }

    private void Awake()
    {
        _slot = GetComponentInParent<SingleInstance>();
        var name = BuildingPrefab.GetComponent<BuildingInfo>().Name.GetLocalizedString();
        _data.Value = new NewBuildingData(name, Price.Value);
    }
}
