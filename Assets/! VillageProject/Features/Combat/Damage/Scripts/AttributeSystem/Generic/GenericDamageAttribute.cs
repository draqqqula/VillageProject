using Microsoft.Extensions.DependencyInjection;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using UnityEditor;



[Serializable]
public class GenericDamageAttribute<T> : DamageAttributeBase where T : class
{
    [SerializeReference, SubclassSelector]
    public List<SerializedPropertyValueBase> PropertyValues = new();
    private Action<T> _cachedInjector;

    [SerializeField] public ServiceLifetime ServiceLifetime = ServiceLifetime.Singleton;
    [SerializeField] public bool Injectable = false;

    public override void AddServices(IServiceCollection services)
    {
        services.AddService<T, T>(ServiceLifetime, InjectProperties, Injectable);


        var interfaces = typeof(T)
            .GetInterfaces()
            .ToArray();

        foreach (var iface in interfaces)
        {
            switch (ServiceLifetime)
            {
                case ServiceLifetime.Singleton:
                    services.AddSingleton(iface, provider => provider.GetService<T>());
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped(iface, provider => provider.GetService<T>());
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient(iface, provider => provider.GetService<T>());
                    break;
            }
        }
    }

    private Action<T> BuildPropertyInjector()
    {
        if (PropertyValues.Count == 0)
        {
            return null;
        }

        var targetType = typeof(T);
        var paramInstance = Expression.Parameter(targetType, "instance");

        var expressions = new List<Expression>();

        var valueCache = new Dictionary<string, object>();
        foreach (var prop in PropertyValues)
        {
            valueCache[prop.Name] = prop.Value;
        }

        var fields = targetType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(f => f.GetCustomAttribute<FromDamageAttributePropertyAttribute>() != null);

        foreach (var field in fields)
        {
            if (!valueCache.TryGetValue(field.Name, out var value))
                continue;

            var fieldExpr = Expression.Field(paramInstance, field);
            var constExpr = Expression.Constant(value, field.FieldType);
            var assignExpr = Expression.Assign(fieldExpr, constExpr);
            expressions.Add(assignExpr);
        }

        if (expressions.Count == 0)
        {
            return _ => { };
        }

        var body = Expression.Block(expressions);
        var lambda = Expression.Lambda<Action<T>>(body, paramInstance);
        return lambda.Compile();
    }

    public void InjectProperties(T instance)
    {
        _cachedInjector ??= BuildPropertyInjector();
        if (_cachedInjector != null)
        {
            _cachedInjector.Invoke(instance);
        }
    }

    public void EnsureProperties()
    {
        var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(f => f.GetCustomAttribute<FromDamageAttributePropertyAttribute>() != null)
            .ToList();

        var existingNames = new HashSet<string>(PropertyValues.Select(p => p.Name));


        foreach (var field in fields)
        {
            if (existingNames.Contains(field.Name))
                continue;

            var newEntry = AttributeEditorExtensions.CreateSerializedValue(field.Name, field.FieldType);

            if (newEntry != null)
            {
                PropertyValues.Add(newEntry);
            }
        }
    }
}