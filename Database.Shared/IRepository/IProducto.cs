using Database.Shared.Models;
using System.Collections.Generic;
using Database.Shared.Paginacion;
using Database.Shared.DataBindings;
using Database.Shared.Data;
using Database.Shared.Enumeraciones;
using Database.Shared.Dto;
using Microsoft.EntityFrameworkCore;
using System;

namespace Database.Shared.IRepository
{
    public interface IProducto
    {
        public void UpdateRange(List<Producto> models, bool saveChanges = true);

        public Producto GetByCodigo(string codigo);
        public List<ProductoInventario> GetLotesProducto(int productoId);
        public List<ProductoInventario> GetLotes();
        public List<ProductoInventarioPrecio> GetProductoInventarioPrecioByIdProducto(int productoId);
        public void Add(ProductoInventarioPrecio productoInvPrecio, bool saveChanges = true);
        List<DtoSpGetVentasProductoAnnio> GetVentasProductoAnnioSp(int? annio, int? productoId);
        List<Producto> GetProductosPorIds(List<int> productoIds);
        public List<DtoSpGetComprasProducto> GetComprasProductoSp(int productoId);
        public List<DtoSpGetProductosMasVendidos> GetProductosMasVendidos();
        public List<DtoSpGetProductosMenosVendidos> GetProductosMenosVendidos();
        public void Add(Producto producto, bool saveChanges = true);
        public void Add(List<ProductoInventario> lotes);
        public void Add(MovimientoProducto movimiento);
        public void AddProductoInventario(ProductoInventario productoInventario, bool saveChanges = true);
        public void AddLaboratorioProducto(LaboratorioProducto laboratorioProducto, bool saveChanges = true);
        public int ExistOrAddLaboratorioProducto(LaboratorioProducto laboratorioProducto);
        public int ExistOrAddViaAdmin(Viadmin viaAdmin);
        public int ExistOrAddUnidadMedidaVenta(UnidadMedidaVenta unidadMedidaVenta);
        public void AddUnidadMedidaVenta(UnidadMedidaVenta unidadMedidaVenta, bool saveChanges = true);
        public void AddRangeProductoInventario(List<Producto> productoInventario);
        public void AddRangeProductoInventarioExistente(List<ProductoInventario> productoInventario, bool saveChanges = true);
        public int ExistOrAddUnidadMedidaCompra(UnidadMedidaCompra unidadMedidaCompra);
        public List<Producto> GetList();
        List<MovimientoProducto> GetMovimientos(DateTime fechaInicio, DateTime fechaFin, List<int> productosIds);
        List<MovimientoProducto> GetMovimientos(DateTime? fechaInicio, DateTime? fechaFin, List<int> productosIds);

        List<MovimientoProductoNacional> GetMovimientosNacional(DateTime? fechaInicio, DateTime? fechaFin, List<int> productosIds);

        IList<Producto> GetProductos(int? tipoBodega, int? tipoProducto, bool eliminados = false);
        public IList<ExamenLabClinicoInsumo> GetInsumosAsignadosExamenLab(int? examenId, bool eliminados = false);
        public List<Producto> GetListPdf();
        public IList<Producto> GetListado();
        public Bodega GetSucursalIdByTipoBodega(int bodegaId);
        public IList<Producto> GetListadoFaltantesFarmacia(string searchString);
        public IList<Producto> GetListadoFaltantesClinica(string searchString);
        public Producto Get(int id, bool includeRelatedEntities = true);
        public Producto GetPorNumeroDeReferencia(string id, bool includeRelatedEntities = true);
        public IList<Producto> GetPorNumeroDeReferenciaList(string id, bool includeRelatedEntities = true);

        public Producto GetPorNumeroDeReferenciayNombre(string id, bool includeRelatedEntities = true);
        public IList<RetornoProductoPOS> BuscarPorNombreYReferenciaBusquedaAjax(string searchString);
        public List<ProductoInventario> BuscarProductosNombre(string searchString);
        public void Update(Producto producto, bool saveChanges = true);
        public void Update(ProductoInventario productoInventario);
        public void Update(ProductoInventarioPrecio producto);
        public void SaveChanges();
        public PaginacionList<Producto> PaginacionProductosFarmaciaMedicamentos(string searchString, int? pageNumber, int pageSize, int? terapeuticoId);
        public PaginacionList<Producto> PaginacionProductosFarmaciaInsumosMedicos(string searchString, int? pageNumber, int pageSize, int? categoriaId);
        public PaginacionList<Producto> PaginacionProductosClinicaMedicamentos(string searchString, int? pageNumber, int pageSize, int? terapeuticoId);
        public List<Producto> GetInventarioProductos(
            int? tipoBodega,
            int? tipoProducto,
            int? categoriaId,
            int? grupoTerapeuticoId,
            int? sucursalId,
            int? bodegaId,
            int? ambienteId,
            bool eliminado = false);
        public List<Producto> GetInventarioProductosconPrecios
           (
           int? tipoBodega
           , int? tipoProducto
           , int? categoriaId
           , int? grupoTerapeuticoId
           , int? sucursalId,
           int? bodegaId,
           int? ambienteId
           , bool eliminado = false
           );
        public ProductoInventario GetRegistroInventarioProducto(int productoInventarioId, int? productoId = null);
        List<ProductoInventario> BuscarRegistrosProductosInventario(int productoId, int? unidadMedidaVentaId);

