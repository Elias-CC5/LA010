using Hotel.Web.Models;
using Hotel.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hotel.Web.Controllers;

public class ReservasController : Controller
{
    private readonly IReservaRepositorio _reservaRepo;
    private readonly IHabitacionRepositorio _habitacionRepo;

    public ReservasController(IReservaRepositorio reservaRepo, IHabitacionRepositorio habitacionRepo)
    {
        _reservaRepo = reservaRepo;
        _habitacionRepo = habitacionRepo;
    }

    public async Task<IActionResult> Index()
    {
        var reservas = await _reservaRepo.ObtenerTodasAsync();
        return View(reservas);
    }

    public async Task<IActionResult> Disponibles(DateTime? ingreso, DateTime? salida)
    {
        if (ingreso.HasValue && salida.HasValue)
        {
            if (salida <= ingreso)
            {
                ModelState.AddModelError("", "La fecha de salida debe ser posterior a la fecha de ingreso.");
                return View(new List<Habitacion>());
            }

            ViewBag.Ingreso = ingreso;
            ViewBag.Salida = salida;
            ViewBag.Noches = (salida.Value - ingreso.Value).Days;

            var habitacionesLibres = await _reservaRepo.ObtenerHabitacionesDisponiblesAsync(ingreso.Value, salida.Value);
            return View(habitacionesLibres);
        }

        return View(new List<Habitacion>());
    }

    public async Task<IActionResult> Create()
    {
        var huespedes = await _reservaRepo.ObtenerHuespedesActivosAsync();
        ViewBag.IdHuesped = new SelectList(huespedes, "IdHuesped", "Nombre");
        ViewBag.IdHabitacion = new SelectList(await _habitacionRepo.ObtenerHabitacionesActivasAsync(null), "IdHabitacion", "Numero");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Reserva reserva)
    {
        ModelState.Remove("IdHabitacionNavigation");
        ModelState.Remove("IdHuespedNavigation");

        if (reserva.FechaSalida <= reserva.FechaIngreso)
        {
            ModelState.AddModelError("FechaSalida", "La fecha de salida debe ser posterior al ingreso.");
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        if (reserva.FechaIngreso < today)
        {
            ModelState.AddModelError("FechaIngreso", "La fecha de ingreso no puede ser anterior a hoy.");
        }

        var dtIngreso = reserva.FechaIngreso.ToDateTime(TimeOnly.MinValue);
        var dtSalida = reserva.FechaSalida.ToDateTime(TimeOnly.MinValue);

        if (await _reservaRepo.HabitacionEstaReservadaAsync(reserva.IdHabitacion, dtIngreso, dtSalida))
        {
            ModelState.AddModelError("", "La habitación no está disponible en las fechas seleccionadas.");
        }

        if (ModelState.IsValid)
        {
            await _reservaRepo.CrearReservaAsync(reserva);
            TempData["Mensaje"] = "Reserva creada exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        var huespedes = await _reservaRepo.ObtenerHuespedesActivosAsync();
        ViewBag.IdHuesped = new SelectList(huespedes, "IdHuesped", "Nombre", reserva.IdHuesped);
        ViewBag.IdHabitacion = new SelectList(await _habitacionRepo.ObtenerHabitacionesActivasAsync(null), "IdHabitacion", "Numero", reserva.IdHabitacion);
        return View(reserva);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        await _reservaRepo.CancelarReservaAsync(id);
        TempData["Mensaje"] = "Reserva cancelada correctamente.";
        return RedirectToAction(nameof(Index));
    }
}