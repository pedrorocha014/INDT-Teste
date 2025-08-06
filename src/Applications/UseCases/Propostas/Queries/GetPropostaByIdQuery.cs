using Core.PropostaAggregate;
using MediatR;

namespace Applications.UseCases.Propostas.Queries;

public class GetPropostaByIdQuery : IRequest<PropostaSeguro?>
{
    public int Id { get; set; }
} 