using Booking.Application.Abstractions;
using Booking.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.Infrastructure;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddInfrastructure()
        {
            services.AddScoped<IHomeRepository, InMemoryHomeRepository>();
        }
    }
}