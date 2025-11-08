using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class HitVFXDamageAttribute : DamageAttributeBase
{
    public class Effect : IDamageExecutable
    {
        private GameObject _prefab;

        public Effect(GameObject prefab)
        {
            _prefab = prefab;
        }

        public bool TryExecute(DamageInteractionContext context)
        {
            if (context.SourceAttributes.TryGetService<TransformDamageAttribute.Data>(out var transformAttribute))
            {
                GameObject.Instantiate(_prefab, transformAttribute.Transform.position, Quaternion.identity);
            }
            return true;
        }
    }

    [SerializeField] private GameObject _prefab;

    public override void AddServices(IServiceCollection services)
    {
        services.AddSingleton(new Effect(_prefab));
        services.AddSingleton<IDamageExecutable>(provider => provider.GetService<Effect>());
    }
}