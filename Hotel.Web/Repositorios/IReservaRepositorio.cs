using Hotel.Web.Models;

namespace Hotel.Web.Repositorios;

public interface IReservaRepositorio
{
    Task<List<Habitacion>> ObtenerHabitacionesDisponiblesAsync(DateTime ingreso, DateTime salida);
    Task<bool> HabitacionEstaReservadaAsync(int idHabitacion, DateTime ingreso, DateTime salida);
    Task CrearReservaAsync(Reserva reserva);
    Task<List<Reserva>> ObtenerTodasAsync();
    Task CancelarReservaAsync(int id);
    Task<List<Huesped>> ObtenerHuespedesActivosAsync();
}