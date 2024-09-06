using MediatR;
using CarRentalService.Application.DTOs;

namespace CarRentalService.Application.Queries;

public class GetVehicleAvailabilityQuery : IRequest<List<VehicleTypeDto>>
{
    public required DateTime PickupDate { get; init; }
    public required DateTime ReturnDate { get; init; }
    public List<string> VehicleTypes { get; init; } = new();
}