using Booking.Application.Abstractions;
using Booking.Domain.Entities;

namespace Booking.Infrastructure.Repositories;

public sealed class InMemoryHomeRepository : IHomeRepository
{
    private static readonly Dictionary<int, Home> Homes;

    static InMemoryHomeRepository()
    {
        Homes = new Dictionary<int, Home>(capacity: 1000);

        for (int i = 1; i <= 1000; i++)
        {
            Homes[i] = new Home(
                id: i,
                name: $"Home {i}",
                availableSlots: GenerateSlots());
        }
    }

    // Fast enumeration for filtering
    public Task<IReadOnlyCollection<Home>> GetAllAsync(CancellationToken ct)
    {
        return Task.FromResult<IReadOnlyCollection<Home>>(Homes.Values);
    }

    // Useful O(1) lookup
    public Task<Home?> GetByIdAsync(int id, CancellationToken ct)
    {
        Homes.TryGetValue(id, out var home);

        return Task.FromResult(home);
    }

    private static IEnumerable<DateOnly> GenerateSlots()
    {
        var start = DateOnly.FromDateTime(DateTime.UtcNow);

        for (int i = 0; i < 30; i++)
        {
            yield return start.AddDays(i);
        }
    }
}