using System.Net;
using System.Text.Json;

namespace ApiClientes.Infrastructure.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var message = "Erro interno do servidor";
        var errors = new List<string>();

        switch (exception)
        {
            case ArgumentException argEx:
                code = HttpStatusCode.BadRequest;
                message = "Erro de validação";
                errors.Add(argEx.Message);
                break;

            case InvalidOperationException invOpEx:
                code = HttpStatusCode.BadRequest;
                message = "Operação inválida";
                errors.Add(invOpEx.Message);
                break;

            case KeyNotFoundException:
                code = HttpStatusCode.NotFound;
                message = "Recurso não encontrado";
                errors.Add(exception.Message);
                break;

            default:
                errors.Add(message);
                break;
        }

        var result = JsonSerializer.Serialize(new
        {
            error = message,
            errors,
            details = exception.Message
        });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        return context.Response.WriteAsync(result);
    }
}