using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace sistema.Models
{
    public class EmpleadoBaseViewModel
    {
        public Empleado Empleado { get; set; } = new Empleado();
        public IFormFile imagen { get; set; }
        public IFormFile FirmaEmpleadoImagen { get; set; }

        public SelectList ListaSucursales { get; set; }
        public bool Modificar { get; set; }

        // Nueva propiedad: Listado de especialidades
        public IEnumerable<SelectListItem> Especialidades { get; set; }

        // Nueva propiedad: ID de la especialidad seleccionada
        public int? EspecialidadIdSeleccionada { get; set; }

          // ✅ Pivot desde KO (no depende del submit del <form>)
        // Valores esperados según lo que definimos: "Empleado" | "Medico"
        public string TipoEmpleadoFormulario { get; set; } = "Empleado";

        public void Init(ISucursal _sucursalRepository, IEspecialidad _especialidadRepository)
        {
            // Inicializar sucursales
            ListaSucursales = new SelectList(_sucursalRepository.GetList(), "Id", "NombreSucursal");

            // Inicializar especialidades
            Especialidades = _especialidadRepository.GetAll()
                .Select(e => new SelectListItem 
                {
                    Value = e.Id.ToString(),
                    Text = e.NombreEspecialidad
                });
        }

        public int Id
        {
            get { return Empleado.Id; }
            set { Empleado.Id = value; }
        }
    }
}
