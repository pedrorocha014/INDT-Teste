namespace ContratacaoService.Dtos;

public class ContratoResponse
{
    public int Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public int PropostaId { get; set; }
    public PropostaResponse? PropostaSeguro { get; set; }
}

public class PropostaResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
} 