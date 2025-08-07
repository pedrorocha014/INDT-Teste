using Core.PropostaAggregate;
using MediatR;
using Ardalis.Result;

namespace Applications.UseCases.Contratacoes.Queries;

public class ListContratosQuery : IRequest<Result<IEnumerable<Contratacao>>>
{
} 