        //Buscar un productoInventario por el id del producto
        public ProductoInventario GetRegistroInventarioProductoxProductoid(int ProductoId);
        public ProductoInventario GetProductoBodegaUnidad(int bodegaId, int? unidadMedidaVentaId);
        public ProductoInventario GetValidarProductoInventario(int bodegaId, int? unidadMedidaVentaId, int productoId);//Valida si exites un producto en inventario con el productoId , bodegaId, UnidadVentaId paraa actualizar stock o crear unno nuevo 
        public void UpdateRegistroInventario(ProductoInventario productoInventario, bool saveChanges = true);
        List<ProductoInventarioPrecio> ConsultarPreciosProductoInventario(int productoInventarioId);
        PaginacionList<Producto> PaginacionBodegaMedicamentos(string searchString, int? pageNumber, int pageSize, int? terapeuticoId);
        public PaginacionList<ProductoInventario> PaginacionProductosFaltantes(string sortOrder, string searchString, int? pageNumber, int pageSize);

        //public PaginacionList<Producto> PaginacionProductosFaltantes(string sortOrder, string searchString, int? pageNumber, int pageSize);
        public PaginacionList<Producto> PaginacionProductosVencimiento(string sortOrder, string searchString, int? pageNumber, int pageSize);
        public IList<ProductoYCodigo> GetListParaCotizacion();
        public Producto GetProdutoById(int id);
        public Producto GetProductoByTipoBodegaAndNombre(int tipoBodega, string nombre);
        public Producto GetPorNumeroDeReferenciaClinica(string id, bool includeRelatedEntities = true);
        public IList<RetornoProductoPOSClinica> BuscarPorNombreYReferenciaBusquedaAjaxClinica(string searchString);
        public IList<Producto> GetListadoProductos();
        public List<Producto> GetProductos(int? ambienteId);
        IList<Producto> GetListadoProductosBodega();
        public IList<Producto> FiltrarPorBusquedaYTerapeutico(string searchString, int? terapeuticoId, int tipoBodega);
        public IList<Producto> FiltrarPorBusquedaYCategoria(string searchString, int? categoriaId, int tipoBodega);
        public PaginacionList<ProductoInventario> PaginacionProductosFaltantesClinica(string sortOrder, string searchString, int? pageNumber, int pageSize);
        //public PaginacionList<Producto> PaginacionProductosFaltantesClinica(string sortOrder, string searchString, int? pageNumber, int pageSize);
        //public PaginacionList<Producto> PaginacionVencidosFarmacia(string sortOrder, string searchString, int? pageNumber, int pageSize);
        public PaginacionList<ProductoInventario> PaginacionVencidosFarmacia(string sortOrder, string searchString, int? pageNumber, int pageSize);
        //public PaginacionList<Producto> PaginacionProximosAVencerFarmacia(string sortOrder, string searchString, int? pageNumber, int pageSize);
        public PaginacionList<ProductoInventario> PaginacionProximosAVencerFarmacia(string sortOrder, string searchString, int? pageNumber, int pageSize);

        //public PaginacionList<Producto> PaginacionVencidosClinica(string sortOrder, string searchString, int? pageNumber, int pageSize);
        public PaginacionList<ProductoInventario> PaginacionVencidosClinica(string sortOrder, string searchString, int? pageNumber, int pageSize);
        public PaginacionList<ProductoInventario> PaginacionProximosAVencerClinica(string sortOrder, string searchString, int? pageNumber, int pageSize);

        public PaginacionList<ProductoInventario> PaginacionVencidosLab(string sortOrder, string searchString, int? pageNumber, int pageSize);
        PaginacionList<ProductoInventario> PaginacionProximosAVencerLab(string sortOrder, string searchString, int? pageNumber, int pageSize);

        Producto GetByName(string producto, bool includeRelatedEntities = true);
        int GetTotalMedicamentosFarmacia();
        int GetTotalInsumosFarmacia();

