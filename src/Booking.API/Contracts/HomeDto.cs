namespace Booking.API.Contracts;

public record HomeDto(int HomeId, string HomeName, IReadOnlyCollection<string> AvailableSlots);