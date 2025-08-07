using System.Reflection;
using Applications.Services;
using Applications.Configs;
using Applications.UseCases.Contratacoes.Commands;
using Applications.UseCases.Contratacoes.Queries;
using Applications.UseCases.Propostas.Commands;
using Applications.UseCases.Propostas.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Applications;

public static class ApplicationsExtensions
{
    public static IServiceCollection AddApplicationsServices(this IServiceCollection services, IConfiguration configuration)
    {
        var mediatRAssemblies = new[]
        {
            Assembly.GetAssembly(typeof(CreatePropostaCommand)),
            Assembly.GetAssembly(typeof(ListPropostasQuery)),
            Assembly.GetAssembly(typeof(GetPropostaByIdQuery)),
            Assembly.GetAssembly(typeof(UpdatePropostaCommand)),
            Assembly.GetAssembly(typeof(ListContratosQuery)),
            Assembly.GetAssembly(typeof(CreateContratoCommand)),
            Assembly.GetAssembly(typeof(ContratarPropostaCommand))
        };

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(mediatRAssemblies));

        services.AddSingleton<IKafkaProducerService, KafkaProducerService>();

        var serviceConfig = new ServiceConfig
        {
            PropostaService = new PropostaServiceConfig
            {
                BaseUrl = configuration.GetSection("Services:PropostaService:BaseUrl").Value ?? "http://localhost:5276"
            }
        };
        services.AddSingleton(serviceConfig);

        return services;
    }
}