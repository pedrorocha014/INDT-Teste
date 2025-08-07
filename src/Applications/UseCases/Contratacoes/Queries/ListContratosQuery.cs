using Core.PropostaAggregate;
using MediatR;

namespace Applications.UseCases.Contratacoes.Queries;

public class ListContratosQuery : IRequest<IEnumerable<Contratacao>>
{
} 