using Infrastructure;
using Applications;
using Applications.UseCases.Contratacoes.Commands;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var mediatRAssemblies = new[]
{
    Assembly.GetAssembly(typeof(ContratarPropostaCommand)),
};

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(mediatRAssemblies));

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationsServices();

builder.Services.AddControllers();

builder.Services.AddHttpClient();

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