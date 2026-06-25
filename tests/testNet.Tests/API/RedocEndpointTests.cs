using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace testNet.Tests.API;

public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public IntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetRedoc_ShouldReturn200WithHtml()
    {
        var response = await _client.GetAsync("/redoc");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.ToString().Should().Be("text/html");

        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain("Redoc");
        html.Should().Contain("swagger.json");
    }

    [Fact]
    public async Task SwaggerJson_ShouldStillBeAvailable()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.ToString().Should().Contain("application/json");
    }

    [Fact]
    public async Task SeedEndpoint_ShouldResetAndReturnOk()
    {
        var response = await _client.PostAsync("/api/seed", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("message");
        json.Should().Contain("count");
        json.Should().Contain("10");
    }

    [Fact]
    public async Task SeedEndpoint_ProductsExistAfterSeed()
    {
        await _client.PostAsync("/api/seed", null);

        var response = await _client.GetAsync("/api/products");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("LAP-001");
        json.Should().Contain("DIS-001");
    }
}
