using Booking.Domain.Entities;

namespace Booking.Application.Services;

public interface IHomeStore
{
    IReadOnlyCollection<Home> GetAll();
}