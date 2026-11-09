using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Fcg.CatalogAPI;
using Fcg.CatalogAPI.Extensions;
using Fcg.CatalogAPI.Middleware;
using Fcg.Application;
using Fcg.Infrastructure;
using Fcg.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(
    $"appsettings.{builder.Environment.EnvironmentName}.local.json",
    optional: true,
    reloadOnChange: true);

ConfigureSerilog(builder);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FIAP Cloud Games CatalogAPI",
        Version = "v1",
        Description = "CatalogAPI da FIAP Cloud Games"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "CatalogAPI da FIAP Cloud Games"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApiAuthenticationResponses();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseStatusCodePages(async statusCodeContext =>
{
    if (statusCodeContext.HttpContext.Response.StatusCode == StatusCodes.Status404NotFound)
        await ApiResponseWriter.WriteErrorAsync(statusCodeContext.HttpContext, StatusCodes.Status404NotFound, "Recurso não encontrado.");
});
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<FcgDbContext>();
    await db.Database.MigrateAsync();
}
catch (Exception ex)
{
    Log.Warning(ex, "Falha ao aplicar migrations/seed. Verifique a connection string do Postgres.");
}

app.Run();

static void ConfigureSerilog(WebApplicationBuilder builder)
{
    var seqSection = builder.Configuration.GetSection("Seq");
    var serverUrl = seqSection["ServerUrl"];
    var apiKey = seqSection["ApiKey"];
    var minimumLevel = Enum.TryParse<LogEventLevel>(seqSection["MinimumLevel"], true, out var level)
        ? level
        : LogEventLevel.Information;

    var loggerConfig = new LoggerConfiguration()
        .MinimumLevel.Is(minimumLevel)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "CatalogAPI");

    var overrides = seqSection.GetSection("LevelOverride").GetChildren();
    foreach (var item in overrides)
    {
        if (Enum.TryParse<LogEventLevel>(item.Value, true, out var overrideLevel))
            loggerConfig.MinimumLevel.Override(item.Key, overrideLevel);
    }

    if (builder.Environment.IsDevelopment())
        loggerConfig.WriteTo.Console();

    if (!string.IsNullOrWhiteSpace(serverUrl))
        loggerConfig.WriteTo.Seq(serverUrl, apiKey: string.IsNullOrWhiteSpace(apiKey) ? null : apiKey);

    Log.Logger = loggerConfig.CreateLogger();
    builder.Host.UseSerilog();
}

public partial class Program;
