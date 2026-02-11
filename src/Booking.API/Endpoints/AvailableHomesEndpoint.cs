using Booking.API.Contracts;
using Booking.Application.UseCases;

namespace Booking.API.Endpoints;

public static class AvailableHomesEndpoint
{
    public static void MapAvailableHomes(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/available-homes", (
                DateOnly startDate,
                DateOnly endDate,
                GetAvailableHomesUseCase useCase) =>
            {
                var homes = useCase.ExecuteAsync(startDate, endDate);

                var result = new List<HomeDto>(homes.Count);

                foreach (var home in homes)
                {
                    var slots = new List<string>();

                    foreach (var date in home.GetAvailableInRange(startDate, endDate))
                    {
                        slots.Add(date.ToString("yyyy-MM-dd"));
                    }

                    result.Add(new HomeDto(HomeId: home.Id, HomeName: home.Name, AvailableSlots: slots));
                }

                return Results.Ok(new AvailableHomeResponse(Status: "OK", Homes: result));
            })
            .WithName("GetAvailableHomes");
    }
}