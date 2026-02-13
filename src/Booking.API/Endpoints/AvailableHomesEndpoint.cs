using Booking.API.Contracts;
using Booking.Application.UseCases;

namespace Booking.API.Endpoints;

public static class AvailableHomesEndpoint
{
    public static void MapAvailableHomes(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/available-homes", async (
                DateOnly startDate,
                DateOnly endDate,
                GetAvailableHomesUseCase useCase) =>
            {
                var homes = await useCase.ExecuteAsync(startDate, endDate)
                    .Select(h =>
                    {
                        var slots = h.AvailableSlots.Select(s => s.ToString("yyyy-MM-dd")).ToList();

                        return new HomeDto(
                            HomeId: h.Id,
                            HomeName: h.Name,
                            AvailableSlots: slots);
                    })
                    .ToListAsync();

                return Results.Ok(new AvailableHomeResponse(Status: "OK", Homes: homes));
            })
            .WithName("GetAvailableHomes");
    }
}