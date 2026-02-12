using System.Net;
using System.Text.Json;
using Booking.API.Contracts;
using FluentAssertions;

namespace Booking.Api.IntegrationTests;

public class AvailableHomesTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private static JsonSerializerOptions SerializerOptions => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    [Fact]
    public async Task GetAvailableHomes_ReturnsOk()
    {
        // Act
        var startDate = new DateOnly(2026, 7, 15);
        var endDate = new DateOnly(2026, 7, 18);

        var response = await _client.GetAsync(
            $"api/available-homes?startDate={startDate:MM/dd/yyyy}&endDate={endDate:MM/dd/yyyy}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task StartDateAfterEndDate_ReturnsInternalServerError()
    {
        // Act
        var startDate = new DateOnly(2026, 7, 18);
        var endDate = new DateOnly(2026, 7, 15);

        var response = await _client.GetAsync(
            $"api/available-homes?startDate={startDate:MM/dd/yyyy}&endDate={endDate:MM/dd/yyyy}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task GetAvailableHomes_ReturnsAvailableHomes()
    {
        // Act
        var startDate = new DateOnly(2026, 7, 15);
        var endDate = new DateOnly(2026, 7, 18);

        var response = await _client.GetAsync(
            $"api/available-homes?startDate={startDate:MM/dd/yyyy}&endDate={endDate:MM/dd/yyyy}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var availableHomeResponse = await DeserializeResponse(response);

        availableHomeResponse.Homes.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAvailableHomes_ReturnsEmptyList_WhenNoHomesAvailable()
    {
        // Act
        var startDate = new DateOnly(2026, 7, 18);
        var endDate = new DateOnly(2026, 7, 20);

        var response = await _client.GetAsync(
            $"api/available-homes?startDate={startDate:MM/dd/yyyy}&endDate={endDate:MM/dd/yyyy}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var availableHomeResponse = await DeserializeResponse(response);

        availableHomeResponse.Homes.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAvailableHomes_Returns3Homes_When3HomesAvailable()
    {
        // Act
        // Because of the deterministic seeding of the in-memory repository, we know that on 7/17/2026 there are exactly 3 homes available.
        var startDate = new DateOnly(2026, 7, 17);
        var endDate = new DateOnly(2026, 7, 17);

        var response = await _client.GetAsync(
            $"api/available-homes?startDate={startDate:MM/dd/yyyy}&endDate={endDate:MM/dd/yyyy}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var availableHomeResponse = await DeserializeResponse(response);

        availableHomeResponse.Homes.Count.Should().Be(3);
    }

    private async Task<AvailableHomeResponse> DeserializeResponse(HttpResponseMessage httpResponseMessage)
    {
        var content = await httpResponseMessage.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<AvailableHomeResponse>(content, SerializerOptions)!;
    }
}