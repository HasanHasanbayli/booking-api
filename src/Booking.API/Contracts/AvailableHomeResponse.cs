namespace Booking.API.Contracts;

public record AvailableHomeResponse(string Status, IReadOnlyCollection<HomeDto> Homes);