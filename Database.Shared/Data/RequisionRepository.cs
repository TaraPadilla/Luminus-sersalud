using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Enumeraciones;
using Npgsql;

namespace Database.Shared.Data
{
    public class RequisionRepository : IRequision
    {
        private readonly Context _context;

        public RequisionRepository(Context context)
        {
            _context = context;
        }

        public async Task<Requision> CrearAsync(Requision requision, IEnumerable<RequisionDetalle> detalles, string? usuario)
        {
            if (requision == null) throw new ArgumentNullException(nameof(requision));
            if (detalles == null) throw new ArgumentNullException(nameof(detalles));

            // Normaliza detalles (evita duplicados por ProductoInventarioId)
            var detallesList = detalles
                .Where(d => d != null)
                .GroupBy(d => d.ProductoInventarioId)
                .Select(g =>
                {
                    var first = g.First();
                    first.CantidadSolicitada = g.Sum(x => x.CantidadSolicitada);
                    return first;
                })
                .ToList();

            if (!detallesList.Any())
                throw new InvalidOperationException("No hay detalles para guardar la requisición.");

            // Reintentos por concurrencia (serializable/unique)
            const int maxRetries = 5;
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                await using var tx = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

                try
                {
                    // Generación de correlativos dentro de la transacción SERIALIZABLE
                    // (y validados por UNIQUE en BD)
                    requision.NumeroRequisicion ??= await SiguienteNumeroRequisicionAsync();
                    requision.NumeroOrden ??= await SiguienteNumeroOrdenAsync();

                    requision.CreadoEn = DateTime.Now;
                    requision.CreadoPor = usuario;

                    // Guardar encabezado
                    _context.Requision.Add(requision);
                    await _context.SaveChangesAsync();

                    // Guardar detalle
                    foreach (var det in detallesList)
                    {
                        det.RequisionId = requision.Id;
                        det.CreadoEn = DateTime.Now;
                        _context.RequisionDetalle.Add(det);
                    }

                    // Historial inicial
                    _context.RequisionHistorial.Add(new RequisionHistorial
                    {
                        RequisionId = requision.Id,
                        EstadoAnterior = null,
                        EstadoNuevo = requision.Estado,
                        Observacion = "Creada",
                        CreadoEn = DateTime.Now,
                        CreadoPor = usuario
                    });

                    await _context.SaveChangesAsync();
                    await tx.CommitAsync();

                    return requision;
                }
                catch (PostgresException pgex) when (pgex.SqlState == PostgresErrorCodes.UniqueViolation
                                                   || pgex.SqlState == PostgresErrorCodes.SerializationFailure)
                {
                    await tx.RollbackAsync();

                    // Si chocó por UNIQUE (NumeroRequisicion o NumeroOrden) o por SERIALIZABLE,
                    // reintentamos. En el siguiente intento se recalculan números.
                    requision.NumeroRequisicion = null;
                    requision.NumeroOrden = null;

                    if (attempt == maxRetries)
                        throw new Exception("No se pudo generar un número único para la requisición/orden después de varios intentos.", pgex);
                }
                catch (DbUpdateException dbex) // por si el provider envuelve el error
                {
                    await tx.RollbackAsync();

                    // Intenta detectar unique violation dentro del DbUpdateException
                    if (dbex.InnerException is PostgresException innerPg
                        && (innerPg.SqlState == PostgresErrorCodes.UniqueViolation
                            || innerPg.SqlState == PostgresErrorCodes.SerializationFailure))
                    {
                        requision.NumeroRequisicion = null;
                        requision.NumeroOrden = null;

                        if (attempt == maxRetries)
                            throw new Exception("No se pudo generar un número único para la requisición/orden después de varios intentos.", dbex);

                        continue;
                    }

                    throw;
                }
                catch
                {
                    await tx.RollbackAsync();
                    throw;
                }
            }

            // No debería llegar aquí
            throw new Exception("No se pudo crear la requisición.");
        }

