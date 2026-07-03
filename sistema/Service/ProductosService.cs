// using Database.Shared.Dto;
// using Database.Shared.Enumeraciones;
// using Database.Shared.IRepository;
// using Database.Shared.Models;
// using farmamest.Service.IService;
// using Microsoft.CodeAnalysis.CSharp.Syntax;
// using sistema.Models;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;

// namespace farmamest.Service
// {
//     public class ProductosService : IProductosService
//     {
//         private readonly IProducto _productoRepository;

//         public ProductosService(IProducto productoRepository)
//         {
//             _productoRepository = productoRepository;
//         }
//         public List<Producto> GetInventarioBySp(int? tipoProductoId, int? grupoTerapeuticoId, int? bodegaId, int? sucursalId, int? ambienteId)
//         {
//             var data = _productoRepository.GetInventarioSP(tipoProductoId, grupoTerapeuticoId, bodegaId, sucursalId, ambienteId);
//             return data;
//         }
//         public List<MovimientoProductoViewModel> GetHistoricoProductos(DateTime fechaInicio, DateTime fechaFin, List<int> productosIds)
//         {
//             var historico = new List<MovimientoProductoViewModel>();
//             var movimientos = _productoRepository.GetMovimientos(fechaInicio, fechaFin, productosIds);
//             if (movimientos != null)
//             {
//                 foreach (var movimiento in movimientos)
//                 {
//                     var productoInventario = movimiento.ProductoInventario ?? new ProductoInventario();
//                     var producto = productoInventario.Producto ?? new Producto();
//                     var tipoProducto = producto.TipoProducto ?? new TipoProducto();
//                     var unidad = productoInventario.UnidadMedidaVenta ?? new UnidadMedidaVenta();
//                     var ambiente = producto.Ambiente ?? new Ambiente();
//                     var bodega = productoInventario.Bodega ?? new Bodega();
//                     var usuario = movimiento.UsuarioRealiza ?? new User();
//                     var tipoMovimiento = movimiento.TipoMovimientoProducto ?? new TipoMovimientoProducto();
//                     decimal? costoPonderadoCompra = null;
//                     if (movimiento.TipoMovimientoProductoId == (int)TipoMovimientoProductoEnum.EntradaCompra)
//                         costoPonderadoCompra = (productoInventario.PrecioCosto ?? 0) * movimiento.Cantidad;
//                     decimal? costoPonderadoVenta = null;
//                     if (movimiento.TipoMovimientoProductoId == (int)TipoMovimientoProductoEnum.SalidaVenta)
//                         costoPonderadoVenta = movimiento.PrecioUnitario * movimiento.Cantidad;
//                     historico.Add(new MovimientoProductoViewModel
//                     {
//                         Fecha = movimiento.Fecha,
//                         Medicamento = producto.NombreProducto,
//                         Cantidad = movimiento.Cantidad,
//                         Lote = productoInventario.Lote,
//                         FechaVencimientoLote = productoInventario.FechaVencimientoArticuloCompra,
//                         UnidadNombre = unidad.Nombre,
//                         DescripcionMovimiento = movimiento.DescripcionMovimiento,
//                         AmbienteId = producto.AmbienteId,
//                         CostoPonderadoCompra = costoPonderadoCompra,
//                         CostoPonderadoVenta = costoPonderadoVenta,
//                         AmbienteNombre = ambiente.NombreAmbiente,
//                         BodegaId = productoInventario.Id,
//                         BodegaNombre = bodega.NombreBodega,
//                         UsuarioNombre = usuario.UserName,
//                         SaldoActual = movimiento.SaldoActual,
//                         TipoMovimientoNombre = tipoMovimiento.Nombre,
//                         TipoProductoNombre = tipoProducto.NombreTipoProducto
//                     });
//                 }
//             }
//             return historico;
//         }
//         public List<DtoSpInventarioProductos> GetInventario(int? ambienteId, int? bodegaId)
//         {
//             return _productoRepository.GetInventarioSp_Nuevo(null, null, bodegaId, null, ambienteId);
//         }
//         public void RealizarDescuentoInventario(int productoId, int? unidadMedidaVentaId, decimal cantidad)
//         {
//             //Aca se busca un registro de inventario que sea de dicho producto y tenga dicha unidad de
//             //venta
//             var productosInventario = _productoRepository
//                 .BuscarRegistrosProductosInventario(productoId, unidadMedidaVentaId);
//             if (productosInventario != null && productosInventario.Count > 0 && productosInventario.Where(a => a.Stock > 0).Count() > 0)
//             {
//                 //Se elije uno de los registros de esa lista para hacer el descuento
//                 var productoInventarioRegistroSeleccionado = productosInventario
//                     .Where(a => a.Stock > 0)
//                     .OrderByDescending(a => a.Id)
//                     .FirstOrDefault();
//                 if (productoInventarioRegistroSeleccionado != null)
//                 {
//                     productoInventarioRegistroSeleccionado.Stock -= cantidad;
//                     _productoRepository.Update(productoInventarioRegistroSeleccionado);
//                 }
//             }
//         }
//     }
// }
using Database.Shared.Dto;
using Database.Shared.Enumeraciones;
using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Service.IService;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using sistema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace farmamest.Service
{
    public class ProductosService : IProductosService
    {
        private readonly IProducto _productoRepository;

        public ProductosService(IProducto productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public List<Producto> GetInventarioBySp(int? tipoProductoId, int? grupoTerapeuticoId, int? bodegaId, int? sucursalId, int? ambienteId)
        {
            var data = _productoRepository.GetInventarioSP(tipoProductoId, grupoTerapeuticoId, bodegaId, sucursalId, ambienteId);
            return data;
        }

        // EXISTENTE (se mantiene): redirigimos al nuevo overload para centralizar lógica.
        public List<MovimientoProductoViewModel> GetHistoricoProductos(DateTime fechaInicio, DateTime fechaFin, List<int> productosIds)
        {
            return GetHistoricoProductos((DateTime?)fechaInicio, (DateTime?)fechaFin, productosIds);
        }

        public List<MovimientoProductoNacionalViewModel> GetHistoricoProductosNacional(DateTime fechaInicio, DateTime fechaFin, List<int> productosIds)
        {
            return GetHistoricoProductosNacional((DateTime?)fechaInicio, (DateTime?)fechaFin, productosIds);
        }

        // NUEVO (Opción A): fechas opcionales. null => sin filtro (lo resuelve el repositorio en el paso 3).
        public List<MovimientoProductoViewModel> GetHistoricoProductos(DateTime? fechaInicio, DateTime? fechaFin, List<int> productosIds)
        {
            // Normalización mínima y literal (sin asumir "fin de día", porque el picker manda hora).
            if (fechaInicio.HasValue && !fechaFin.HasValue)
                fechaFin = fechaInicio;

            if (!fechaInicio.HasValue && fechaFin.HasValue)
                fechaInicio = fechaFin;

            if (fechaInicio.HasValue && fechaFin.HasValue && fechaInicio.Value > fechaFin.Value)
            {
                var tmp = fechaInicio;
                fechaInicio = fechaFin;
                fechaFin = tmp;
            }

            var historico = new List<MovimientoProductoViewModel>();

            // PASO 3: este método del repositorio debe aceptar DateTime? para poder "no filtrar por fecha".
            // Aquí no inventamos rangos.
            var movimientos = _productoRepository.GetMovimientos(fechaInicio, fechaFin, productosIds);

            if (movimientos != null)
            {
                foreach (var movimiento in movimientos)
                {
                    var productoInventario = movimiento.ProductoInventario ?? new ProductoInventario();
                    var producto = productoInventario.Producto ?? new Producto();
                    var tipoProducto = producto.TipoProducto ?? new TipoProducto();
                    var unidad = productoInventario.UnidadMedidaVenta ?? new UnidadMedidaVenta();
                    var ambiente = producto.Ambiente ?? new Ambiente();
                    var bodega = productoInventario.Bodega ?? new Bodega();
                    var usuario = movimiento.UsuarioRealiza ?? new User();
                    var tipoMovimiento = movimiento.TipoMovimientoProducto ?? new TipoMovimientoProducto();

                    decimal? costoPonderadoCompra = null;
                    if (movimiento.TipoMovimientoProductoId == (int)TipoMovimientoProductoEnum.EntradaCompra)
                        costoPonderadoCompra = (productoInventario.PrecioCosto ?? 0) * movimiento.Cantidad;

                    decimal? costoPonderadoVenta = null;
                    if (movimiento.TipoMovimientoProductoId == (int)TipoMovimientoProductoEnum.SalidaVenta)
                        costoPonderadoVenta = movimiento.PrecioUnitario * movimiento.Cantidad;

                    historico.Add(new MovimientoProductoViewModel
                    {
                        Fecha = movimiento.Fecha,
                        Medicamento = producto.NombreProducto,
                        Cantidad = movimiento.Cantidad,
                        Lote = productoInventario.Lote,
                        FechaVencimientoLote = productoInventario.FechaVencimientoArticuloCompra,
                        UnidadNombre = unidad.Nombre,
                        DescripcionMovimiento = movimiento.DescripcionMovimiento,
                        AmbienteId = producto.AmbienteId,
                        CostoPonderadoCompra = costoPonderadoCompra,
                        CostoPonderadoVenta = costoPonderadoVenta,
                        AmbienteNombre = ambiente.NombreAmbiente,
                        BodegaId = productoInventario.BodegaId,
                        BodegaNombre = bodega.NombreBodega,
                        UsuarioNombre = usuario.UserName,
                        SaldoActual = movimiento.SaldoActual,
                        TipoMovimientoNombre = tipoMovimiento.Nombre,
                        TipoProductoNombre = tipoProducto.NombreTipoProducto
                    });
                }
            }

            return historico;
        }


        // NUEVO (Opción A): fechas opcionales. null => sin filtro (lo resuelve el repositorio en el paso 3).
        public List<MovimientoProductoNacionalViewModel> GetHistoricoProductosNacional(DateTime? fechaInicio, DateTime? fechaFin, List<int> productosIds)
        {
            // Normalización mínima y literal (sin asumir "fin de día", porque el picker manda hora).
            if (fechaInicio.HasValue && !fechaFin.HasValue)
                fechaFin = fechaInicio;

            if (!fechaInicio.HasValue && fechaFin.HasValue)
                fechaInicio = fechaFin;

            if (fechaInicio.HasValue && fechaFin.HasValue && fechaInicio.Value > fechaFin.Value)
            {
                var tmp = fechaInicio;
                fechaInicio = fechaFin;
                fechaFin = tmp;
            }

            var historico = new List<MovimientoProductoNacionalViewModel>();

            // PASO 3: este método del repositorio debe aceptar DateTime? para poder "no filtrar por fecha".
            // Aquí no inventamos rangos.
            var movimientos = _productoRepository.GetMovimientosNacional(fechaInicio, fechaFin, productosIds);

            if (movimientos != null)
            {
                foreach (var movimiento in movimientos)
                {

                    var productoInventario = movimiento.ProductoInventario ?? new ProductoInventario();
                    var producto = productoInventario.Producto ?? new Producto();
                    var tipoProducto = producto.TipoProducto ?? new TipoProducto();
                    var unidad = productoInventario.UnidadMedidaVenta ?? new UnidadMedidaVenta();
                    var ambiente = producto.Ambiente ?? new Ambiente();
                    var bodega = productoInventario.Bodega ?? new Bodega();
                    var usuario = movimiento.UsuarioRealiza ?? new User();
                    var usuarioEntrega = movimiento.UsuarioEntrega ?? new User();

                    var tipoMovimiento = movimiento.TipoMovimientoProducto ?? new TipoMovimientoProducto();

                    // decimal? costoPonderadoCompra = null;
                    // if (movimiento.TipoMovimientoProductoId == (int)TipoMovimientoProductoEnum.EntradaCompra)
                    //     costoPonderadoCompra = (productoInventario.PrecioCosto ?? 0) * movimiento.Cantidad;

                    // decimal? costoPonderadoVenta = null;
                    // if (movimiento.TipoMovimientoProductoId == (int)TipoMovimientoProductoEnum.SalidaVenta)
                    //     costoPonderadoVenta = movimiento.PrecioUnitario * movimiento.Cantidad;

                    decimal? costoPonderadoCompra = null;
                    decimal? costoPonderadoVenta = null;

                    if (movimiento.TipoMovimientoProductoId == (int)TipoMovimientoProductoEnum.Entrada)
                    {
                        costoPonderadoCompra = (productoInventario.PrecioCosto ?? 0) * movimiento.Cantidad;
                    }

                    if (movimiento.TipoMovimientoProductoId == (int)TipoMovimientoProductoEnum.Salida)
                    {
                        costoPonderadoVenta = movimiento.PrecioUnitario * movimiento.Cantidad;
                    }

                    historico.Add(new MovimientoProductoNacionalViewModel
                    {
                        Fecha = movimiento.Fecha,
                        Medicamento = producto.NombreProducto,
                        Cantidad = movimiento.Cantidad,
                        Lote = productoInventario.Lote,
                        FechaVencimientoLote = productoInventario.FechaVencimientoArticuloCompra,
                        UnidadNombre = unidad.Nombre,
                        DescripcionMovimiento = movimiento.DescripcionMovimiento,
                        AmbienteId = producto.AmbienteId,
                        CostoPonderadoCompra = costoPonderadoCompra,
                        CostoPonderadoVenta = costoPonderadoVenta,
                        AmbienteNombre = ambiente.NombreAmbiente,
                        BodegaId = productoInventario.BodegaId,
                        BodegaNombre = bodega.NombreBodega,
                        UsuarioNombre = usuario.UserName,
                        UsuarioEntrega = usuarioEntrega.UserName,
                        SaldoActual = movimiento.SaldoActual,
                        TipoMovimientoNombre = tipoMovimiento.Nombre,
                        TipoProductoNombre = tipoProducto.NombreTipoProducto,
                        PrecioUnitario = movimiento.PrecioUnitario,
                        PrecioCosto = (decimal)productoInventario.PrecioCosto,
                        TotalEntrada = (decimal)(movimiento.Cantidad * productoInventario.PrecioCosto),
                        TotalSalida = movimiento.Cantidad * movimiento.PrecioUnitario,
                        ProductoInventarioId = movimiento.ProductoInventarioId

                    });
                }
            }

            return historico;
        }



        public List<DtoSpInventarioProductos> GetInventario(int? ambienteId, int? bodegaId)
        {
            return _productoRepository.GetInventarioSp_Nuevo(null, null, bodegaId, null, ambienteId);
        }

        // public List<DtoSpInventarioProductos> GetInventarioEmergencias(int? tipoProductoId,int? ambienteId, int? bodegaId)
        // {
        //     return _productoRepository.GetInventarioSp_Nuevo(tipoProductoId, null, bodegaId, null, ambienteId);
        // }

        public void RealizarDescuentoInventario(int productoId, int? unidadMedidaVentaId, decimal cantidad)
        {
            Console.WriteLine($"[INICIO] ProductoId={productoId}, UMId={unidadMedidaVentaId}, Cant={cantidad}");

            var productosInventario = _productoRepository.BuscarRegistrosProductosInventario(productoId, unidadMedidaVentaId);

            if (productosInventario == null || !productosInventario.Any())
            {
                Console.WriteLine("[ERROR] No se encontraron registros de inventario.");
                return;
            }
            Console.WriteLine($"[OK] Registros encontrados: {productosInventario.Count}");

            var registrosConStock = productosInventario.Where(a => a.Stock > 0).ToList();
            if (!registrosConStock.Any())
            {
                Console.WriteLine("[ERROR] Todos los registros tienen Stock <= 0.");
                foreach (var r in productosInventario)
                    Console.WriteLine($"  Id={r.Id}, Stock={r.Stock}");
                return;
            }

            var seleccionado = registrosConStock.OrderByDescending(a => a.Id).First();
            seleccionado.Stock -= cantidad;
            _productoRepository.Update(seleccionado);
            _productoRepository.SaveChanges(); // ¡IMPORTANTE! Si no guardas, los cambios se pierden
            Console.WriteLine($"[OK] Descuento aplicado. Nuevo stock: {seleccionado.Stock}");
        }
    }
}
