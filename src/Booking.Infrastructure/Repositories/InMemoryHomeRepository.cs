using System.Collections.Immutable;
using Booking.Application.Abstractions;
using Booking.Domain.Entities;

namespace Booking.Infrastructure.Repositories;

public sealed class InMemoryHomeRepository : IHomeRepository
{
    private static readonly ImmutableArray<Home> Homes;

    static InMemoryHomeRepository()
    {
        var builder = ImmutableArray.CreateBuilder<Home>();

        for (int i = 1; i <= 10; i++)
        {
            builder.Add(new Home(
                id: i,
                name: $"Home {i}",
                availableSlots: GenerateSlots()));
        }

        Homes = builder.ToImmutable();
    }

    public Task<IReadOnlyList<Home>> GetAllAsync(CancellationToken ct)
    {
        return Task.FromResult<IReadOnlyList<Home>>(Homes);
    }

    private static IEnumerable<DateOnly> GenerateSlots()
    {
        for (int i = 0; i < 30; i++)
        {
            yield return new DateOnly(2025, 7, 1).AddDays(i);
        }
    }
}