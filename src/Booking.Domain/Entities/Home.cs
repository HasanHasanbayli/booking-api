namespace Booking.Domain.Entities;

public sealed class Home(int id, string name, IEnumerable<DateOnly> availableSlots)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    public readonly HashSet<DateOnly> AvailableSlots = new(availableSlots);

    public bool IsAvailableFor(DateOnly startDate, DateOnly endDate)
    {
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            if (!AvailableSlots.Contains(date))
            {
                return false;
            }
        }

        return true;
    }

    public IEnumerable<DateOnly> GetAvailableInRange(DateOnly startDate, DateOnly endDate)
    {
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            if (AvailableSlots.Contains(date))
            {
                yield return date;
            }
        }
    }
}