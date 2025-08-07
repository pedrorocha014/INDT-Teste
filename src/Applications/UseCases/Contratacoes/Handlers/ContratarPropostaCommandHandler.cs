using Applications.UseCases.Contratacoes.Commands;
using Infrastructure.Context;
using MediatR;
using Core.PropostaAggregate;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Applications.Services;
using Applications.Dtos;
using Applications.Configs;

namespace Applications.UseCases.Contratacoes.Handlers;

public class ContratarPropostaCommandHandler : IRequestHandler<ContratarPropostaCommand>
{
    private readonly ILogger<ContratarPropostaCommandHandler> _logger;
    private readonly HttpClient _httpClient;
    private readonly IKafkaProducerService _kafkaProducerService;
    private readonly ServiceConfig _serviceConfig;

    public ContratarPropostaCommandHandler(
        ILogger<ContratarPropostaCommandHandler> logger,
        HttpClient httpClient,
        IKafkaProducerService kafkaProducerService,
        ServiceConfig serviceConfig)
    {
        _logger = logger;
        _httpClient = httpClient;
        _kafkaProducerService = kafkaProducerService;
        _serviceConfig = serviceConfig;
    }

    public async Task Handle(ContratarPropostaCommand request, CancellationToken cancellationToken)
    {
        var propostaExterna = await ConsultarPropostaExterna(request.PropostaId, cancellationToken);

        if (propostaExterna != null)
        {
            if (propostaExterna.Status != "Aprovada")
            {
                throw new InvalidOperationException($"Proposta {request.PropostaId} não está aprovada. Status atual: {propostaExterna.Status}");
            }

            var evento = new PropostaContratadaEvent
            {
                PropostaId = propostaExterna.Id,
                DataContratacao = DateTimeOffset.UtcNow
            };

            await _kafkaProducerService.EnviarMensagemAsync("proposta-contratada", evento, cancellationToken);

            _logger.LogInformation("Proposta {PropostaId} contratada e evento enviado para o Kafka", request.PropostaId);

            return;
        }

        throw new InvalidOperationException($"Proposta com ID {request.PropostaId} não encontrada na API externa");
    }

    private async Task<PropostaExternaResponse?> ConsultarPropostaExterna(int propostaId, CancellationToken cancellationToken)
    {
        try
        {
            var url = $"{_serviceConfig.PropostaService.BaseUrl}/api/proposta/{propostaId}";
            
            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Erro na API externa: {StatusCode} para proposta {PropostaId}", response.StatusCode, propostaId);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var propostaExterna = JsonConvert.DeserializeObject<PropostaExternaResponse>(content);

            _logger.LogInformation("Proposta {PropostaId} consultada com sucesso na API externa", propostaId);

            return propostaExterna;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar API externa para proposta {PropostaId}", propostaId);
            throw;
        }
    }
}

public class PropostaExternaResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
