using FluentValidation;

namespace CarRentalService.Application.Queries;

public class GetVehicleAvailabilityQueryValidator : AbstractValidator<GetVehicleAvailabilityQuery>
{
    public GetVehicleAvailabilityQueryValidator()
    {
        // TODO: refactor common validations move it into a reusable class 
        RuleFor(x => x.PickupDate)
            .NotEmpty().WithMessage("Pickup date is required.")
            .LessThan(x => x.ReturnDate).WithMessage("Pickup date must be before return date.");

        RuleFor(x => x.ReturnDate)
            .NotEmpty().WithMessage("Return date is required.");
    }
}