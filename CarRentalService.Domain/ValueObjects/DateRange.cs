namespace CarRentalService.Domain.ValueObjects;

public class DateRange
{
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }

    public DateRange(DateTime startDate, DateTime endDate)
    {
        // We should validate the dates here
        StartDate = startDate;
        EndDate = endDate;
    }

    public bool Overlaps(DateRange other) =>
        StartDate < other.EndDate && EndDate > other.StartDate;
}