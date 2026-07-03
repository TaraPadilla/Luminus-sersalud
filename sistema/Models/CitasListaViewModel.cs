using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.Models;
using Database.Shared.IRepository;
using Database.Shared.Paginacion;

namespace sistema.Models
{
    public class CitasListaViewModel
    {
        public int? EmpleadoId { get; set; }
        public SelectList MedicosSelectList { get; set; }
        public int? SucursalId { get; set; }
        public SelectList SucursalesSelectList { get; set; }
        public PaginacionList<Citas> ListaCitas { get; set; }

        public void Init(IEmpleado empleadoRepository,ISucursal sucursalRepository)
        {
            MedicosSelectList = new SelectList(empleadoRepository.GetList(), "Id", "NombreYApellidos");
            SucursalesSelectList = new SelectList(sucursalRepository.GetList(), "Id", "NombreSucursal");
        }
    }
}