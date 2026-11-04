using Hotel.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Web.Repositorios;

public class HabitacionRepositorio : IHabitacionRepositorio
{
    private readonly HotelContext _context;

    public HabitacionRepositorio(HotelContext context)
    {
        _context = context;
    }

    public async Task<List<Habitacion>> ObtenerHabitacionesActivasAsync(int? tipoId)
    {
        var query = _context.Habitacions
            .Include(h => h.IdTipoHabitacionNavigation)
            .Where(h => h.Activo)
            .AsNoTracking();

        if (tipoId.HasValue)
        {
            query = query.Where(h => h.IdTipoHabitacion == tipoId.Value);
        }

        return await query.OrderBy(h => h.Numero).ToListAsync();
    }

    public async Task<Habitacion?> ObtenerPorIdAsync(int id)
    {
        return await _context.Habitacions
            .Include(h => h.IdTipoHabitacionNavigation)
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.IdHabitacion == id);
    }

    public async Task<bool> ExisteNumeroAsync(string numero, int? idExcluir = null)
    {
        return await _context.Habitacions.AnyAsync(h => h.Numero == numero && (!idExcluir.HasValue || h.IdHabitacion != idExcluir.Value));
    }

    public async Task AgregarAsync(Habitacion habitacion)
    {
        habitacion.Activo = true;
        await _context.Habitacions.AddAsync(habitacion);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Habitacion habitacion)
    {
        _context.Habitacions.Update(habitacion);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarLogicoAsync(int id)
    {
        var hab = await _context.Habitacions.FindAsync(id);
        if (hab != null)
        {
            hab.Activo = false;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<TipoHabitacion>> ObtenerTiposAsync()
    {
        return await _context.TipoHabitacions.AsNoTracking().ToListAsync();
    }
}