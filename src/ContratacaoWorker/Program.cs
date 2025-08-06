using Applications;
using Applications.UseCases.Contratacoes.Commands;
using ContratacaoWorker;
using ContratacaoWorker.Services;
using Infrastructure;
using System.Reflection;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddApplicationsServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Registrar HttpClient
builder.Services.AddHttpClient();

var mediatRAssemblies = new[]
{
    Assembly.GetAssembly(typeof(CreateContratoCommand))
};

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(mediatRAssemblies));

builder.Services.AddSingleton<IKafkaConsumerService, KafkaConsumerService>();
builder.Services.AddScoped<IPropostaContratadaHandler, PropostaContratadaHandler>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
