using CarRentalService.Application.Commands;
using CarRentalService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalService.Endpoints;

public static class CarRentalEndpoints
{
    public static void RegisterCarRentalEndpoints(this WebApplication app)
    {
        // TODO: Test API Endpoints
        var carRental = app.MapGroup("/car-rental");
        
        carRental.MapPost("/reservations", CreateReservation);
        carRental.MapGet("/availability", GetVehicleAvailability);
    }

    static async Task<IResult> CreateReservation (IMediator mediator, CreateReservationCommand command)
    {
        var result = await mediator.Send(command);
        
        return result
            ? TypedResults.Ok("Reservation created successfully.")
            : TypedResults.BadRequest("No vehicles available for the given date range.");
    }
    
    static async Task<IResult> GetVehicleAvailability (IMediator mediator, [FromQuery] DateTime pickupDate, [FromQuery] DateTime returnDate, [FromQuery] string[]? vehicleTypes)
    {
        var query = new GetVehicleAvailabilityQuery
        {
            PickupDate = pickupDate,
            ReturnDate = returnDate,
            VehicleTypes = (vehicleTypes ?? []).ToList() // Default to an empty list if no types are provided
        };
    
        var availability = await mediator.Send(query);
        return TypedResults.Ok(availability);
    }
}