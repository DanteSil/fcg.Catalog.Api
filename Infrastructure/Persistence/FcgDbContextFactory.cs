using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Fcg.Infrastructure.Persistence;

public class FcgDbContextFactory : IDesignTimeDbContextFactory<FcgDbContext>
{
    public FcgDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' não configurada. Defina em appsettings ou variáveis de ambiente.");

        var optionsBuilder = new DbContextOptionsBuilder<FcgDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        return new FcgDbContext(optionsBuilder.Options);
    }
}
