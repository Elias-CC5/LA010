using Hotel.Web.Models;
using Hotel.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hotel.Web.Controllers;

public class HabitacionesController : Controller
{
    private readonly IHabitacionRepositorio _repo;

    public HabitacionesController(IHabitacionRepositorio repo)
    {
        _repo = repo;
    }

    public async Task<IActionResult> Index(int? tipoId)
    {
        var habitaciones = await _repo.ObtenerHabitacionesActivasAsync(tipoId);
        var tipos = await _repo.ObtenerTiposAsync();

        ViewBag.Tipos = new SelectList(tipos, "IdTipoHabitacion", "Nombre", tipoId);
        return View(habitaciones);
    }

    public async Task<IActionResult> Details(int id)
    {
        var habitacion = await _repo.ObtenerPorIdAsync(id);
        if (habitacion == null) return NotFound();
        return View(habitacion);
    }

    public async Task<IActionResult> Create()
    {
        var tipos = await _repo.ObtenerTiposAsync();
        ViewBag.IdTipoHabitacion = new SelectList(tipos, "IdTipoHabitacion", "Nombre");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Habitacion habitacion)
    {
        ModelState.Remove("IdTipoHabitacionNavigation");

        if (await _repo.ExisteNumeroAsync(habitacion.Numero))
        {
            ModelState.AddModelError("Numero", "El número de habitación ya existe.");
        }

        if (ModelState.IsValid)
        {
            await _repo.AgregarAsync(habitacion);
            TempData["Mensaje"] = "Habitación creada exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        var tipos = await _repo.ObtenerTiposAsync();
        ViewBag.IdTipoHabitacion = new SelectList(tipos, "IdTipoHabitacion", "Nombre", habitacion.IdTipoHabitacion);
        return View(habitacion);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var habitacion = await _repo.ObtenerPorIdAsync(id);
        if (habitacion == null) return NotFound();

        var tipos = await _repo.ObtenerTiposAsync();
        ViewBag.IdTipoHabitacion = new SelectList(tipos, "IdTipoHabitacion", "Nombre", habitacion.IdTipoHabitacion);
        return View(habitacion);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Habitacion habitacion)
    {
        if (id != habitacion.IdHabitacion) return NotFound();
        ModelState.Remove("IdTipoHabitacionNavigation");

        if (await _repo.ExisteNumeroAsync(habitacion.Numero, habitacion.IdHabitacion))
        {
            ModelState.AddModelError("Numero", "El número de habitación ya existe.");
        }

        if (ModelState.IsValid)
        {
            await _repo.ActualizarAsync(habitacion);
            TempData["Mensaje"] = "Habitación actualizada exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        var tipos = await _repo.ObtenerTiposAsync();
        ViewBag.IdTipoHabitacion = new SelectList(tipos, "IdTipoHabitacion", "Nombre", habitacion.IdTipoHabitacion);
        return View(habitacion);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var habitacion = await _repo.ObtenerPorIdAsync(id);
        if (habitacion == null) return NotFound();
        return View(habitacion);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _repo.EliminarLogicoAsync(id);
        TempData["Mensaje"] = "Habitación eliminada (desactivada) correctamente.";
        return RedirectToAction(nameof(Index));
    }
}