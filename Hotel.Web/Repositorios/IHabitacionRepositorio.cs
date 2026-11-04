using Hotel.Web.Models;

namespace Hotel.Web.Repositorios;

public interface IHabitacionRepositorio
{
    Task<List<Habitacion>> ObtenerHabitacionesActivasAsync(int? tipoId);
    Task<Habitacion?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteNumeroAsync(string numero, int? idExcluir = null);
    Task AgregarAsync(Habitacion habitacion);
    Task ActualizarAsync(Habitacion habitacion);
    Task EliminarLogicoAsync(int id);
    Task<List<TipoHabitacion>> ObtenerTiposAsync();
}