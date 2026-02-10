using Booking.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.Application;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddApplication()
        {
            services.AddScoped<GetAvailableHomesUseCase>();
        }
    }
}