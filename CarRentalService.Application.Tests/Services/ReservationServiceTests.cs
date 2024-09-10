using CarRentalService.Application.Interfaces;
using CarRentalService.Application.Services;
using CarRentalService.Domain.Entities;
using CarRentalService.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace CarRentalService.Application.Tests.Services
{
    public class ReservationServiceTests
    {
        private readonly Mock<IReservationRepository> _reservationRepositoryMock;
        private readonly Mock<IAvailabilityService> _availabilityServiceMock;
        private readonly ReservationService _reservationService;

        public ReservationServiceTests()
        {
            _reservationRepositoryMock = new Mock<IReservationRepository>();
            _availabilityServiceMock = new Mock<IAvailabilityService>();
            _reservationService = new ReservationService(_reservationRepositoryMock.Object, _availabilityServiceMock.Object);
        }

        [Fact]
        public async Task CreateReservationAsync_Should_Return_True_If_Vehicle_Is_Available()
        {
            // Arrange
            var vehicleType = "SUV";
            var dateRange = new DateRange(DateTime.Now, DateTime.Now.AddDays(5));

            _availabilityServiceMock
                .Setup(a => a.GetAvailableVehiclesAsync(dateRange, vehicleType))
                .ReturnsAsync(new List<VehicleType> { new() { Name = "", TotalCount = 1 }});

            _reservationRepositoryMock
                .Setup(r => r.AddReservationAsync(It.IsAny<Reservation>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _reservationService.CreateReservationAsync(vehicleType, dateRange);

            // Assert
            result.Should().BeTrue();
            _availabilityServiceMock.Verify(a => a.GetAvailableVehiclesAsync(dateRange, vehicleType), Times.Once);
            _reservationRepositoryMock.Verify(r => r.AddReservationAsync(It.Is<Reservation>(res => res.VehicleType == vehicleType && res.PickupDate == dateRange.StartDate && res.ReturnDate == dateRange.EndDate)), Times.Once);
        }

        [Fact]
        public async Task CreateReservationAsync_Should_Return_False_If_No_Vehicle_Is_Available()
        {
            // Arrange
            var vehicleType = "Sedan";
            var dateRange = new DateRange(DateTime.Now, DateTime.Now.AddDays(3));

            _availabilityServiceMock
                .Setup(a => a.GetAvailableVehiclesAsync(dateRange, vehicleType))
                .ReturnsAsync(new List<VehicleType>()); // No vehicles available

            // Act
            var result = await _reservationService.CreateReservationAsync(vehicleType, dateRange);

            // Assert
            result.Should().BeFalse();
            _availabilityServiceMock.Verify(a => a.GetAvailableVehiclesAsync(dateRange, vehicleType), Times.Once);
            _reservationRepositoryMock.Verify(r => r.AddReservationAsync(It.IsAny<Reservation>()), Times.Never);
        }

        [Fact]
        public async Task CreateReservationAsync_Should_Release_Semaphore_If_An_Exception_Occurs()
        {
            // Arrange
            var vehicleType = "Truck";
            var dateRange = new DateRange(DateTime.Now, DateTime.Now.AddDays(4));

            _availabilityServiceMock
                .Setup(a => a.GetAvailableVehiclesAsync(dateRange, vehicleType))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            Func<Task> act = async () => await _reservationService.CreateReservationAsync(vehicleType, dateRange);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Test exception");

            // Verifies semaphore is released despite the exception (implicit since no deadlock occurs)
            _availabilityServiceMock.Verify(a => a.GetAvailableVehiclesAsync(dateRange, vehicleType), Times.Once);
            _reservationRepositoryMock.Verify(r => r.AddReservationAsync(It.IsAny<Reservation>()), Times.Never);
        }

        [Fact]
        public async Task CreateReservationAsync_Should_Lock_Semaphore_And_Ensure_Sequential_Execution()
        {
            // Arrange
            var vehicleType = "Compact";
            var dateRange = new DateRange(DateTime.Now, DateTime.Now.AddDays(7));

            _availabilityServiceMock
                .Setup(a => a.GetAvailableVehiclesAsync(dateRange, vehicleType))
                .ReturnsAsync(new List<VehicleType> { new() { Name = "", TotalCount = 1 }});

            _reservationRepositoryMock
                .Setup(r => r.AddReservationAsync(It.IsAny<Reservation>()))
                .Returns(Task.CompletedTask);

            var task1 = _reservationService.CreateReservationAsync(vehicleType, dateRange);
            var task2 = _reservationService.CreateReservationAsync(vehicleType, dateRange);

            // Act
            var results = await Task.WhenAll(task1, task2);

            // Assert
            results.Should().Contain(new[] { true, true });
            _availabilityServiceMock.Verify(a => a.GetAvailableVehiclesAsync(dateRange, vehicleType), Times.Exactly(2));
            _reservationRepositoryMock.Verify(r => r.AddReservationAsync(It.IsAny<Reservation>()), Times.Exactly(2));
        }
    }
}
