using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Core.PropostaAggregate.Enums;
using Npgsql;
using Infrastructure.Context;
using Npgsql.NameTranslation;
using System.Globalization;

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

        services.AddSingleton<NpgsqlDataSource>(_ =>
        {
            NpgsqlDataSourceBuilder dataSourceBuilder = new(postgresConnectionString);
            AddEnum<StatusProposta>();

            return dataSourceBuilder.Build();

            void AddEnum<TEnum>(INpgsqlNameTranslator? nameTranslator = null) where TEnum : struct, Enum
            {
                dataSourceBuilder.MapEnum<TEnum>(
                    pgName: NpgsqlSnakeCaseNameTranslator.ConvertToSnakeCase(typeof(TEnum).Name, CultureInfo.InvariantCulture),
                    nameTranslator
                );
            }
        });

        services.AddDbContext<PostgresDbContext>((sp, options) =>
            options.UseNpgsql(sp.GetRequiredService<NpgsqlDataSource>(),
                builder => builder.MigrationsAssembly("Migrations")
                    .MigrationsHistoryTable("__EFMigrationsHistory_" + PostgresDbContext.CurrentSchema,
                        PostgresDbContext.CurrentSchema)));
    }
}
