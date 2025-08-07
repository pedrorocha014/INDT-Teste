using MediatR;
using Ardalis.Result;

namespace Applications.UseCases.Contratacoes.Commands;

public class CreateContratoCommand : IRequest<Result>
{
    public int PropostaId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}