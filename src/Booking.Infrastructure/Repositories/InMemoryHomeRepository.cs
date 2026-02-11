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

        for (int i = 0; i <= Capacity; i++)
        {
            var slots = GenerateSlots().ToArray();

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

    private static IEnumerable<DateOnly> GenerateSlots()
    {
        var start = DateOnly.FromDateTime(DateTime.UtcNow);
        var random = new Random();

        for (int i = 0; i < 5; i++)
        {
            int randomDays = random.Next(0, 5);
            yield return start.AddDays(randomDays);
        }
    }
}