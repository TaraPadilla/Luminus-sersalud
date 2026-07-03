using Database.Shared.IRepository;
using Database.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Paginacion;
using System;
using Database.Shared.Enumeraciones;


namespace Database.Shared.Data
{
    public class CompraRepository : ICompra
    {
        private readonly Context _context = null;
        public CompraRepository(Context context)
        {
            _context = context;
        }
        public Compra Add(Compra compra, bool saveChanges = true)
        {
            _context.Compras.Add(compra);
            if (saveChanges)
            {
                _context.SaveChanges();
            }
            return compra;
        }
        public void Add(OrdenCompra ordenCompra)
        {
            _context.OrdenesCompra.Add(ordenCompra);
            _context.SaveChanges();
        }
        public DetalleCompraUnidadVentaPrecio Add(DetalleCompraUnidadVentaPrecio precio, bool saveChanges = true)
        {
            _context.DetalleComprasUnidadesVentaPrecio.Add(precio);
            if (saveChanges)
            {
                _context.SaveChanges();
            }
            return precio;
        }
        public DetalleCompra Add(DetalleCompra detalle, bool saveChanges = true)
        {
            _context.Add(detalle);
            if (saveChanges)
            {
                _context.SaveChanges();
            }
            return detalle;
        }
        //public List<Compra> GetList() => _context.Compras.Include(a => a.Proveedor).Include(x => x.Recepciones).ThenInclude(x => x.EstadoRecepcion).Where(x => x.Eliminado == false).ToList();
        public List<CompraTipoDocumento> GetListTipoDocumento() =>
            _context.CompraTiposDocumento
            .Where(x => x.Eliminado == false).ToList();
        //public List<Recepcion> GetRecepciones() => _context.Recepciones.Include(x => x.Compra).ThenInclude(x => x.Proveedor).Include(x => x.EstadoRecepcion).ToList();
        public List<Compra> GetListadoFecha(DateTime inicio, DateTime final) => _context.Compras.Include(a => a.Proveedor).Include(a => a.DetalleCompras)
          .ThenInclude(a => a.Producto).Include(a => a.Empleado).Where(a => a.FechaCompra <= final && a.FechaCompra >= inicio).ToList();
        public List<Compra> GetListadoFechaEmpleado(DateTime inicio, DateTime final, int? id) => _context.Compras.Include(a => a.Proveedor).Include(a => a.DetalleCompras)
       .ThenInclude(a => a.Producto).Include(a => a.Empleado).Where(a => a.FechaCompra <= final && a.FechaCompra >= inicio).Where(a => a.EmpleadoId == id).ToList();
        public void saveChanges()
        {
            _context.SaveChanges();
        }
        public Compra Get(int id, bool includeRelatedEntities = true)
        {
            var compra = _context.Compras.AsQueryable();

            if (includeRelatedEntities)
            {
                compra = compra
                .Include(a => a.DetalleCompras)
                    .ThenInclude(a => a.DetalleComprasUbicaciones)
                    .ThenInclude(a => a.DetalleCompraUbicacionPrecios)
                .Include(a => a.DetalleCompras)
                    .ThenInclude(a => a.Producto)
                .Include(a => a.Empleado)
                .Include(a => a.Proveedor)
                .Include(a => a.TipoCompra);
            }

            return compra.Where(a => a.Id == id).FirstOrDefault();
        }
        public OrdenCompra GetOrdenCompra(int id, bool includeRelatedEntities = true)
        {
            var ordenCompra = _context.OrdenesCompra.AsQueryable();

            if (includeRelatedEntities)
            {
                ordenCompra = ordenCompra
                .Include(a => a.DetalleOrdenesCompra)
                    .ThenInclude(a => a.Producto)
                .Include(a => a.DetalleOrdenesCompra)
                    .ThenInclude(a => a.DetalleOrdenCompraUbicaciones)
                .Include(a => a.DetalleOrdenesCompra)
                    .ThenInclude(a => a.DetalleOrdenCompraUbicaciones)
                    .ThenInclude(a => a.DetalleOrdenCompraUbicacionPrecios)
                .Include(a => a.Empleado)
                .Include(a => a.Proveedor)
                .Include(a => a.TipoCompra);
            }

            return ordenCompra
                .Where(a => a.Id == id && !a.Eliminado).FirstOrDefault();
        }
        public List<DetalleCompra> GetDetalle(int id, bool includeRelatedEntities = true)
        {

            var detalle = _context.DetalleCompras.AsQueryable();

            if (includeRelatedEntities)
            {
                detalle = detalle
                    .Include(x => x.Producto)
                        .ThenInclude(a => a.ProductosInventario)
                        .ThenInclude(a => a.Bodega)
                    .Include(x => x.Producto)
                        .ThenInclude(a => a.ProductosInventario)
                        .ThenInclude(a => a.Compra)
                        .ThenInclude(a => a.DetalleCompras)
                    .Include(x => x.Producto)
                        .ThenInclude(a => a.ProductosInventario)
                        .ThenInclude(a => a.Compra)
                        .ThenInclude(a => a.Proveedor)
                    .Include(x => x.UnidadMedidaCompra);
            }

            return detalle.Where(x => x.CompraId == id).ToList();

        }
        //public List<DetalleOrdenCompra> GetDetalleOrdenCompra(int id, bool includeRelatedEntities = true)
        //{

