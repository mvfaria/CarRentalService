using CarRentalService.Application.Interfaces;
using CarRentalService.Domain.Entities;

namespace CarRentalService.Infrastructure.Repositories;

public class VehicleTypeRepository : IVehicleTypeRepository
{
    private readonly List<VehicleType> _vehicleTypes = new()
    {
        new() { Name = "Compact", TotalCount = 3 },
        new() { Name = "Sedan", TotalCount = 2 },
        new() { Name = "SUV", TotalCount = 1 },
        new() { Name = "Van", TotalCount = 1 }
    };

    public Task<List<VehicleType>> GetVehicleTypesAsync() => Task.FromResult(_vehicleTypes);
}