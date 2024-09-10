using CarRentalService.Application.Behaviors;
using CarRentalService.Application.Commands;
using CarRentalService.Infrastructure.Repositories;
using CarRentalService.Application.Interfaces;
using CarRentalService.Application.Queries;
using CarRentalService.Application.Services;
using CarRentalService.Endpoints;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

// Register FluentValidation Validators
builder.Services.AddValidatorsFromAssemblyContaining<CreateReservationCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<GetVehicleAvailabilityQueryValidator>();

// Add custom pipeline behavior for validation
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddSingleton<IReservationRepository, ReservationRepository>();
builder.Services.AddSingleton<IVehicleTypeRepository, VehicleTypeRepository>();
builder.Services.AddScoped<IAvailabilityService, AvailabilityService>();
builder.Services.AddScoped<IReservationService, ReservationService>();

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

// TODO: API Tests to Ensure Validation is Triggered
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
        if (exception is ValidationException validationException)
        {
            var errors = validationException.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new { Errors = errors });
        }
    });
});


app.Run();