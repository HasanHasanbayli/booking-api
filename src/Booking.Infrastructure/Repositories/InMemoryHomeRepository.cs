using Booking.Application.Abstractions;
using Booking.Domain.Entities;

namespace Booking.Infrastructure.Repositories;

public sealed class InMemoryHomeRepository : IHomeRepository
{
    private static readonly Dictionary<int, Home> Homes;
    private static readonly Dictionary<DateOnly, HashSet<int>> DateIndex;
    private const int Capacity = 10;

    static InMemoryHomeRepository()
    {
        Homes = new Dictionary<int, Home>(Capacity);
        DateIndex = new Dictionary<DateOnly, HashSet<int>>();

        SeedDeterministicHomes();
    }

    public Task<IReadOnlyCollection<Home>> GetAvailableAsync(DateOnly startDate, DateOnly endDate)
    {
        HashSet<int>? resultSet = null;

        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            if (!DateIndex.TryGetValue(date, out var homesForDate))
            {
                return Task.FromResult<IReadOnlyCollection<Home>>([]);
            }

            if (resultSet == null)
            {
                resultSet = new HashSet<int>(homesForDate);
            }
            else
            {
                resultSet.IntersectWith(homesForDate);
            }

            if (resultSet.Count == 0)
            {
                break;
            }
        }

        if (resultSet == null || resultSet.Count == 0)
        {
            return Task.FromResult<IReadOnlyCollection<Home>>([]);
        }

        return Task.FromResult<IReadOnlyCollection<Home>>(resultSet.Select(id => Homes[id]).ToList());
    }

    private static void SeedDeterministicHomes()
    {
        var july15 = new DateOnly(2026, 7, 15);
        var july16 = new DateOnly(2026, 7, 16);
        var july17 = new DateOnly(2026, 7, 17);
        var july18 = new DateOnly(2026, 7, 18);
        var july20 = new DateOnly(2026, 7, 20);
        var july21 = new DateOnly(2026, 7, 21);

        var homes = new[]
        {
            new Home(1, "Valid Home", [july15, july16]),
            new Home(2, "Invalid Home", [july15, july17]),
            new Home(3, "Extra Days Home", [july15, july16, july17, july18]),
            new Home(4, "Other Dates", [july17]),
            new Home(5, "Invalid Home 2", [july18, july20, july21])
        };

        foreach (var home in homes)
        {
            Homes[home.Id] = home;

            foreach (var date in home.AvailableSlots)
            {
                if (!DateIndex.TryGetValue(date, out var homeSet))
                {
                    homeSet = [];
                    DateIndex[date] = homeSet;
                }

                homeSet.Add(home.Id);
            }
        }
    }
}