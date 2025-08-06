using ContratacaoWorker.Services;

namespace ContratacaoWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IKafkaConsumerService _kafkaConsumerService;

    public Worker(ILogger<Worker> logger, IKafkaConsumerService kafkaConsumerService)
    {
        _logger = logger;
        _kafkaConsumerService = kafkaConsumerService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("ContratacaoWorker iniciado");
            
            // Iniciar o consumo do Kafka
            await _kafkaConsumerService.StartConsumingAsync(stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("ContratacaoWorker parado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no ContratacaoWorker");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Parando ContratacaoWorker");
        
        // Parar o consumo do Kafka
        await _kafkaConsumerService.StopConsumingAsync();
        
        await base.StopAsync(cancellationToken);
    }
}
