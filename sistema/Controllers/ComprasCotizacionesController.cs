using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Database.Shared.Models;
using Database.Shared.Data;
using Database.Shared;
using Database.Shared.IRepository;
using sistema.Models;

namespace sistema.Controllers
{
    public class ComprasCotizacionesController : Controller
    {
        private readonly Context _context;
        private readonly IProveedor _proveedorRepository;

        public ComprasCotizacionesController(IProveedor proveedorRepository, Context context)
        {
            _proveedorRepository = proveedorRepository;
            _context = context;
        }

        [HttpPost]
        public IActionResult MostrarProductos(string productos, int bodegaDestinoId)
        {
            List<ProductoTrasladoToCompraViewModel> listaProductos = new List<ProductoTrasladoToCompraViewModel>();

            // 1. Deserializar y completar información de productos
            if (!string.IsNullOrEmpty(productos))
            {
                try
                {
                    var productosList = JsonConvert.DeserializeObject<List<ProductoTrasladoToCompraViewModel>>(productos);
                    var productoInventarioIds = productosList.Select(p => p.ProductoInventarioId).ToList();

                    var productosDB = _context.ProductosInventario
                        .AsNoTracking()
                        .Where(pi => productoInventarioIds.Contains(pi.Id))
                        .Include(pi => pi.Producto)
                        .Select(pi => new
                        {
                            pi.Id,
                            pi.ProductoId,
                            CodigoReferencia = pi.Producto.CodigoReferencia,
                            ProductoNombre = pi.Producto.NombreProducto,
                        })
                        .ToList();

                    listaProductos = productosList.Select(p => new ProductoTrasladoToCompraViewModel
                    {
                        ProductoInventarioId = p.ProductoInventarioId,
                        ProductoId = productosDB.FirstOrDefault(db => db.Id == p.ProductoInventarioId)?.ProductoId ?? 0,
                        CodigoReferencia = productosDB.FirstOrDefault(db => db.Id == p.ProductoInventarioId)?.CodigoReferencia ?? "N/A",
                        ProductoNombre = productosDB.FirstOrDefault(db => db.Id == p.ProductoInventarioId)?.ProductoNombre ?? "Desconocido",
                        CantidadTrasladada = p.CantidadTrasladada
                    }).ToList();
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Error al procesar los productos: " + ex.Message;
                }
            }

            // 2. Obtener Proveedores Activos
            // Traemos la entidad completa para poder loguear el ID en la trazabilidad
            var proveedoresActivos = _proveedorRepository.GetList()
                .Where(p => !p.Eliminado)
                .ToList();

            List<string> nombresProveedores = proveedoresActivos.Select(p => p.Nombre).ToList();
            ViewBag.Proveedores = nombresProveedores;

            // 3. CONSULTAR PRECIOS HISTÓRICOS
            var idsProductos = listaProductos.Select(x => x.ProductoId).Distinct().ToList();
            
            // Diccionario: { "NombreProveedor": { ProductoID: { precio: 10.5, cantidad: 0 } } }
            var diccionarioPrecios = ObtenerPreciosHistoricos(idsProductos, nombresProveedores);
            
            ViewBag.PreciosHistoricos = diccionarioPrecios;
            ViewBag.AmbienteId = bodegaDestinoId;
            ViewBag.SucursalId = 6; 

            // ==========================================================================================
            // TRAZABILIDAD EN CONSOLA (Log detallado solicitado)
            // ==========================================================================================
            Console.WriteLine("================================================================================");
            Console.WriteLine($"[TRAZABILIDAD COTIZACION] {DateTime.Now}");
            Console.WriteLine($"Ambiente: {bodegaDestinoId} | Sucursal: 6");
            Console.WriteLine("--------------------------------------------------------------------------------");

            foreach (var prov in proveedoresActivos)
            {
                // Verificamos si tenemos datos para este proveedor en el diccionario
                if (diccionarioPrecios.TryGetValue(prov.Nombre, out var productosDelProv))
                {
                    foreach (var prod in listaProductos)
                    {
                        decimal precio = 0;
                        // Intentamos extraer el precio del objeto anónimo almacenado en el diccionario
                        if (productosDelProv.TryGetValue(prod.ProductoId, out var datosObj))
                        {
                            // Usamos reflection o dynamic porque guardamos un objeto anónimo
                            dynamic datosDynamic = datosObj;
                            precio = datosDynamic.precio;
                        }

                        Console.WriteLine($"Proveedor: [{prov.Id}] {prov.Nombre} | Producto: [{prod.ProductoId}] {prod.ProductoNombre} | Cantidad: {prod.CantidadTrasladada} | Precio Histórico: {precio}");
                    }
                }
            }
            Console.WriteLine("================================================================================");
            // ==========================================================================================

            return View(listaProductos);
        }

        // Método privado para buscar precios en DetalleCompra -> Compra -> Proveedor
        private Dictionary<string, Dictionary<int, object>> ObtenerPreciosHistoricos(List<int> productoIds, List<string> proveedores)
        {
            var resultado = new Dictionary<string, Dictionary<int, object>>();

            try
            {
                if (productoIds == null || !productoIds.Any() || proveedores == null || !proveedores.Any())
                    return resultado;

                // Consulta a la Base de Datos
                var historial = _context.DetalleCompras
                    .AsNoTracking()
                    .Include(d => d.Compra)
                    .ThenInclude(c => c.Proveedor)
                    .Where(d =>
                        productoIds.Contains(d.ProductoId) &&
                        d.Compra != null &&
                        !d.Compra.Eliminado &&
                        !d.Eliminado &&
                        d.Compra.Proveedor != null &&
                        proveedores.Contains(d.Compra.Proveedor.Nombre)
                    )
                    .Select(d => new
                    {
                        d.ProductoId,
                        ProveedorNombre = d.Compra.Proveedor.Nombre,
                        d.Precio,
                        d.Compra.FechaCompra
                    })
                    .ToList();

                // Agrupación en memoria para obtener el último precio por (Proveedor, Producto)
                var ultimosDatos = historial
                    .GroupBy(x => new { x.ProductoId, x.ProveedorNombre })
                    .Select(g => g.OrderByDescending(x => x.FechaCompra).First())
                    .ToList();

                // Construcción del Diccionario de Respuesta
                foreach (var nombreProv in proveedores)
                {
                    var productosDelProveedor = new Dictionary<int, object>();

                    foreach (var idProd in productoIds)
                    {
                        var dato = ultimosDatos.FirstOrDefault(x => x.ProveedorNombre == nombreProv && x.ProductoId == idProd);
                        decimal precioEncontrado = dato != null ? dato.Precio : 0;

                        // Objeto listo para ser consumido por el JS de la vista
                        productosDelProveedor.Add(idProd, new { precio = precioEncontrado, cantidad = 0 });
                    }

                    resultado.Add(nombreProv, productosDelProveedor);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Falló la obtención de precios históricos: {ex.Message}");
                // Retornar vacío en caso de error
            }

            return resultado;
        }
    }
}