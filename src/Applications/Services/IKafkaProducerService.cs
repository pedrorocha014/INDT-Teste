namespace Applications.Services;

public interface IKafkaProducerService
{
    Task EnviarMensagemAsync<T>(string topico, T mensagem, CancellationToken cancellationToken = default);
} 