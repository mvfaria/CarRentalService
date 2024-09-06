using CarRentalService.Domain.ValueObjects;
using FluentAssertions;

namespace CarRentalService.Domain.Tests.ValueObjects;

public class DateRangeTests
{
    [Fact]
    public void DateRange_Should_Create_With_Valid_Dates()
    {
        // Arrange
        var startDate = new DateTime(2024, 9, 1);
        var endDate = new DateTime(2024, 9, 10);

        // Act
        var dateRange = new DateRange(startDate, endDate);

        // Assert
        dateRange.Should().NotBeNull();
        dateRange.StartDate.Should().Be(startDate);
        dateRange.EndDate.Should().Be(endDate);
    }

    [Fact]
    public void Overlaps_Should_Return_True_When_DateRanges_Overlap()
    {
        // Arrange
        var range1 = new DateRange(new DateTime(2024, 9, 1), new DateTime(2024, 9, 10));
        var range2 = new DateRange(new DateTime(2024, 9, 5), new DateTime(2024, 9, 15));

        // Act
        var result = range1.Overlaps(range2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Overlaps_Should_Return_False_When_DateRanges_Do_Not_Overlap()
    {
        // Arrange
        var range1 = new DateRange(new DateTime(2024, 9, 1), new DateTime(2024, 9, 10));
        var range2 = new DateRange(new DateTime(2024, 9, 11), new DateTime(2024, 9, 20));

        // Act
        var result = range1.Overlaps(range2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Overlaps_Should_Return_True_When_DateRanges_Touch_But_Do_Not_Overlap()
    {
        // Arrange
        var range1 = new DateRange(new DateTime(2024, 9, 1), new DateTime(2024, 9, 10));
        var range2 = new DateRange(new DateTime(2024, 9, 10), new DateTime(2024, 9, 15));

        // Act
        var result = range1.Overlaps(range2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Overlaps_Should_Return_True_When_One_DateRange_Completely_Within_Another()
    {
        // Arrange
        var range1 = new DateRange(new DateTime(2024, 9, 1), new DateTime(2024, 9, 30));
        var range2 = new DateRange(new DateTime(2024, 9, 10), new DateTime(2024, 9, 20));

        // Act
        var result = range1.Overlaps(range2);

        // Assert
        result.Should().BeTrue();
    }

    // Placeholder for future validation logic test
    [Fact(Skip = "Date validation not implemented yet")]
    public void DateRange_Should_Throw_If_EndDate_Before_StartDate()
    {
        // Arrange
        var startDate = new DateTime(2024, 9, 10);
        var endDate = new DateTime(2024, 9, 1);

        // Act
        Action act = () => new DateRange(startDate, endDate);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("End date must be after start date");
    }
}