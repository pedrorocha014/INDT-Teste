using Core.PropostaAggregate.Enums;

namespace Core.PropostaAggregate;

public class PropostaSeguro{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Cpf { get; set; }
    public StatusProposta Status { get; set; } = StatusProposta.EmAnalise;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; init; } = DateTimeOffset.UtcNow;
}
