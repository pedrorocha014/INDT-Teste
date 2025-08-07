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

        // Captura o argumento --connection
        var connectionStringIndex = Array.FindIndex(args, a => a == "--connection");
        if (connectionStringIndex == -1 || connectionStringIndex + 1 >= args.Length)
        {
            throw new InvalidOperationException("Connection string not provided. Use --connection \"...\"");
        }

        var connectionString = args[connectionStringIndex + 1];

        optionsBuilder.UseNpgsql(connectionString);

        return new PostgresDbContext(optionsBuilder.Options);
    }
}