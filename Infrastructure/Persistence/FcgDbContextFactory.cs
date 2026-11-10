using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Fcg.Infrastructure.Persistence;

public class FcgDbContextFactory : IDesignTimeDbContextFactory<FcgDbContext>
{
    public FcgDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
            ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? Environments.Development;

        var builder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true);

        if (environment == Environments.Development)
            builder.AddUserSecrets<FcgDbContextFactory>(optional: true);

        var configuration = builder.AddEnvironmentVariables()
            .AddJsonFile($"appsettings.{environment}.local.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' não configurada. Use Segredos do Usuário ou ConnectionStrings__DefaultConnection.");

        var optionsBuilder = new DbContextOptionsBuilder<FcgDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        return new FcgDbContext(optionsBuilder.Options);
    }
}
