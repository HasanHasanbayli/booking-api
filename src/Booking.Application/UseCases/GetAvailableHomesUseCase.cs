using Booking.Application.Abstractions;
using Booking.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Booking.Application.UseCases;

public class GetAvailableHomesUseCase(IHomeRepository repository)
{
    public async IAsyncEnumerable<Home> ExecuteAsync(
        DateOnly startDate,
        DateOnly endDate)
    {
        if (startDate > endDate)
            throw new BadHttpRequestException("Invalid date range");

        var homes = await repository.GetAvailableAsync(startDate, endDate);

        foreach (var home in homes.Where(h => h.IsAvailableFor(startDate, endDate)))
        {
            yield return home;
        }
    }
}