        //    var detalle = _context.DetalleOrdenesCompra.AsQueryable();

        //    if (includeRelatedEntities)
        //    {
        //        detalle = detalle
        //            .Include(x => x.DetalleOrdenCompraUbicaciones)
        //                .ThenInclude(a => a.Bodega)
        //            .Include(x => x.DetalleOrdenCompraUbicaciones)
        //                .ThenInclude(a => a.UnidadMedidaVenta)
        //            .Include(x => x.DetalleOrdenCompraUbicaciones)
        //                .ThenInclude(a => a.DetalleOrdenCompraUbicacionPrecios)
        //                .ThenInclude(a => a.Precio)
        //            .Include(x => x.Producto)
        //                .ThenInclude(a => a.ProductoEquivalencias)
        //            .Include(x => x.Producto)
        //                .ThenInclude(a => a.ProductosInventario)
        //                .ThenInclude(a => a.Bodega)
        //            .Include(x => x.Producto)
        //                .ThenInclude(a => a.ProductosInventario)
        //                .ThenInclude(a => a.Compra)
        //                .ThenInclude(a => a.Proveedor)
        //            .Include(x => x.UnidadMedidaCompra);
        //    }

        //    return detalle.Where(x => x.OrdenCompraId == id).ToList();

        //}
        public List<DetalleCompra> GetDetalles(int id, bool includeRelatedEntities = true)
        {

            var detalle = _context.DetalleCompras.AsQueryable();

            if (includeRelatedEntities)
            {
                detalle = detalle
                    .Include(x => x.DetalleComprasUbicaciones)
                        .ThenInclude(a => a.Bodega)
                    .Include(x => x.DetalleComprasUbicaciones)
                        .ThenInclude(a => a.UnidadMedidaVenta)
                    .Include(x => x.DetalleComprasUbicaciones)
                        .ThenInclude(a => a.DetalleCompraUbicacionPrecios)
                        .ThenInclude(a => a.Precio)
                    .Include(x => x.Producto)
                        .ThenInclude(a => a.ProductoEquivalencias)
                    .Include(x => x.Producto)
                        .ThenInclude(a => a.ProductosInventario)
                        .ThenInclude(a => a.Bodega)
                    .Include(x => x.Producto)
                        .ThenInclude(a => a.ProductosInventario)
                        .ThenInclude(a => a.Compra)
                        .ThenInclude(a => a.Proveedor)
                    .Include(x => x.UnidadMedidaCompra);
            }

            return detalle.Where(x => x.CompraId == id).ToList();

        }
        
        //public List<DetalleCompra> GetAllDetalle()
        //{

        //    var detalle = _context.DetalleCompras
        //        .Include(x => x.Compra)
        //            .ThenInclude(x => x.Proveedor)
        //        .Include(x => x.Producto)
        //        .AsQueryable();


        //    return detalle.ToList();

        //}


        //public DetalleCompra GetDetalleC(int id, bool includeRelatedEntities = true)
        //{

        //    var detalle = _context.DetalleCompras.AsQueryable();

        //    if (includeRelatedEntities)
        //    {
        //        detalle = detalle.Include(x => x.Producto);
        //    }

        //    return detalle.Where(x => x.Id == id).SingleOrDefault();

