using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Models; // Espacio de nombres de tus entidades originales
using TuProyecto.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;

public class OrganizacionController : Controller
{
    private readonly Database.Shared.Context _dbContext;

    public OrganizacionController(Database.Shared.Context dbContext)
    {
        _dbContext = dbContext;
    }

    // Muestra el formulario (Crea nuevo o edita existente)
    public async Task<IActionResult> Index()
    {
        // Cargamos todos los departamentos con sus hijos y nietos
        var departamentos = await _dbContext.DepartamentosOrg
            .Include(d => d.Unidades)
                .ThenInclude(u => u.Secciones)
            .Where(d => !d.Eliminada) // Filtro opcional de borrado lógico
            .ToListAsync();

        return View(departamentos);
    }
    public async Task<IActionResult> Editor(int? id)
    {
        var model = new OrganizacionVM();

        if (id.HasValue)
        {
            var depto = await _dbContext.DepartamentosOrg
                .Include(d => d.Unidades)
                .ThenInclude(u => u.Secciones)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (depto != null)
            {
                model.Id = depto.Id;
                model.Nombre = depto.Nombre;
                model.Unidades = depto.Unidades.Where(u => !u.Eliminada).Select(u => new UnidadVM
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Secciones = u.Secciones.Where(s => !s.Eliminada).Select(s => new SeccionVM
                    {
                        Id = s.Id,
                        Nombre = s.Nombre
                    }).ToList()
                }).ToList();
            }
        }

        // ¡IMPORTANTE! Devolver PartialView
        return PartialView("_EditorModal", model);
    }

    [HttpPost]
    public async Task<IActionResult> Guardar(OrganizacionVM model)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("_EditorModal", model);
        }

        var depto = await _dbContext.DepartamentosOrg
            .Include(d => d.Unidades)
            .ThenInclude(u => u.Secciones)
            .FirstOrDefaultAsync(d => d.Id == model.Id) ?? new DepartamentoOrg();

        depto.Nombre = model.Nombre;

        // 1. LÓGICA DE BORRADO: Unidades
        // Obtenemos los IDs que vienen del formulario (los que > 0)
        var unidadesEnFormulario = model.Unidades.Select(u => u.Id).Where(id => id > 0).ToList();

        // Si una unidad está en la BD pero no en el formulario, se marca como eliminada
        foreach (var dbUnidad in depto.Unidades.Where(u => !u.Eliminada))
        {
            if (!unidadesEnFormulario.Contains(dbUnidad.Id))
            {
                dbUnidad.Eliminada = true;
                // Opcional: También marcamos en cascada sus secciones como eliminadas
                foreach (var s in dbUnidad.Secciones) s.Eliminada = true;
            }
        }

        // 2. PROCESAR UNIDADES (Actualizar o Crear)
        foreach (var uVM in model.Unidades)
        {
            var unidad = depto.Unidades.FirstOrDefault(u => u.Id == uVM.Id && u.Id != 0);
            if (unidad == null)
            {
                unidad = new UnidadOrg();
                depto.Unidades.Add(unidad);
            }
            unidad.Nombre = uVM.Nombre;

            // 3. LÓGICA DE BORRADO: Secciones de esta unidad
            var seccionesEnFormulario = uVM.Secciones.Select(s => s.Id).Where(id => id > 0).ToList();

            foreach (var dbSeccion in unidad.Secciones.Where(s => !s.Eliminada))
            {
                if (!seccionesEnFormulario.Contains(dbSeccion.Id))
                {
                    dbSeccion.Eliminada = true;
                }
            }

            // 4. PROCESAR SECCIONES (Actualizar o Crear)
            foreach (var sVM in uVM.Secciones)
            {
                var seccion = unidad.Secciones.FirstOrDefault(s => s.Id == sVM.Id && s.Id != 0);
                if (seccion == null)
                {
                    seccion = new SeccionOrg();
                    unidad.Secciones.Add(seccion);
                }
                seccion.Nombre = sVM.Nombre;
            }
        }

        if (depto.Id == 0) _dbContext.DepartamentosOrg.Add(depto);

        await _dbContext.SaveChangesAsync();

        return Json(new { success = true });
    }
}