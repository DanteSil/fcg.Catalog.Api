using Fcg.CatalogAPI;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Fcg.CatalogAPI.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddApiAuthenticationResponses(this IServiceCollection services)
    {
        services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.Events = new JwtBearerEvents
            {
                OnChallenge = async context =>
                {
                    context.HandleResponse();

                    if (!context.Response.HasStarted)
                        await ApiResponseWriter.WriteErrorAsync(context.HttpContext, StatusCodes.Status401Unauthorized, "Não autenticado.");
                },
                OnForbidden = async context =>
                {
                    if (!context.Response.HasStarted)
                        await ApiResponseWriter.WriteErrorAsync(context.HttpContext, StatusCodes.Status403Forbidden, "Não autorizado.");
                }
            };
        });

        return services;
    }
}