        //}
        public void Update(Compra model, bool saveChanges = true)
        {

            _context.Entry(model).State = EntityState.Modified;

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }
        public void Update(OrdenCompra ordenCompra)
        {
            _context.Entry(ordenCompra).State = EntityState.Modified;
            _context.SaveChanges();
        }
        public void Delete(int id, bool savechanges = true)
        {
            var set = _context.Set<DetalleCompra>();
            var entity = set.Find(id);
            set.Remove(entity);

            if (true)
            {
                _context.SaveChanges();

            }

        }
        public List<Compra> GetListaTodas()
        {
            return _context.Compras
                .Include(a => a.Proveedor)
            .Include(a => a.TipoCompra)
            .Include(a => a.Empleado)
            .Include(a => a.DetalleCompras)
                .ThenInclude(x => x.Producto)
            .ToList();
        }
        public PaginacionList<Compra> PaginacionOrdenesCompra(string sortOrder, string searchString, int? pageNumber, int pageSize,
            string fechaInicial, string fechaFinal, int? comprobante, string proveedor, string vendedor)
        {
            var compra = _context.Compras.Where(a => a.CompraTipoDocumentoId != null
            && (int)a.CompraTipoDocumentoId == (int)CompraTipoDocumentoEnum.OrdenCompra
            && !a.Eliminado).AsQueryable();


            // para hacer la busqueda, el IsNullOrEmpty verifica que no este vacia la cadena entrante
            if (!string.IsNullOrEmpty(searchString))
            {
                compra = compra.Where(s => s.Proveedor.Nombre.Contains(searchString)
                || s.Id.ToString().Contains(searchString));
            }

            if (!string.IsNullOrEmpty(fechaInicial) && !string.IsNullOrEmpty(fechaFinal))
            {
                DateTime fechaInicio = DateTime.Parse(fechaInicial);
                DateTime fechaFin = DateTime.Parse(fechaFinal);
                compra = compra.Where(x => x.FechaCompra.Date >= fechaInicio && x.FechaCompra.Date <= fechaFin);
            }

            if (comprobante != null && comprobante != 0)
            {
                compra = compra.Where(x => x.Id == comprobante);
            }

            if (!string.IsNullOrEmpty(proveedor))
            {
                compra = compra.Where(x =>
                    x.Proveedor.Nombre != null &&
                    x.Proveedor.Nombre.ToLower().Trim() == proveedor.ToLower().Trim());
            }


            if (!string.IsNullOrEmpty(vendedor))
            {
                compra = compra.Where(x =>
                    x.Empleado.Nombre != null &&
                    x.Empleado.Nombre.ToLower().Trim() == vendedor.ToLower().Trim());
            }

            // esto que hice no es buena practica, lo ideal seria hacer un patron por estrategia
            // visitar : https://refactoring.guru/es/design-patterns/strategy
            // asi como lo tengo funciona pero no es tan tan tan recomendado
            // quizas mas adelante lo mejoremos con un patron de estrategia.

            switch (sortOrder)
            {
                case "Nombre_desc":
                    compra = compra.OrderByDescending(s => s.Proveedor.Nombre);
                    break;

                default:
                    compra = compra.OrderBy(s => s.Proveedor.Nombre);
                    break;
            }

            compra = compra.OrderByDescending(a => a.Id);

            return PaginacionList<Compra>.CreateAsyncc(compra
            .Include(a => a.Proveedor)
            .Include(a => a.Empleado)
            //.Include(a => a.TipoCompra)
            .Include(a => a.DetalleCompras
            .OrderByDescending(a => a.Id)), pageNumber ?? 1, pageSize);
        }
        public PaginacionList<Compra> PaginacionComprasPeticion(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var compra = _context.Compras.Where(x => x.Estado == false).AsQueryable();


            // para hacer la busqueda, el IsNullOrEmpty verifica que no este vacia la cadena entrante
            if (!string.IsNullOrEmpty(searchString))
            {
                compra = compra.Where(s => s.NoComprobante.Contains(searchString) || s.Id.ToString().Contains(searchString));
            }

            // esto que hice no es buena practica, lo ideal seria hacer un patron por estrategia
            // visitar : https://refactoring.guru/es/design-patterns/strategy
            // asi como lo tengo funciona pero no es tan tan tan recomendado
            // quizas mas adelante lo mejoremos con un patron de estrategia.

            switch (sortOrder)
            {
                case "Nombre_desc":
                    compra = compra.OrderByDescending(s => s.NoComprobante);
                    break;

                default:
                    compra = compra.OrderBy(s => s.NoComprobante);
                    break;
            }

            return PaginacionList<Compra>.CreateAsyncc(compra
            .Include(a => a.Proveedor)
            .Include(a => a.Empleado)
            .Include(a => a.TipoCompra)
            .Include(a => a.DetalleCompras), pageNumber ?? 1, pageSize);
        }
        public List<Compra> ListaComprados()
        {
            return _context.Compras
                .Include(x => x.Empleado)
                .Include(x => x.Proveedor)
                .Include(x => x.DetalleCompras)
                .Where(x => !x.Eliminado
                && x.OrdenCompraRecibida).ToList();
        }
        public PaginacionList<Compra> PaginacionCompras(string sortOrder, string searchString, int? pageNumber, int pageSize,
            string fechaInicial, string fechaFinal, string comprobante, string proveedor, string vendedor,
            int numeroCompra)
        {
            var compra = _context.Compras.Include(x => x.Empleado).Where(a => !a.Eliminado && a.OrdenCompraRecibida).AsQueryable();


            // para hacer la busqueda, el IsNullOrEmpty verifica que no este vacia la cadena entrante
            if (!string.IsNullOrEmpty(searchString))
            {
                compra = compra.Where(s => s.NoComprobante.Contains(searchString) || s.Id.ToString().Contains(searchString));
            }

            if (!string.IsNullOrEmpty(fechaInicial) && !string.IsNullOrEmpty(fechaFinal))
            {
                DateTime fechaInicio = DateTime.Parse(fechaInicial);
                DateTime fechaFin = DateTime.Parse(fechaFinal);
                compra = compra.Where(x => x.FechaCompra.Date >= fechaInicio && x.FechaCompra.Date <= fechaFin);
            }

            if (comprobante != null)
            {
                compra = compra.Where(x => x.NoComprobante == comprobante);
            }


            if (numeroCompra != 0)
            {
                compra = compra.Where(x => x.Id == numeroCompra);
            }

            if (!string.IsNullOrEmpty(proveedor))
            {
                compra = compra.Where(x =>
                    x.Proveedor.Nombre != null &&
                    x.Proveedor.Nombre.ToLower().Trim() == proveedor.ToLower().Trim());
            }


            if (!string.IsNullOrEmpty(vendedor))
            {
                //compra = compra.Where(x =>
                //    x.Empleado.NombreYApellidos.ToLower().Trim() == vendedor.ToLower().Trim());
                compra = compra.Where(x =>
                    x.Empleado.Nombre != null &&
                    x.Empleado.Nombre == vendedor);
            }

            // esto que hice no es buena practica, lo ideal seria hacer un patron por estrategia
            // visitar : https://refactoring.guru/es/design-patterns/strategy
            // asi como lo tengo funciona pero no es tan tan tan recomendado
            // quizas mas adelante lo mejoremos con un patron de estrategia.

            switch (sortOrder)
            {
                case "Nombre_desc":
                    compra = compra.OrderByDescending(s => s.Id);
                    break;

                default:
                    compra = compra.OrderByDescending(s => s.Id);
                    break;
            }

            return PaginacionList<Compra>.CreateAsyncc(compra
                .Include(a => a.Proveedor)
                .Include(a => a.Empleado)
                .Include(a => a.DetalleCompras), pageNumber ?? 1, pageSize);
        }
        //public PaginacionList<Compra> PaginacionComprasComprados(string sortOrder, string searchString, int? pageNumber, int pageSize)
        //{
        //    var compra = _context.Compras.Where(x => x.Estado == true).AsQueryable();


