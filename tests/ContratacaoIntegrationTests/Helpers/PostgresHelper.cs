using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace ContratacaoIntegrationTests.Helpers;

public static class PostgresHelper
{
    public static PostgresDbContext? Context { get; private set; }

    public const string ConnectionString =
       "Server=localhost;Port=5434;Database=indt_test_db;Username=postgres;Password=postgres";
    private static Respawner? _respawner;

    public static async Task<bool> ResetAsync()
    {
        await using NpgsqlConnection connection = new(ConnectionString);
        await connection.OpenAsync();

        _respawner ??= await Respawner.CreateAsync(connection,
            new RespawnerOptions { DbAdapter = DbAdapter.Postgres, TablesToIgnore = ["VersionInfo"] });

        await _respawner.ResetAsync(connection);

        return true;
    }

    public static void SetContext(IServiceScope serviceScope)
    {
        Context ??= serviceScope.ServiceProvider.GetRequiredService<PostgresDbContext>();
    }

    [Pure]
    public static async Task<T?> FetchEntityWithTimeoutAsync<T>(
       DbSet<T> dbSet,
       Expression<Func<T, bool>> predicate,
       TimeSpan? timeout = null,
       TimeSpan? interval = null
    )
       where T : class
    {
        timeout ??= TimeSpan.FromSeconds(14);
        interval ??= TimeSpan.FromSeconds(2);
        DateTimeOffset endTime = DateTimeOffset.UtcNow.Add(timeout.Value);

        while (DateTimeOffset.UtcNow < endTime)
        {
            T? entity = await
                dbSet
                    .Where(predicate)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

            if (entity != null)
                return entity;

            await Task.Delay(interval.Value);
        }

        return null;
    }

    [Pure]
    public static async Task<T?> FetchMutableEntityWithTimeoutAsync<T>(
        DbSet<T> dbSet,
        Expression<Func<T, bool>> predicate,
        TimeSpan? timeout = null,
        TimeSpan? interval = null
    )
        where T : class
    {
        timeout ??= TimeSpan.FromSeconds(14);
        interval ??= TimeSpan.FromSeconds(2);
        DateTimeOffset endTime = DateTimeOffset.UtcNow.Add(timeout.Value);

        while (DateTimeOffset.UtcNow < endTime)
        {
            T? entity = await
                dbSet
                    .Where(predicate)
                    .FirstOrDefaultAsync();

            if (entity != null)
                return entity;

            await Task.Delay(interval.Value);
        }

        return null;
    }
}
