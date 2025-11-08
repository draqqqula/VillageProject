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
        services.AddSingleton(serviceType, provider => provider.GetServiceAndInject(implementationInstance));
    }

    public static object GetServiceAndInject(this IServiceProvider provider, object implementationInstance)
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

    public static void AddSingletonWithAction<TService, TImplementation>(this IServiceCollection services, Action<TImplementation> resolveAction) where TImplementation : class
    {
        services.AddKeyedSingleton<TImplementation>(typeof(TImplementation));
        services.AddSingleton(typeof(TService), provider => provider.Resolve(resolveAction));
    }

    public static void AddScopedWithAction<TService, TImplementation>(this IServiceCollection services, Action<TImplementation> resolveAction) where TImplementation : class
    {
        services.AddKeyedScoped<TImplementation>(typeof(TImplementation));
        services.AddScoped(typeof(TService), provider => provider.Resolve(resolveAction));
    }

    public static void AddTransientWithAction<TService, TImplementation>(this IServiceCollection services, Action<TImplementation> resolveAction) where TImplementation : class
    {
        services.AddKeyedTransient<TImplementation>(typeof(TImplementation));
        services.AddTransient(typeof(TService), provider => provider.Resolve(resolveAction));
    }

    public static void AddSingletonInjectable<TService, TImplementation>(this IServiceCollection services, Action<TImplementation> resolveAction) where TImplementation : class
    {
        services.AddKeyedSingleton<TImplementation>(typeof(TImplementation));
        services.AddSingleton(typeof(TService), provider => provider.ResolveAndInject(resolveAction));
    }

    public static void AddScopedInjectable<TService, TImplementation>(this IServiceCollection services, Action<TImplementation> resolveAction) where TImplementation : class
    {
        services.AddKeyedScoped<TImplementation>(typeof(TImplementation));
        services.AddScoped(typeof(TService), provider => provider.ResolveAndInject(resolveAction));
    }

    public static void AddTransientInjectable<TService, TImplementation>(this IServiceCollection services, Action<TImplementation> resolveAction) where TImplementation : class
    {
        services.AddKeyedTransient<TImplementation>(typeof(TImplementation));
        services.AddTransient(typeof(TService), provider => provider.ResolveAndInject(resolveAction));
    }

    private static object Resolve<TImplementation>(this IServiceProvider provider, Action<TImplementation> resolveAction)
    {
        var service = provider.GetKeyedService<TImplementation>(typeof(TImplementation));
        resolveAction?.Invoke(service);
        return service;
    }

    private static object ResolveAndInject<TImplementation>(this IServiceProvider provider, Action<TImplementation> resolveAction)
    {
        var service = provider.GetKeyedService<TImplementation>(typeof(TImplementation));
        resolveAction?.Invoke(service);

        var zenjectContainer = provider.GetService<DiContainer>();
        if (zenjectContainer != null)
        {
            zenjectContainer.Inject(service);
        }

        return service;
    }

    public static void AddService<TService, TImplementation>(this IServiceCollection services, 
        ServiceLifetime lifetime, 
        Action<TImplementation> resolveAction,
        bool injectable) where TService : class where TImplementation : class, TService
    {
        switch (lifetime)
        {
            case ServiceLifetime.Singleton:
                {
                    if (injectable)
                    {
                        if (resolveAction != null)
                        {
                            services.AddSingletonInjectable<TService, TImplementation>(resolveAction);
                        }
                        else
                        {
                            services.AddSingletonInjectable<TService, TImplementation>();
                        }    
                    }
                    else
                    {
                        if (resolveAction != null)
                        {
                            services.AddSingletonWithAction<TService, TImplementation>(resolveAction);
                        }
                        else
                        {
                            services.AddSingleton<TService, TImplementation>();
                        }
                    }
                }
                break;
            case ServiceLifetime.Scoped:
                {
                    if (injectable)
                    {
                        if (resolveAction != null)
                        {
                            services.AddScopedInjectable<TService, TImplementation>(resolveAction);
                        }
                        else
                        {
                            services.AddScopedInjectable<TService, TImplementation>();
                        }
                    }
                    else
                    {
                        if (resolveAction != null)
                        {
                            services.AddScopedWithAction<TService, TImplementation>(resolveAction);
                        }
                        else
                        {
                            services.AddScoped<TService, TImplementation>();
                        }
                    }
                }
                break;
            case ServiceLifetime.Transient:
                {
                    if (injectable)
                    {
                        if (resolveAction != null)
                        {
                            services.AddTransientInjectable<TService, TImplementation>(resolveAction);
                        }
                        else
                        {
                            services.AddTransientInjectable<TService, TImplementation>();
                        }
                    }
                    else
                    {
                        if (resolveAction != null)
                        {
                            services.AddTransientWithAction<TService, TImplementation>(resolveAction);
                        }
                        else
                        {
                            services.AddTransient<TService, TImplementation>();
                        }
                    }
                }
                break;
        }
    }
}