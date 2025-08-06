using Infrastructure;
using Applications;
using Applications.UseCases.Contratacoes.Commands;
using Applications.UseCases.Propostas.Commands;
using Applications.UseCases.Propostas.Queries;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationsServices();

var mediatRAssemblies = new[]
{
    Assembly.GetAssembly(typeof(CreatePropostaCommand)),
    Assembly.GetAssembly(typeof(ListPropostasQuery)),
    Assembly.GetAssembly(typeof(GetPropostaByIdQuery)),
    Assembly.GetAssembly(typeof(UpdatePropostaCommand)),
};

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(mediatRAssemblies));

builder.Services.AddHttpClient();

builder.Services.AddControllers();

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
