
using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sistema.Models
{
    public class MovimientoProductoNacionalViewModel
    {
        public DateTime Fecha { get; set; }
        public string TipoMovimientoNombre { get; set; }
        public string DescripcionMovimiento { get; set; }
        public string Medicamento { get; set; }
        //public string Equivalencia { get; set; }
        public string UnidadNombre { get; set; }
        public string Lote { get; set; }
        public DateTime? FechaVencimientoLote { get; set; }
        public decimal Cantidad { get; set; }
        public decimal? CostoPonderadoCompra { get; set; }
        public decimal? CostoPonderadoVenta { get; set; }
        public decimal MontoTotal { get; set; }
        public string UsuarioNombre { get; set; }
        public string ProveedorBodegaCliente { get; set; }
        public decimal SaldoActual { get; set; }
        //public SelectList BodegaSelectList { get; set; }
        public int? AmbienteId { get; set; }
        public string AmbienteNombre { get; set; }
        //public SelectList AmbienteSelectList { get; set; }
        public int? BodegaId { get; set; }
        public string BodegaNombre { get; set; }
        //public int? TipoProductoId { get; set; }
        public string TipoProductoNombre { get; set; }
        //public List<MovimientoProducto> MovimientosProducto { get; set; }
        //public List<Bodega> BodegasExistentes { get; set; }
        public SelectList TipoProductoSelectList { get; set; }


        public decimal PrecioUnitario { get; set; }

        public decimal PrecioCosto { get; set; }

        public decimal TotalEntrada { get; set; }

        public decimal TotalSalida { get; set; }

        public string UsuarioEntrega { get; set; }
        public string UsuarioSolicita { get; set; }

        public int ProductoInventarioId { get; set; }


        //En proceso de eliminarse
        //public void Init(
        //   IDespegablesProducto _categoriaRepository,
        //   ISucursal _sucursalRepository,
        //   IBodega _bodegaRepository,
        //   IDespegablesProducto _categoriasRepository,
        //   IAmbiente _ambienteRepository)
        //{


        //    //AmbienteSelectList = new SelectList(_ambienteRepository.GetList(), "Id", "NombreAmbiente");

        //    //var bodegas = _bodegaRepository.GetList();
        //    //if (AmbienteId != null)
        //    //{
        //    //    bodegas = bodegas.Where(a => a.AmbienteId == (int)AmbienteId).ToList();
        //    //}
        //    //BodegaSelectList = new SelectList(bodegas, "Id", "BodegaSucursalText");


        //    //TipoProductoSelectList = new SelectList(_categoriaRepository.ListarTipoProductos(), "Id", "NombreTipoProducto");
        //    //BodegasExistentes = _bodegaRepository.GetList();
        //}
    }
}
