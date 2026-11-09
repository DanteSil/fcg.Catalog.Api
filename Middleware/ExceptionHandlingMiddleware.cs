using System.Net;
using FluentValidation;
using Fcg.CatalogAPI.Models;
using Fcg.Application.Common;
using Fcg.Domain.Common;

namespace Fcg.CatalogAPI.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (status, message, errors) = exception switch
        {
            ValidationException validation => (
                HttpStatusCode.BadRequest,
                "Dados inválidos.",
                validation.Errors.Select(e => e.ErrorMessage).ToArray()),
            DomainException domain => (HttpStatusCode.BadRequest, domain.Message, Array.Empty<string>()),
            NotFoundException notFound => (HttpStatusCode.NotFound, notFound.Message, Array.Empty<string>()),
            ConflictException conflict => (HttpStatusCode.Conflict, conflict.Message, Array.Empty<string>()),
            UnauthorizedAppException unauthorized => (HttpStatusCode.Unauthorized, unauthorized.Message, Array.Empty<string>()),
            ForbiddenException forbidden => (HttpStatusCode.Forbidden, forbidden.Message, Array.Empty<string>()),
            UnauthorizedAccessException ua => (HttpStatusCode.Unauthorized, ua.Message, Array.Empty<string>()),
            _ => (HttpStatusCode.InternalServerError, "Erro interno inesperado.", Array.Empty<string>())
        };

        if (status == HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Unhandled exception");
        else
            _logger.LogWarning(exception, "Handled exception: {Message}", message);

        context.Response.StatusCode = (int)status;
        await context.Response.WriteAsJsonAsync(BaseResponse.Failure((int)status, message, errors));
    }
}
