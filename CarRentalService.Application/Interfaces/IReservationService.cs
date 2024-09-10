using CarRentalService.Domain.ValueObjects;

namespace CarRentalService.Application.Interfaces;

public interface IReservationService
{
    Task<bool> CreateReservationAsync(string vehicleType, DateRange dateRange);
}