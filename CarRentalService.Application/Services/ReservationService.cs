using CarRentalService.Application.Interfaces;
using CarRentalService.Domain.Entities;
using CarRentalService.Domain.ValueObjects;

namespace CarRentalService.Application.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IAvailabilityService _availabilityService;
    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    public ReservationService(IReservationRepository reservationRepository, IAvailabilityService availabilityService)
    {
        _reservationRepository = reservationRepository;
        _availabilityService = availabilityService;
    }

    public async Task<bool> CreateReservationAsync(string vehicleType, DateRange dateRange)
    {
        await _semaphore.WaitAsync(); // Lock to ensure only one request checks availability at a time

        try
        {
            var availableVehicles = await _availabilityService.GetAvailableVehiclesAsync(dateRange, vehicleType);

            if (!availableVehicles.Any())
                return false; // No vehicles available

            await _reservationRepository.AddReservationAsync(new Reservation
            {
                VehicleType = vehicleType,
                PickupDate = dateRange.StartDate,
                ReturnDate = dateRange.EndDate
            });

            return true;
        }
        finally
        {
            _semaphore.Release(); // Release the lock
        }
    }
}