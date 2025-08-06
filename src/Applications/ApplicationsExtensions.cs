using System.Reflection;
using Applications.Services;
using Applications.UseCases.Contratacoes.Commands;
using Applications.UseCases.Propostas.Commands;
using Applications.UseCases.Propostas.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Applications;

public static class ApplicationsExtensions
{
    public static IServiceCollection AddApplicationsServices(this IServiceCollection services)
    {
        services.AddSingleton<IKafkaProducerService, KafkaProducerService>();

        return services;
    }
}