using Hotel.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Web.Repositorios;

public class ReservaRepositorio : IReservaRepositorio
{
    private readonly HotelContext _context;

    public ReservaRepositorio(HotelContext context)
    {
        _context = context;
    }

    public async Task<List<Habitacion>> ObtenerHabitacionesDisponiblesAsync(DateTime ingreso, DateTime salida)
    {
        var ingresoDate = DateOnly.FromDateTime(ingreso);
        var salidaDate = DateOnly.FromDateTime(salida);

        return await _context.Habitacions
            .Include(h => h.IdTipoHabitacionNavigation)
            .Where(h => h.Activo && !_context.Reservas.Any(r =>
                r.IdHabitacion == h.IdHabitacion &&
                r.Estado != "Cancelada" &&
                r.FechaIngreso < salidaDate &&
                r.FechaSalida > ingresoDate))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> HabitacionEstaReservadaAsync(int idHabitacion, DateTime ingreso, DateTime salida)
    {
        var ingresoDate = DateOnly.FromDateTime(ingreso);
        var salidaDate = DateOnly.FromDateTime(salida);

        return await _context.Reservas.AnyAsync(r =>
            r.IdHabitacion == idHabitacion &&
            r.Estado != "Cancelada" &&
            r.FechaIngreso < salidaDate &&
            r.FechaSalida > ingresoDate);
    }

    public async Task CrearReservaAsync(Reserva reserva)
    {
        reserva.Estado = "Pendiente";
        await _context.Reservas.AddAsync(reserva);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Reserva>> ObtenerTodasAsync()
    {
        return await _context.Reservas
            .Include(r => r.IdHuespedNavigation)
            .Include(r => r.IdHabitacionNavigation)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task CancelarReservaAsync(int id)
    {
        var reserva = await _context.Reservas.FindAsync(id);
        if (reserva != null && (reserva.Estado == "Pendiente" || reserva.Estado == "Confirmada"))
        {
            reserva.Estado = "Cancelada";
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Huesped>> ObtenerHuespedesActivosAsync()
    {
        return await _context.Huespeds.Where(h => h.Activo).AsNoTracking().ToListAsync();
    }
}