using System.Reflection;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Messaging.Excentions;

public static class MassTransitExtensions
{
    public static IServiceCollection AddMassTransitWithAssemblies(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddMassTransit(configure =>
        {
            configure.SetKebabCaseEndpointNameFormatter();

            configure.SetInMemorySagaRepositoryProvider();

            configure.AddConsumers(assemblies);
            configure.AddSagaStateMachines(assemblies);
            configure.AddSagas(assemblies);
            configure.AddActivities(assemblies);

            configure.UsingInMemory((context, configurator) =>
            {
                configurator.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}