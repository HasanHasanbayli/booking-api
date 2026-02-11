using Booking.Domain.Entities;

namespace Booking.Application.Abstractions;

public interface IHomeRepository
{
    Task<IReadOnlyList<Home>> GetAllAsync(CancellationToken ct);
}