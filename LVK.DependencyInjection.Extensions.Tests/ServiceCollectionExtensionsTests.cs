using System;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

// ReSharper disable InvokeAsExtensionMethod
#pragma warning disable CS8600
#pragma warning disable CS8625
#pragma warning disable CS8602

namespace LVK.DependencyInjection.Extensions.Tests;

public class ServiceCollectionExtensionsTests
{
    [Test]
    public void BootstrapWithType_NullServices_ThrowsArgumentNullException()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensions.Bootstrap(null, typeof(TestBootstrapper)));

        Assert.That(exception.ParamName, Is.EqualTo("services"));
    }

    [Test]
    public void BootstrapWithType_NullBootstrapperType_ThrowsArgumentNullException()
    {
        ServiceCollection services = new();

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensions.Bootstrap(services, null));

        Assert.That(exception.ParamName, Is.EqualTo("servicesBootstrapper"));
    }

    [Test]
    public void BootstrapWithType_WithValidBootstrapper_CallsBootstrapper()
    {
        ServiceCollection services = new();
        TestBootstrapper.BootstrapCallCount = 0;

        ServiceCollectionExtensions.Bootstrap(services, typeof(TestBootstrapper));

        Assert.That(TestBootstrapper.BootstrapCallCount, Is.EqualTo(1));
    }

    [Test]
    public void BootstrapWithType_WithValidBootstrapperCalledTwice_CallsBootstrapperOnlyOnce()
    {
        ServiceCollection services = new();
        TestBootstrapper.BootstrapCallCount = 0;

        ServiceCollectionExtensions.Bootstrap(services, typeof(TestBootstrapper));
        ServiceCollectionExtensions.Bootstrap(services, typeof(TestBootstrapper));

        Assert.That(TestBootstrapper.BootstrapCallCount, Is.EqualTo(1));
    }

    [Test]
    public void BootstrapWithType_WithTypeThatDoesNotImplementInterface_ThrowsArgumentException()
    {
        ServiceCollection services = new();
        TestBootstrapper.BootstrapCallCount = 0;

        ArgumentException exception = Assert.Throws<ArgumentException>(() => ServiceCollectionExtensions.Bootstrap(services, typeof(InvalidServiceBootstrapper)));

        Assert.That(exception.ParamName, Is.EqualTo("servicesBootstrapper"));
    }

    [Test]
    public void GenericBootstrap_NullServices_ThrowsArgumentNullException()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensions.Bootstrap<TestBootstrapper>(null));

        Assert.That(exception.ParamName, Is.EqualTo("services"));
    }

    [Test]
    public void GenericBootstrap_WithValidBootstrapper_CallsBootstrapper()
    {
        ServiceCollection services = new();
        TestBootstrapper.BootstrapCallCount = 0;

        ServiceCollectionExtensions.Bootstrap<TestBootstrapper>(services);

        Assert.That(TestBootstrapper.BootstrapCallCount, Is.EqualTo(1));
    }

    [Test]
    public void GenericBootstrap_WithValidBootstrapperCalledTwice_CallsBootstrapperOnlyOnce()
    {
        ServiceCollection services = new();
        TestBootstrapper.BootstrapCallCount = 0;

        ServiceCollectionExtensions.Bootstrap<TestBootstrapper>(services);
        ServiceCollectionExtensions.Bootstrap<TestBootstrapper>(services);

        Assert.That(TestBootstrapper.BootstrapCallCount, Is.EqualTo(1));
    }

    [Test]
    public void Bootstrap_WithTwoServiceCollections_CallsBootstrapperTwice()
    {
        ServiceCollection services1 = new();
        ServiceCollection services2 = new();

        TestBootstrapper.BootstrapCallCount = 0;
        services1.Bootstrap<TestBootstrapper>();
        services2.Bootstrap<TestBootstrapper>();

        Assert.That(TestBootstrapper.BootstrapCallCount, Is.EqualTo(2));
    }

    [Test]
    public void Bootstrap_WithTwoServiceCollectionsCalledRepeatedly_CallsBootstrapperOnlyTwice()
    {
        ServiceCollection services1 = new();
        ServiceCollection services2 = new();

        TestBootstrapper.BootstrapCallCount = 0;
        services1.Bootstrap<TestBootstrapper>();
        services2.Bootstrap<TestBootstrapper>();
        services1.Bootstrap<TestBootstrapper>();
        services2.Bootstrap<TestBootstrapper>();

        Assert.That(TestBootstrapper.BootstrapCallCount, Is.EqualTo(2));
    }

    [Test]
    public void Bootstrap_WithTwoServiceCollectionsAndOtherServicesRegisteredFirst_CallsBootstrapperTwice()
    {
        ServiceCollection services1 = new();
        ServiceCollection services2 = new();

        services1.AddTransient<string>(provider => "Service");
        services2.AddTransient<string>(provider => "Service");

        TestBootstrapper.BootstrapCallCount = 0;
        services1.Bootstrap<TestBootstrapper>();
        services1.Bootstrap<TestBootstrapper>();

        Assert.That(TestBootstrapper.BootstrapCallCount, Is.EqualTo(1));
    }

    [Test]
    public void AddFuncFactory_NullServices_ThrowsArgumentNullException()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => ServiceCollectionExtensions.AddFuncFactory<ITestService>(null));

        Assert.That(exception.ParamName, Is.EqualTo("services"));
    }

    [Test]
    public void AddFuncFactoryWhenResolved_ReturnsDelegateThatResolvesService()
    {
        ServiceCollection services = new();

        services.AddTransient<ITestService, TestService>();
        services.AddFuncFactory<ITestService>();
        ServiceProvider provider = services.BuildServiceProvider();

        Func<ITestService>? factory = provider.GetService<Func<ITestService>>();

        Assert.That(factory, Is.Not.Null);
        Assert.That(factory(), Is.InstanceOf<TestService>());
    }

    [Test]
    public void AddFuncFactory_ResolvedAndCalledTwice_ResolvesToDifferentInstances()
    {
        ServiceCollection services = new();

        services.AddTransient<ITestService, TestService>();
        services.AddFuncFactory<ITestService>();
        ServiceProvider provider = services.BuildServiceProvider();

        Func<ITestService>? factory = provider.GetService<Func<ITestService>>();

        Assert.That(factory, Is.Not.Null);
        ITestService instance1 = factory();
        ITestService instance2 = factory();
        Assert.That(instance1, Is.Not.SameAs(instance2));
    }
}