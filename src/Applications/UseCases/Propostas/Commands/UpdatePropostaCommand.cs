using Core.PropostaAggregate;
using Core.PropostaAggregate.Enums;
using MediatR;

namespace Applications.UseCases.Propostas.Commands;

public record UpdatePropostaCommand : IRequest<PropostaSeguro>
{
    public required int PropostaId { get; init; }
    public required StatusProposta Status { get; init; }
}