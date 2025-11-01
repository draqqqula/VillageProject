using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class ZEnjectServiceProvider : IServiceProvider, IKeyedServiceProvider, IDisposable, IAsyncDisposable
{
    private ServiceProvider _innerServiceProvider;
    private DiContainer _container;

    public ZEnjectServiceProvider(ServiceProvider serviceProvider, DiContainer container)
    {
        _innerServiceProvider = serviceProvider;
        _container = container;
    }

    public void Dispose()
    {
        _innerServiceProvider.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return _innerServiceProvider.DisposeAsync();
    }

    public object GetKeyedService(Type serviceType, object serviceKey)
    {
        return _innerServiceProvider.GetKeyedService(serviceType, serviceKey);
    }

    public object GetRequiredKeyedService(Type serviceType, object serviceKey)
    {
        return _innerServiceProvider.GetRequiredKeyedService(serviceType, serviceKey);
    }

    public object GetService(Type serviceType)
    {
        _innerServiceProvider.CreateScope();
        var service = _innerServiceProvider.GetService(serviceType);
        return service;
    }
}