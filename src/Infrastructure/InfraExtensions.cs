using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Core.PropostaAggregate.Enums;
using Npgsql;
using Infrastructure.Context;
using Infrastructure.Configs;
using Npgsql.NameTranslation;
using System.Globalization;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure;

public static class InfraExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContexts(configuration);
        services.AddKafkaConfig(configuration);

        return services;
    }

    private static void AddDbContexts(this IServiceCollection services, IConfiguration config)
    {
        string? postgresConnectionString = config.GetConnectionString("PostgresConnection");

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(postgresConnectionString);
        dataSourceBuilder.MapEnum<StatusProposta>();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<PostgresDbContext>(options => options.UseNpgsql(dataSource));
    }

    private static void AddKafkaConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var kafkaConfig = new KafkaConfig
        {
            BootstrapServers = configuration.GetSection("Kafka:BootstrapServers").Value ?? "localhost:9092",
            Consumer = new ConsumerConfig
            {
                GroupId = configuration.GetSection("Kafka:Consumer:GroupId").Value ?? "contratacao-worker-group",
                AutoOffsetReset = configuration.GetSection("Kafka:Consumer:AutoOffsetReset").Value ?? "Earliest",
                EnableAutoCommit = bool.Parse(configuration.GetSection("Kafka:Consumer:EnableAutoCommit").Value ?? "false")
            },
            Producer = new ProducerConfig
            {
                ClientId = configuration.GetSection("Kafka:Producer:ClientId").Value ?? "contratacao-service-producer"
            },
            Topics = new TopicConfig
            {
                PropostaContratada = configuration.GetSection("Kafka:Topics:PropostaContratada").Value ?? "proposta-contratada",
                PropostaContratadaDlq = configuration.GetSection("Kafka:Topics:PropostaContratadaDlq").Value ?? "proposta-contratada.dlq"
            }
        };

        services.AddSingleton(kafkaConfig);
    }

    public static PropertyBuilder<TProperty> HasEnumType<TProperty>(this PropertyBuilder<TProperty> builder, string? schema = PostgresDbContext.CurrentSchema)
    {
        Type enumType = typeof(TProperty);
        Type? underlyingType = enumType.IsGenericType && enumType.GetGenericTypeDefinition() == typeof(Nullable<>)
            ? Nullable.GetUnderlyingType(enumType)
            : enumType;

        return builder.HasColumnType(schema + '.' + NpgsqlSnakeCaseNameTranslator.ConvertToSnakeCase(underlyingType.Name,CultureInfo.InvariantCulture));
    }
}
