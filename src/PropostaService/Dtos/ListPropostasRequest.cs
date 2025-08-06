namespace PropostaService.Dtos;

public record ListPropostasRequest
{
    public string? Name {  get; set; }
    public string? Cpf { get; set; }
    public string? Status { get; set; }
} 