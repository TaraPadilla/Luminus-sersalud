using Database.Shared.Models;
using Database.Shared.IRepository;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Paginacion;
using System;
using Database.Shared.DataBindings;
using Database.Shared.Enumeraciones;
using System.Security.Permissions;
using Database.Shared.Dto;

namespace Database.Shared.Data
{
    public class ProductosRepository : IProducto
    {

        private readonly Context _context = null;

        public ProductosRepository(Context context)
        {
            _context = context;
        }

        public Producto GetByCodigo(string codigo)
        {
            return _context.Productos
                .Include(p => p.ProductosInventario)
                .ThenInclude(pi => pi.ProductosInventarioPrecios)
                .Include(p => p.ProductoEquivalencias)
                .FirstOrDefault(p => p.CodigoReferencia == codigo && !p.Eliminado);
        }


        public void Add(ProductoInventarioPrecio productoInvPrecio, bool saveChanges = true)
        {
            _context.ProductosInventarioPrecios.Add(productoInvPrecio);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }
        public void Add(Producto producto, bool saveChanges = true)
        {
            if (producto.Id == 0)
                _context.Productos.Add(producto);
            else
            {
                _context.Entry(producto).State = EntityState.Modified;
            }

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }
        public List<Producto> GetProductosPorIds(List<int> productoIds)
        {
            if (productoIds == null || !productoIds.Any())
                return new List<Producto>();

            return _context.Productos
                           .Where(p => productoIds.Contains(p.Id))
                           .ToList();
        }
        public void Add(MovimientoProducto movimiento)
        {
            _context.MovimientosProducto.Add(movimiento);
            _context.SaveChanges();
        }
        public void Add(List<ProductoInventario> lotes)
        {
            _context.ProductosInventario.AddRange(lotes);
            _context.SaveChanges();
        }
        public List<ProductoInventario> GetLotesProducto(int productoId)
        {
            return _context.ProductosInventario
                .Include(a => a.Bodega)
                .Include(a => a.Compra).ThenInclude(a => a.Proveedor)
                .Where(a => a.ProductoId == productoId
                && !a.Eliminado
                && a.Stock > 0)
                .ToList();
        }
        public List<ProductoInventario> GetLotes()
        {
            return _context.ProductosInventario
                .Where(a => !a.Eliminado)
                .ToList();
        }
        public void AddProductoInventario(ProductoInventario productoInventario, bool saveChanges = true)
        {
            _context.ProductosInventario.Add(productoInventario);
            if (saveChanges)
            {
                SaveChanges();
            }
        }
        public void AddRangeProductoInventario(List<Producto> productoInventario)
        {
            _context.Productos.AddRange(productoInventario);
            _context.SaveChanges();
        }

        public void AddRangeProductoInventarioExistente(List<ProductoInventario> productoInventario, bool saveChanges = true)
        {
            _context.ProductosInventario.AddRange(productoInventario);
            if (saveChanges)
            {
                SaveChanges();
            }
        }
        public void AddRangeProductoEquivalencia(List<ProductoEquivalencia> productoEquivalencias, bool saveChanges = true)
        {
            _context.ProductosEquivalencias.AddRange(productoEquivalencias);
            if (saveChanges)
            {
                SaveChanges();
            }
        }


        public void AddListProductoInventario(List<ProductoInventario> productoInventario, bool saveChanges = true)
        {
            _context.ProductosInventario.AddRange(productoInventario);
            if (saveChanges)
            {
                SaveChanges();
            }
        }
        public List<DtoSpGetVentasProductoAnnio> GetVentasProductoAnnioSp(int? annio, int? productoId)
        {
            var resultadoSp = _context.DtoSpGetVentasProductoAnnio
                .FromSqlInterpolated($"SELECT * FROM get_ventas_producto_annio({productoId},{annio});")
                .ToList();
            return resultadoSp;
        }
        public List<DtoSpGetComprasProducto> GetComprasProductoSp(int productoId)
        {
            var resultadoSp = _context.DtoSpGetComprasProducto
                .FromSqlInterpolated($"SELECT * FROM get_compras_producto({productoId});")
                .ToList();
            return resultadoSp;
        }
        public List<DtoSpGetProductosMasVendidos> GetProductosMasVendidos()
        {
            var resultadoSp = _context.DtoSpGetProductosMasVendidos
                .FromSqlInterpolated($"SELECT * FROM get_productos_mas_vendidos();")
                .ToList();
            return resultadoSp;
        }
        public List<DtoSpGetProductosMenosVendidos> GetProductosMenosVendidos()
        {
            var resultadoSp = _context.DtoSpGetProductosMenosVendidos
                .FromSqlInterpolated($"SELECT * FROM get_productos_menos_vendidos();")
                .ToList();
            return resultadoSp;
        }
        public List<Producto> GetList() => _context.Productos
            .Include(a => a.Viadmin)
            .Include(a => a.ProductosInventario)
            .Where(x => x.Eliminado == false)
            .Where(x => x.NombreProducto != null)
            .ToList();
        // public List<MovimientoProducto> GetMovimientos(DateTime fechaInicio, DateTime fechaFin, List<int> productosIds)
        // {
        //     var movimientos = _context.MovimientosProducto
        //         .Include(a => a.TipoMovimientoProducto)
        //         .Include(a => a.ProductoInventario)
        //             .ThenInclude(a => a.Producto)
        //             .ThenInclude(a => a.Ambiente)
        //         .Include(a => a.ProductoInventario)
        //             .ThenInclude(a => a.Producto)
        //             .ThenInclude(a => a.TipoProducto)
        //         .Include(a => a.ProductoInventario)
        //             .ThenInclude(a => a.Bodega)
        //         .Include(a => a.ProductoInventario)
        //             .ThenInclude(a => a.UnidadMedidaVenta)
        //         .Include(a => a.UsuarioRealiza)
        //         .ToList();

        //     //Filtro por IDS de producto

        //     return movimientos;
        // }
        public List<MovimientoProducto> GetMovimientos(DateTime fechaInicio, DateTime fechaFin, List<int> productosIds)
        {
            // Mantener compatibilidad: este obliga filtro por fechas (porque son DateTime no-null)
            return GetMovimientos((DateTime?)fechaInicio, (DateTime?)fechaFin, productosIds);
        }

        public List<MovimientoProductoNacional> GetMovimientosNacional(DateTime fechaInicio, DateTime fechaFin, List<int> productosIds)
        {
            // Mantener compatibilidad: este obliga filtro por fechas (porque son DateTime no-null)
            return GetMovimientosNacional((DateTime?)fechaInicio, (DateTime?)fechaFin, productosIds);
        }



