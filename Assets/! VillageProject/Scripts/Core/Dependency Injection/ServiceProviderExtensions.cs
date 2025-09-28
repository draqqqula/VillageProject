using Microsoft.Extensions.DependencyInjection;
using System;

public static class ServiceProviderExtensions
{
    public static bool TryGetService<T>(this IServiceProvider services, out T service)
    {
        service = services.GetService<T>();
        return service != null;
    }
}
