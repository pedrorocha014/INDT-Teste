using Applications;
using ContratacaoWorker;
using ContratacaoWorker.Services;
using Infrastructure;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddApplicationsServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);

// Registrar HttpClient
builder.Services.AddHttpClient();

builder.Services.AddSingleton<IKafkaConsumerService, KafkaConsumerService>();
builder.Services.AddScoped<IPropostaContratadaHandler, PropostaContratadaHandler>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
