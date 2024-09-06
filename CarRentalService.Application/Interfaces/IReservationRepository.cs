using CarRentalService.Domain.Entities;

namespace CarRentalService.Application.Interfaces;

public interface IReservationRepository
{
    Task<List<Reservation>> GetReservationsAsync();
    Task AddReservationAsync(Reservation reservation);
}