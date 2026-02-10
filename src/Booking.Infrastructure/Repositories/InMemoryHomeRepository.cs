using Booking.Application.Abstractions;
using Booking.Domain.Entities;

namespace Booking.Infrastructure.Repositories;

public class InMemoryHomeRepository : IHomeRepository
{
    public InMemoryHomeRepository()
    {
        LoadDataWhileStarting();
    }

    private static readonly HashSet<Home> Homes = [];

    private void LoadDataWhileStarting()
    {
        if (Homes.Count == 0)
        {
            for (int i = 0; i < 20; i++)
            {
                Homes.Add(new Home(i, $"Home {i}", new HashSet<DateOnly>
                {
                    new(2025, 7, 15),
                    new(2025, 7, 16),
                    new(2025, 7, 17)
                }));
            }
        }
    }

    public Task<HashSet<Home>> GetAllAsync(CancellationToken ct)
    {
        return Task.FromResult(Homes);
    }
}