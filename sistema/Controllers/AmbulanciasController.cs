using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Database.Shared;
using Database.Shared.Models;

public class AmbulanciasController : Controller
{
    private readonly Context _context;

    public AmbulanciasController(Context context)
    {
        _context = context;
    }

    public IActionResult Nueva()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Nueva(Ambulancia ambulancia, int? hospitalizacionId)
    {
        if (hospitalizacionId.HasValue)
            ambulancia.HospitalizacionId = hospitalizacionId.Value;

        _context.Ambulancias.Add(ambulancia);
        await _context.SaveChangesAsync();

        return ambulancia.TipoViaje == "IGSS"
            ? RedirectToAction("ListaIgss")
            : RedirectToAction("ListaPrivadas");
    }



    [HttpGet]
    public async Task<IActionResult> ObtenerPorHospitalizacion(int hospitalizacionId)
    {
        var ambulancias = await _context.Ambulancias
            .Where(a => a.HospitalizacionId == hospitalizacionId)
            .Select(a => new
            {
                a.Id,
                a.TipoViaje,
                a.Precio
            })
            .ToListAsync();

        return Json(ambulancias);
    }

    public async Task<IActionResult> ListaPrivadas()
    {
        var privadas = await _context.Ambulancias
            .Where(a => a.TipoViaje == "Privado")
            .ToListAsync();
        return View("Lista", privadas);
    }

    public async Task<IActionResult> ListaIgss()
    {
        var igss = await _context.Ambulancias
            .Where(a => a.TipoViaje == "IGSS")
            .ToListAsync();
        return View("Lista", igss);
    }

    public async Task<IActionResult> Detalle(int id)
    {
        var ambulancia = await _context.Ambulancias.FirstOrDefaultAsync(a => a.Id == id);
        if (ambulancia == null) return NotFound();

        return View("Detalle", ambulancia);
    }



}
