using MediatR;
using Core.PropostaAggregate;

namespace Applications.UseCases.Contratacoes.Commands;

public class ContratarPropostaCommand : IRequest
{
    public int PropostaId { get; set; }
} 