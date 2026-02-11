namespace Booking.Domain.Entities;

public sealed class Home
{
    public int Id { get; }
    public string Name { get; }

    private readonly HashSet<DateOnly> _availableSlots;

    public Home(int id, string name, IEnumerable<DateOnly> availableSlots)
    {
        Id = id;
        Name = name;
        _availableSlots = new HashSet<DateOnly>(availableSlots);
    }

    public IReadOnlyCollection<DateOnly> AvailableSlots => _availableSlots;

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
}