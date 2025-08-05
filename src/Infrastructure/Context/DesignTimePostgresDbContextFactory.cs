using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Context;

public class DesignTimePostgresDbContextFactory : IDesignTimeDbContextFactory<PostgresDbContext>
{
    public PostgresDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PostgresDbContext>();
        var connectionString = "Server=localhost;Port=5433;Database=indt_db;Username=postgres;Password=postgres";

        optionsBuilder.UseNpgsql(connectionString);
        return new PostgresDbContext(optionsBuilder.Options);
    }
}