using CarRentalService.Infrastructure.Repositories;
using CarRentalService.Application.Interfaces;
using CarRentalService.Application.Services;
using CarRentalService.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

builder.Services.AddSingleton<IReservationRepository, ReservationRepository>();
builder.Services.AddSingleton<IVehicleTypeRepository, VehicleTypeRepository>();
builder.Services.AddScoped<IAvailabilityService, AvailabilityService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "CarRentalService";
    config.Title = "CarRentalService v1";
    config.Version = "v1";
});

var app = builder.Build();
app.RegisterCarRentalEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "CarRentalService";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

app.Run();