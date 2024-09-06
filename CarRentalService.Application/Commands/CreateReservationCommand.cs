using MediatR;

namespace CarRentalService.Application.Commands;

public class CreateReservationCommand : IRequest<bool>
{
    public required string VehicleType { get; set; }
    public required DateTime PickupDate { get; set; }
    public required DateTime ReturnDate { get; set; }
}