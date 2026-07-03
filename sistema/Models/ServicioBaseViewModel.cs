using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;

namespace sistema.Models
{
    public class ServicioBaseViewModel
    {
        public int? Id { get; set; }
        public string CodigoInterno { get; set; }
        public string NombreServicio { get; set; }
        public string Descripcion { get; set; }

        //Duracion del servicio
        public int DuracionHoras { get; set; }
        public int DuracionMinutos { get; set; }

        //Precio mostrar en reservas
        public int? PrecioMostrarId { get; set; }

        public List<InsumoServicioBaseViewModel> InsumosUtilizados { get; set; }
        public List<PrecioServicioBaseViewModel> Precios { get; set; }
        public List<ServicioSucursalViewModel> Sucursales { get; set; }

        public List<InsumosAsignadosExamen> InsumosAsignadosExamen { get; set; }


        //public VentaServicio ventaServicio { get; set; } = new VentaServicio();
        //public IList<DetalleServicio> DetalleServicios { get; set; }
        public bool Modificar { get; set; }
    }
    public class InsumoExistenteServicioBaseViewModel
    {
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public decimal precioUnidadCompra { get; set; }
    }
    public class InsumoEquivalenciaUnidadBaseViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
    public class InsumoServicioBaseViewModel
    {
        public int? Id { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public int UnidadMedidaVentaId { get; set; }
        public string UnidadMedidaVentaNombre { get; set; }
        public decimal CantidadUtilizada { get; set; }
        public bool Nuevo { get; set; }
    }
    public class PrecioServicioBaseViewModel
    {
        public bool Activar { get; set; }
        public int PrecioId { get; set; }
        public string PrecioNombre { get; set; }
        public decimal PrecioValor { get; set; }
    }
    public class InsumosAsignadosExamen
    {
        public int? Id { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public int? UnidadMedidaVentaId { get; set; }
        public string UnidadMedidaVentaNombre { get; set; }
        public decimal CantidadUtilizada { get; set; }
        public bool Nuevo { get; set; }
        public decimal PrecioCostoInsumo { get; set; }

        public decimal TotalInsumo { get; set; }


    }
}