using CarRentalService.Application.Commands;
using CarRentalService.Application.Interfaces;
using CarRentalService.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace CarRentalService.Application.Tests.Commands
{
    public class CreateReservationHandlerTests
    {
        private readonly Mock<IReservationService> _reservationServiceMock;
        private readonly CreateReservationHandler _handler;

        public CreateReservationHandlerTests()
        {
            _reservationServiceMock = new Mock<IReservationService>();
            _handler = new CreateReservationHandler(_reservationServiceMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Call_CreateReservationAsync_Once()
        {
            // Arrange
            var command = new CreateReservationCommand
            {
                VehicleType = "SUV",
                PickupDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddDays(5)
            };

            var dateRange = new DateRange(command.PickupDate, command.ReturnDate);

            _reservationServiceMock
                .Setup(s => s.CreateReservationAsync(command.VehicleType, It.IsAny<DateRange>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            _reservationServiceMock.Verify(s => s.CreateReservationAsync(command.VehicleType, It.Is<DateRange>(dr => dr.StartDate == command.PickupDate && dr.EndDate == command.ReturnDate)), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_False_If_Reservation_Fails()
        {
            // Arrange
            var command = new CreateReservationCommand
            {
                VehicleType = "Sedan",
                PickupDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddDays(2)
            };

            _reservationServiceMock
                .Setup(s => s.CreateReservationAsync(command.VehicleType, It.IsAny<DateRange>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
            _reservationServiceMock.Verify(s => s.CreateReservationAsync(command.VehicleType, It.IsAny<DateRange>()), Times.Once);
        }
    }
}
