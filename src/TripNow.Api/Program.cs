using Microsoft.Extensions.Hosting;
using TripNow.Domain.Interfaces;
using TripNow.Domain.Services;
using TripNow.Infrastructure.Persistence;
using TripNow.Infrastructure.Persistence.Repositories;
using TripNow.Infrastructure.Services;
using Wolverine;
using Wolverine.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<TripNowDbContext>("tripnow-db");

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();

builder.Services.AddHttpClient<IRiskEvaluationService, RiskEvaluationService>(client =>
{
    client.BaseAddress = new Uri("https://vuc42ahokh5whcd5r5in7tj2km0jvjxu.lambda-url.us-east-1.on.aws/swagger/index.html"); // Placeholder, will be configured via service discovery
});

builder.Host.UseWolverine();
builder.Services.AddWolverineHttp();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapWolverineEndpoints();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
