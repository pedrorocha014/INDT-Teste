namespace PropostaService.Dtos;

public record CreatePropostaRequest
{
    public required string Name { get; init; }
    public required string Cpf { get; init; }
}