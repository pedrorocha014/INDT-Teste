using MediatR;

namespace Applications.UseCases.Contratacoes.Commands;

public class CreateContratoCommand : IRequest<bool>
{
    public int PropostaId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}