using Microsoft.EntityFrameworkCore;
using Core.PropostaAggregate;
using Npgsql;
using Npgsql.NameTranslation;
using System.Globalization;
using Core.PropostaAggregate.Enums;
using Infrastructure.EfEntityConfig;

namespace Infrastructure.Context;

public class PostgresDbContext : DbContext
{
    public const string CurrentSchema = "public";

    public DbSet<PropostaSeguro> PropostasSeguro { get; set; }

    public PostgresDbContext(DbContextOptions<PostgresDbContext> options)
    : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
        => builder.HasPostgresEnum<StatusProposta>();
}
