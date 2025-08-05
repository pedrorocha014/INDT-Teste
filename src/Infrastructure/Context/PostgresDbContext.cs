using Microsoft.EntityFrameworkCore;
using Core.PropostaAggregate;

namespace Infrastructure.Context;

public class PostgresDbContext : DbContext
{
    public const string CurrentSchema = "public";

    public DbSet<PropostaSeguro> PropostasSeguro { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Server=localhost;Port=5433;Database=indt_db;Username=postgres;Password=postgres");
}
