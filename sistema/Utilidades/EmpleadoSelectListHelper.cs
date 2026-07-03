using System.Collections.Generic;
using System.Linq;
using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace sistema.Utilidades
{
    public static class EmpleadoSelectListHelper
    {
        public static SelectList Crear(IEnumerable<Empleado> empleados)
        {
            var lista = (empleados ?? Enumerable.Empty<Empleado>())
                .Where(e => e != null
                            && !e.Eliminado
                            && !string.IsNullOrWhiteSpace(e.NombreYApellidos))
                .OrderBy(e => e.Nombre)
                .ThenBy(e => e.Apellido)
                .ToList();

            return SelectListHelper.Create(lista, "Id", "NombreYApellidos");
        }

        public static SelectList Crear(IEmpleado empleadoRepository)
            => Crear(empleadoRepository?.GetList());
    }
}
