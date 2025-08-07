namespace Infrastructure.Configs;

public class KafkaConfig
{
    public string BootstrapServers { get; set; } = string.Empty;
    public ConsumerConfig Consumer { get; set; } = new();
    public ProducerConfig Producer { get; set; } = new();
    public TopicConfig Topics { get; set; } = new();
}

public class ConsumerConfig
{
    public string GroupId { get; set; } = string.Empty;
    public string AutoOffsetReset { get; set; } = "Earliest";
    public bool EnableAutoCommit { get; set; } = false;
}

public class ProducerConfig
{
    public string ClientId { get; set; } = string.Empty;
}

public class TopicConfig
{
    public string PropostaContratada { get; set; } = "proposta-contratada";
    public string PropostaContratadaDlq { get; set; } = "proposta-contratada.dlq";
}