        public List<MovimientoProducto> GetMovimientos(DateTime? fechaInicio, DateTime? fechaFin, List<int> productosIds)
        {
            // IMPORTANTÍSIMO: no materializar (ToList) antes de filtrar.
            var query = _context.MovimientosProducto
                .Include(a => a.TipoMovimientoProducto)
                .Include(a => a.ProductoInventario)
                    .ThenInclude(a => a.Producto)
                    .ThenInclude(a => a.Ambiente)
                .Include(a => a.ProductoInventario)
                    .ThenInclude(a => a.Producto)
                    .ThenInclude(a => a.TipoProducto)
                .Include(a => a.ProductoInventario)
                    .ThenInclude(a => a.Bodega)
                .Include(a => a.ProductoInventario)
                    .ThenInclude(a => a.UnidadMedidaVenta)
                .Include(a => a.UsuarioRealiza)
                .AsQueryable();

            // Filtro por fechas SOLO si vienen
            if (fechaInicio.HasValue)
                query = query.Where(m => m.Fecha >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(m => m.Fecha <= fechaFin.Value);

            // Filtro por ids de producto SOLO si vienen
            if (productosIds != null && productosIds.Count > 0)
                query = query.Where(m => m.ProductoInventario != null
                                      && productosIds.Contains(m.ProductoInventario.ProductoId));

            // Aquí sí materializamos
            return query.ToList();
        }

        public List<MovimientoProductoNacional> GetMovimientosNacional(DateTime? fechaInicio, DateTime? fechaFin, List<int> productosIds)
        {
            // IMPORTANTÍSIMO: no materializar (ToList) antes de filtrar.
            var query = _context.MovimientosProductoNacional
                .Include(a => a.TipoMovimientoProducto)
                .Include(a => a.ProductoInventario)
                    .ThenInclude(a => a.Producto)
                    .ThenInclude(a => a.Ambiente)
                .Include(a => a.ProductoInventario)
                    .ThenInclude(a => a.Producto)
                    .ThenInclude(a => a.TipoProducto)
                .Include(a => a.ProductoInventario)
                    .ThenInclude(a => a.Bodega)
                .Include(a => a.ProductoInventario)
                    .ThenInclude(a => a.UnidadMedidaVenta)
                .Include(a => a.UsuarioRealiza)
                .Include(a => a.UsuarioEntrega)
                .AsQueryable();

            // Filtro por fechas SOLO si vienen
            if (fechaInicio.HasValue)
                query = query.Where(m => m.Fecha >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(m => m.Fecha <= fechaFin.Value);

            // Filtro por ids de producto SOLO si vienen
            if (productosIds != null && productosIds.Count > 0)
                query = query.Where(m => m.ProductoInventario != null
                                      && productosIds.Contains(m.ProductoInventario.ProductoId));

            // Aquí sí materializamos
            return query.ToList();
        }


        public List<Producto> GetListPdf() => _context.Productos.Include(a => a.GrupoTProducto)
                  .Include(a => a.LaboratorioProducto)
                  .OrderBy(a => a.NombreProducto)
                  .Where(a => a.Eliminado == false && a.TipoBodegaId == 2 && a.TipoProductoId == (int)TipoProductoEnum.Medicamentos)
                  .ToList();

        public IList<ProductoYCodigo> GetListParaCotizacion()
        {
            return _context.Productos
            .Include(a => a.Viadmin)
            .Where(x => x.Eliminado == false)

            .Select(a => new ProductoYCodigo
            {
                CodigoReferencia = a.CodigoReferencia,
                ProductoYCodigoDeBarras = a.GetProductosYCodigoDeBarras
            })
            .ToList();
        }

        public IList<Producto> GetListado()
        {
            return _context.Productos
                    .Where(x => x.Eliminado == false
                    && x.TipoBodegaId == (int)TipoBodegaEnum.Farmacia
                    && x.TipoProductoId == (int)TipoProductoEnum.Medicamentos).ToList();
        }
        public IList<Producto> GetProductos(int? tipoBodega, int? tipoProducto, bool eliminados = false)
        {
            var productos = _context.Productos
                .Include(p => p.ProductoEquivalencias).ThenInclude(p => p.UnidadMedidaCompra)
                .Include(p => p.ProductoEquivalencias).ThenInclude(p => p.UnidadMedidaVenta)
                .Where(a => a.Eliminado == eliminados);
            if (tipoBodega != null)
                productos = productos
                    .Where(p => p.TipoBodegaId == tipoBodega);
            if (tipoProducto != null)
                productos = productos
                    .Where(p => p.TipoProductoId == tipoProducto);

            return productos.ToList();
        }

        public IList<ExamenLabClinicoInsumo> GetInsumosAsignadosExamenLab(int? examenId, bool eliminados = false)
        {
            var examenesconInsumosAsignados = _context.ExamenLabClinicoInsumo
                .Include(p => p.Producto).ThenInclude(p => p.ProductosInventario)
                .Include(p => p.Producto).ThenInclude(p => p.ProductoEquivalencias).ThenInclude(p => p.UnidadMedidaVenta)
                //.Include(p => p.ProductoEquivalencias).ThenInclude(p => p.UnidadMedidaVenta)
                .Where(a => a.ExamenLabClinicoId == examenId && a.Eliminado == eliminados);


            return examenesconInsumosAsignados.ToList();
        }

        public void EliminarRegistroInventario(ProductoInventario productoInventario)
        {
            _context.ProductosInventario.Remove(productoInventario);
            _context.SaveChanges();
        }

        public IList<Producto> GetListadoProductos()
        {
            return _context.Productos
            .Include(a => a.PresentacionProducto)
            .Include(a => a.Categoria)
            .Where(a => a.Eliminado == false && a.TipoBodegaId == 1).ToList();
        }
        public List<Producto> GetProductos(int? ambienteId)
        {
            return _context.Productos
            .Include(a => a.PresentacionProducto)
            .Include(a => a.TipoProducto)
            .Include(a => a.TipoBodega)
            .Include(a => a.Categoria)
            .Where(a => a.Eliminado == false && a.AmbienteId == ambienteId).ToList();
        }

        public IList<Producto> GetListadoProductosBodega()
        {
            return _context.Productos
            .Include(a => a.PresentacionProducto)
            .Include(a => a.Categoria)
            .Where(a => a.Eliminado == false && a.TipoBodegaId == 3).ToList();
        }



        // SELECT *FROM PRODUCTOS WHERE ID = <id>
        public Producto Get(int id, bool includeRelatedEntities = true)
        {
            var productos = _context.Productos.AsQueryable();

            if (includeRelatedEntities)
            {
                productos = productos
                    .Include(a => a.Viadmin)
                    .Include(a => a.TipoProducto)
                    .Include(a => a.ProductoEquivalencias)
                    .Include(a => a.ProductosInventario)
                    .Include(a => a.ProductosInventario)
                        .ThenInclude(a => a.UnidadMedidaCompra)
                    .Include(a => a.ProductosInventario)
                        .ThenInclude(a => a.UnidadMedidaVenta)

                    ;
                if (productos != null)
                {
                    foreach (var producto in productos)
                    {
                        producto.ProductoEquivalencias = producto.ProductoEquivalencias
                            .Where(a => !a.Eliminada)
                            .ToList();
                    }
                }
            }

            return productos
               .Where(a => a.Id == id)
               .SingleOrDefault();
        }

        public Producto GetInsumoDuplicadoClinica(string NombreProducto, int TipoBodegaId)
        {
            var producto = _context.Productos.AsQueryable();
            return producto.Where(a => a.NombreProducto == NombreProducto && a.TipoBodegaId == TipoBodegaId && a.Eliminado == false).FirstOrDefault();
        }

        public Producto GetByName(string producto, bool includeRelatedEntities = true)
        {
            var productos = _context.Productos.AsQueryable();

            if (includeRelatedEntities)
            {
                productos = productos.Include(a => a.Viadmin)
                        .Include(a => a.ProductosInventario)
                            .ThenInclude(x => x.UnidadMedidaCompra)
                        .Include(a => a.ProductosInventario)
                            .ThenInclude(x => x.UnidadMedidaVenta)
                        .Include(a => a.ProductosInventario)
                            .ThenInclude(x => x.UnidadMedidaCompra)
                        ;
            }

            return productos
               .Where(a => a.NombreProducto.ToLower().Trim() == producto.ToLower()
               && !a.Eliminado)
               .FirstOrDefault();
        }

        public void Update(Producto model, bool saveChanges = true)
        {
            _context.Entry(model).State = EntityState.Modified;

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        public void UpdateRange(List<Producto> models, bool saveChanges = true)
        {
            _context.Productos.UpdateRange(models);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }


        public void Update(ProductoInventario productoInventario)
        {
            _context.Entry(productoInventario).State = EntityState.Modified;
            _context.SaveChanges();
        }
        public void Update(ProductoInventarioPrecio model)
        {
            _context.Entry(model).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public PaginacionList<Producto> PaginacionProductosFarmaciaMedicamentos(string searchString, int? pageNumber, int pageSize, int? terapeuticoId)
        {
            var productos = _context.Productos.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                productos = productos.Where(s =>
                s.NombreProducto.Contains(searchString)
                || s.CodigoReferencia.Contains(searchString)
                || s.LaboratorioProducto.NombreLaboratorioProducto.Contains(searchString));
            }

            if (terapeuticoId != null) productos = productos.Where(a => a.GrupoTProductoId == terapeuticoId);

            return PaginacionList<Producto>.CreateAsyncc(productos
            .Include(a => a.GrupoTProducto)
            .Include(a => a.LaboratorioProducto)
            .OrderBy(a => a.NombreProducto)
            .Where(a => a.Eliminado == false && a.TipoBodegaId == 1 && a.TipoProductoId == (int)TipoProductoEnum.Medicamentos),
            pageNumber ?? 1, pageSize);
        }

        public IList<Producto> FiltrarPorBusquedaYTerapeutico(string searchString, int? terapeuticoId, int tipoBodega)
        {
            var productos = _context.Productos.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                productos = productos.Where(s =>
                s.NombreProducto.Contains(searchString)
                || s.CodigoReferencia.Contains(searchString))
                ;
            }

            if (terapeuticoId != 0) productos = productos.Where(a => a.GrupoTProductoId == terapeuticoId);

            return productos
            .Include(a => a.GrupoTProducto)
            .Include(a => a.PresentacionProducto)
            .OrderBy(a => a.NombreProducto)
            .Where(a => a.Eliminado == false && a.TipoBodegaId == tipoBodega && a.TipoProductoId == (int)TipoProductoEnum.Medicamentos).ToList();
        }

        public IList<Producto> FiltrarPorBusquedaYCategoria(string searchString, int? categoriaId, int tipoBodega)
        {
            var productos = _context.Productos.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                productos = productos.Where(s =>
                s.NombreProducto.Contains(searchString)
                || s.CodigoReferencia.Contains(searchString))
                ;
            }

            if (categoriaId != 0) productos = productos.Where(a => a.CategoriaId == categoriaId);

            return productos
            .Include(a => a.Categoria)
            .Include(a => a.Marca)
            .OrderBy(a => a.NombreProducto)
            .Where(a => a.Eliminado == false && a.TipoBodegaId == tipoBodega && a.TipoProductoId == (int)TipoProductoEnum.InsumosMedicos).ToList();
        }

        public PaginacionList<Producto> PaginacionProductosFarmaciaInsumosMedicos(string searchString, int? pageNumber, int pageSize, int? categoriaId)
        {

            var productos = _context.Productos.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                productos = productos.Where(s =>
                s.NombreProducto.Contains(searchString)
                || s.CodigoReferencia.Contains(searchString))
                ;
            }

            if (categoriaId != null) productos = productos.Where(a => a.CategoriaId == categoriaId);

            return PaginacionList<Producto>.CreateAsyncc(productos
            .Include(a => a.Categoria)
            .OrderBy(a => a.NombreProducto)
            .Where(a => a.Eliminado == false && a.TipoBodegaId == 1 && a.TipoProductoId == (int)TipoProductoEnum.InsumosMedicos),

            pageNumber ?? 1, pageSize);
        }

