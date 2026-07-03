using Database.Shared.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace sistema.Models
{
    public class EmergenciaViewModel
    {
        public bool HabilitarEdicion { get; set; }
        public string EmergenciasFechaRegistro { get; set; }
        public int? EmergenciaId { get; set; }
        public int? SucursalId { get; set; }
        public int? HospitalizacionId { get; set; }
        public string HabitacionNombre { get; set; }
        public SelectList ListaSucursales { get; set; }
        //Encabezado
        public int? CodigoVendedor { get; set; }
        public string PacienteNombre { get; set; }
        public int? PacienteId { get; set; }
        public string Responsable { get; set; }
        public string PacienteNit { get; set; }
        public string PacienteDireccion { get; set; }
        /// <summary>
        /// TRUE cuando se hospitaliza a la persona
        /// </summary>
        public bool EmergenciaIngresada { get; set; }
        public string EmergenciaValorTotal { get; set; }

        public bool Pagado { get; set; }

        public string Observaciones { get; set; }


        //Detalle
        public List<EmergenciaProductoAgregadoViewModel> Productos { get; set; }
        public List<EmergenciaServicioAgregadoViewModel> Servicios { get; set; }
        public List<EmergenciaExamenAgregadoViewModel> Examenes { get; set; }


        public void Init(ISucursal _sucursalRepository)
        {
            ListaSucursales = new SelectList(_sucursalRepository.GetList(), "Id", "NombreSucursal");
        }
    }
}
