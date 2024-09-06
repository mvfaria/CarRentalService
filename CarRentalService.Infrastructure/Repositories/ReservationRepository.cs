using CarRentalService.Application.Interfaces;
using CarRentalService.Domain.Entities;

namespace CarRentalService.Infrastructure.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly List<Reservation> _reservations = new();

    public Task<List<Reservation>> GetReservationsAsync() => Task.FromResult(_reservations);

    public Task AddReservationAsync(Reservation reservation)
    {
        _reservations.Add(reservation);
        return Task.CompletedTask;
    }
}