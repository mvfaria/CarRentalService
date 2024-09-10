using FluentValidation;

namespace CarRentalService.Application.Commands;

public class CreateReservationCommandValidator : AbstractValidator<CreateReservationCommand>
{
    public CreateReservationCommandValidator()
    {
        // TODO: refactor common validations move it into a reusable class
        RuleFor(x => x.PickupDate)
            .NotEmpty().WithMessage("Pickup date is required.")
            .LessThan(x => x.ReturnDate).WithMessage("Pickup date must be before return date.");

        RuleFor(x => x.ReturnDate)
            .NotEmpty().WithMessage("Return date is required.");

        RuleFor(x => x.VehicleType)
            .NotEmpty().WithMessage("Vehicle type is required.");
    }
}