namespace Booking.Domain.Entities;

public sealed class Home(int id, string name, IEnumerable<DateOnly> availableSlots)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    private readonly HashSet<DateOnly> _availableSlots = [..availableSlots];

    public bool IsAvailableFor(DateOnly startDate, DateOnly endDate)
    {
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            if (!_availableSlots.Contains(date))
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
            if (_availableSlots.Contains(date))
            {
                yield return date;
            }
        }
    }
}