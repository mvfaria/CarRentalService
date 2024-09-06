using CarRentalService.Application.Interfaces;
using CarRentalService.Application.Services;
using CarRentalService.Domain.Entities;
using CarRentalService.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace CarRentalService.Application.Tests.Services;

public class AvailabilityServiceTests
{
    private readonly Mock<IReservationRepository> _reservationRepositoryMock;
    private readonly Mock<IVehicleTypeRepository> _vehicleTypeRepositoryMock;
    private readonly AvailabilityService _availabilityService;

    public AvailabilityServiceTests()
    {
        _reservationRepositoryMock = new Mock<IReservationRepository>();
        _vehicleTypeRepositoryMock = new Mock<IVehicleTypeRepository>();
        _availabilityService = new AvailabilityService(_reservationRepositoryMock.Object, _vehicleTypeRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAvailableVehiclesAsync_Should_Return_All_VehicleTypes_When_No_Types_Requested()
    {
        // Arrange
        var dateRange = new DateRange(new DateTime(2024, 9, 1), new DateTime(2024, 9, 10));
        var vehicleTypes = new List<VehicleType>
        {
            new VehicleType { Name = "Compact", TotalCount = 5 },
            new VehicleType { Name = "SUV", TotalCount = 3 }
        };

        var reservations = new List<Reservation>(); // No reservations

        ReposSetup(reservations, vehicleTypes);

        // Act
        var availableVehicles = await _availabilityService.GetAvailableVehiclesAsync(dateRange, new List<string>());

        // Assert
        availableVehicles.Should().HaveCount(2);
        availableVehicles.Should().Contain(v => v.Name == "Compact" && v.TotalCount == 5);
        availableVehicles.Should().Contain(v => v.Name == "SUV" && v.TotalCount == 3);
    }

    [Fact]
    public async Task GetAvailableVehiclesAsync_Should_Return_Only_Requested_Types()
    {
        // Arrange
        var dateRange = new DateRange(new DateTime(2024, 9, 1), new DateTime(2024, 9, 10));
        var vehicleTypes = new List<VehicleType>
        {
            new VehicleType { Name = "Compact", TotalCount = 5 },
            new VehicleType { Name = "SUV", TotalCount = 3 }
        };

        var reservations = new List<Reservation>();

        _reservationRepositoryMock.Setup(x => x.GetReservationsAsync())
            .ReturnsAsync(reservations);

        _vehicleTypeRepositoryMock.Setup(x => x.GetVehicleTypesAsync())
            .ReturnsAsync(vehicleTypes);

        // Act
        var availableVehicles = await _availabilityService.GetAvailableVehiclesAsync(dateRange, new List<string> { "SUV" });

        // Assert
        availableVehicles.Should().HaveCount(1);
        availableVehicles.First().Name.Should().Be("SUV");
        availableVehicles.First().TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task GetAvailableVehiclesAsync_Should_Adjust_Available_Count_Based_On_Reservations()
    {
        // Arrange
        var dateRange = new DateRange(new DateTime(2024, 9, 1), new DateTime(2024, 9, 10));
        var vehicleTypes = new List<VehicleType>
        {
            new VehicleType { Name = "Compact", TotalCount = 5 }
        };

        var reservations = new List<Reservation>
        {
            new Reservation { VehicleType = "Compact", PickupDate = new DateTime(2024, 9, 2), ReturnDate = new DateTime(2024, 9, 5) },
            new Reservation { VehicleType = "Compact", PickupDate = new DateTime(2024, 9, 8), ReturnDate = new DateTime(2024, 9, 9) }
        };

        _reservationRepositoryMock.Setup(x => x.GetReservationsAsync())
            .ReturnsAsync(reservations);

        _vehicleTypeRepositoryMock.Setup(x => x.GetVehicleTypesAsync())
            .ReturnsAsync(vehicleTypes);

        // Act
        var availableVehicles = await _availabilityService.GetAvailableVehiclesAsync(dateRange, new List<string> { "Compact" });

        // Assert
        availableVehicles.Should().HaveCount(1);
        availableVehicles.First().Name.Should().Be("Compact");
        availableVehicles.First().TotalCount.Should().Be(3); // 5 total - 2 reservations
    }

    [Fact]
    public async Task GetAvailableVehiclesAsync_Should_Only_Return_Types_With_Available_Vehicles()
    {
        // Arrange
        var dateRange = new DateRange(new DateTime(2024, 9, 1), new DateTime(2024, 9, 10));
        var vehicleTypes = new List<VehicleType>
        {
            new VehicleType { Name = "Compact", TotalCount = 5 },
            new VehicleType { Name = "SUV", TotalCount = 3 }
        };

        var reservations = new List<Reservation>
        {
            new Reservation { VehicleType = "SUV", PickupDate = new DateTime(2024, 9, 1), ReturnDate = new DateTime(2024, 9, 10) },
            new Reservation { VehicleType = "SUV", PickupDate = new DateTime(2024, 9, 1), ReturnDate = new DateTime(2024, 9, 10) },
            new Reservation { VehicleType = "SUV", PickupDate = new DateTime(2024, 9, 1), ReturnDate = new DateTime(2024, 9, 10) }
        };

        _reservationRepositoryMock.Setup(x => x.GetReservationsAsync())
            .ReturnsAsync(reservations);

        _vehicleTypeRepositoryMock.Setup(x => x.GetVehicleTypesAsync())
            .ReturnsAsync(vehicleTypes);

        // Act
        var availableVehicles = await _availabilityService.GetAvailableVehiclesAsync(dateRange, new List<string>());

        // Assert
        availableVehicles.Should().HaveCount(1); // Only Compact is available
        availableVehicles.First().Name.Should().Be("Compact");
        availableVehicles.First().TotalCount.Should().Be(5);
    }

    [Fact]
    public async Task GetAvailableVehiclesAsync_Should_Handle_Single_Requested_Vehicle_Type()
    {
        // Arrange
        var dateRange = new DateRange(new DateTime(2024, 9, 1), new DateTime(2024, 9, 10));
        var vehicleTypes = new List<VehicleType>
        {
            new VehicleType { Name = "SUV", TotalCount = 3 }
        };

        var reservations = new List<Reservation>();

        _reservationRepositoryMock.Setup(x => x.GetReservationsAsync())
            .ReturnsAsync(reservations);

        _vehicleTypeRepositoryMock.Setup(x => x.GetVehicleTypesAsync())
            .ReturnsAsync(vehicleTypes);

        // Act
        var availableVehicles = await _availabilityService.GetAvailableVehiclesAsync(dateRange, "SUV");

        // Assert
        availableVehicles.Should().HaveCount(1);
        availableVehicles.First().Name.Should().Be("SUV");
        availableVehicles.First().TotalCount.Should().Be(3);
    }
        
    private void ReposSetup(List<Reservation> reservations, List<VehicleType> vehicleTypes)
    {
        _reservationRepositoryMock.Setup(x => x.GetReservationsAsync())
            .ReturnsAsync(reservations);

        _vehicleTypeRepositoryMock.Setup(x => x.GetVehicleTypesAsync())
            .ReturnsAsync(vehicleTypes);
    }
}