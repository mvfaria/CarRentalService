using CarRentalService.Domain.Entities;

namespace CarRentalService.Application.Interfaces;

public interface IVehicleTypeRepository
{
    Task<List<VehicleType>> GetVehicleTypesAsync();
}