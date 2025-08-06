using Core.PropostaAggregate.Enums;
using System.Text.Json.Serialization;

namespace PropostaService.Dtos;

public record PropostaResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Cpf { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public StatusProposta Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
} 