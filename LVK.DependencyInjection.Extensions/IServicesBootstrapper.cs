using Microsoft.Extensions.DependencyInjection;

namespace LVK.DependencyInjection.Extensions;

public interface IServicesBootstrapper
{
    void Bootstrap(IServiceCollection services);
}