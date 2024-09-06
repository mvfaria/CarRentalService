using CarRentalService.Infrastructure.Repositories;
using CarRentalService.Application.Interfaces;
using CarRentalService.Application.Services;
using CarRentalService.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
builder.Services.AddSingleton<IReservationRepository, ReservationRepository>();
builder.Services.AddSingleton<IVehicleTypeRepository, VehicleTypeRepository>();
builder.Services.AddTransient<IAvailabilityService, AvailabilityService>();

var app = builder.Build();
app.RegisterCarRentalEndpoints();
app.Run();