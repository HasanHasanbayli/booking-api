using Booking.Domain.Entities;

namespace Booking.Application.Abstractions;

public interface IHomeRepository
{
    IReadOnlyCollection<Home> GetAvailableAsync(DateOnly startDate, DateOnly endDate);
}