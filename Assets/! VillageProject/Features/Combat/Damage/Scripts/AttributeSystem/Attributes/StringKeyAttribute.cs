using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class StringKeyAttribute : DamageAttributeBase
{
    public class StringKey : IKeyAttribute
    {
        private readonly string _key;
        public StringKey(string key) 
        {
            _key = key;
        }

        public object Key => _key;
    }

    [field: SerializeField] public string Key { get; private set; }

    public override void AddServices(IServiceCollection services)
    {
        services.AddSingleton<IKeyAttribute>(new StringKey(Key));
    }
}