        int GetTotalMedicamentosClinica(int? grupoTerapeuticoId, int? sucursalId);
        public int GetTotalInsumosClinica(int? categoriaId);

        int GetTotalMedicamentosBodega();
        int GetTotalInsumosBodega();
        IList<Producto> GetProductosLaboratorio(int? laboratorioId = null);

        PaginacionList<Producto> PaginacionProductosBodegaInsumosMedicos(string searchString, int? pageNumber, int pageSize, int? categoriaId);

        IList<Producto> GetMedicamentosFarmaciaList();


        PaginacionList<Producto> PaginacionProductosLabMedicamentos(string searchString, int? pageNumber, int pageSize, int? terapeuticoId);
        int GetTotalMedicamentosLab();
        PaginacionList<Producto> PaginacionProductosLabInsumosMedicos(string searchString, int? pageNumber, int pageSize, int? categoriaId);
        int GetTotalInsumosLab();
        PaginacionList<ProductoInventario> PaginacionProductosFaltantesLab(string sortOrder, string searchString, int? pageNumber, int pageSize);
        //PaginacionList<Producto> PaginacionVencidosLab(string sortOrder, string searchString, int? pageNumber, int pageSize);

        IList<Producto> GetListadoFaltantesLab(string searchString);

        #region Unidades
        List<UnidadMedidaCompra> GetUnidadesCompra();
        List<UnidadMedidaVenta> GetUnidadesVenta();
        void AddUnidadCompra(UnidadMedidaCompra model);
        void AddUnidadVenta(UnidadMedidaVenta model);
        #endregion
        List<ProductoEquivalencia> GetEquivalenciasProducto(int productoId);
        public ProductoInventarioPrecio GetProductoInventarioPrecio(int productoInventarioPrecioId);
        public List<ProductoInventario> GetStocks(int productoId, int? sucursalId);

        public ProductoInventario GetStocksVencido(int Id);//Stock vencidos 
        //public ProductoInventario GetStockActualProducto(int productoId, int? bodegaId);
        public ProductoInventario GetProductoByIdAndBodegaId(int productoId, int? bodegaId);

        public Producto GetInsumoDuplicadoClinica(string NombreProducto, int TipoBodegaId);
        public ProductoInventario GetProductoInventariobyId(int productoId);
        public Proveedor GetProveedorExistente(string nombre);
        void EliminarRegistroInventario(ProductoInventario productoInventario);

        public void AddListProductoInventario(List<ProductoInventario> productoInventario, bool saveChanges = true);
        public List<Producto> GetProductosByBodegaAndByTipoProducto(int? tipoBodega, int? tipoProducto, bool eliminado = false);
        public List<AuditoriaNuevoSP> GetBandejaAuditoriaNuevo(int? tipoBodega, int? tipoProducto);
        public List<VentaVentaUnificadaProductoExistenteSP> GetListaVenta(int sucursalId, int ambienteId);
        public List<Producto> GetInventarioSP(int? tipoProductoId, int? grupoTerapeuticoId, int? bodegaId, int? sucursalId, int? ambienteId);
        public List<DtoSpInventarioProductos> GetInventarioSp_Nuevo(int? tipoProductoId, int? grupoTerapeuticoId, int? bodegaId, int? sucursalId, int? ambienteId);
        void AddRangeProductoEquivalencia(List<ProductoEquivalencia> productoEquivalencias, bool saveChanges = true);
        public List<ProductoInventario> GetProductosInventarioStockBajo();

        public int ExistOrAddPresentacionProducto(PresentacionProducto presentacionProducto);
        public void AddPresentacionProducto(PresentacionProducto presentacionProducto, bool saveChanges = true);

        public List<ProductoInventarioPrecio> GetPreciosProducto(int productoId);

        public UnidadMedidaVenta GetUnidadMedidaVentaByProductoId(int productoId);
        public List<DetalleVenta> GetDetalleVentasByProductoInventarioId(int productoInventarioId);
        public void EliminarDetalleVenta(DetalleVenta detalleVenta);

        public string GetNombrePrecio(int precioId);
        public ProductoInventarioPrecio GetProductoInventarioPrecioPorInventarioYTipoPrecio(int productoInventarioId, int precioId);

        // =========================================================
        // NUEVO: Requisición (sin Traslados)
        // =========================================================
        List<ProductoInventario> GetInventarioDisponiblePorBodega(int bodegaId);


        ProductoInventarioPrecio ObtenerPrecioProductoPorDefecto(int productoId);

        List<ProductoInventarioDto> GetProductosHospitalizacion(int? bodegaId = null);
    }
}