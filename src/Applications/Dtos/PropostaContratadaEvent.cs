namespace Applications.Dtos;

public record PropostaContratadaEvent
{
    public int PropostaId { get; set; }
    public DateTimeOffset DataContratacao { get; set; }
} 