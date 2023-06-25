using System;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using NUnit.Framework.Internal;

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
        var services = new ServiceCollection();

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensions.Bootstrap(services, null));

        Assert.That(exception.ParamName, Is.EqualTo("servicesBootstrapper"));
    }

    [Test]
    public void BootstrapWithType_WithValidBootstrapper_CallsBootstrapper()
    {
        var services = new ServiceCollection();
        TestBootstrapper.BootstrapCallCount = 0;

        ServiceCollectionExtensions.Bootstrap(services, typeof(TestBootstrapper));

        Assert.That(TestBootstrapper.BootstrapCallCount, Is.EqualTo(1));
    }

    [Test]
    public void BootstrapWithType_WithValidBootstrapperCalledTwice_CallsBootstrapperOnlyOnce()
    {
        var services = new ServiceCollection();
        TestBootstrapper.BootstrapCallCount = 0;

        ServiceCollectionExtensions.Bootstrap(services, typeof(TestBootstrapper));
        ServiceCollectionExtensions.Bootstrap(services, typeof(TestBootstrapper));

        Assert.That(TestBootstrapper.BootstrapCallCount, Is.EqualTo(1));
    }

    [Test]
    public void BootstrapWithType_WithTypeThatDoesNotImplementInterface_ThrowsArgumentException()
    {
        var services = new ServiceCollection();
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
        var services = new ServiceCollection();
        TestBootstrapper.BootstrapCallCount = 0;

        ServiceCollectionExtensions.Bootstrap<TestBootstrapper>(services);

        Assert.That(TestBootstrapper.BootstrapCallCount, Is.EqualTo(1));
    }

    [Test]
    public void GenericBootstrap_WithValidBootstrapperCalledTwice_CallsBootstrapperOnlyOnce()
    {
        var services = new ServiceCollection();
        TestBootstrapper.BootstrapCallCount = 0;

        ServiceCollectionExtensions.Bootstrap<TestBootstrapper>(services);
        ServiceCollectionExtensions.Bootstrap<TestBootstrapper>(services);

        Assert.That(TestBootstrapper.BootstrapCallCount, Is.EqualTo(1));
    }

    [Test]
    public void Bootstrap_WithTwoServiceCollections_CallsBootstrapperTwice()
    {
        var services1 = new ServiceCollection();
        var services2 = new ServiceCollection();

        TestBootstrapper.BootstrapCallCount = 0;
        services1.Bootstrap<TestBootstrapper>();
        services1.Bootstrap<TestBootstrapper>();

        Assert.That(TestBootstrapper.BootstrapCallCount, Is.EqualTo(1));
    }

    [Test]
    public void Bootstrap_WithTwoServiceCollectionsAndOtherServicesRegisteredFirst_CallsBootstrapperTwice()
    {
        var services1 = new ServiceCollection();
        var services2 = new ServiceCollection();

        services1.AddTransient<string>(provider => "Service");
        services2.AddTransient<string>(provider => "Service");

        TestBootstrapper.BootstrapCallCount = 0;
        services1.Bootstrap<TestBootstrapper>();
        services1.Bootstrap<TestBootstrapper>();

        Assert.That(TestBootstrapper.BootstrapCallCount, Is.EqualTo(1));
    }
}