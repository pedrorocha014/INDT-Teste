using Applications.Dtos;

namespace ContratacaoWorker.Services;

public interface IPropostaContratadaHandler
{
    Task HandleAsync(PropostaContratadaEvent evento, CancellationToken cancellationToken);
} 