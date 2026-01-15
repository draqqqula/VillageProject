using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class BurningAttribute : DamageAttributeBase
{
    public class Effect : IDamageExecutable
    {
        private GameObject _vfx;
        public Effect(GameObject vfx)
        {
            _vfx = vfx;
        }

        public int GetPriority()
        {
            return 0;
        }

        public bool TryExecute(DamageInteractionContext context)
        {
            if (context.TargetAttributes.TryGetService<TransformDamageAttribute.Data>(out var transform))
            {
                GameObject.Instantiate(_vfx, transform.Transform);
            }
            return true;
        }
    }

    [SerializeField] private GameObject _vfx;

    public override void AddServices(IServiceCollection services)
    {
        services.AddSingleton<IDamageExecutable>(new Effect(_vfx));
    }
}