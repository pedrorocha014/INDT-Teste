using Infrastructure.Context;
using Microsoft.Extensions.DependencyInjection;
using PropostaIntegrationTests.Helpers;

namespace PropostaIntegrationTests;

public class SharedTestFixture : IDisposable
{
    private readonly CustomWebApiFactory<PropostaServiceWebApi.Program> _apiFactory;

    public readonly HttpClient HttpClient;
    public readonly IServiceScope ServiceScope;

    public readonly PostgresDbContext PostgresDbContext;

    public SharedTestFixture()
    {
        _apiFactory = new CustomWebApiFactory<PropostaServiceWebApi.Program>();
        HttpClient = _apiFactory.CreateClient();
        ServiceScope = _apiFactory.Services.CreateScope();

        PostgresDbContext = ServiceScope.ServiceProvider.GetRequiredService<PostgresDbContext>();
        PostgresHelper.SetContext(ServiceScope);
    }

    public void Dispose()
    {
        _apiFactory.Dispose();
        ServiceScope.Dispose();
        HttpClient.Dispose();
        PostgresDbContext.Dispose();
        GC.SuppressFinalize(this);
    }
}

[CollectionDefinition(nameof(SharedTestCollection))]
public class SharedTestCollection : ICollectionFixture<SharedTestFixture>;