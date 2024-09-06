using CarRentalService.Domain.Entities;
using CarRentalService.Domain.ValueObjects;

namespace CarRentalService.Application.Interfaces
{
    public interface IAvailabilityService
    {
        Task<List<VehicleType>> GetAvailableVehiclesAsync(DateRange dateRange, List<string> requestedVehicleTypes);
        Task<List<VehicleType>> GetAvailableVehiclesAsync(DateRange dateRange, string requestedVehicleType);
    }
}