        public PaginacionList<Producto> PaginacionProductosClinicaMedicamentos(string searchString, int? pageNumber, int pageSize, int? terapeuticoId)
        {
            var productos = _context.Productos.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                productos = productos.Where(s =>
                s.NombreProducto.Contains(searchString)
                || s.CodigoReferencia.Contains(searchString))
                ;
            }

            if (terapeuticoId != null) productos = productos.Where(a => a.GrupoTProductoId == terapeuticoId);

            return PaginacionList<Producto>.CreateAsyncc(productos
            .Include(a => a.GrupoTProducto)
            .OrderBy(a => a.NombreProducto)
            .Where(a => a.Eliminado == false && a.TipoBodegaId == (int)TipoBodegaEnum.Clinica
            && a.TipoProductoId == (int)TipoProductoEnum.Medicamentos),
            pageNumber ?? 1, pageSize);
        }

        public PaginacionList<Producto> PaginacionProductosLabMedicamentos(string searchString, int? pageNumber, int pageSize, int? terapeuticoId)
        {
            var productos = _context.Productos.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                productos = productos.Where(s =>
                s.NombreProducto.Contains(searchString)
                || s.CodigoReferencia.Contains(searchString))
                ;
            }

            if (terapeuticoId != null) productos = productos.Where(a => a.GrupoTProductoId == terapeuticoId);

