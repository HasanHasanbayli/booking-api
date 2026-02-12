using Booking.Application.Abstractions;
using Booking.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Booking.Application.UseCases;

public class GetAvailableHomesUseCase(IHomeRepository repository)
{
    public IReadOnlyCollection<Home> ExecuteAsync(
        DateOnly startDate,
        DateOnly endDate)
    {
        if (startDate > endDate)
            throw new BadHttpRequestException("Invalid date range");

        var homes =  repository.GetAvailableAsync(startDate, endDate);

        var result = new List<Home>(homes.Count);

        foreach (var home in homes)
        {
            if (home.IsAvailableFor(startDate, endDate))
            {
                result.Add(home);
            }
        }

        return result;
    }
}