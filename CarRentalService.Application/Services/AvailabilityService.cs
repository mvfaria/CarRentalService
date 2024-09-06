using CarRentalService.Application.Interfaces;
using CarRentalService.Domain.ValueObjects;
using CarRentalService.Domain.Entities;

namespace CarRentalService.Application.Services;

public class AvailabilityService : IAvailabilityService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IVehicleTypeRepository _vehicleTypeRepository;

    public AvailabilityService(IReservationRepository reservationRepository, IVehicleTypeRepository vehicleTypeRepository)
    {
        _reservationRepository = reservationRepository;
        _vehicleTypeRepository = vehicleTypeRepository;
    }

    public async Task<List<VehicleType>> GetAvailableVehiclesAsync(DateRange dateRange, List<string> requestedVehicleTypes)
    {
        var reservations = await _reservationRepository.GetReservationsAsync();
        var allVehicleTypes = await _vehicleTypeRepository.GetVehicleTypesAsync();

        // Filter vehicle types if specific types are requested
        var vehicleTypesToCheck = requestedVehicleTypes.Any()
            ? allVehicleTypes.Where(v => requestedVehicleTypes.Contains(v.Name)).ToList()
            : allVehicleTypes.ToList();

        var availableVehicleTypes = new List<VehicleType>();
        foreach (var vehicleType in vehicleTypesToCheck)
        {
            var reservedCount = reservations.Count(r =>
                r.VehicleType == vehicleType.Name &&
                dateRange.Overlaps(new DateRange(r.PickupDate, r.ReturnDate)));
            
            availableVehicleTypes.Add(new()
            {
                Name = vehicleType.Name,
                TotalCount = vehicleType.TotalCount - reservedCount // Adjust available count based on reservations
            });
        }

        return availableVehicleTypes.Where(vt => vt.TotalCount > 0).ToList(); // Only return types with available vehicles
    }

    public async Task<List<VehicleType>> GetAvailableVehiclesAsync(DateRange dateRange, string requestedVehicleType)
    {
        return await GetAvailableVehiclesAsync(dateRange, [requestedVehicleType]);
    }
}