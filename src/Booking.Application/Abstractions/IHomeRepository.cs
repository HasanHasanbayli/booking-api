using Booking.Domain.Entities;

namespace Booking.Application.Abstractions;

public interface IHomeRepository
{
    Task<HashSet<Home>> GetAllAsync(CancellationToken ct);
}