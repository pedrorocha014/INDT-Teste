namespace Core.PropostaAggregate;

public class Contratacao
{
    public int Id { get; set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    public int PropostaId { get; set; }
    public PropostaSeguro? PropostaSeguro { get; set; }
}
