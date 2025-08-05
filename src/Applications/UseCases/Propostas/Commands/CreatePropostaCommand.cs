using MediatR;
using Core.PropostaAggregate;

namespace Applications.UseCases.Propostas.Commands;

public record CreatePropostaCommand : IRequest<PropostaSeguro>
{
    public required string Name { get; init; }
    public required string Cpf { get; init; }
} 