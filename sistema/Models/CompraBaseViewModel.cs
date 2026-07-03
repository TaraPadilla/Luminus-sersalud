using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;
using Database.Shared.Data;
using System;
using farmamest.Models;
using sistema.Utilidades;

namespace sistema.Models
{
    public class CompraBaseViewModel
    {
        public bool ConfigProrrateoHabilitado { get; set; }
        public string EstablecimientoDireccion { get; set; }

        public int? CompraId { get; set; }
        public int? OrdenCompraId { get; set; }
        //public int? SucursalId { get; set; }
        public bool EstadoCompra { get; set; }
        public bool OrdenCompraRecibida { get; set; }
        //Encabezado

        public string EncabezadoNoComprobante { get; set; }
        public int? EncabezadoEmpleadoId { get; set; }
        public string EncabezadoEmpleadoNombre { get; set; }
        public string EncabezadoProveedor { get; set; }
        public SelectList ListaProveedores { get; set; }
        public int? EncabezadoTipoBodegaId { get; set; }

        public int? CompraAmbienteId { get; set; }

        //Ambiente
        public int? EncabezadoAmbienteId { get; set; }
        public bool AmbienteBloqueado { get; set; } //Determina si se permite modificar el ambiente en la compra

        public int? EncabezadoSucursalId { get; set; }
        public SelectList ListaSucursales { get; set; }
        public int? EncabezadoTipoDocumentoId { get; set; }
        public SelectList ListaTipoDocumento { get; set; }
        public bool TipoDocumentoBloqueado { get; set; }//Determina si se permite modificar el tipo
                                                        //de documento en la compra, sea Orden o Compra

        public DateTime? OrdenCompraEncabezadoFechaEstimadaRecepcion { get; internal set; }
        public string EncabezadoFechaLimite { get; set; }
        public string EncabezadoFechaRecepcion { get; set; }
        public string EncabezadoObservacion { get; set; }
        public int EncabezadoTipoCompraId { get; set; }
        public string EncabezadoTipoCompraNombre { get; set; }
        public SelectList ListaTipoCompra { get; set; }
        public int DiasCredito { get; set; }
        public decimal ValorTotalCompra { get; set; }

        //PreciosVenta
        public List<CompraPrecioVentaViewModel> PreciosVenta { get; set; }
        //ProductosComprados
        public List<CompraProductoCompradoViewModel> ProductosComprados { get; set; }
        //Ubicaciones
        public List<CompraUbicacionViewModel> Ubicaciones { get; set; }
        //UnidadesVenta
        public List<CompraUnidadVentaViewModel> UnidadesVenta { get; set; }

        //campos para fecha de vencimiento de articulos comprados
        public string precioUnidadCompra { get; set; }
        public string precioUnidadVenta { get; set; }
        public decimal StockActual { get; set; }


        public void Init(IProveedor proveedorRepository, ICompra compraRepository, ISucursal sucursalRepository)
        {
            ListaProveedores = new SelectList(proveedorRepository.GetList(), "Nombre", "Nombre");
            ListaTipoCompra = new SelectList(compraRepository.TipoCompraLista(), "Id", "Tipo");
            ListaSucursales = new SelectList(sucursalRepository.GetList(), "Id", "NombreSucursal");
            ListaTipoDocumento = new SelectList(compraRepository.GetListTipoDocumento(), "Id", "NombreTipoDocumento");
        }

        public void Init(IEmpleado empleadoRepository)
        {
            ListaEmpleados = EmpleadoSelectListHelper.Crear(empleadoRepository);
        }

        //Campos pendientes normalizaci�n
        public SelectList ListaEmpleados { get; set; }
        public Recepcion Recepcion { get; set; } = new Recepcion();
        public List<UltimaCompraProductoViewModel> ListaProductos { get; set; }
        public Dictionary<int, decimal> PrecioUltimaCompraProducto { get; set; }
        public List<ProductoCompraViewModel> Productos { get; set; } = new List<ProductoCompraViewModel>();
        public string ProveedorPrincipal { get;  set; }
        public Dictionary<string, string> ProveedorPrincipalPorItem { get; set; } // Clave: ProductoId

    }
}