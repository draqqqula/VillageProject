using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

[Serializable]
public class GameObjectKeyAttribute : DamageAttributeBase
{
    public class GameObjectKey : IKeyAttribute
    {
        public GameObjectKey(GameObject gameObject)
        {
            _gameObject = gameObject;
        }

        private GameObject _gameObject;
        public object Key => _gameObject;
    }

    public override void AddServices(IServiceCollection services)
    {
        services.AddSingleton<IKeyAttribute, GameObjectKey>();
    }
}