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
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = startDate.AddDays(5);

        var response = await _client.GetAsync(
            $"api/available-homes?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<AvailableHomeResponse> DeserializeResponse(HttpResponseMessage httpResponseMessage)
    {
        var content = await httpResponseMessage.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<AvailableHomeResponse>(content, SerializerOptions)!;
    }
}