using Database.Shared.Data;
using Database.Shared.Dto;
using Database.Shared.Enumeraciones;
using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Models;
using farmamest.Service.IService;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using sistema.Models;
using sistema.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sistema.Service
{
    public class ComprasService : IComprasService
    {
        private readonly ICompra _compraRepository = null;
        private readonly IProducto _productoRepository = null;
        public ComprasService(ICompra compraRepository, IProducto productoRepository)
        {
            _compraRepository = compraRepository;
            _productoRepository = productoRepository;
        }
        public void AgregarProductosAInventario(Compra compra, string userId = null)
        {
            var fecha = DateTime.Now;
            if (compra.DetalleCompras != null)
            {
                var lotesAgregar = new List<ProductoInventario>();

                foreach (var producto in compra.DetalleCompras)
                {
                    // ⚠️ Caso normal: si HAY ubicaciones, usamos la lógica actual
                    if (producto.DetalleComprasUbicaciones != null && producto.DetalleComprasUbicaciones.Any())
                    {
                        foreach (var ubicacion in producto.DetalleComprasUbicaciones)
                        {
                            var productoInventario = new ProductoInventario
                            {
                                ProductoId = producto.ProductoId,
                                BodegaId = ubicacion.BodegaId,
                                CompraId = compra.Id,
                                UnidadMedidaVentaId = ubicacion.UnidadMedidaVentaId,
                                FechaVencimientoArticuloCompra = producto.FechaVencimiento,
                                PrecioCosto = producto.Precio,
                                Lote = producto.Lote,
                                FechaRecepcionLote = fecha,
                                Stock = ubicacion.Cantidad,

                                // Políticas de devolución
                                ManejaPoliticaDevolucion = producto.ManejaPoliticaDevolucion,
                                ManejaPoliticaDevolucionPersonalizada = producto.ManejaPoliticaDevolucionPersonalizada,
                                ManejaPoliticaDevolucionProveedor = producto.ManejaPoliticaDevolucionProveedor,
                                PoliticaDevolucionPersonalizadaDias = producto.PoliticaDevolucionPersonalizadaDias,

                                // Crédito
                                ManejaCredito = producto.ManejaCredito,
                                ManejaCreditoPersonalizado = producto.ManejaCreditoPersonalizado,
                                ManejaCreditoProveedor = producto.ManejaCreditoProveedor,
                                CreditoPersonalizadoDias = producto.CreditoPersonalizadoDias
                            };

                            if (ubicacion.DetalleCompraUbicacionPrecios != null)
                            {
                                foreach (var precio in ubicacion.DetalleCompraUbicacionPrecios)
                                {
                                    productoInventario.ProductosInventarioPrecios
                                        .Add(new ProductoInventarioPrecio
                                        {
                                            PrecioId = precio.PrecioId,
                                            Valor = precio.PrecioValor
                                        });
                                }
                            }

                            lotesAgregar.Add(productoInventario);
                        }
                    }
                    else
                    {
                        // ⚠️ Fallback de PRUEBA:
                        // Si NO hay ubicaciones, generamos un solo lote por detalle
                        // usando la bodega de la compra y la cantidad del detalle.
                        var productoInventario = new ProductoInventario
                        {
                            ProductoId = producto.ProductoId,
                            // Usamos la bodega de la compra; si es nullable, ajusta según tu modelo
                            BodegaId = compra.TipoBodegaId,
                            CompraId = compra.Id,
                            // Para la prueba usamos la misma unidad de medida de compra
                            UnidadMedidaVentaId = producto.UnidadMedidaCompraId,
                            FechaVencimientoArticuloCompra = producto.FechaVencimiento,
                            PrecioCosto = producto.Precio,
                            Lote = producto.Lote,
                            FechaRecepcionLote = fecha,
                            Stock = producto.Cantidad,

                            ManejaPoliticaDevolucion = producto.ManejaPoliticaDevolucion,
                            ManejaPoliticaDevolucionPersonalizada = producto.ManejaPoliticaDevolucionPersonalizada,
                            ManejaPoliticaDevolucionProveedor = producto.ManejaPoliticaDevolucionProveedor,
                            PoliticaDevolucionPersonalizadaDias = producto.PoliticaDevolucionPersonalizadaDias,

                            ManejaCredito = producto.ManejaCredito,
                            ManejaCreditoPersonalizado = producto.ManejaCreditoPersonalizado,
                            ManejaCreditoProveedor = producto.ManejaCreditoProveedor,
                            CreditoPersonalizadoDias = producto.CreditoPersonalizadoDias
                        };

                        // En este fallback NO tenemos precios por ubicación, así que
                        // dejamos ProductosInventarioPrecios vacío.
                        lotesAgregar.Add(productoInventario);
                    }
                }

                _productoRepository.Add(lotesAgregar);

                if (lotesAgregar != null)
                {
                    foreach (var lote in lotesAgregar)
                    {
                        _productoRepository.Add(new MovimientoProducto
                        {
                            Fecha = fecha,
                            DescripcionMovimiento = "Compra de producto",
                            UsuarioRealizaId = userId,
                            ProductoInventarioId = lote.Id,
                            Cantidad = lote.Stock,
                            SaldoActual = lote.Stock,
                            TipoMovimientoProductoId = (int)TipoMovimientoProductoEnum.EntradaCompra
                        });
                    }
                }
            }
        }

    }
}
