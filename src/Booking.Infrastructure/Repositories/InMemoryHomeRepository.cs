using Booking.Application.Abstractions;
using Booking.Domain.Entities;

namespace Booking.Infrastructure.Repositories;

public sealed class InMemoryHomeRepository : IHomeRepository
{
    private static readonly Dictionary<int, Home> Homes;
    private static readonly Dictionary<DateOnly, HashSet<int>> DateIndex;
    private const int Capacity = 100_000;

    static InMemoryHomeRepository()
    {
        Homes = new Dictionary<int, Home>(Capacity);
        DateIndex = new Dictionary<DateOnly, HashSet<int>>();

        for (int i = 1; i <= Capacity; i++)
        {
            var slots = GenerateSlots(i).ToArray();

            var home = new Home(
                id: i,
                name: $"Home {i}",
                availableSlots: slots);

            Homes[i] = home;

            foreach (var date in slots)
            {
                if (!DateIndex.TryGetValue(date, out var homeSet))
                {
                    homeSet = [];
                    DateIndex[date] = homeSet;
                }

                homeSet.Add(i);
            }
        }
    }

    public IReadOnlyCollection<Home> GetAvailableAsync(DateOnly startDate, DateOnly endDate)
    {
        HashSet<int>? resultSet = null;

        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            if (!DateIndex.TryGetValue(date, out var homesForDate))
            {
                return [];
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
            return [];
        }

        return resultSet.Select(id => Homes[id]).ToList();
    }

    private static IEnumerable<DateOnly> GenerateSlots(int num)
    {
        var start = DateOnly.FromDateTime(DateTime.UtcNow);

        for (int i = 0; i < 30; i++)
        {
            yield return start.AddDays(i + num);
        }
    }
}