using Booking.Application;
using Booking.Application.UseCases;
using Booking.Infrastructure;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/available-homes", async (
        [FromQuery] DateOnly startDate, DateOnly endDate,
        [FromServices] GetAvailableHomesUseCase service, CancellationToken ct) =>
    {
        var homes = await service.ExecuteAsync(startDate, endDate, ct);

        return Results.Ok(new
        {
            status = "OK",
            homes = homes.Select(h => new
            {
                homeId = h.Id,
                homeName = h.Name,
                availableSlots = h.AvailableSlots
                    .Where(d => d >= startDate && d <= endDate)
                    .Select(d => d.ToString("yyyy-MM-dd"))
            })
        });
    })
    .WithName("GetAvailableHomes");

app.Run();