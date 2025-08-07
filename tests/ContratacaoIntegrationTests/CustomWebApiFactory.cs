using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using ContratacaoIntegrationTests.Helpers;
using Microsoft.AspNetCore.Hosting;

namespace ContratacaoIntegrationTests;

public class CustomWebApiFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureHostConfiguration(config =>
        {
            config.AddInMemoryCollection([
                new KeyValuePair<string, string?>("ConnectionStrings:PostgresConnection", PostgresHelper.ConnectionString),
                new KeyValuePair<string, string?>("Services:PropostaService:BaseUrl", "http://localhost:5276"),
                new KeyValuePair<string, string?>("Kafka:BootstrapServers", "localhost:9092"),
                new KeyValuePair<string, string?>("Kafka:Producer:ClientId", "contratacao-service-test"),
                new KeyValuePair<string, string?>("Kafka:Topics:PropostaContratada", "proposta-contratada-test"),
                new KeyValuePair<string, string?>("Kafka:Topics:PropostaContratadaDlq", "proposta-contratada-test.dlq"),
                new KeyValuePair<string, string?>("IS_INTEGRATION_TEST", "true"),
                new KeyValuePair<string, string?>("ASPNETCORE_ENVIRONMENT", "Test")
            ]);
        });

        return base.CreateHost(builder);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
}
