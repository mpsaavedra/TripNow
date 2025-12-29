using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using TripNow.Application.Features.Reservations.Create;
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


builder.Services.AddWolverineHttp();
builder.Host.UseWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(typeof(TripNow.Application.Features.Reservations.Create.CreateReservationHandler).Assembly);
});

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<TripNowDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseHttpsRedirection();
app.MapWolverineEndpoints();


app.Run();
