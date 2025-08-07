using MediatR;
using Core.PropostaAggregate;
using Ardalis.Result;

namespace Applications.UseCases.Contratacoes.Commands;

public class ContratarPropostaCommand : IRequest<Result>
{
    public int PropostaId { get; set; }
} 