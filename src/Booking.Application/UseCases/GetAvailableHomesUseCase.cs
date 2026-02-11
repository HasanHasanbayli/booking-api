using Booking.Application.Abstractions;
using Booking.Domain.Entities;

namespace Booking.Application.UseCases;

public class GetAvailableHomesUseCase(IHomeRepository repository)
{
    public async Task<IReadOnlyCollection<Home>> ExecuteAsync(
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken ct)
    {
        if (startDate > endDate)
            throw new ArgumentException("Invalid date range");

        var homes = await repository.GetAllAsync(ct);

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