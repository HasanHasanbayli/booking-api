using Booking.Application.Abstractions;
using Booking.Domain.Entities;

namespace Booking.Infrastructure.Repositories;

public class InMemoryHomeRepository : IHomeRepository
{
    private static readonly IReadOnlyCollection<Home> Homes =
    [
        new(1, "Home 1", new HashSet<DateOnly>
        {
            new(2025, 7, 15),
            new(2025, 7, 16),
            new(2025, 7, 17)
        }),
        new(1, "Home 2", new HashSet<DateOnly>
        {
            new(2025, 7, 17),
            new(2025, 7, 18)
        })
    ];

    public Task<IReadOnlyCollection<Home>> GetAllAsync(CancellationToken ct)
    {
        return Task.FromResult(Homes);
    }
}