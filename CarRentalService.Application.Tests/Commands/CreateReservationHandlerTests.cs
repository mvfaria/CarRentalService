using CarRentalService.Application.Commands;
using CarRentalService.Application.Interfaces;
using CarRentalService.Domain.Entities;
using CarRentalService.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace CarRentalService.Application.Tests.Commands;

public class CreateReservationHandlerTests
{
    private readonly Mock<IReservationRepository> _reservationRepositoryMock;
    private readonly Mock<IAvailabilityService> _availabilityServiceMock;
    private readonly CreateReservationHandler _handler;

    public CreateReservationHandlerTests()
    {
        _reservationRepositoryMock = new Mock<IReservationRepository>();
        _availabilityServiceMock = new Mock<IAvailabilityService>();
        _handler = new CreateReservationHandler(_reservationRepositoryMock.Object, _availabilityServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_CreateReservation_When_VehicleIsAvailable()
    {
        // Arrange
        var command = new CreateReservationCommand
        {
            PickupDate = new DateTime(2024, 9, 1),
            ReturnDate = new DateTime(2024, 9, 10),
            VehicleType = "SUV"
        };

        var availableVehicles = new List<VehicleType>
        {
            new VehicleType { Name = "SUV", TotalCount = 1 }
        };

        _availabilityServiceMock
            .Setup(x => x.GetAvailableVehiclesAsync(It.IsAny<DateRange>(), It.IsAny<string>()))
            .ReturnsAsync(availableVehicles);

        _reservationRepositoryMock
            .Setup(x => x.AddReservationAsync(It.IsAny<Reservation>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue(); // Reservation should succeed
        _reservationRepositoryMock.Verify(x => x.AddReservationAsync(It.IsAny<Reservation>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFalse_When_NoVehiclesAreAvailable()
    {
        // Arrange
        var command = new CreateReservationCommand
        {
            PickupDate = new DateTime(2024, 9, 1),
            ReturnDate = new DateTime(2024, 9, 10),
            VehicleType = "SUV"
        };

        _availabilityServiceMock
            .Setup(x => x.GetAvailableVehiclesAsync(It.IsAny<DateRange>(), It.IsAny<string>()))
            .ReturnsAsync(new List<VehicleType>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse(); // Reservation should fail
        _reservationRepositoryMock.Verify(x => x.AddReservationAsync(It.IsAny<Reservation>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Call_AddReservationAsync_With_Correct_Reservation()
    {
        // Arrange
        var command = new CreateReservationCommand
        {
            PickupDate = new DateTime(2024, 9, 1),
            ReturnDate = new DateTime(2024, 9, 10),
            VehicleType = "Sedan"
        };

        var availableVehicles = new List<VehicleType>
        {
            new VehicleType { Name = "Sedan", TotalCount = 1 }
        };

        _availabilityServiceMock
            .Setup(x => x.GetAvailableVehiclesAsync(It.IsAny<DateRange>(), It.IsAny<string>()))
            .ReturnsAsync(availableVehicles);

        Reservation? createdReservation = null;
        _reservationRepositoryMock
            .Setup(x => x.AddReservationAsync(It.IsAny<Reservation>()))
            .Callback<Reservation>(reservation => createdReservation = reservation)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        createdReservation.Should().NotBeNull();
        createdReservation!.VehicleType.Should().Be("Sedan");
        createdReservation.PickupDate.Should().Be(command.PickupDate);
        createdReservation.ReturnDate.Should().Be(command.ReturnDate);
    }
}