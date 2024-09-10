using CarRentalService.Application.Interfaces;
using CarRentalService.Domain.Entities;
using CarRentalService.Domain.ValueObjects;
using MediatR;

namespace CarRentalService.Application.Commands;

public class CreateReservationHandler : IRequestHandler<CreateReservationCommand, bool>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IAvailabilityService _availabilityService;

    public CreateReservationHandler(IReservationRepository reservationRepository, IAvailabilityService availabilityService)
    {
        _reservationRepository = reservationRepository;
        _availabilityService = availabilityService;
    }

    public async Task<bool> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        var dateRange = new DateRange(request.PickupDate, request.ReturnDate);
        var availableVehicles = await _availabilityService.GetAvailableVehiclesAsync(dateRange, request.VehicleType);

        if (!availableVehicles.Any())
            return false; // No vehicles available

        await _reservationRepository.AddReservationAsync(new Reservation
        {
            VehicleType = request.VehicleType,
            PickupDate = request.PickupDate,
            ReturnDate = request.ReturnDate
        });

        return true;
    }
}