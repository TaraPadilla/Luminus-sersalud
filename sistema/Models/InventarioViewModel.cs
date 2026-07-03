using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Database.Shared.Paginacion;
using Database.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using farmamest.Models;

namespace sistema.Models
{
    public class InventarioViewModel
    {
        public List<PresentacionProducto> TodasLasPresentaciones { get; set; }
        public SelectList SegurosSelectList { get; set; }
        public SelectList GrupoTerapeuticoSelectList { get; set; }
        public SelectList SucursalSelectList { get; set; }
        public SelectList TipoProductoSelectList { get; set; }
        public int? BodegaId { get; set; }

        public int? SeguroId { get; set; }

        public SelectList BodegaSelectList { get; set; }
        public int? AmbienteId { get; set; }
        public SelectList AmbienteSelectList { get; set; }
        public PaginacionList<Producto> Productos { get; set; }
        public List<Producto> ProductosInventario { get; set; }
        public List<ProductoProveedorViewModel> Proveedores { get; set; }
        public List<Bodega> BodegasExistentes { get; set; }
        public string buscar { get; set; }
        public string currentFilter { get; set; }
        public int? pageNumber { get; set; }
        public int? terapeuticoId { get; set; }
        public int? sucursalId { get; set; }
        public int TotalMedicamentos { get; set; }

        //Bodega y tipo de producto
        public string NombreBodega { get; set; }
        public int? TipoProductoId { get; set; }
        public string NombreTipoProductos { get; set; }
        public List<AuditoriaNuevoSP> AuditoriaSP { get; set; }
        public List<Precio> Precios { get; set; }
        //Variable utiiza para la importacion de Excel
        //Seria una buena practica tener un modelo para
        //la vista de importacion de inventario y no utilizar la misma
        //de la Vista de Inventario
        public string BorrarInventarioActual { get; set; } //si,no
        public void Init(
            IDespegablesProducto _categoriaRepository,
            ISucursal _sucursalRepository,
            IBodega _bodegaRepository,
            IDespegablesProducto _categoriasRepository,
            IAmbiente _ambienteRepository,
            ISeguro _seguroRepository = null)
        {
            GrupoTerapeuticoSelectList = new SelectList(_categoriaRepository.ListarGrupoT(), "Id", "NombreGrupoT");
            SucursalSelectList = new SelectList(_sucursalRepository.GetList(), "Id", "NombreSucursal");
            AmbienteSelectList = new SelectList(_ambienteRepository.GetList(), "Id", "NombreAmbiente");

            var bodegas = _bodegaRepository.GetList();
            if (AmbienteId != null)
            {
                bodegas = bodegas.Where(a => a.AmbienteId == (int)AmbienteId).ToList();
            }
            BodegaSelectList = new SelectList(bodegas, "Id", "BodegaSucursalText");

            TodasLasPresentaciones = new List<PresentacionProducto>();
            SegurosSelectList = new SelectList(_seguroRepository.GetList(), "Id", "Nombre");
            TipoProductoSelectList = new SelectList(_categoriaRepository.ListarTipoProductos(), "Id", "NombreTipoProducto");
            BodegasExistentes = _bodegaRepository.GetList();
        }
    }
}