using Booking.Application.Abstractions;
using Booking.Infrastructure.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.Api.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing repository registration if needed
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IHomeRepository));

            if (descriptor != null)
                services.Remove(descriptor);

            // Register in-memory repository for tests
            services.AddSingleton<IHomeRepository, InMemoryHomeRepository>();
        });
    }
}