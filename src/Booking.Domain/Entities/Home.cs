namespace Booking.Domain.Entities;

public sealed class Home(int id, string name, IEnumerable<DateOnly> availableSlots)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    public readonly HashSet<DateOnly> AvailableSlots = new(availableSlots);

    public bool IsAvailableFor(DateOnly startDate, DateOnly endDate)
    {
        var requestedRange = EnumerateRange(startDate, endDate).ToHashSet();

        if (!requestedRange.All(date => AvailableSlots.Contains(date)))
        {
            return false;
        }

        AvailableSlots.IntersectWith(requestedRange);

        return true;
    }

    private static IEnumerable<DateOnly> EnumerateRange(DateOnly start, DateOnly end)
    {
        for (var date = start; date <= end; date = date.AddDays(1))
        {
            yield return date;
        }
    }
}