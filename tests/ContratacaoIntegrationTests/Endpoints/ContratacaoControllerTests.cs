using System.Net;
using System.Text;
using System.Text.Json;
using ContratacaoIntegrationTests.Helpers;
using ContratacaoService.Dtos;
using WireMock.Server;

namespace ContratacaoIntegrationTests.Endpoints;

[Collection(nameof(SharedTestCollection))]
public class ContratacaoControllerTests : IClassFixture<SharedTestFixture>, IDisposable
{
    private readonly SharedTestFixture _fixture;
    private WireMockServer server;

    public ContratacaoControllerTests(SharedTestFixture fixture)
    {
        _fixture = fixture;
        server = WireMockServer.Start(5276);
    }
    
    [Fact]
    public async Task ContratarProposta_ComPropostaAprovada_DeveRetornarSucesso()
    {
        // Arrange
        var propostaId = 1;

        server.Given(WireMock.RequestBuilders.Request.Create()
            .WithPath($"/api/proposta/{propostaId}")
            .UsingGet())
            .RespondWith(WireMock.ResponseBuilders.Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody($$"""
                        {
                            "id": {{propostaId}},
                            "status": "Aprovada",
                            "cliente": "Cliente Teste",
                            "createdAt": "2024-01-01T00:00:00Z",
                            "updatedAt": "2024-01-01T00:00:00Z"
                        }
                        """));

        var request = new ContratarPropostaRequest { PropostaId = propostaId };
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _fixture.HttpClient.PostAsync("/api/contratacao", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    public void Dispose()
    {
        _fixture.Dispose();
        server.Stop();
    }
}