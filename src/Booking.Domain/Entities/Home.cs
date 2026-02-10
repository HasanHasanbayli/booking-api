namespace Booking.Domain.Entities;

public class Home
{
    public int Id { get; }
    public string Name { get; }
    public IReadOnlySet<DateOnly> AvailableSlots { get; }

    public Home(int id, string name, IReadOnlySet<DateOnly> availableSlots)
    {
        Id = id;
        Name = name;
        AvailableSlots = availableSlots;
    }

    public bool IsAvailableFor(IReadOnlySet<DateOnly> requestedDates)
    {
        return requestedDates.All(AvailableSlots.Contains);
    }
}