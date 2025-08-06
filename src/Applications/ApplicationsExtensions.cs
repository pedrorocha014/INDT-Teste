using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
            Assembly.GetAssembly(typeof(UpdatePropostaCommand))
        };

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(mediatRAssemblies));

        return services;
    }
}