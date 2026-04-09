using System.Text.Json;
using DeviceManagement.Api.Middleware;
using DeviceManagement.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;

namespace DeviceManagement.Tests.Unit.Middleware;

public sealed class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ReturnsValidationErrorPayload()
    {
        var middleware = CreateMiddleware(
            _ => throw new ValidationException("Invalid input."),
            isDevelopment: true);
        var ctx = new DefaultHttpContext();
        ctx.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(ctx);

        Assert.Equal(400, ctx.Response.StatusCode);
        Assert.Equal("application/json", ctx.Response.ContentType);

        ctx.Response.Body.Position = 0;
        var body = await new StreamReader(ctx.Response.Body).ReadToEndAsync();
        using var doc = JsonDocument.Parse(body);
        Assert.Equal("Invalid input.", doc.RootElement.GetProperty("message").GetString());
    }

    [Fact]
    public async Task InvokeAsync_ReturnsGenericMessage_InProduction_ForUnknownException()
    {
        var middleware = CreateMiddleware(
            _ => throw new InvalidOperationException("Sensitive details"),
            isDevelopment: false);
        var ctx = new DefaultHttpContext();
        ctx.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(ctx);

        Assert.Equal(500, ctx.Response.StatusCode);
        ctx.Response.Body.Position = 0;
        var body = await new StreamReader(ctx.Response.Body).ReadToEndAsync();
        using var doc = JsonDocument.Parse(body);
        Assert.Equal("An unexpected error occurred.", doc.RootElement.GetProperty("message").GetString());
    }

    [Fact]
    public async Task InvokeAsync_ReturnsExceptionMessage_InDevelopment_ForUnknownException()
    {
        var middleware = CreateMiddleware(
            _ => throw new InvalidOperationException("Detailed failure"),
            isDevelopment: true);
        var ctx = new DefaultHttpContext();
        ctx.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(ctx);

        Assert.Equal(500, ctx.Response.StatusCode);
        ctx.Response.Body.Position = 0;
        var body = await new StreamReader(ctx.Response.Body).ReadToEndAsync();
        using var doc = JsonDocument.Parse(body);
        Assert.Equal("Detailed failure", doc.RootElement.GetProperty("message").GetString());
    }

    private static ExceptionHandlingMiddleware CreateMiddleware(
        RequestDelegate next,
        bool isDevelopment)
    {
        return new ExceptionHandlingMiddleware(
            next,
            NullLogger<ExceptionHandlingMiddleware>.Instance,
            new FakeHostEnvironment(isDevelopment));
    }

    private sealed class FakeHostEnvironment : IHostEnvironment
    {
        public FakeHostEnvironment(bool isDevelopment)
        {
            EnvironmentName = isDevelopment ? "Development" : "Production";
        }

        public string EnvironmentName { get; set; }
        public string ApplicationName { get; set; } = "DeviceManagement.Tests.Unit";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