        public async Task<Requision?> GetByIdAsync(int id)
        {
            return await _context.Requision
                .Include(r => r.BodegaOrigen)
                .Include(r => r.BodegaDestino)
                .Include(r => r.RequisionHistoriales)
                .Include(r => r.RequisionDetalles)
                    .ThenInclude(d => d.ProductoInventario)
                        .ThenInclude(i => i.UnidadMedidaVenta) // Cadena 1: Hasta la unidad
                .Include(r => r.RequisionDetalles)
                    .ThenInclude(d => d.ProductoInventario)
                        .ThenInclude(i => i.Producto)          // Cadena 2: Hasta el producto
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Requision?> GetByNumeroRequisicionAsync(int numeroRequisicion)
        {
            return await _context.Requision
                .Include(r => r.BodegaOrigen)
                .Include(r => r.BodegaDestino)
                .Include(r => r.RequisionDetalles)
                    .ThenInclude(d => d.ProductoInventario)
                        .ThenInclude(pi => pi.Producto)
                .Include(r => r.RequisionHistoriales)
                .FirstOrDefaultAsync(r => r.NumeroRequisicion == numeroRequisicion);
        }

        public async Task<Requision?> GetByNumeroOrdenAsync(int numeroOrden)
        {
            return await _context.Requision
                .Include(r => r.BodegaOrigen)
                .Include(r => r.BodegaDestino)
                .Include(r => r.RequisionDetalles)
                    .ThenInclude(d => d.ProductoInventario)
                        .ThenInclude(pi => pi.Producto)
                .Include(r => r.RequisionHistoriales)
                .FirstOrDefaultAsync(r => r.NumeroOrden == numeroOrden);
        }

        public async Task<bool> CambiarEstadoAsync(int requisionId, int nuevoEstado, string? usuario, string? observacion)
        {
            await using var tx = await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                var req = await _context.Requision.FirstOrDefaultAsync(x => x.Id == requisionId);
                if (req == null) return false;

                var estadoAnterior = req.Estado;
                if (estadoAnterior == nuevoEstado) return true;

                req.Estado = nuevoEstado;

                _context.RequisionHistorial.Add(new RequisionHistorial
                {
                    RequisionId = req.Id,
                    EstadoAnterior = estadoAnterior,
                    EstadoNuevo = nuevoEstado,
                    Observacion = observacion,
                    CreadoEn = DateTime.Now,
                    CreadoPor = usuario
                });

                await _context.SaveChangesAsync();
                await tx.CommitAsync();
                return true;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<List<Requision>> ListarAsync(int top = 200)
        {
            if (top <= 0) top = 200;

            // Listado solo de encabezados (para la tabla). No incluimos detalles para no cargar demasiado.
            return await _context.Requision
                .AsNoTracking()
                .Include(r => r.BodegaOrigen)
                .Include(r => r.BodegaDestino)
                .OrderByDescending(r => r.Id)
                .Take(top)
                .ToListAsync();
        }

        public async Task<IRequision.PagedResult<RequisionListaItemDto>> ListarDataTableAsync(
            int start,
            int length,
            string? search,
            string? orderColumn,
            string? orderDir)
        {
            if (start < 0) start = 0;
            if (length <= 0) length = 10;

            var query = _context.Requision
                .AsNoTracking()
                .Include(r => r.BodegaOrigen)
                .Include(r => r.BodegaDestino)
                .AsQueryable();

            // Total sin filtros
            var recordsTotal = await query.CountAsync();

            // Filtro global
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();

                query = query.Where(r =>
                    (r.SolicitanteNombre != null && r.SolicitanteNombre.ToLower().Contains(s)) ||
                    (r.NumeroRequisicion != null && r.NumeroRequisicion.ToString().Contains(s)) ||
                    (r.NumeroOrden != null && r.NumeroOrden.ToString().Contains(s)) ||
                    (r.BodegaOrigen != null && r.BodegaOrigen.NombreBodega != null && r.BodegaOrigen.NombreBodega.ToLower().Contains(s)) ||
                    (r.BodegaDestino != null && r.BodegaDestino.NombreBodega != null && r.BodegaDestino.NombreBodega.ToLower().Contains(s)) ||
                    (r.Departamento != null && r.Departamento.ToLower().Contains(s)) ||
                    (r.UnidadSeccion != null && r.UnidadSeccion.ToLower().Contains(s))
                );
            }

            // Total filtrado
            var recordsFiltered = await query.CountAsync();

            // Orden controlado (mapeo explícito)
            var desc = string.Equals(orderDir, "desc", StringComparison.OrdinalIgnoreCase);

            query = (orderColumn ?? string.Empty) switch
            {
                "NumeroRequisicion" => desc ? query.OrderByDescending(x => x.NumeroRequisicion) : query.OrderBy(x => x.NumeroRequisicion),
                "NumeroOrden" => desc ? query.OrderByDescending(x => x.NumeroOrden) : query.OrderBy(x => x.NumeroOrden),
                "FechaSolicitud" => desc ? query.OrderByDescending(x => x.FechaSolicitud) : query.OrderBy(x => x.FechaSolicitud),
                "SolicitanteNombre" => desc ? query.OrderByDescending(x => x.SolicitanteNombre) : query.OrderBy(x => x.SolicitanteNombre),
                "BodegaOrigenNombre" => desc ? query.OrderByDescending(x => x.BodegaOrigen!.NombreBodega) : query.OrderBy(x => x.BodegaOrigen!.NombreBodega),
                "BodegaDestinoNombre" => desc ? query.OrderByDescending(x => x.BodegaDestino!.NombreBodega) : query.OrderBy(x => x.BodegaDestino!.NombreBodega),
                "DepartamentoNombre" => desc ? query.OrderByDescending(x => x.Departamento) : query.OrderBy(x => x.Departamento),
                "UnidadNombre" => desc ? query.OrderByDescending(x => x.UnidadSeccion) : query.OrderBy(x => x.UnidadSeccion),
                "Estado" => desc ? query.OrderByDescending(x => x.Estado) : query.OrderBy(x => x.Estado),
                _ => query.OrderByDescending(x => x.Id)
            };

            var items = await query
                .Skip(start)
                .Take(length)
                .Select(r => new RequisionListaItemDto
                {
                    RequisicionId = r.Id,
                    NumeroRequisicion = r.NumeroRequisicion,
                    NumeroOrden = r.NumeroOrden,
                    FechaSolicitud = r.FechaSolicitud,
                    SolicitanteNombre = r.SolicitanteNombre,
                    BodegaOrigenNombre = r.BodegaOrigen != null ? r.BodegaOrigen.NombreBodega : "-",
                    BodegaDestinoNombre = r.BodegaDestino != null ? r.BodegaDestino.NombreBodega : "-",
                    DepartamentoNombre = r.Departamento ?? "-",
                    UnidadNombre = r.UnidadSeccion ?? "-",
                    Estado = r.Estado
                })
                .ToListAsync();

            return new IRequision.PagedResult<RequisionListaItemDto>(
                RecordsTotal: recordsTotal,
                RecordsFiltered: recordsFiltered,
                Items: items
            );
        }

        // ==========================================================
        // Generación de correlativos (sin sequence) dentro de SERIALIZABLE
        // ==========================================================
        private async Task<int> SiguienteNumeroRequisicionAsync()
        {
            // Si quieres 6 dígitos: 000001..999999, esto genera el siguiente.
            var max = await _context.Requision
                .Where(x => x.NumeroRequisicion != null)
                .MaxAsync(x => (int?)x.NumeroRequisicion) ?? 0;

            var next = max + 1;
            if (next > 999999) throw new Exception("Se alcanzó el límite de NumeroRequisicion (999999).");
            return next;
        }

        private async Task<int> SiguienteNumeroOrdenAsync()
        {
            // Si quieres 4 dígitos: 0001..9999
            var max = await _context.Requision
                .Where(x => x.NumeroOrden != null)
                .MaxAsync(x => (int?)x.NumeroOrden) ?? 0;

            var next = max + 1;
            if (next > 9999) throw new Exception("Se alcanzó el límite de NumeroOrden (9999).");
            return next;
        }

        public (int? NumeroRequisicion, int? NumeroOrden) ObtenerUltimoRegistro()
        {
            var ultimo = _context.Requision
                .AsNoTracking()
                .OrderByDescending(r => r.Id)
                .Select(r => new
                {
                    r.NumeroRequisicion,
                    r.NumeroOrden
                })
                .FirstOrDefault();

            if (ultimo == null) return (0, 0);

            return (ultimo.NumeroRequisicion, ultimo.NumeroOrden);
        }

        public void Update(Requision requision)
        {
            _context.Requision.Update(requision);
            _context.SaveChanges();
        }

        public async Task<bool> ActualizarCantidadesDespachoPorProductoAsync(List<RequisionDetalle> items)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (items == null || !items.Any()) return false;

                    foreach (var item in items)
                    {
                        var detalleExistente = await _context.RequisionDetalle
                            .FirstOrDefaultAsync(rd => rd.RequisionId == item.RequisionId
                                                 && rd.ProductoInventario.ProductoId == item.Id);

                        if (detalleExistente != null)
                        {
                            detalleExistente.CantidadDespachada = item.CantidadDespachada;
                            _context.RequisionDetalle.Update(detalleExistente);
                        }
                    }

                    int requisicionId = items.First().RequisionId;
                    var cabecera = await _context.Requision.FindAsync(requisicionId);

                    if (cabecera != null)
                    {
                        cabecera.Estado = 5;
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }

        // ==========================================================
        // NUEVO: Entrega a Kardex (Estado 6)
        // - Mover inventario (origen ↓, destino ↑) preservando lote/vencimiento
        // - Registrar Kardex en MovimientosProductoNacional (Salida/Entrada)
        // REGLA: CantidadDespachada null => 0 => no se mueve ni registra
        // ==========================================================
        public async Task<(bool Exitoso, string Mensaje)> ProcesarEntregaAKardexAsync(
            int requisionId,
            string usuarioId,
            string rutaFirmaAlmacen,
            string? nombreAlmacen,
            string usuarioId2)
        {
            await using var tx = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            try
            {
                var req = await _context.Requision
                    .Include(r => r.RequisionDetalles)
                        .ThenInclude(d => d.ProductoInventario)
                    .FirstOrDefaultAsync(r => r.Id == requisionId);

                if (req == null)
                    return (false, "Requisición no encontrada.");

                // Idempotencia
                if (req.Estado == 6)
                    return (false, "La requisición ya fue entregada a Kardex.");

                // Flujo esperado: 5 -> 6
                if (req.Estado != 5)
                    return (false, $"Estado inválido para entrega a Kardex. Estado actual: {req.Estado}.");

                // Guardar firma + nombre encargado
                req.AutorizacionAlmacen = rutaFirmaAlmacen;
                req.EncargadoAlmacenNombre = nombreAlmacen;

                var estadoAnterior = req.Estado;
                var movimientos = new List<MovimientoProductoNacional>();

                // Documento/Descripción base para el kardex
                var docRequisicion = req.NumeroRequisicion.HasValue ? $"REQ {req.NumeroRequisicion}" : $"REQID {req.Id}";
                var docOrden = req.NumeroOrden.HasValue ? $"ORD {req.NumeroOrden}" : "ORD -";
                var docBase = $"{docRequisicion} / {docOrden}";

                if (req.RequisionDetalles == null || req.RequisionDetalles.Count == 0)
                {
                    req.Estado = 6;

                    _context.RequisionHistorial.Add(new RequisionHistorial
                    {
                        RequisionId = req.Id,
                        EstadoAnterior = estadoAnterior,
                        EstadoNuevo = 6,
                        Observacion = "Entregado a Kardex (sin detalles).",
                        CreadoEn = DateTime.Now,
                        CreadoPor = usuarioId
                    });

                    await _context.SaveChangesAsync();
                    await tx.CommitAsync();
                    return (true, "Entrega a Kardex realizada (sin detalles).");
                }

                foreach (var det in req.RequisionDetalles)
                {
                    var cantidad = det.CantidadDespachada ?? 0;
                    if (cantidad <= 0)
                        continue;

                    var origen = det.ProductoInventario;
                    if (origen == null)
                        return (false, $"Detalle sin ProductoInventario cargado. ProductoInventarioId: {det.ProductoInventarioId}");

                    // Validación de consistencia con la requisición
                    if (!origen.BodegaId.HasValue || origen.BodegaId.Value != req.BodegaOrigenId)
                        return (false, $"El lote origen no pertenece a la bodega origen de la requisición. ProductoInventarioId: {origen.Id}");

                    if (origen.Stock < cantidad)
                        return (false, $"Stock insuficiente en origen para el lote {origen.Lote ?? "-"} (PI:{origen.Id}). Stock:{origen.Stock} Despachado:{cantidad}");

                    // 1) Descontar origen
                    origen.Stock -= cantidad;

                    // 2) Sumar destino preservando lote/vencimiento (y precio)
                    var destino = await _context.ProductosInventario
                        .FirstOrDefaultAsync(pi =>
                            pi.Eliminado == false
                            && pi.BodegaId == req.BodegaDestinoId
                            && pi.ProductoId == origen.ProductoId
                            && pi.UnidadMedidaVentaId == origen.UnidadMedidaVentaId
                            && pi.Lote == origen.Lote
                            && pi.FechaVencimientoArticuloCompra == origen.FechaVencimientoArticuloCompra
                            && pi.PrecioCosto == origen.PrecioCosto);

                    if (destino == null)
                    {
                        destino = new ProductoInventario
                        {
                            ProductoId = origen.ProductoId,
                            CompraId = origen.CompraId,
                            UnidadMedidaVentaId = origen.UnidadMedidaVentaId,
                            UnidadMedidaCompraId = origen.UnidadMedidaCompraId,

                            BodegaId = req.BodegaDestinoId,
                            Lote = origen.Lote,
                            FechaVencimientoArticuloCompra = origen.FechaVencimientoArticuloCompra,
                            FechaRecepcionLote = origen.FechaRecepcionLote,

                            PrecioCosto = origen.PrecioCosto,
                            StockMinimo = origen.StockMinimo,
                            Eliminado = false,
                            Facturado = origen.Facturado,

                            // Se clonan políticas del lote
                            ManejaPoliticaDevolucion = origen.ManejaPoliticaDevolucion,
                            ManejaPoliticaDevolucionProveedor = origen.ManejaPoliticaDevolucionProveedor,
                            ManejaPoliticaDevolucionPersonalizada = origen.ManejaPoliticaDevolucionPersonalizada,
                            PoliticaDevolucionPersonalizadaDias = origen.PoliticaDevolucionPersonalizadaDias,

                            ManejaPoliticaDevolucionVencimiento = origen.ManejaPoliticaDevolucionVencimiento,
                            ManejaPoliticaDevolucionVencimientoProveedor = origen.ManejaPoliticaDevolucionVencimientoProveedor,
                            ManejaPoliticaDevolucionVencimientoPersonalizada = origen.ManejaPoliticaDevolucionVencimientoPersonalizada,
                            PoliticaDevolucionVencimientoPersonalizadaDias = origen.PoliticaDevolucionVencimientoPersonalizadaDias,

                            ManejaCredito = origen.ManejaCredito,
                            ManejaCreditoProveedor = origen.ManejaCreditoProveedor,
                            ManejaCreditoPersonalizado = origen.ManejaCreditoPersonalizado,
                            CreditoPersonalizadoDias = origen.CreditoPersonalizadoDias,

                            Stock = 0
                        };

                        _context.ProductosInventario.Add(destino);
                    }

                    destino.Stock += cantidad;

                    var precioUnitario = origen.PrecioCosto ?? 0m;
                    var montoTotal = precioUnitario * cantidad;

                    // 3) Kardex nacional - ORIGEN (Salida)
                    movimientos.Add(new MovimientoProductoNacional
                    {
                        ProductoInventarioId = origen.Id,
                        Fecha = DateTime.Now,
                        DescripcionMovimiento = $"{docBase} - SALIDA por entrega a Kardex",
                        Cantidad = cantidad,
                        PrecioUnitario = precioUnitario,
                        MontoTotal = montoTotal,
                        UsuarioRealizaId = usuarioId,
                        UsuarioEntregaId = usuarioId2,
                        ProveedorBodegaCliente = $"Bodega destino: {req.BodegaDestinoId}",
                        SaldoActual = origen.Stock,
                        TipoMovimientoProductoId = (int)TipoMovimientoProductoEnum.Salida
                    });

                    // Kardex nacional - DESTINO (Entrada)
                    movimientos.Add(new MovimientoProductoNacional
                    {
                        ProductoInventario = destino, // navegación: soporta destino nuevo sin Id
                        Fecha = DateTime.Now,
                        DescripcionMovimiento = $"{docBase} - ENTRADA por entrega a Kardex",
                        Cantidad = cantidad,
                        PrecioUnitario = precioUnitario,
                        MontoTotal = montoTotal,
                        UsuarioRealizaId = usuarioId,
                        UsuarioEntregaId = usuarioId2,
                        ProveedorBodegaCliente = $"Bodega origen: {req.BodegaOrigenId}",
                        SaldoActual = destino.Stock,
                        TipoMovimientoProductoId = (int)TipoMovimientoProductoEnum.Entrada
                    });
                }

                // Entregado a Kardex siempre lleva a 6, con o sin productos despachados
                req.Estado = 6;

                _context.RequisionHistorial.Add(new RequisionHistorial
                {
                    RequisionId = req.Id,
                    EstadoAnterior = estadoAnterior,
                    EstadoNuevo = 6,
                    Observacion = movimientos.Count == 0
                        ? "Entregado a Kardex (sin productos despachados)."
                        : "Entregado a Kardex (movimiento de inventario + kardex nacional).",
                    CreadoEn = DateTime.Now,
                    CreadoPor = usuarioId
                });

                if (movimientos.Count > 0)
                    _context.MovimientosProductoNacional.AddRange(movimientos);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return (true, "Entrega a Kardex realizada correctamente.");
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return (false, "Error al procesar entrega a Kardex: " + ex.GetBaseException().Message);
            }
        }
    }
}