        //    // para hacer la busqueda, el IsNullOrEmpty verifica que no este vacia la cadena entrante
        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        compra = compra.Where(s => s.NoComprobante.Contains(searchString) || s.Id.ToString().Contains(searchString));
        //    }

        //    // esto que hice no es buena practica, lo ideal seria hacer un patron por estrategia
        //    // visitar : https://refactoring.guru/es/design-patterns/strategy
        //    // asi como lo tengo funciona pero no es tan tan tan recomendado
        //    // quizas mas adelante lo mejoremos con un patron de estrategia.

        //    switch (sortOrder)
        //    {
        //        case "Nombre_desc":
        //            compra = compra.OrderByDescending(s => s.NoComprobante);
        //            break;

        //        default:
        //            compra = compra.OrderBy(s => s.NoComprobante);
        //            break;
        //    }

        //    return PaginacionList<Compra>.CreateAsyncc(compra.Include(a => a.Proveedor)
        //        .Include(a => a.DetalleCompras), pageNumber ?? 1, pageSize);
        //}

        //public PaginacionList<Recepcion> PaginacionRecepciones(string sortOrder, string searchString, int? pageNumber, int pageSize)
        //{
        //    var recepcion = _context.Recepciones.AsQueryable();


        //    // para hacer la busqueda, el IsNullOrEmpty verifica que no este vacia la cadena entrante
        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        recepcion = recepcion.Where(s => s.Compra.NoComprobante.Contains(searchString) || s.Id.ToString().Contains(searchString));
        //    }

