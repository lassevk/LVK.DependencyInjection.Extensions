using Microsoft.Extensions.DependencyInjection;

namespace LVK.DependencyInjection.Extensions.Tests;

#pragma warning disable CS8600, CS8625, CS8602
public class TestBootstrapper : IServicesBootstrapper
{
    public static int BootstrapCallCount;

    public void Bootstrap(IServiceCollection services)
    {
        BootstrapCallCount++;
    }
}