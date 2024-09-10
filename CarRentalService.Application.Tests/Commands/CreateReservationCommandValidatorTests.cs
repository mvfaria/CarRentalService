using CarRentalService.Application.Commands;
using FluentValidation.TestHelper;

namespace CarRentalService.Application.Tests.Commands;

public class CreateReservationCommandValidatorTests
{
    private readonly CreateReservationCommandValidator _validator;

    public CreateReservationCommandValidatorTests()
    {
        _validator = new CreateReservationCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_PickupDate_Is_Empty()
    {
        // Arrange
        var command = new CreateReservationCommand
        {
            PickupDate = default,
            ReturnDate = DateTime.Now.AddDays(1),
            VehicleType = "SUV"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PickupDate);
    }

    [Fact]
    public void Should_Have_Error_When_ReturnDate_Is_Empty()
    {
        // Arrange
        var command = new CreateReservationCommand
        {
            PickupDate = DateTime.Now,
            ReturnDate = default,
            VehicleType = "SUV"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ReturnDate);
    }

    [Fact]
    public void Should_Have_Error_When_VehicleType_Is_Empty()
    {
        // Arrange
        var command = new CreateReservationCommand
        {
            PickupDate = DateTime.Now,
            ReturnDate = DateTime.Now.AddDays(1),
            VehicleType = string.Empty
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.VehicleType);
    }

    [Fact]
    public void Should_Have_Error_When_PickupDate_Is_After_ReturnDate()
    {
        // Arrange
        var command = new CreateReservationCommand
        {
            PickupDate = DateTime.Now.AddDays(2),
            ReturnDate = DateTime.Now.AddDays(1),
            VehicleType = "SUV"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PickupDate);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        // Arrange
        var command = new CreateReservationCommand
        {
            PickupDate = DateTime.Now,
            ReturnDate = DateTime.Now.AddDays(1),
            VehicleType = "SUV"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}