using CarRentalService.Application.Queries;
using FluentValidation.TestHelper;

namespace CarRentalService.Application.Tests.Queries;

public class GetVehicleAvailabilityQueryValidatorTests
{
    private readonly GetVehicleAvailabilityQueryValidator _validator;

    public GetVehicleAvailabilityQueryValidatorTests()
    {
        _validator = new GetVehicleAvailabilityQueryValidator();
    }

    [Fact]
    public void Should_Have_Error_When_PickupDate_Is_Empty()
    {
        // Arrange
        var query = new GetVehicleAvailabilityQuery
        {
            PickupDate = default,
            ReturnDate = DateTime.Now.AddDays(1),
            VehicleTypes = new List<string> { "SUV" }
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PickupDate);
    }

    [Fact]
    public void Should_Have_Error_When_ReturnDate_Is_Empty()
    {
        // Arrange
        var query = new GetVehicleAvailabilityQuery
        {
            PickupDate = DateTime.Now,
            ReturnDate = default,
            VehicleTypes = new List<string> { "SUV" }
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ReturnDate);
    }

    [Fact]
    public void Should_Have_Error_When_PickupDate_Is_After_ReturnDate()
    {
        // Arrange
        var query = new GetVehicleAvailabilityQuery
        {
            PickupDate = DateTime.Now.AddDays(2),
            ReturnDate = DateTime.Now.AddDays(1),
            VehicleTypes = new List<string> { "SUV" }
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PickupDate);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Query_Is_Valid()
    {
        // Arrange
        var query = new GetVehicleAvailabilityQuery
        {
            PickupDate = DateTime.Now,
            ReturnDate = DateTime.Now.AddDays(1),
            VehicleTypes = new List<string> { "SUV" }
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}