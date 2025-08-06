using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Applications.Services;

public class KafkaProducerService : IKafkaProducerService
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducerService> _logger;

    public KafkaProducerService(ILogger<KafkaProducerService> logger)
    {
        _logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = "localhost:9092",
            ClientId = "contratacao-service-producer"
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task EnviarMensagemAsync<T>(string topico, T mensagem, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonConvert.SerializeObject(mensagem);
            var key = Guid.NewGuid().ToString();

            var result = await _producer.ProduceAsync(topico, new Message<string, string>
            {
                Key = key,
                Value = json
            }, cancellationToken);

            _logger.LogInformation("Mensagem enviada para o tópico {Topico} com sucesso. Offset: {Offset}", 
                topico, result.Offset);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar mensagem para o tópico {Topico}", topico);
            throw;
        }
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }
} 