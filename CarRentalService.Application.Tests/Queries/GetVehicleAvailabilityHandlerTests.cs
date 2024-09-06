using CarRentalService.Application.DTOs;
using CarRentalService.Application.Interfaces;
using CarRentalService.Application.Queries;
using CarRentalService.Domain.Entities;
using CarRentalService.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace CarRentalService.Application.Tests.Queries;

public class GetVehicleAvailabilityHandlerTests
{
    private readonly Mock<IAvailabilityService> _availabilityServiceMock;
    private readonly GetVehicleAvailabilityHandler _handler;

    public GetVehicleAvailabilityHandlerTests()
    {
        _availabilityServiceMock = new Mock<IAvailabilityService>();
        _handler = new GetVehicleAvailabilityHandler(_availabilityServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_AvailableVehicles_When_RequestedVehicleTypes_Are_Provided()
    {
        // Arrange
        var query = new GetVehicleAvailabilityQuery
        {
            PickupDate = new DateTime(2024, 9, 1),
            ReturnDate = new DateTime(2024, 9, 10),
            VehicleTypes = new List<string> { "SUV", "Compact" }
        };

        var availableVehicles = new List<VehicleType>
        {
            new VehicleType { Name = "SUV", TotalCount = 2 },
            new VehicleType { Name = "Compact", TotalCount = 3 }
        };

        _availabilityServiceMock
            .Setup(x => x.GetAvailableVehiclesAsync(It.IsAny<DateRange>(), It.IsAny<List<string>>()))
            .ReturnsAsync(availableVehicles);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(v => v.Name == "SUV" && v.AvailableCount == 2);
        result.Should().Contain(v => v.Name == "Compact" && v.AvailableCount == 3);
    }

    [Fact]
    public async Task Handle_Should_Return_EmptyList_When_No_Vehicles_Available()
    {
        // Arrange
        var query = new GetVehicleAvailabilityQuery
        {
            PickupDate = new DateTime(2024, 9, 1),
            ReturnDate = new DateTime(2024, 9, 10),
            VehicleTypes = new List<string> { "SUV", "Compact" }
        };

        var availableVehicles = new List<VehicleType>(); // No available vehicles

        _availabilityServiceMock
            .Setup(x => x.GetAvailableVehiclesAsync(It.IsAny<DateRange>(), It.IsAny<List<string>>()))
            .ReturnsAsync(availableVehicles);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_Use_All_VehicleTypes_If_No_Types_Are_Requested()
    {
        // Arrange
        var query = new GetVehicleAvailabilityQuery
        {
            PickupDate = new DateTime(2024, 9, 1),
            ReturnDate = new DateTime(2024, 9, 10),
            VehicleTypes = new List<string>() // Empty list means all vehicle types should be considered
        };

        var availableVehicles = new List<VehicleType>
        {
            new VehicleType { Name = "SUV", TotalCount = 2 },
            new VehicleType { Name = "Compact", TotalCount = 3 },
            new VehicleType { Name = "Van", TotalCount = 1 }
        };

        _availabilityServiceMock
            .Setup(x => x.GetAvailableVehiclesAsync(It.IsAny<DateRange>(), It.IsAny<List<string>>()))
            .ReturnsAsync(availableVehicles);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(v => v.Name == "SUV" && v.AvailableCount == 2);
        result.Should().Contain(v => v.Name == "Compact" && v.AvailableCount == 3);
        result.Should().Contain(v => v.Name == "Van" && v.AvailableCount == 1);
    }

    [Fact]
    public async Task Handle_Should_Map_To_VehicleTypeDto()
    {
        // Arrange
        var query = new GetVehicleAvailabilityQuery
        {
            PickupDate = new DateTime(2024, 9, 1),
            ReturnDate = new DateTime(2024, 9, 10),
            VehicleTypes = new List<string> { "SUV" }
        };

        var availableVehicles = new List<VehicleType>
        {
            new VehicleType { Name = "SUV", TotalCount = 2 }
        };

        _availabilityServiceMock
            .Setup(x => x.GetAvailableVehiclesAsync(It.IsAny<DateRange>(), It.IsAny<List<string>>()))
            .ReturnsAsync(availableVehicles);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Should().BeOfType<VehicleTypeDto>();
        result.First().Name.Should().Be("SUV");
        result.First().AvailableCount.Should().Be(2);
    }
}