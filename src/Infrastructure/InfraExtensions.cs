using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Core.PropostaAggregate.Enums;
using Npgsql;
using Infrastructure.Context;
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

    public static PropertyBuilder<TProperty> HasEnumType<TProperty>(this PropertyBuilder<TProperty> builder, string? schema = PostgresDbContext.CurrentSchema)
    {
        Type enumType = typeof(TProperty);
        Type? underlyingType = enumType.IsGenericType && enumType.GetGenericTypeDefinition() == typeof(Nullable<>)
            ? Nullable.GetUnderlyingType(enumType)
            : enumType;

        return builder.HasColumnType(schema + '.' + NpgsqlSnakeCaseNameTranslator.ConvertToSnakeCase(underlyingType.Name,CultureInfo.InvariantCulture));
    }
}
