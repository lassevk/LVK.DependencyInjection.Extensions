using Microsoft.Extensions.DependencyInjection;

namespace LVK.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    private static IServiceCollection? _PreviousServiceCollection;
    private static BootstrapperCollection? _PreviousBootstrapperCollection;

    public static IServiceCollection AddFuncFactory<T>(this IServiceCollection services)
        where T : class
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        services.AddSingleton<Func<T>>(provider => () => provider.GetService<T>() ??
            throw new InvalidOperationException($"Unable to resolve service of type {typeof(T).FullName}"));

        return services;
    }

    public static IServiceCollection Bootstrap(this IServiceCollection services, Type servicesBootstrapper)
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        _ = servicesBootstrapper ?? throw new ArgumentNullException(nameof(servicesBootstrapper));

        BootstrapperCollection bootstrapperCollection = GetBootstrapperCollection(services);
        if (!bootstrapperCollection.TryAdd(servicesBootstrapper))
            return services;

        IServicesBootstrapper? bootstrapper = Activator.CreateInstance(servicesBootstrapper) as IServicesBootstrapper;
        if (bootstrapper is null)
            throw new ArgumentException($"{servicesBootstrapper.FullName} does not implement IServicesBootstrapper", nameof(servicesBootstrapper));

        bootstrapper.Bootstrap(services);
        return services;
    }

    public static IServiceCollection Bootstrap<T>(this IServiceCollection services)
        where T: class, IServicesBootstrapper, new()
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        BootstrapperCollection bootstrapperCollection = GetBootstrapperCollection(services);
        if (!bootstrapperCollection.TryAdd(typeof(T)))
            return services;

        new T().Bootstrap(services);
        return services;
    }

    private static BootstrapperCollection GetBootstrapperCollection(IServiceCollection services)
    {
        if (ReferenceEquals(services, _PreviousServiceCollection))
            return _PreviousBootstrapperCollection!;

        _PreviousServiceCollection = services;
        Type type = typeof(BootstrapperCollection);
        _PreviousBootstrapperCollection =
            services.FirstOrDefault(descriptor =>
                    descriptor.Lifetime == ServiceLifetime.Singleton && descriptor.ServiceType == type)
                ?.ImplementationInstance as BootstrapperCollection;

        if (_PreviousBootstrapperCollection is null)
        {
            _PreviousBootstrapperCollection = new();
            services.AddSingleton<BootstrapperCollection>(_PreviousBootstrapperCollection);
        }

        return _PreviousBootstrapperCollection;
    }
}