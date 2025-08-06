namespace PropostaService.Dtos;

public record UpdatePropostaStatusRequest
{
    public required string Status { get; set; }
} 