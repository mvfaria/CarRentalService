using CarRentalService.Application.Interfaces;
using CarRentalService.Domain.ValueObjects;
using MediatR;

namespace CarRentalService.Application.Commands;

public class CreateReservationHandler : IRequestHandler<CreateReservationCommand, bool>
{
    private readonly IReservationService _reservationService;

    public CreateReservationHandler(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    public async Task<bool> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        var dateRange = new DateRange(request.PickupDate, request.ReturnDate);
        return await _reservationService.CreateReservationAsync(request.VehicleType, dateRange);
    }
}