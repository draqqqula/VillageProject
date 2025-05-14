using R3;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

public abstract class BuildingMenuItemBase : ObservableBehaviour
{
    protected void Reset()
    {
        GetComponentInParent<BuildingInfo>().Refresh();
    }
    public abstract bool IsAvailable();
    public abstract void ShowPreview();
    public abstract void HidePreview();
    public abstract bool TryPerform();
    public abstract GameObject GetUI(Transform transform);
}

public abstract class BuildingMenuItemBase<T> : BuildingMenuItemBase
{
    [Inject] private DiContainer _container;
    public abstract ReadOnlyReactiveProperty<T> Data { get; }
    public sealed override GameObject GetUI(Transform transform)
    {
        var component = _container.Resolve<IFactory<Transform, DataDisplay<T>>>().Create(transform);
        return component.gameObject;
    }
}