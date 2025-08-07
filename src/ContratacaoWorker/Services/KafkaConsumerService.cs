using Confluent.Kafka;
using Newtonsoft.Json;
using Applications.Dtos;
using Infrastructure.Configs;

namespace ContratacaoWorker.Services;

public class KafkaConsumerService : IKafkaConsumerService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly IProducer<string, string> _dlqProducer;
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public KafkaConsumerService(
        ILogger<KafkaConsumerService> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "contratacao-worker-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = "localhost:9092",
            ClientId = "contratacao-worker-dlq-producer"
        };

        _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        _dlqProducer = new ProducerBuilder<string, string>(producerConfig).Build();
    }

    public async Task StartConsumingAsync(CancellationToken cancellationToken)
    {
        try
        {
            _consumer.Subscribe(TopicNames.PropostaContratada);
            _logger.LogInformation("Iniciando consumo do tópico proposta-contratada");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(cancellationToken);
                    
                    if (consumeResult?.Message?.Value != null)
                    {
                        var result = await ProcessarMensagemAsync(consumeResult, cancellationToken);
                        
                        if (result)
                        {
                            _consumer.Commit(consumeResult);
                            _logger.LogInformation("Mensagem processada com sucesso e removida do tópico. Offset: {Offset}", consumeResult.Offset);
                        }
                        else
                        {
                            // Não faz commit, a mensagem permanecerá no tópico para reprocessamento
                            _logger.LogWarning("Mensagem não processada com sucesso, permanecerá no tópico. Offset: {Offset}", consumeResult.Offset);
                        }
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Erro ao consumir mensagem do Kafka");
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Consumo cancelado");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro inesperado ao processar mensagem");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao iniciar consumo do Kafka");
            throw;
        }
    }

    public Task StopConsumingAsync()
    {
        _consumer?.Close();
        _consumer?.Dispose();
        _dlqProducer?.Dispose();
        _logger.LogInformation("Consumo do Kafka parado");
        return Task.CompletedTask;
    }

    private async Task<bool> ProcessarMensagemAsync(ConsumeResult<string, string> consumeResult, CancellationToken cancellationToken)
    {
        try
        {
            var evento = JsonConvert.DeserializeObject<PropostaContratadaEvent>(consumeResult.Message.Value);
            
            if (evento != null)
            {
                // Criar um scope para o handler
                using var scope = _serviceScopeFactory.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<IPropostaContratadaHandler>();
                
                await handler.HandleAsync(evento, cancellationToken);
                _logger.LogInformation("Evento de proposta contratada processado: PropostaId {PropostaId}", evento.PropostaId);
                
                return true; // Processamento bem-sucedido
            }
            else
            {
                _logger.LogWarning("Mensagem inválida recebida: {Mensagem}", consumeResult.Message.Value);
                await SendToDlq(consumeResult, "Mensagem inválida - não foi possível deserializar");
                return false; // Processamento falhou
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Erro ao deserializar mensagem: {Mensagem}", consumeResult.Message.Value);
            await SendToDlq(consumeResult, $"Erro de deserialização: {ex.Message}");
            return false; // Processamento falhou
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar mensagem: {Mensagem}", consumeResult.Message.Value);
            await SendToDlq(consumeResult, $"Erro de processamento: {ex.Message}");
            return false; // Processamento falhou
        }
    }

    private async Task SendToDlq(ConsumeResult<string, string> consumeResult, string motivo)
    {
        try
        {
            var dlqMessage = new
            {
                OriginalMessage = consumeResult.Message.Value,
                OriginalTopic = consumeResult.Topic,
                OriginalPartition = consumeResult.Partition,
                OriginalOffset = consumeResult.Offset,
                Timestamp = DateTimeOffset.UtcNow,
                Motivo = motivo
            };

            var dlqJson = JsonConvert.SerializeObject(dlqMessage);
            var key = Guid.NewGuid().ToString();

            var result = await _dlqProducer.ProduceAsync(TopicNames.PropostaContratadaDlq, new Message<string, string>
            {
                Key = key,
                Value = dlqJson
            });

            _logger.LogInformation("Mensagem enviada para DLQ. Offset original: {Offset}, DLQ Offset: {DlqOffset}", 
                consumeResult.Offset, result.Offset);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar mensagem para DLQ. Offset original: {Offset}", consumeResult.Offset);
        }
    }
} 