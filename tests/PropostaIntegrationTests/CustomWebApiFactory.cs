using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using PropostaIntegrationTests.Helpers;
using Microsoft.AspNetCore.Hosting;

namespace PropostaIntegrationTests;

public class CustomWebApiFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureHostConfiguration(config =>
        {
            config.AddInMemoryCollection([
                new KeyValuePair<string, string?>("ConnectionStrings:PostgresConnection", PostgresHelper.ConnectionString),
                new KeyValuePair<string, string?>("IS_INTEGRATION_TEST", "true"),
                new KeyValuePair<string, string?>("ASPNETCORE_ENVIRONMENT", "Test")
            ]);
        });

        return base.CreateHost(builder);
    }
}

public class HostApplicationFactory<TEntryPoint>(Action<IWebHostBuilder> configuration)
    : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddInMemoryCollection([
                new KeyValuePair<string, string?>("ConnectionStrings:PostgresConnection", PostgresHelper.ConnectionString),
                new KeyValuePair<string, string?>("IS_INTEGRATION_TEST", "true"),
                new KeyValuePair<string, string?>("ASPNETCORE_ENVIRONMENT", "Test")
            ]);
        });

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        configuration(builder.Configure(_ => { }));

    public Task RunHostAsync(CancellationToken cancellationToken)
    {
        var host = Services.GetRequiredService<IHost>();
        return host.WaitForShutdownAsync(cancellationToken);
    }
}
