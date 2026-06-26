using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using testNet.API.Middleware;
using testNet.Domain.Exceptions;

namespace testNet.Tests.API;

public class ExceptionMiddlewareTests
{
    private static (ExceptionMiddleware middleware, DefaultHttpContext context) CreateMiddleware(Func<HttpContext, Task> next)
    {
        var context = new DefaultHttpContext();
        var bodyStream = new MemoryStream();
        context.Response.Body = bodyStream;
        var logger = NullLogger<ExceptionMiddleware>.Instance;
        var middleware = new ExceptionMiddleware(_ => next(context), logger);
        return (middleware, context);
    }

    private static async Task<(HttpStatusCode statusCode, JsonDocument body)> ReadResponse(HttpContext context)
    {
        context.Response.Body.Position = 0;
        var doc = await JsonDocument.ParseAsync(context.Response.Body);
        return ((HttpStatusCode)context.Response.StatusCode, doc);
    }

    [Fact]
    public async Task InvokeAsync_WhenNoException_ShouldCallNextAndNotAlterResponse()
    {
        var (middleware, context) = CreateMiddleware(ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status200OK;
            return Task.CompletedTask;
        });

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task InvokeAsync_WhenDomainException_ShouldReturnBadRequest()
    {
        var (middleware, context) = CreateMiddleware(_ => throw new DomainException("bad domain"));

        await middleware.InvokeAsync(context);

        var (statusCode, body) = await ReadResponse(context);
        statusCode.Should().Be(HttpStatusCode.BadRequest);
        body.RootElement.GetProperty("error").GetString().Should().Be("bad domain");
        body.RootElement.GetProperty("statusCode").GetInt32().Should().Be(400);
    }

    [Fact]
    public async Task InvokeAsync_WhenKeyNotFoundException_ShouldReturnNotFound()
    {
        var (middleware, context) = CreateMiddleware(_ => throw new KeyNotFoundException("missing"));

        await middleware.InvokeAsync(context);

        var (statusCode, body) = await ReadResponse(context);
        statusCode.Should().Be(HttpStatusCode.NotFound);
        body.RootElement.GetProperty("error").GetString().Should().Be("missing");
    }

    [Fact]
    public async Task InvokeAsync_WhenUnexpectedException_ShouldReturnInternalServerError()
    {
        var (middleware, context) = CreateMiddleware(_ => throw new InvalidOperationException("boom"));

        await middleware.InvokeAsync(context);

        var (statusCode, body) = await ReadResponse(context);
        statusCode.Should().Be(HttpStatusCode.InternalServerError);
        body.RootElement.GetProperty("error").GetString().Should().Be("An unexpected error occurred.");
        body.RootElement.GetProperty("statusCode").GetInt32().Should().Be(500);
    }
}