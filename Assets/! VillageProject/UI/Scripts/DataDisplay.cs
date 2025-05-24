using System.Collections;
using UnityEngine;
using Zenject;

public abstract class DataDisplay<T> : SelfRegistered
{
    public abstract void Load(T data);

    public override void RegisterSelf(DiContainer container)
    {
        var factory = new PrefabToComponentFactory<DataDisplay<T>>(gameObject);
        container.BindInstance<IFactory<Transform, DataDisplay<T>>>(factory).AsSingle();
    }
}