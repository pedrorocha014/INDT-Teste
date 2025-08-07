using System.Net;
using System.Net.Http.Json;
using Bogus;
using Bogus.Extensions.Brazil;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using PropostaService.Dtos;
using PropostaResponse = PropostaService.Dtos.PropostaResponse;

namespace PropostaIntegrationTests.Endpoints;

[Collection(nameof(SharedTestCollection))]
public class PropostaControllerTests : IClassFixture<SharedTestFixture>
{
    private readonly SharedTestFixture _fixture;
    private readonly HttpClient _httpClient;
    private readonly PostgresDbContext _dbContext;
    public Faker _faker;

    public PropostaControllerTests(SharedTestFixture fixture)
    {
        _fixture = fixture;
        _httpClient = fixture.HttpClient;
        _dbContext = fixture.PostgresDbContext;
        _faker = new Faker("pt_BR");
    }

    [Fact]
    public async Task List_DeveRetornarListaDePropostas()
    {
        // Arrange
        await _dbContext.PropostasSeguro.AddAsync(
            new Core.PropostaAggregate.PropostaSeguro
            {
                Cpf = _faker.Person.Cpf(),
                Name = _faker.Person.FullName,
                Status = Core.PropostaAggregate.Enums.StatusProposta.EmAnalise
            }
        );
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _httpClient.GetAsync("/api/propostas");
        var propostas = await response.Content.ReadFromJsonAsync<List<PropostaResponse>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(propostas);
        Assert.IsType<List<PropostaResponse>>(propostas);
    }

    [Fact]
    public async Task GetById_DeveRetornarPropostaCorreta()
    {
        // Arrange
        var proposta = new Core.PropostaAggregate.PropostaSeguro
        {
            Cpf = _faker.Person.Cpf(),
            Name = _faker.Person.FullName,
            Status = Core.PropostaAggregate.Enums.StatusProposta.EmAnalise
        };
        await _dbContext.PropostasSeguro.AddAsync(proposta);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _httpClient.GetAsync($"/api/propostas/{proposta.Id}");
        var propostaResponse = await response.Content.ReadFromJsonAsync<PropostaResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(propostaResponse);
        Assert.IsType<PropostaResponse>(propostaResponse);
    }

    [Fact]
    public async Task Post_DeveCriarPropostaNoBanco()
    {
        // Arrange
        var request = new CreatePropostaRequest
        {
            Cpf = _faker.Person.Cpf(),
            Name = _faker.Person.FullName
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync($"/api/propostas", request);

        var propostaSalva = _dbContext.PropostasSeguro.First(p => p.Cpf == request.Cpf);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(propostaSalva);
        Assert.Equal(Core.PropostaAggregate.Enums.StatusProposta.EmAnalise, propostaSalva.Status);
    }

    [Fact]
    public async Task Update_DeveAtualizarStatusDaProposta()
    {
        // Arrange
        var proposta = new Core.PropostaAggregate.PropostaSeguro
        {
            Cpf = _faker.Person.Cpf(),
            Name = _faker.Person.FullName,
            Status = Core.PropostaAggregate.Enums.StatusProposta.EmAnalise
        };
        await _dbContext.PropostasSeguro.AddAsync(proposta);
        await _dbContext.SaveChangesAsync();

        var request = new UpdatePropostaStatusRequest
        {
            Status = "Aprovada"
        };

        // Act
        var response = await _httpClient.PatchAsJsonAsync($"/api/propostas/{proposta.Id}", request);

        var propostaAtualizada = await _dbContext.PropostasSeguro
            .AsNoTracking()
            .FirstAsync(p => p.Id == proposta.Id);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.NotNull(propostaAtualizada);
        Assert.Equal(Core.PropostaAggregate.Enums.StatusProposta.Aprovada, propostaAtualizada.Status);
    }
}
