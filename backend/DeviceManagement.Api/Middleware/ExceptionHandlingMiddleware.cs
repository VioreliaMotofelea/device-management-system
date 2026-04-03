using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using DeviceManagement.Application.Exceptions;

namespace DeviceManagement.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            if (context.Response.HasStarted)
            {
                _logger.LogError(ex, "Response had already started; cannot write error body.");
                throw;
            }

            await HandleAsync(context, ex);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception ex)
    {
        var (statusCode, message, logLevel) = MapException(ex);

        Log(ex, logLevel, statusCode, message);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = MediaTypeNames.Application.Json;

        var body = new ErrorResponse(message, context.TraceIdentifier);
        await context.Response.WriteAsync(JsonSerializer.Serialize(body, JsonOptions));
    }

    private (int statusCode, string message, LogLevel logLevel) MapException(Exception ex)
    {
        return ex switch
        {
            AppException app => (app.StatusCode, app.Message, app.StatusCode >= 500 ? LogLevel.Error : LogLevel.Debug),
            ArgumentException arg => (400, arg.Message, LogLevel.Debug),
            _ => (
                500,
                _environment.IsDevelopment() ? ex.Message : "An unexpected error occurred.",
                LogLevel.Error)
        };
    }

    private void Log(Exception ex, LogLevel level, int statusCode, string clientMessage)
    {
        if (level == LogLevel.Error)
            _logger.LogError(ex, "Request failed with status {StatusCode}: {Message}", statusCode, clientMessage);
        else
            _logger.LogDebug(ex, "Request failed with status {StatusCode}: {Message}", statusCode, clientMessage);
    }

    private sealed record ErrorResponse(string Message, string? TraceId);
}
