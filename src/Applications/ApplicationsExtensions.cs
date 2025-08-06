using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
        var mediatRAssemblies = new[]
        {
            Assembly.GetAssembly(typeof(CreatePropostaCommand)),
            Assembly.GetAssembly(typeof(ListPropostasQuery)),
            Assembly.GetAssembly(typeof(GetPropostaByIdQuery)),
            Assembly.GetAssembly(typeof(UpdatePropostaCommand)),
            Assembly.GetAssembly(typeof(ContratarPropostaCommand))
        };

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(mediatRAssemblies));

        services.AddSingleton<IKafkaProducerService, KafkaProducerService>();

        return services;
    }
}