            return PaginacionList<Producto>.CreateAsyncc(productos
            .Include(a => a.GrupoTProducto)
            .OrderBy(a => a.NombreProducto)
            .Where(a => a.Eliminado == false && a.TipoBodegaId == 4 && a.TipoProductoId == (int)TipoProductoEnum.Medicamentos),
            pageNumber ?? 1, pageSize);
        }

        public PaginacionList<Producto> PaginacionBodegaMedicamentos(string searchString, int? pageNumber, int pageSize, int? terapeuticoId)
        {
            var productos = _context.Productos.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                productos = productos.Where(s =>
                s.NombreProducto.Contains(searchString)
                || s.CodigoReferencia.Contains(searchString)
                || s.LaboratorioProducto.NombreLaboratorioProducto.Contains(searchString));
            }

            if (terapeuticoId != null) productos = productos.Where(a => a.GrupoTProductoId == terapeuticoId);

            return PaginacionList<Producto>.CreateAsyncc(productos
            .Include(a => a.GrupoTProducto)
            .Include(a => a.LaboratorioProducto)
            .OrderBy(a => a.NombreProducto)
            .Where(a => a.Eliminado == false && a.TipoBodegaId == 3 && a.TipoProductoId == (int)TipoProductoEnum.Medicamentos),
            pageNumber ?? 1, pageSize);
        }

        public PaginacionList<Producto> PaginacionProductosBodegaInsumosMedicos(string searchString, int? pageNumber, int pageSize, int? categoriaId)
        {

            var productos = _context.Productos.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                productos = productos.Where(s =>
                s.NombreProducto.Contains(searchString)
                || s.CodigoReferencia.Contains(searchString))
                ;
            }

            if (categoriaId != null) productos = productos.Where(a => a.CategoriaId == categoriaId);

            return PaginacionList<Producto>.CreateAsyncc(productos
            .Include(a => a.Categoria)
            .OrderBy(a => a.NombreProducto)
            .Where(a => a.Eliminado == false && a.TipoBodegaId == 3 && a.TipoProductoId == (int)TipoProductoEnum.InsumosMedicos),

            pageNumber ?? 1, pageSize);
        }
        public List<VentaVentaUnificadaProductoExistenteSP> GetListaVenta(int sucursalId, int ambienteId)
        {
            return _context.VentaVentaUnificadaProductoExistenteSP.FromSqlInterpolated($"SELECT * FROM consultas_productos_existentes_venta({ambienteId},{sucursalId});").ToList();
        }
        public List<Producto> GetInventarioSP(int? tipoProductoId, int? grupoTerapeuticoId, int? bodegaId, int? sucursalId, int? ambienteId)
        {
            // Use LINQ instead of mapping inventario_productos to Producto: the SP does not return
            // every Producto column (e.g. PresentacionProductoId2), which breaks FromSql materialization.
            var productoQuery = _context.Productos
                .AsNoTracking()
                .Where(p => !p.Eliminado);

            if (tipoProductoId.HasValue)
                productoQuery = productoQuery.Where(p => p.TipoProductoId == tipoProductoId);

            if (grupoTerapeuticoId.HasValue)
                productoQuery = productoQuery.Where(p => p.GrupoTProductoId == grupoTerapeuticoId);

            if (ambienteId.HasValue)
                productoQuery = productoQuery.Where(p => p.AmbienteId == ambienteId);

            if (bodegaId.HasValue || sucursalId.HasValue)
            {
                productoQuery = productoQuery.Where(p => p.ProductosInventario.Any(pi =>
                    (!bodegaId.HasValue || pi.BodegaId == bodegaId) &&
                    (!sucursalId.HasValue || (pi.Bodega != null && pi.Bodega.SucursalId == sucursalId))));
            }

            var productoIds = productoQuery
                .Select(p => p.Id)
                .Distinct()
                .ToList();

            if (!productoIds.Any()) return new List<Producto>();

            var productos = _context.Productos
                .Where(p => productoIds.Contains(p.Id) && !p.Eliminado)
                .Include(p => p.ProductosInventario.Where(pi => !pi.Eliminado && pi.Stock > 0))
                    .ThenInclude(pi => pi.UnidadMedidaVenta)
                .Include(p => p.ProductosInventario)
                    .ThenInclude(pi => pi.Bodega)
                .Include(p => p.ProductosInventario)
                    .ThenInclude(pi => pi.ProductosInventarioPrecios)
                        .ThenInclude(pip => pip.Precio)
                .Include(p => p.ProductoEquivalencias)
                    .ThenInclude(pe => pe.UnidadMedidaVenta)
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Include(p => p.GrupoTProducto)
                .Include(p => p.PresentacionProducto)
                .Include(p => p.Viadmin)
                .Include(p => p.LaboratorioProducto)
                .ToList();

            return productos;
        }



        public List<DtoSpInventarioProductos> GetInventarioSp_Nuevo(int? tipoProductoId, int? grupoTerapeuticoId, int? bodegaId, int? sucursalId, int? ambienteId)
        {
            var resultadoSp = _context.DtoSpInventarioProductos
                .FromSqlInterpolated($"SELECT * FROM get_inventario({tipoProductoId},{grupoTerapeuticoId},{bodegaId},{sucursalId},{ambienteId});")
                .ToList();
            return resultadoSp;
        }
        public List<Producto> GetInventarioProductos
            (
            int? tipoBodega
            , int? tipoProducto
            , int? categoriaId
            , int? grupoTerapeuticoId
            , int? sucursalId,
            int? bodegaId,
            int? ambienteId
            , bool eliminado = false
            )
        {
            var productos = _context.Productos
                .Include(a => a.Categoria)
                .Include(a => a.Marca)
                .Include(a => a.GrupoTProducto)
                .Include(a => a.LaboratorioProducto)
                .Include(a => a.Viadmin)
                .Include(a => a.PresentacionProducto)
                .Include(p => p.ProductosInventario)
                .Where(a => a.Eliminado == eliminado)
                .OrderBy(a => a.NombreProducto)
                .ToList();

            if (tipoBodega != null)
                productos = productos.Where(a => a.TipoBodegaId == (int)tipoBodega).ToList();

            if (tipoProducto != null)
                productos = productos.Where(a => a.TipoProductoId == (int)tipoProducto).ToList();

            if (categoriaId != null)
                productos = productos.Where(a => a.CategoriaId == (int)categoriaId).ToList();

            if (grupoTerapeuticoId != null)
                productos = productos.Where(a => a.GrupoTProductoId == (int)grupoTerapeuticoId).ToList();


            foreach (var producto in productos)
            {
                //Se consulta de esta manera para evitar excepcion de tipo timeout
                //debido a la cantidad de entidades relacionadas
                producto.ProductosInventario = _context.ProductosInventario
                    .Include(a => a.Bodega)
                    .Include(a => a.UnidadMedidaVenta)
                    .Include(a => a.ProductosInventarioPrecios).ThenInclude(a => a.Precio)
                    .Where(p => p.ProductoId == producto.Id
                    && p.Eliminado == eliminado)
                    .ToList();


                //Filtro Ambiente
                if (ambienteId != null)
                {
                    producto.ProductosInventario = producto.ProductosInventario
                        .Where(a => a.Bodega != null && a.Bodega.AmbienteId == ambienteId)
                        .ToList();
                }
                else
                {
                    //Se filtra por bodega siempre u cuando el ambienteID sea NULO
                    //Filtro Bodega
                    if (bodegaId != null)
                    {
                        producto.ProductosInventario = producto.ProductosInventario
                                .Where(a => a.BodegaId == bodegaId)
                            .ToList();
                    }
                }

                if (sucursalId != null)
                {
                    producto.ProductosInventario = producto.ProductosInventario
                            .Where(a => a.Bodega != null
                            && a.Bodega.SucursalId != null
                            && a.Bodega.SucursalId == sucursalId)
                        .ToList();
                }


            }

            return productos;
        }

        public List<ProductoInventario> BuscarRegistrosProductosInventario(int productoId, int? unidadMedidaVentaId)
        {
            return _context.ProductosInventario
                .Where(a => a.ProductoId == productoId
                && a.UnidadMedidaVentaId == unidadMedidaVentaId)
                .ToList();
        }
        //Trae todos los productos que tiene precios en sus inventarios
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
           )
        {
            var productos = _context.Productos
                .Include(a => a.Categoria)
                .Include(a => a.Marca)
                .Include(a => a.GrupoTProducto)
                .Include(a => a.LaboratorioProducto)
                .Include(a => a.Viadmin)
                .Include(a => a.PresentacionProducto)
                .Where(a => a.Eliminado == eliminado)
                .OrderBy(a => a.NombreProducto)
                .ToList();

            if (tipoBodega != null)
                productos = productos.Where(a => a.TipoBodegaId == (int)tipoBodega).ToList();

            if (tipoProducto != null)
                productos = productos.Where(a => a.TipoProductoId == (int)tipoProducto).ToList();

            if (categoriaId != null)
                productos = productos.Where(a => a.CategoriaId == (int)categoriaId).ToList();

            if (grupoTerapeuticoId != null)
                productos = productos.Where(a => a.GrupoTProductoId == (int)grupoTerapeuticoId).ToList();


            var productosAChequear = new List<Producto>(productos);

            foreach (var producto in productosAChequear)
            {
                //Se consulta de esta manera para evitar excepcion de tipo timeout
                //debido a la cantidad de entidades relacionadas
                producto.ProductosInventario = _context.ProductosInventario
                    .Include(a => a.Bodega)
                    .Include(a => a.UnidadMedidaVenta)
                    .Include(a => a.ProductosInventarioPrecios).ThenInclude(a => a.Precio)
                    .Where(p => p.ProductoId == producto.Id
                    && p.Eliminado == eliminado)
                    .ToList();

                //Filtro Ambiente
                if (ambienteId != null)
                {
                    producto.ProductosInventario = producto.ProductosInventario
                        .Where(a => a.Bodega != null && a.Bodega.AmbienteId == ambienteId)
                        .ToList();
                }
                else
                {
                    //Se filtra por bodega siempre u cuando el ambienteID sea NULO
                    //Filtro Bodega
                    if (bodegaId != null)
                    {
                        producto.ProductosInventario = producto.ProductosInventario
                            .Where(a => a.BodegaId == bodegaId)
                            .ToList();
                    }
                }

                if (sucursalId != null)
                {
                    producto.ProductosInventario = producto.ProductosInventario
                        .Where(a => a.Bodega != null
                        && a.Bodega.SucursalId != null
                        && a.Bodega.SucursalId == sucursalId)
                        .ToList();
                }

                // Verificar si la cantidad de elementos en ProductosInventario no es igual a 1 y eliminar el producto si es necesario
                if (producto.ProductosInventario.Count != 1)
                {
                    // Remover el producto de la lista
                    // Suponiendo que 'productos' es una lista
                    productos.Remove(producto);
                }


            }

            return productos;
        }
        public Bodega GetSucursalIdByTipoBodega(int bodegaId)
        {
            return _context.Bodegas.Where(a => a.Id == bodegaId).FirstOrDefault();
        }

        public ProductoInventario GetRegistroInventarioProducto(int productoInventarioId, int? productoId = null)
        {
            if (productoId != null)
            {
                return _context.ProductosInventario
                    .Where(a => a.ProductoId == (int)productoId && a.Stock > 0)
                    .OrderBy(a => a.FechaVencimientoArticuloCompra)
                    .FirstOrDefault();
            }
            return _context.ProductosInventario
                .Where(a => a.Id == productoInventarioId)
                .FirstOrDefault();
        }

        //Buscar un productoInventario por el id del producto
        public ProductoInventario GetRegistroInventarioProductoxProductoid(int ProductoId)
        {
            return _context.ProductosInventario
                .Where(a => a.ProductoId == ProductoId)
                .FirstOrDefault();
        }

        public ProductoInventario GetProductoBodegaUnidad(int bodegaId, int? unidadMedidaVentaId)
        {
            return _context.ProductosInventario
                .Include(a => a.ProductosInventarioPrecios)
                .Where(a => a.BodegaId == bodegaId
                && a.UnidadMedidaVentaId == unidadMedidaVentaId
                && !a.Eliminado)
                .FirstOrDefault();
        }
        public List<DetalleVenta> GetDetalleVentasByProductoInventarioId(int productoInventarioId)
        {
            return _context.DetalleVentas
                .Where(dv => dv.ProductoInventarioId == productoInventarioId)
                .ToList();
        }

        public void EliminarDetalleVenta(DetalleVenta detalleVenta)
        {
            _context.DetalleVentas.Remove(detalleVenta);
            _context.SaveChanges();
        }

        public ProductoInventario GetValidarProductoInventario(int bodegaId, int? unidadMedidaVentaId, int productoId)//valida si existe un producto actual en inventario igual entonces solo actualiza de locontrario retorna un valor null para crear uno nuevo 
        {
            return _context.ProductosInventario
                .Include(a => a.ProductosInventarioPrecios)
                .Where(a => a.BodegaId == bodegaId
                && a.UnidadMedidaVentaId == unidadMedidaVentaId
                && a.ProductoId == productoId
                && !a.Eliminado)
                .FirstOrDefault();
        }
        public void UpdateRegistroInventario(ProductoInventario productoInventario, bool saveChanges = true)
        {
            _context.Entry(productoInventario).State = EntityState.Modified;
            if (saveChanges)
            {
                SaveChanges();
            }
        }
        public List<ProductoInventarioPrecio> ConsultarPreciosProductoInventario(int productoInventarioId)
        {
            return _context.ProductosInventarioPrecios
                .Include(a => a.Precio)
                .Where(a => a.ProductoInventarioId == productoInventarioId
                && !a.Precio.Eliminado)
                .ToList();
        }
        public PaginacionList<Producto> PaginacionProductosLabInsumosMedicos(string searchString, int? pageNumber, int pageSize, int? categoriaId)
        {

            var productos = _context.Productos.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                productos = productos.Where(s =>
                s.NombreProducto.Contains(searchString)
                || s.CodigoReferencia.Contains(searchString))
                ;
            }

            if (categoriaId != null) productos = productos.Where(a => a.CategoriaId == categoriaId);

            return PaginacionList<Producto>.CreateAsyncc(productos
            .Include(a => a.Categoria)
            .OrderBy(a => a.NombreProducto)
            .Where(a => a.Eliminado == false && a.TipoBodegaId == 4 && a.TipoProductoId == (int)TipoProductoEnum.InsumosMedicos),

            pageNumber ?? 1, pageSize);
        }

        public PaginacionList<ProductoInventario> PaginacionProductosFaltantes(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            //var productos = _context.Productos.AsQueryable()
            //    .Where(a => a.ProductosInventario. Stock <= 5 && a.TipoBodegaId == 1),
            //    ;
            var productos = _context.ProductosInventario.AsQueryable()
                .Include(a => a.Producto).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                productos = productos.Where(s => s.Producto.NombreProducto.Contains(searchString) || s.Producto.CodigoReferencia.Contains(searchString));
            }


            return PaginacionList<ProductoInventario>.CreateAsyncc(productos
            .Where(a => a.Eliminado == false)
            .Where(a => a.Stock <= 5 && a.BodegaId == 1),
            pageNumber ?? 1, pageSize);
        }

        public IList<Producto> GetListadoFaltantesFarmacia(string searchString)
        {
            var producto = _context.Productos.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                producto = producto.Where(s => s.NombreProducto.Contains(searchString) || s.CodigoReferencia.Contains(searchString));
            }

            return producto.Where(x => x.Eliminado == false)
            .Where(a => a.Stock <= 5 && a.TipoBodegaId == 1)
            .ToList();
        }

        public PaginacionList<ProductoInventario> PaginacionProductosFaltantesClinica(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            //var productos = _context.Productos.AsQueryable();

            //if (!string.IsNullOrEmpty(searchString))
            //{
            //    productos = productos.Where(s => s.NombreProducto.Contains(searchString) || s.CodigoReferencia.Contains(searchString));
            //}


            //return PaginacionList<Producto>.CreateAsyncc(productos
            //.Where(a => a.Eliminado == false)
            //.Where(a => a.Stock <= 1 && a.TipoBodegaId == 2 && a.TipoProductoId == (int)TipoProductoEnum.Medicamentos
            //    || a.Stock <= 5 && a.TipoBodegaId == 2 && a.TipoProductoId == (int)TipoProductoEnum.InsumosMedicos),
            //pageNumber ?? 1, pageSize);

            var productos = _context.ProductosInventario.AsQueryable()
               .Include(a => a.Producto).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                productos = productos.Where(s => s.Producto.NombreProducto.Contains(searchString) || s.Producto.CodigoReferencia.Contains(searchString));
            }


            return PaginacionList<ProductoInventario>.CreateAsyncc(productos
            .Where(a => a.Eliminado == false)
            .Where(a => a.Stock <= 1 && a.BodegaId == 2 && a.Producto.TipoProductoId == 1
                || a.Stock <= 5 && a.BodegaId == 2 && a.Producto.TipoProductoId == 2),
            pageNumber ?? 1, pageSize);
        }

        public PaginacionList<ProductoInventario> PaginacionProductosFaltantesLab(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var productos = _context.ProductosInventario.AsQueryable()
              .Include(a => a.Producto).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                productos = productos.Where(s => s.Producto.NombreProducto.Contains(searchString) || s.Producto.CodigoReferencia.Contains(searchString));
            }

            return PaginacionList<ProductoInventario>.CreateAsyncc(productos
            .Where(a => a.Eliminado == false)
            .Where(a => a.Stock <= 1 && a.BodegaId == 4 && a.Producto.TipoProductoId == 1
                || a.Stock <= 5 && a.BodegaId == 4 && a.Producto.TipoProductoId == 2),
            pageNumber ?? 1, pageSize);
        }

        public IList<Producto> GetListadoFaltantesClinica(string searchString)
        {
            var producto = _context.Productos.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                producto = producto.Where(s => s.NombreProducto.Contains(searchString) || s.CodigoReferencia.Contains(searchString));
            }

            return producto.Where(x => x.Eliminado == false)
            .Where(a => a.Stock <= 5 && a.TipoBodegaId == 2)
            .ToList();
        }

        public IList<Producto> GetListadoFaltantesLab(string searchString)
        {
            var producto = _context.Productos.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                producto = producto.Where(s => s.NombreProducto.Contains(searchString) || s.CodigoReferencia.Contains(searchString));
            }

            return producto.Where(x => x.Eliminado == false)
            .Where(a => a.Stock <= 5 && a.TipoBodegaId == 4)
            .ToList();
        }


        public PaginacionList<Producto> PaginacionProductosVencimiento(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var productos = _context.Productos.AsQueryable();

            var fecha = (DateTime.Now).AddDays(-4);

            if (!string.IsNullOrEmpty(searchString))
            {
                productos = productos.Where(s => s.NombreProducto.Contains(searchString) || s.CodigoReferencia.Contains(searchString));
            }


            switch (sortOrder)
            {
                case "Nombre_desc":
                    productos = productos.OrderByDescending(s => s.NombreProducto);
                    break;

                default:
                    productos = productos.OrderBy(s => s.NombreProducto);
                    break;
            }

            return PaginacionList<Producto>.CreateAsyncc(productos.Where(a => a.Eliminado == false).Where(a => a.FechaVencimiento > fecha), pageNumber ?? 1, pageSize);
        }


        public Producto GetPorNumeroDeReferencia(string id, bool includeRelatedEntities = true)
        {
            return _context.Productos.Where(a => a.CodigoReferencia == id)
            .Where(a => a.Eliminado == false && a.Stock > 0 && a.TipoBodegaId == 1).SingleOrDefault();
        }

        public IList<Producto> GetPorNumeroDeReferenciaList(string id, bool includeRelatedEntities = true)
        {
            return _context.Productos.Where(a => a.CodigoReferencia == id)
            .Where(a => a.Eliminado == false && a.Stock > 0 && a.TipoBodegaId == 1).ToList();
        }

        public Producto GetPorNumeroDeReferenciaClinica(string id, bool includeRelatedEntities = true)
        {
            return _context.Productos
                .Include(a => a.ProductosInventario
                .Where(a => a.Stock > 0))
            .Where(a => a.CodigoReferencia.Trim() == id && a.Eliminado == false
            && a.TipoBodegaId == (int)TipoBodegaEnum.Clinica
            && a.ProductosInventario.Count() > 0).SingleOrDefault();
        }

        public Producto GetProdutoById(int id)
        {
            return _context.Productos.Where(a => a.Id == id)
            .Where(a => a.Eliminado == false)
            .SingleOrDefault();
        }


        public Producto GetPorNumeroDeReferenciayNombre(string id, bool includeRelatedEntities = true)
        {
            var productos = _context.Productos.AsQueryable();

            if (includeRelatedEntities)
            {
                productos = productos.Include(a => a.Viadmin);
            }

            return productos
               .Where(a => a.CodigoReferencia == id || a.NombreProducto.Contains(id))
               .SingleOrDefault();
        }



        public IList<RetornoProductoPOSClinica> BuscarPorNombreYReferenciaBusquedaAjaxClinica(string searchString)
        {
            var productosInventario = _context.ProductosInventario.AsQueryable();
            return productosInventario
                .Include(a => a.Producto).ThenInclude(a => a.Viadmin)
                .Include(a => a.Producto).ThenInclude(a => a.PresentacionProducto)
                .Include(a => a.Producto).ThenInclude(a => a.GrupoTProducto)
                .Include(a => a.Producto).ThenInclude(a => a.LaboratorioProducto)
                .Include(a => a.Producto).ThenInclude(a => a.Categoria)
                .Include(a => a.Producto).ThenInclude(a => a.Marca)
                .Include(a => a.Producto).ThenInclude(a => a.Grupo)
                .Include(a => a.UnidadMedidaVenta)
                .Where(s => s.Producto.NombreProducto.Contains(searchString)
                            || s.Producto.CodigoReferencia.Contains(searchString))
                .Where(a => !a.Producto.Eliminado
                            && a.Producto.TipoBodegaId == (int)TipoBodegaEnum.Clinica
                            && a.Stock > 0)
                .Select(a => new RetornoProductoPOSClinica
                {
                    viadmin = a.Producto.Viadmin.NombreViadmin,
                    presentacion = a.Producto.PresentacionProducto.PresentProducto,
                    grupoT = a.Producto.GrupoTProducto.NombreGrupoT,
                    lab = a.Producto.LaboratorioProducto.NombreLaboratorioProducto,
                    categoria = a.Producto.Categoria.NombreCategoria,
                    marca = a.Producto.Marca.NombreMarca,
                    grupo = a.Producto.Grupo.NombreGrupo,
                    nombreProducto = a.Producto.NombreProducto,
                    stock = a.Stock.ToString(),
                    codigoReferencia = a.Producto.CodigoReferencia,
                    tipoProductoId = (int)a.Producto.TipoProductoId,
                    dosis = a.Producto.Dosis,
                    precio_5 = a.Producto.Precio_5.ToString(),
                    activoYConcentracion = a.Producto.ActivoYConcentracion,
                    unidadVenta = a.UnidadMedidaVenta.Nombre,
                    imagen = a.Producto.Imagen
                })
                .ToList();
        }
        public List<ProductoInventario> BuscarProductosNombre(string searchString)
        {
            var productosInventario = _context.ProductosInventario.AsQueryable();
            return productosInventario
                .Include(a => a.ProductosInventarioPrecios).ThenInclude(a => a.Precio)
                .Include(a => a.Producto).ThenInclude(a => a.Viadmin)
                .Include(a => a.Producto).ThenInclude(a => a.PresentacionProducto)
                .Include(a => a.Producto).ThenInclude(a => a.GrupoTProducto)
                .Include(a => a.Producto).ThenInclude(a => a.LaboratorioProducto)
                .Include(a => a.Producto).ThenInclude(a => a.Categoria)
                .Include(a => a.Producto).ThenInclude(a => a.Marca)
                .Include(a => a.Producto).ThenInclude(a => a.Grupo)
                .Include(a => a.UnidadMedidaVenta)
                .Where(s => s.Producto.NombreProducto.ToLower().Contains(searchString.ToLower())
                            || s.Producto.CodigoReferencia.ToLower().Contains(searchString.ToLower()))
                .Where(a => !a.Producto.Eliminado
                            && a.Stock > 0)
                .ToList();
        }
        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public Producto GetProductoByTipoBodegaAndNombre(int tipoBodega, string nombre)
        {
            return _context.Productos
            .Where(a => a.TipoBodegaId == tipoBodega && a.NombreProducto == nombre && a.Eliminado == false)
            .SingleOrDefault();
        }


        public PaginacionList<ProductoInventario> PaginacionVencidosFarmacia(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            //var productos = _context.Productos.AsQueryable();

            //if (!string.IsNullOrEmpty(searchString))
            //{
            //    productos = productos.Where(s => s.NombreProducto.Contains(searchString) || s.CodigoReferencia.Contains(searchString));
            //}



            //return PaginacionList<Producto>.CreateAsyncc(productos
            //.Where(a => a.Eliminado == false)
            //.Where(a => a.TipoBodegaId == 1 && a.FechaVencimiento != null && a.FechaVencimiento < DateTime.Now),
            //pageNumber ?? 1, pageSize);

            var productos = _context.ProductosInventario.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                productos = productos.Where(s => s.Producto.NombreProducto.Contains(searchString) || s.Producto.CodigoReferencia.Contains(searchString));
            }



            return PaginacionList<ProductoInventario>.CreateAsyncc(productos
            .Include(a => a.Producto)
            .Where(a => a.Eliminado == false)
            .Where(a => a.BodegaId == 1 && a.FechaVencimientoArticuloCompra != null && a.FechaVencimientoArticuloCompra < DateTime.Now),
            pageNumber ?? 1, pageSize);
        }

        public PaginacionList<ProductoInventario> PaginacionProximosAVencerFarmacia(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var productos = _context.ProductosInventario
                .Where(a => !a.Eliminado && !a.Producto.Eliminado)
                .Where(a => a.FechaVencimientoArticuloCompra > DateTime.Now)
                .Where(a => a.FechaVencimientoArticuloCompra.HasValue &&
                (a.FechaVencimientoArticuloCompra.Value - DateTime.Now).Days >= 0 &&
                 (a.FechaVencimientoArticuloCompra.Value - DateTime.Now).Days <= 90)
                .Where(a => a.Bodega != null && a.Bodega.AmbienteId == (int)AmbienteEnum.Farmacia)
                .AsQueryable();


            if (!string.IsNullOrEmpty(searchString))
            {
                productos = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ProductoInventario, Producto>)productos.Where(s =>
                s.Producto.NombreProducto.Contains(searchString)
                || s.Producto.CodigoReferencia.Contains(searchString))
                ;
            }

            return PaginacionList<ProductoInventario>.CreateAsyncc(productos
            .Include(a => a.Producto)
            .OrderBy(a => a.Producto.NombreProducto),

            pageNumber ?? 1, pageSize);

        }

        public PaginacionList<ProductoInventario> PaginacionVencidosClinica(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            //var productos = _context.Productos.AsQueryable();

            //if (!string.IsNullOrEmpty(searchString))
            //{
            //    productos = productos.Where(s => s.NombreProducto.Contains(searchString) || s.CodigoReferencia.Contains(searchString));
            //}


            //return PaginacionList<Producto>.CreateAsyncc(productos
            //.Where(a => a.Eliminado == false)
            //.Where(a => a.TipoBodegaId == 2 && a.FechaVencimiento != null && a.FechaVencimiento < DateTime.Now),
            //pageNumber ?? 1, pageSize);

            var productos = _context.ProductosInventario.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                productos = productos.Where(s => s.Producto.NombreProducto.Contains(searchString) || s.Producto.CodigoReferencia.Contains(searchString));
            }



            return PaginacionList<ProductoInventario>.CreateAsyncc(productos
            .Include(a => a.Producto)
            .Where(a => a.Eliminado == false)
            .Where(a => a.BodegaId == 2 && a.FechaVencimientoArticuloCompra != null && a.FechaVencimientoArticuloCompra < DateTime.Now),
            pageNumber ?? 1, pageSize);

        }




        public PaginacionList<ProductoInventario> PaginacionProximosAVencerClinica(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {

            var productos = _context.ProductosInventario
                .Where(a => !a.Eliminado && !a.Producto.Eliminado)
                .Where(a => a.FechaVencimientoArticuloCompra > DateTime.Now)
                .Where(a => a.FechaVencimientoArticuloCompra.HasValue &&
                (a.FechaVencimientoArticuloCompra.Value - DateTime.Now).Days >= 0 &&
                 (a.FechaVencimientoArticuloCompra.Value - DateTime.Now).Days <= 90)
                .Where(a => a.Bodega != null && a.Bodega.AmbienteId == (int)AmbienteEnum.Clinica)
                .AsQueryable();


            if (!string.IsNullOrEmpty(searchString))
            {
                productos = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ProductoInventario, Producto>)productos.Where(s =>
                s.Producto.NombreProducto.Contains(searchString)
                || s.Producto.CodigoReferencia.Contains(searchString))
                ;
            }

            return PaginacionList<ProductoInventario>.CreateAsyncc(productos
            .Include(a => a.Producto)
            .OrderBy(a => a.Producto.NombreProducto),

            pageNumber ?? 1, pageSize);
            //var productos = _context.Productos.AsQueryable();

            //if (!string.IsNullOrEmpty(searchString))
            //{
            //    productos = productos.Where(s =>
            //    s.NombreProducto.Contains(searchString)
            //    || s.CodigoReferencia.Contains(searchString))
            //    ;
            //}

            //return PaginacionList<Producto>.CreateAsyncc(productos
            //.OrderBy(a => a.NombreProducto)
            //.Where(a => a.Eliminado == false && a.TipoBodegaId == 2 && a.FechaVencimiento != null)
            //// .Where(a => EF.Functions.DateDiffDay( DateTime.Now, a.FechaVencimiento.Value) > 0)
            //.Where(a => DateTime.Now.DayOfWeek - a.FechaVencimiento.Value.DayOfWeek > 0)
            //// .Where(a => Math.Abs(EF.Functions.DateDiffDay(a.FechaVencimiento.Value, DateTime.Now)) >= 0)
            //.Where(a => Math.Abs((a.FechaVencimiento.Value.DayOfWeek - DateTime.Now.DayOfWeek)) >= 0)
            //// .Where(a => Math.Abs(EF.Functions.DateDiffDay(a.FechaVencimiento.Value, DateTime.Now)) <= 30),
            //.Where(a => Math.Abs((a.FechaVencimiento.Value.DayOfWeek - DateTime.Now.DayOfWeek)) <= 30),
            //pageNumber ?? 1, pageSize);
        }


        public PaginacionList<ProductoInventario> PaginacionVencidosLab(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            //var productos = _context.Productos.AsQueryable();

            //if (!string.IsNullOrEmpty(searchString))
            //{
            //    productos = productos.Where(s => s.NombreProducto.Contains(searchString) || s.CodigoReferencia.Contains(searchString));
            //}


            //return PaginacionList<Producto>.CreateAsyncc(productos
            //.Where(a => a.Eliminado == false)
            //.Where(a => a.TipoBodegaId == 4 && a.FechaVencimiento != null && a.FechaVencimiento < DateTime.Now),
            //pageNumber ?? 1, pageSize);

            var productos = _context.ProductosInventario.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                productos = productos.Where(s => s.Producto.NombreProducto.Contains(searchString) || s.Producto.CodigoReferencia.Contains(searchString));
            }



            return PaginacionList<ProductoInventario>.CreateAsyncc(productos
            .Include(a => a.Producto)
            .Where(a => a.Eliminado == false)
            .Where(a => a.BodegaId == 4 && a.FechaVencimientoArticuloCompra != null && a.FechaVencimientoArticuloCompra < DateTime.Now),
            pageNumber ?? 1, pageSize);
        }


        public PaginacionList<ProductoInventario> PaginacionProximosAVencerLab(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var productos = _context.ProductosInventario
             .Where(a => a.FechaVencimientoArticuloCompra > DateTime.Now)
             .Where(a => a.FechaVencimientoArticuloCompra.HasValue &&
             (a.FechaVencimientoArticuloCompra.Value - DateTime.Now).Days >= 0 &&
              (a.FechaVencimientoArticuloCompra.Value - DateTime.Now).Days <= 90)
             .Where(a => a.BodegaId == 4)
             .AsQueryable();


            if (!string.IsNullOrEmpty(searchString))
            {
                productos = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ProductoInventario, Producto>)productos.Where(s =>
                s.Producto.NombreProducto.Contains(searchString)
                || s.Producto.CodigoReferencia.Contains(searchString))
                ;
            }

            return PaginacionList<ProductoInventario>.CreateAsyncc(productos
            .Include(a => a.Producto)
            .OrderBy(a => a.Producto.NombreProducto),

            pageNumber ?? 1, pageSize);
        }

        public int GetTotalMedicamentosFarmacia()
        {
            return _context.Productos.Count(a => a.Eliminado == false && a.TipoBodegaId == 1 && a.TipoProductoId == (int)TipoProductoEnum.Medicamentos);
        }

        public int GetTotalInsumosFarmacia()
        {
            return _context.Productos.Count(a => a.Eliminado == false && a.TipoBodegaId == 1 && a.TipoProductoId == (int)TipoProductoEnum.InsumosMedicos);
        }

        public int GetTotalMedicamentosClinica(int? grupoTerapeuticoId, int? sucursalId)
        {
            int cantidadProductos = 0;
            var productos = _context.ProductosInventario
                .Include(a => a.Producto)
                .ToList();

            //Filtro grupo terapeutico
            if (grupoTerapeuticoId != null)
            {
                productos = productos
                    .Where(a => a.Producto.GrupoTProductoId == (int)grupoTerapeuticoId)
                    .ToList();
            }

            if (productos != null)
            {
                cantidadProductos = productos
                    .Select(a => a.ProductoId)
                    .Distinct()
                    .Count();
            }
            return cantidadProductos;
        }

        public int GetTotalMedicamentosLab()
        {
            return _context.Productos.Count(a => a.Eliminado == false && a.TipoBodegaId == 4 && a.TipoProductoId == (int)TipoProductoEnum.Medicamentos);
        }

        public int GetTotalInsumosClinica(int? categoriaId)
        {
            if (categoriaId == null)
                return _context.Productos.Count(a => a.Eliminado == false
                && a.TipoBodegaId == (int)TipoBodegaEnum.Clinica
                && a.TipoProductoId == (int)TipoProductoEnum.InsumosMedicos);
            else
                return _context.Productos.Count(a => a.Eliminado == false
                && a.TipoBodegaId == (int)TipoBodegaEnum.Clinica
                && a.TipoProductoId == (int)TipoProductoEnum.InsumosMedicos
                && a.CategoriaId == (int)categoriaId);
        }


        public int GetTotalInsumosLab()
        {
            return _context.Productos.Count(a => a.Eliminado == false && a.TipoBodegaId == 4 && a.TipoProductoId == (int)TipoProductoEnum.InsumosMedicos);
        }

        public int GetTotalMedicamentosBodega()
        {
            return _context.Productos.Count(a => a.Eliminado == false && a.TipoBodegaId == 3 && a.TipoProductoId == (int)TipoProductoEnum.Medicamentos);
        }

        public int GetTotalInsumosBodega()
        {
            return _context.Productos.Count(a => a.Eliminado == false && a.TipoBodegaId == 3 && a.TipoProductoId == (int)TipoProductoEnum.InsumosMedicos);
        }

        public IList<Producto> GetProductosLaboratorio(int? laboratorioId = null)
        {
            var productos = _context.Productos
                .Include(a => a.PresentacionProducto)
                .Where(p => !p.Eliminado
                && p.TipoBodegaId == (int)TipoBodegaEnum.Farmacia);

            if (laboratorioId == null)
            {
                return productos
                    .OrderBy(p => p.NombreProducto)
                    .ToList();
            }
            else
            {
                return productos
                    .Where(p => p.LaboratorioProductoId == laboratorioId)
                    .OrderBy(p => p.NombreProducto)
                    .ToList();
            }
        }


        public IList<RetornoProductoPOS> BuscarPorNombreYReferenciaBusquedaAjax(string searchString)
        {
            var productos = _context.Productos.AsQueryable();

            return productos.Where(s => s.NombreProducto.Contains(searchString))
                    .Include(a => a.Viadmin)
                    .Include(a => a.PresentacionProducto)
                    .Include(a => a.GrupoTProducto)
                    .Include(a => a.LaboratorioProducto)
                    .Include(a => a.Categoria)
                    .Include(a => a.Marca)
                    .Include(a => a.Grupo)
                    .Where(a => a.Eliminado == false && a.TipoBodegaId == (int)TipoBodegaEnum.Farmacia)
                    .Select(a => new RetornoProductoPOS
                    {
                        viadmin = a.Viadmin.NombreViadmin,
                        presentacion = a.PresentacionProducto.PresentProducto,
                        grupoT = a.GrupoTProducto.NombreGrupoT,
                        lab = a.LaboratorioProducto.NombreLaboratorioProducto,
                        categoria = a.Categoria.NombreCategoria,
                        marca = a.Marca.NombreMarca,
                        grupo = a.Grupo.NombreGrupo,
                        nombreProducto = a.NombreProducto,
                        stock = a.Stock.ToString(),
                        codigoReferencia = a.CodigoReferencia,
                        tipoProductoId = (int)a.TipoProductoId,
                        dosis = a.Dosis,
                        precio_5 = a.Precio_5.ToString(),
                        activoYConcentracion = a.ActivoYConcentracion,
                        imagen = a.Imagen
                    })
                    .ToList();
        }

        public IList<Producto> GetMedicamentosFarmaciaList()
        {
            return _context.Productos.Where(a => a.Eliminado == false && a.TipoBodegaId == 1 && a.TipoProductoId == (int)TipoProductoEnum.Medicamentos).ToList();
        }

        #region Unidades
        public List<UnidadMedidaCompra> GetUnidadesCompra()
        {
            return _context.UnidadesMedidaCompra.OrderBy(u => u.Nombre).ToList();
        }
        public List<UnidadMedidaVenta> GetUnidadesVenta()
        {
            return _context.UnidadesMedidaVenta.OrderBy(u => u.Nombre).ToList();
        }
        public void AddUnidadCompra(UnidadMedidaCompra model)
        {
            _context.UnidadesMedidaCompra.Add(model);
            _context.SaveChanges();
        }
        public void AddUnidadVenta(UnidadMedidaVenta model)
        {
            _context.UnidadesMedidaVenta.Add(model);
            _context.SaveChanges();
        }
        #endregion
        public List<ProductoEquivalencia> GetEquivalenciasProducto(int productoId)
        {
            return _context.ProductosEquivalencias
                .Include(p => p.UnidadMedidaCompra)
                .Include(p => p.UnidadMedidaVenta)
                .Where(p => p.ProductoId == productoId && !p.Eliminada)
                .ToList();
        }
        public ProductoInventarioPrecio GetProductoInventarioPrecio(int productoInventarioPrecioId)
        {
            return _context.ProductosInventarioPrecios
                .Where(a => a.Id == productoInventarioPrecioId)
                .FirstOrDefault();
        }
        public ProductoInventario GetProductoInventariobyId(int productoId)
        {
            return _context.ProductosInventario
                .Where(a => a.ProductoId == productoId)
                .FirstOrDefault();
        }
        public List<ProductoInventario> GetStocks(int productoId, int? sucursalId)
        {
            var stocks = _context.ProductosInventario
                .Include(a => a.ProductosInventarioPrecios)
                .Where(a => a.ProductoId == productoId)
                .ToList();
            return stocks;
        }

        public ProductoInventario GetProductoByIdAndBodegaId(int productoId, int? bodegaId)
        {
            return _context.ProductosInventario.Where(a => a.ProductoId == productoId && a.BodegaId == bodegaId)
                .Include(x => x.ProductosInventarioPrecios)
                .ThenInclude(a => a.Precio)
                .FirstOrDefault();
        }
        public Proveedor GetProveedorExistente(string nombre)
        {
            return _context.Proveedores
            .Where(a => a.Nombre == nombre)
            .SingleOrDefault();
        }

        public void AddLaboratorioProducto(LaboratorioProducto laboratorioProducto, bool saveChanges = true)
        {
            _context.LaboratorioProductos.Add(laboratorioProducto);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }
        public void AddPresentacionProducto(PresentacionProducto presentacionProducto, bool saveChanges = true)
        {
            _context.PresentacionProductos.Add(presentacionProducto);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        public int ExistOrAddLaboratorioProducto(LaboratorioProducto laboratorioProducto)
        {

            var lab = _context.LaboratorioProductos.Where(x => x.NombreLaboratorioProducto.ToLower().Trim() == laboratorioProducto.NombreLaboratorioProducto.ToLower().Trim()
                && x.Eliminado == false).FirstOrDefault();
            if (lab != null)
            {
                return lab.Id;
            }
            else
            {

                AddLaboratorioProducto(laboratorioProducto, true);

                return laboratorioProducto.Id;
            }

        }
        public int ExistOrAddPresentacionProducto(PresentacionProducto presentacionProducto)
        {

            var pres = _context.PresentacionProductos.Where(x => x.PresentProducto.ToLower().Trim() == presentacionProducto.PresentProducto.ToLower().Trim()
                && x.Eliminado == false).FirstOrDefault();
            if (pres != null)
            {
                return pres.Id;
            }
            else
            {

                AddPresentacionProducto(presentacionProducto, true);

                return presentacionProducto.Id;
            }

        }
        public void AddViaAdmin(Viadmin viadmin, bool saveChanges = true)
        {
            _context.viadmins.Add(viadmin);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }
        public int ExistOrAddViaAdmin(Viadmin viadmin)
        {

            var data = _context.viadmins.Where(x => x.NombreViadmin.ToLower().Trim() == viadmin.NombreViadmin.ToLower().Trim()
                && x.Eliminado == false).FirstOrDefault();
            if (data != null)
            {
                return data.Id;
            }
            else
            {

                AddViaAdmin(viadmin, true);

                return viadmin.Id;
            }

        }
        public void AddUnidadMedidaVenta(UnidadMedidaVenta unidadMedidaVenta, bool saveChanges = true)
        {
            _context.UnidadesMedidaVenta.Add(unidadMedidaVenta);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }
        public int ExistOrAddUnidadMedidaVenta(UnidadMedidaVenta unidadMedidaVenta)
        {

            var data = _context.UnidadesMedidaVenta
                .Where(x => x.Nombre.ToLower().Trim() == unidadMedidaVenta.Nombre.ToLower().Trim()).FirstOrDefault();
            if (data != null)
            {
                return data.Id;
            }
            else
            {

                AddUnidadMedidaVenta(unidadMedidaVenta, true);

                return unidadMedidaVenta.Id;
            }

        }
        public void AddUnidadMedidaVenta(UnidadMedidaCompra unidadMedidaCompra, bool saveChanges = true)
        {
            _context.UnidadesMedidaCompra.Add(unidadMedidaCompra);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }
        public int ExistOrAddUnidadMedidaCompra(UnidadMedidaCompra unidadMedidaCompra)
        {

            var data = _context.UnidadesMedidaCompra.Where(x => x.Nombre.ToLower().Trim() == unidadMedidaCompra.Nombre.ToLower().Trim()).FirstOrDefault();
            if (data != null)
            {
                return data.Id;
            }
            else
            {

                AddUnidadMedidaVenta(unidadMedidaCompra, true);

                return unidadMedidaCompra.Id;
            }

        }

        public List<AuditoriaNuevoSP> GetBandejaAuditoriaNuevo(int? tipoBodega, int? tipoProducto)
        {
            List<AuditoriaNuevoSP> sp = new List<AuditoriaNuevoSP>();

            if (tipoBodega == null && tipoProducto == null)
            {
                sp = _context.AuditoriaNuevoSP.FromSqlInterpolated($"SELECT * FROM auditoria_nuevo_inventario_producto(NULL,NULL);").ToList();
            }
            else if (tipoBodega == null && tipoProducto != null)
            {
                sp = _context.AuditoriaNuevoSP.FromSqlInterpolated($"SELECT * FROM auditoria_nuevo_inventario_producto({(int)tipoProducto},NULL);").ToList();
            }
            else if (tipoBodega != null && tipoProducto == null)
            {
                sp = _context.AuditoriaNuevoSP.FromSqlInterpolated($"SELECT * FROM auditoria_nuevo_inventario_producto(NULL,{(int)tipoBodega});").ToList();
            }
            else
            {
                sp = _context.AuditoriaNuevoSP.FromSqlInterpolated($"SELECT * FROM auditoria_nuevo_inventario_producto({(int)tipoProducto},{(int)tipoBodega});").ToList();
            }

            return sp;
        }

        public List<Producto> GetProductosByBodegaAndByTipoProducto(int? tipoBodega, int? tipoProducto, bool eliminado = false)
        {
            var productos = _context.Productos
                .Include(a => a.Categoria)
                .Include(a => a.Marca)
                .Include(a => a.GrupoTProducto)
                .Include(a => a.LaboratorioProducto)
                .Include(a => a.Viadmin)
                .Include(a => a.PresentacionProducto)
                .Include(a => a.TipoProducto)
                .Where(a => a.Eliminado == eliminado)
                .OrderBy(a => a.NombreProducto)
                .ToList();

            //if (tipoBodega != null)
            //    productos = productos.Where(a => a.TipoBodegaId == (int)tipoBodega).ToList();

            if (tipoProducto != null)
                productos = productos.Where(a => a.TipoProductoId == (int)tipoProducto).ToList();


            foreach (var producto in productos)
            {
                //Se consulta de esta manera para evitar excepcion de tipo timeout
                //debido a la cantidad de entidades relacionadas
                producto.ProductosInventario = _context.ProductosInventario
                    .Include(a => a.Bodega)
                    .Include(a => a.UnidadMedidaVenta)
                    .Include(a => a.UnidadMedidaCompra)
                    .Include(a => a.ProductosInventarioPrecios).ThenInclude(a => a.Precio)
                    .Where(p => p.ProductoId == producto.Id
                    && p.Eliminado == eliminado)
                    .ToList();



                if (tipoBodega != null)
                {
                    if (producto.ProductosInventario.Count != 0)
                    {
                        producto.ProductosInventario = producto.ProductosInventario
                                .Where(a => a.BodegaId == tipoBodega)
                            .ToList();
                    }
                }

            }

            return productos;
        }

        public List<ProductoInventario> GetProductosInventarioStockBajo()
        {
            List<ProductoInventario> data = new List<ProductoInventario>();

            data = _context.ProductosInventario
                .Include(x => x.UnidadMedidaCompra)
                .Include(x => x.UnidadMedidaVenta)
                .Include(x => x.Compra)
                    .ThenInclude(a => a.DetalleCompras)
                .Include(x => x.Producto)
                    .ThenInclude(x => x.ProductoEquivalencias)
                        .ThenInclude(x => x.UnidadMedidaVenta)
                .Include(x => x.Producto)
                    .ThenInclude(x => x.ProductoEquivalencias)
                        .ThenInclude(x => x.UnidadMedidaVenta)


                .Where(x => x.Stock <= x.StockMinimo).ToList();

            return data;
        }

        public List<ProductoInventarioPrecio> GetProductoInventarioPrecioByIdProducto(int productoId)
        {
            var prueba = _context.ProductosInventarioPrecios
                .Include(x => x.ProductoInventario)
                .Include(x => x.Precio)
                .Where(x => x.ProductoInventario.ProductoId == productoId).ToList();

            var data = _context.ProductosInventarioPrecios
                .Include(x => x.ProductoInventario)
                    .ThenInclude(pi => pi.Bodega)
                .Include(x => x.Precio)
                .Where(x => x.ProductoInventario.ProductoId == productoId
                    && x.ProductoInventario.Bodega != null
                    && (x.ProductoInventario.Bodega.AmbienteId == (int)AmbienteEnum.Hospital
                        || x.ProductoInventario.Bodega.AmbienteId == (int)AmbienteEnum.Farmacia)
                    && !x.Precio.Eliminado).ToList();

            return data;
        }

        public ProductoInventario GetStocksVencido(int Id)
        {
            var stock = _context.ProductosInventario
                .Include(a => a.Producto)
                .Include(a => a.Bodega)
                .Include(a => a.ProductosInventarioPrecios)
                .Where(a => a.Id == Id).FirstOrDefault();
            return stock;
        }

        public List<ProductoInventarioPrecio> GetPreciosProducto(int productoId)
        {
            var data = _context.ProductosInventario
                .Include(x => x.ProductosInventarioPrecios)
                    .ThenInclude(x => x.Precio)
                .Where(x => x.ProductoId == productoId)
                .FirstOrDefault();

            if (data == null)
            {
                return new List<ProductoInventarioPrecio>();
            }
            return data.ProductosInventarioPrecios.Where(x => !x.Precio.Eliminado).ToList();
        }

        public UnidadMedidaVenta GetUnidadMedidaVentaByProductoId(int productoId)
        {
            var data = _context.ProductosInventario
                .Include(x => x.UnidadMedidaVenta)
                .Include(x => x.ProductosInventarioPrecios)
                    .ThenInclude(x => x.Precio)
                .Where(x => x.ProductoId == productoId)
                .FirstOrDefault();

            if (data == null)
            {
                return new UnidadMedidaVenta();
            }
            return data.UnidadMedidaVenta;
        }

        public string GetNombrePrecio(int precioId)
        {
            // Suponiendo que tienes una tabla o entidad llamada "Precios" con una columna "Nombre"
            var precio = _context.Precios.FirstOrDefault(p => p.Id == precioId);
            return precio?.NombrePrecio ?? "Sin Nombre"; // Retorna "Sin Nombre" si no se encuentra
        }

        public ProductoInventarioPrecio GetProductoInventarioPrecioPorInventarioYTipoPrecio(int productoInventarioId, int precioId)
        {
            return _context.ProductosInventarioPrecios
                .FirstOrDefault(p => p.ProductoInventarioId == productoInventarioId && p.PrecioId == precioId && !p.Eliminado);
        }

        public List<ProductoInventario> GetInventarioDisponiblePorBodega(int bodegaId)
        {
            return _context.ProductosInventario
                .Include(x => x.Producto)
                .Include(x => x.UnidadMedidaVenta)
                .Where(x => !x.Eliminado
                            && x.BodegaId.HasValue
                            && x.BodegaId.Value == bodegaId
                            && x.Stock > 0)
                .ToList();
        }

        public ProductoInventarioPrecio ObtenerPrecioProductoPorDefecto(int productoId)
        {
            return _context.ProductosInventario
                .Where(pi => pi.ProductoId == productoId && pi.Stock > 0)
                .SelectMany(pi => pi.ProductosInventarioPrecios)
                .OrderBy(pip => pip.Precio.NombrePrecio == "NORMAL" ? 0 : 1)
                .FirstOrDefault();
        }

        public List<ProductoInventarioDto> GetProductosHospitalizacion(int? bodegaId = null)
        {
            var bodega = bodegaId.GetValueOrDefault(8);
            return _context.ProductosInventario
                .Include(pi => pi.Producto)
                    .ThenInclude(p => p.GrupoTProducto)
                .Include(pi => pi.Bodega)
                .Where(pi => pi.BodegaId == bodega
                          && pi.Stock > 0
                          && !pi.Eliminado
                          && (pi.Producto.TipoProductoId == 1
                              || (pi.Producto.GrupoTProducto != null
                                  && pi.Producto.GrupoTProducto.NombreGrupoT != null
                                  && pi.Producto.GrupoTProducto.NombreGrupoT.ToLower().Contains("psicotrop"))))
                .GroupBy(pi => pi.ProductoId)
                .Select(g => new ProductoInventarioDto
                {
                    ProductoId = g.Key,
                    ProductoCodigo = g.First().Producto.CodigoReferencia,
                    ProductoNombre = g.First().Producto.NombreProducto
                })
                .OrderBy(p => p.ProductoNombre)
                .ToList();
        }


        public List<ProductoInventarioDto> GetProductosConStockHospitalizacion()
        {
            var query = from pi in _context.ProductosInventario
                        join p in _context.Productos on pi.ProductoId equals p.Id
                        join b in _context.Bodegas on pi.BodegaId equals b.Id
                        where b.AmbienteId == 3
                           && pi.BodegaId == 8
                           && pi.Stock > 0
                           && p.TipoProductoId == 1
                        select new ProductoInventarioDto
                        {
                            ProductoId = p.Id,
                            ProductoCodigo = p.CodigoReferencia,
                            ProductoNombre = p.NombreProducto
                        };

            return query.Distinct().OrderBy(x => x.ProductoNombre).ToList();
        }

    }

    public class RetornoProductoPOS
    {
        public string nombreProducto { get; set; }
        public string stock { get; set; }
        public string codigoReferencia { get; set; }
        public string viadmin { get; set; }
        public string presentacion { get; set; }
        public string grupoT { get; set; }
        public string lab { get; set; }
        public string categoria { get; set; }
        public string marca { get; set; }
        public string grupo { get; set; }
        public int tipoProductoId { get; set; }
        public string dosis { get; set; }
        public string precio_5 { get; set; }
        public string activoYConcentracion { get; set; }
        public string imagen { get; set; }



    }

    public class RetornoProductoPOSClinica
    {
        public string nombreProducto { get; set; }
        public string stock { get; set; }
        public string codigoReferencia { get; set; }
        public string viadmin { get; set; }
        public string unidadVenta { get; set; }
        public string presentacion { get; set; }
        public string grupoT { get; set; }
        public string lab { get; set; }
        public string categoria { get; set; }
        public string marca { get; set; }
        public string grupo { get; set; }
        public int tipoProductoId { get; set; }
        public string dosis { get; set; }
        public string precio_5 { get; set; }
        public string activoYConcentracion { get; set; }
        public string imagen { get; set; }

    }
}