        //    // esto que hice no es buena practica, lo ideal seria hacer un patron por estrategia
        //    // visitar : https://refactoring.guru/es/design-patterns/strategy
        //    // asi como lo tengo funciona pero no es tan tan tan recomendado
        //    // quizas mas adelante lo mejoremos con un patron de estrategia.

        //    switch (sortOrder)
        //    {
        //        case "Nombre_desc":
        //            recepcion = recepcion.OrderByDescending(s => s.Compra.NoComprobante);
        //            break;

        //        default:
        //            recepcion = recepcion.OrderBy(s => s.Compra.NoComprobante);
        //            break;
        //    }

        //    return PaginacionList<Recepcion>.CreateAsyncc(recepcion
        //    .Include(x => x.Compra).ThenInclude(x => x.DetalleCompras)
        //    .Include(a => a.Compra).ThenInclude(a => a.TipoCompra)
        //    .Include(x => x.Compra).ThenInclude(x => x.Proveedor)
        //    .Include(a => a.EstadoRecepcion)
        //    .Include(x => x.EstadoRecepcion), pageNumber ?? 1, pageSize);
        //}

        //public Recepcion GetRecepcion(int id, bool includeRelatedEntities = true)
        //{
        //    var recepcion = _context.Recepciones.AsQueryable();

        //    if (includeRelatedEntities)
        //    {
        //        recepcion = recepcion

        //        .Include(a => a.Compra)
        //        .ThenInclude(a => a.DetalleCompras)
        //        .ThenInclude(a => a.Producto)

        //        .Include(a => a.Compra)
        //        .ThenInclude(a => a.Empleado)

        //        .Include(a => a.Compra)
        //        .ThenInclude(a => a.DetalleCompras)
        //        .ThenInclude(a => a.DetalleComprasUnidadesVentaPrecio);
        //    }

        //    return recepcion.Where(x => x.Id == id).SingleOrDefault();
        //}
        public IList<TipoCompra> TipoCompraLista()
        {
            return _context.TipoCompra.OrderBy(x => x.Tipo).ToList();
        }
        //public void Add(ICollection<DetalleOrdenCompra> detalleOrdenesCompra)
        //{
        //    throw new NotImplementedException();
        //}



        //public void Add(DetalleOrdenCompra detalleOrdenCompra)
        //{
        //    _context.Add(detalleOrdenCompra);
        //    _context.SaveChanges();
        //}
        public List<OrdenCompra> GetAll()
        {
            return _context.OrdenesCompra
                .Include(x => x.Proveedor)
                .Include(x => x.Empleado)
                .Where(a => !a.Eliminado).ToList();
        }
        public List<DetalleOrdenCompra> GetDetalleOrdenCompraByIdProducto(int IdProducto)
        {
            return _context.DetalleOrdenesCompra
                .Where(x => x.ProductoId == IdProducto)
                .Include(x => x.OrdenCompra)
                    .ThenInclude(x => x.Proveedor).ToList();
        }
        public DetalleCompra GetUltimoPrecioCompraProducto(int idProducto)
        {
            var data = _context.DetalleCompras
                .Include(x => x.Compra)
                .Include(x => x.Producto)
                .Where(x => x.ProductoId ==idProducto)
                .OrderByDescending(x => x.Compra.FechaCompra).FirstOrDefault();

            return data;
        }
    }

}
