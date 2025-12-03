using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "New Asset Table", menuName = "Asset Table")]
public class EnumAssetTable : ScriptableObject
{
    [Serializable]
    public class Entry<TKey, TValue> where TKey : Enum where TValue : UnityEngine.Object
    {
        [SerializeField] public TKey Key;
        [SerializeField] public TValue Value;
    }


    public abstract class GroupBase
    {
        public abstract void Register(IServiceCollection services);
        public abstract void Register(DiContainer container);
    }

    [Serializable]
    public class Group<TKey, TValue> : GroupBase where TKey : Enum where TValue : UnityEngine.Object
    {
        [SerializeField] public List<Entry<TKey, TValue>> Entries = new List<Entry<TKey, TValue>>();

        public override void Register(IServiceCollection services)
        {
            var dict = BuildDictionary();
            services.AddSingleton(dict);
        }

        public override void Register(DiContainer container)
        {
            var dict = BuildDictionary();
            container.BindInstance(dict).AsSingle();
        }

        private Dictionary<TKey, TValue> BuildDictionary()
        {
            var result = new Dictionary<TKey, TValue>();
            foreach (var entry in Entries)
            {
                result.Add(entry.Key, entry.Value);
            }
            return result;
        }
    }

    [SerializeReference] public GroupBase KeyValuePairs;
}