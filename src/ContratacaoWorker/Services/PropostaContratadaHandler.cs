using MediatR;
using Applications.UseCases.Contratacoes.Commands;

namespace ContratacaoWorker.Services;

public class PropostaContratadaHandler : IPropostaContratadaHandler
{
    private readonly ILogger<PropostaContratadaHandler> _logger;
    private readonly IMediator _mediator;

    public PropostaContratadaHandler(
        ILogger<PropostaContratadaHandler> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task HandleAsync(Applications.Dtos.PropostaContratadaEvent evento, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processando evento de proposta contratada: PropostaId {PropostaId}", evento.PropostaId);

            var command = new CreateContratoCommand { PropostaId = evento.PropostaId, CreatedAt = evento.DataContratacao };

            var result = await _mediator.Send(command);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar evento de proposta contratada: PropostaId {PropostaId}", evento.PropostaId);
            throw;
        }
    }
} 