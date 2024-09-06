using MediatR;
using CarRentalService.Application.DTOs;
using CarRentalService.Application.Interfaces;
using CarRentalService.Domain.ValueObjects;

namespace CarRentalService.Application.Queries;

public class GetVehicleAvailabilityHandler : IRequestHandler<GetVehicleAvailabilityQuery, List<VehicleTypeDto>>
{
    private readonly IAvailabilityService _availabilityService;

    public GetVehicleAvailabilityHandler(IAvailabilityService availabilityService)
    {
        _availabilityService = availabilityService;
    }

    public async Task<List<VehicleTypeDto>> Handle(GetVehicleAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var dateRange = new DateRange(request.PickupDate, request.ReturnDate);
        var availableVehicles = (await _availabilityService.GetAvailableVehiclesAsync(dateRange, request.VehicleTypes))
            .Select(x => new VehicleTypeDto(x.Name, x.TotalCount)) // mapping into DTO
            .ToList();

        return availableVehicles;
    }
}