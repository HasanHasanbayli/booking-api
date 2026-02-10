using Booking.Domain.Entities;

namespace Booking.Application.Abstractions;

public interface IHomeRepository
{
    Task<IReadOnlyCollection<Home>> GetAllAsync(CancellationToken ct);
}