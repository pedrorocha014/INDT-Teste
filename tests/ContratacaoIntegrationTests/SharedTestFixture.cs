using Bogus;
using Infrastructure.Context;
using Microsoft.Extensions.DependencyInjection;
using ContratacaoIntegrationTests.Helpers;

namespace ContratacaoIntegrationTests;

public class SharedTestFixture : IDisposable
{
    private readonly CustomWebApiFactory<ContratacaoServiceWebApi.Program> _apiFactory;

    public readonly HttpClient HttpClient;
    public readonly Faker SharedFaker;
    public readonly IServiceScope ServiceScope;
    public readonly PostgresDbContext PostgresDbContext;

    public SharedTestFixture()
    {
        _apiFactory = new CustomWebApiFactory<ContratacaoServiceWebApi.Program>();
        HttpClient = _apiFactory.CreateClient();

        SharedFaker = new Faker("pt_BR");
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
