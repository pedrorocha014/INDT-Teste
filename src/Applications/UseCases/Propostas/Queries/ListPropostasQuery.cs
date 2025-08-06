using Core.PropostaAggregate;
using Core.PropostaAggregate.Enums;
using MediatR;

namespace Applications.UseCases.Propostas.Queries;

public class ListPropostasQuery : IRequest<IEnumerable<PropostaSeguro>>
{
    public string? Name { get; set; }
    public string? Cpf { get; set; }
    public StatusProposta? Status { get; set; }
} 