using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public static class ServiceCollectionExtensions
{
    public static void AddSingletonInjectable(this IServiceCollection services, Type serviceType, Type implementationType)
    {
        services.AddKeyedSingleton(serviceType, implementationType, implementationType);
        services.AddSingleton(serviceType, provider => provider.GetServiceAndInject(serviceType, implementationType));
    }

    public static void AddScopedInjectable(this IServiceCollection services, Type serviceType, Type implementationType)
    {
        services.AddKeyedScoped(serviceType, implementationType, implementationType);
        services.AddScoped(serviceType, provider => provider.GetServiceAndInject(serviceType, implementationType));
    }

    public static void AddTransientInjectable(this IServiceCollection services, Type serviceType, Type implementationType)
    {
        services.AddKeyedTransient(serviceType, implementationType, implementationType);
        services.AddTransient(serviceType, provider => provider.GetServiceAndInject(serviceType, implementationType));
    }

    public static object GetServiceAndInject(this IServiceProvider provider, Type serviceType, Type implementationType)
    {
        var service = provider.GetRequiredKeyedService(serviceType, implementationType);
        var zenjectContainer = provider.GetService<DiContainer>();
        if (zenjectContainer != null)
        {
            zenjectContainer.Inject(service);
        }
        return service;
    }

    public static void AddSingletonInjectable(this IServiceCollection services, Type serviceType, object implementationInstance)
    {
        services.AddKeyedSingleton(serviceType, implementationInstance, implementationInstance);
        services.AddSingleton(serviceType, provider => provider.GetServiceAndInject(serviceType, implementationInstance));
    }

    public static object GetServiceAndInject(this IServiceProvider provider, Type serviceType, object implementationInstance)
    {
        var zenjectContainer = provider.GetService<DiContainer>();
        if (zenjectContainer != null)
        {
            zenjectContainer.Inject(implementationInstance);
        }
        return implementationInstance;
    }

    public static void AddSingletonInjectable<TService, TImplementation>(this IServiceCollection services)
    {
        AddSingletonInjectable(services, typeof(TService), typeof(TImplementation));
    }

    public static void AddScopedInjectable<TService, TImplementation>(this IServiceCollection services)
    {
        AddScopedInjectable(services, typeof(TService), typeof(TImplementation));
    }

    public static void AddTransientInjectable<TService, TImplementation>(this IServiceCollection services)
    {
        AddTransientInjectable(services, typeof(TService), typeof(TImplementation));
    }

    public static void AddSingletonInjectable<TService>(this IServiceCollection services, TService implementationInstance)
    {
        AddSingletonInjectable(services, typeof(TService), implementationInstance);
    }

    public static void AddSingletonInjectable<TService>(this IServiceCollection services)
    {
        AddSingletonInjectable(services, typeof(TService), typeof(TService));
    }

    public static void AddScopedInjectable<TService>(this IServiceCollection services)
    {
        AddScopedInjectable(services, typeof(TService), typeof(TService));
    }

    public static void AddTransientInjectable<TService>(this IServiceCollection services)
    {
        AddTransientInjectable(services, typeof(TService), typeof(TService));
    }
}