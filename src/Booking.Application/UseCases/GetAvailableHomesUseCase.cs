using Booking.Application.Abstractions;
using Booking.Domain.Entities;

namespace Booking.Application.UseCases;

public class GetAvailableHomesUseCase
{
    private readonly IHomeRepository _repository;

    public GetAvailableHomesUseCase(IHomeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<Home>> ExecuteAsync(
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken ct)
    {
        if (startDate > endDate)
            throw new ArgumentException("Invalid date range");

        var requestedDates = Enumerable
            .Range(0, endDate.DayNumber - startDate.DayNumber + 1)
            .Select(startDate.AddDays)
            .ToHashSet();

        var homes = await _repository.GetAllAsync(ct);

        return homes.Where(h => h.IsAvailableFor(requestedDates)).ToList();
    }
}