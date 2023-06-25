namespace LVK.DependencyInjection.Extensions;

internal class BootstrapperCollection
{
    private readonly HashSet<Type> _AlreadyProcessedBootstrappers = new();

    public bool TryAdd(Type bootstrapperType) => _AlreadyProcessedBootstrappers.Add(bootstrapperType ?? throw new ArgumentNullException(nameof(bootstrapperType)));
}