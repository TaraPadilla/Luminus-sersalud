using Database.Shared.Models;
using Database.Shared.IRepository;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Paginacion;
using Database.Shared.Enumeraciones;

namespace Database.Shared.Data
{
    public class TrasladosRepository : ITraslados
    {

        private readonly Context _context = null;

        public TrasladosRepository(Context context)
        {
            _context = context;
        }

        public void Add(TrasladosProductos traslados, bool saveChanges = true)
        {
            _context.TrasladosProductos.Add(traslados);
            if (saveChanges) _context.SaveChanges();
        }

        public void Add(DetalleTrasladoProductos detalleTrasladoProductos, bool saveChanges = true)
        {
            _context.DetalleTrasladoProductos.Add(detalleTrasladoProductos);
            if (saveChanges) _context.SaveChanges();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        #region Paginacion
        public PaginacionList<TrasladosProductos> PaginacionTraslados(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var traslados = _context.TrasladosProductos.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                traslados = traslados.Where(s => s
                .ResponsableEnviado.Persona.Nombre.Contains(searchString)
                || s.ResponsableRecibido.Persona.Nombre.Contains(searchString)
                || s.Id.ToString().Contains(searchString));
            }

            return PaginacionList<TrasladosProductos>.CreateAsyncc(traslados
            .Include(a => a.EstadoTraslados)
            .Include(a => a.BodegaOrigen)
            .Include(a => a.BodegaDestino)
            .Include(a => a.DetalleTrasladoProductos)
            .Include(a => a.ResponsableEnviado).ThenInclude(a => a.Persona)
            .Include(a => a.ResponsableRecibido).ThenInclude(a => a.Persona)
            .OrderByDescending(a => a.FechaTraslado)
            .Where(a => a.Eliminado == false), pageNumber ?? 1, pageSize);
        }
        public PaginacionList<TrasladosProductos> PaginacionEnTransito(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var traslados = _context.TrasladosProductos.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                traslados = traslados.Where(s => s
                .ResponsableEnviado.Persona.Nombre.Contains(searchString)
                || s.ResponsableRecibido.Persona.Nombre.Contains(searchString)
                || s.Id.ToString().Contains(searchString));
            }

            return PaginacionList<TrasladosProductos>.CreateAsyncc(traslados
            .Include(a => a.EstadoTraslados)
            .Include(a => a.BodegaOrigen)
            .Include(a => a.BodegaDestino)
            .Include(a => a.DetalleTrasladoProductos)
            .Include(a => a.ResponsableEnviado).ThenInclude(a => a.Persona)
            .Include(a => a.ResponsableRecibido).ThenInclude(a => a.Persona)
            .OrderByDescending(a => a.FechaTraslado)
            .Where(a => a.Eliminado == false
            && a.EstadoTrasladosId == (int)EstadoTrasladoEnum.EnTransito), pageNumber ?? 1, pageSize);
        }
        public PaginacionList<TrasladosProductos> PaginacionAceptados(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var traslados = _context.TrasladosProductos.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                traslados = traslados.Where(s => s
                .ResponsableEnviado.Persona.Nombre.Contains(searchString)
                || s.ResponsableRecibido.Persona.Nombre.Contains(searchString)
                || s.Id.ToString().Contains(searchString));
            }

            return PaginacionList<TrasladosProductos>.CreateAsyncc(traslados
            .Include(a => a.EstadoTraslados)
            .Include(a => a.BodegaOrigen)
            .Include(a => a.BodegaDestino)
            .Include(a => a.DetalleTrasladoProductos)
            .Include(a => a.ResponsableEnviado).ThenInclude(a => a.Persona)
            .Include(a => a.ResponsableRecibido).ThenInclude(a => a.Persona)
            .OrderByDescending(a => a.FechaTraslado)
            .Where(a => a.Eliminado == false
            && a.EstadoTrasladosId == (int)EstadoTrasladoEnum.Aceptado), pageNumber ?? 1, pageSize);
        }
        public PaginacionList<TrasladosProductos> PaginacionFaltantes(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var traslados = _context.TrasladosProductos.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                traslados = traslados.Where(s => s
                .ResponsableEnviado.Persona.Nombre.Contains(searchString)
                || s.ResponsableRecibido.Persona.Nombre.Contains(searchString)
                || s.Id.ToString().Contains(searchString));
            }

            return PaginacionList<TrasladosProductos>.CreateAsyncc(traslados
            .Include(a => a.EstadoTraslados)
            .Include(a => a.BodegaOrigen)
            .Include(a => a.BodegaDestino)
            .Include(a => a.DetalleTrasladoProductos)
            .Include(a => a.ResponsableEnviado).ThenInclude(a => a.Persona)
            .Include(a => a.ResponsableRecibido).ThenInclude(a => a.Persona)
            .OrderByDescending(a => a.FechaTraslado)
            .Where(a => a.Eliminado == false
            && a.EstadoTrasladosId == (int)EstadoTrasladoEnum.Faltantes), pageNumber ?? 1, pageSize);
        }
        public PaginacionList<TrasladosProductos> PaginacionCancelados(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var traslados = _context.TrasladosProductos.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                traslados = traslados.Where(s => s
                .ResponsableEnviado.Persona.Nombre.Contains(searchString)
                || s.ResponsableRecibido.Persona.Nombre.Contains(searchString)
                || s.Id.ToString().Contains(searchString));
            }

            return PaginacionList<TrasladosProductos>.CreateAsyncc(traslados
            .Include(a => a.EstadoTraslados)
            .Include(a => a.BodegaOrigen)
            .Include(a => a.BodegaDestino)
            .Include(a => a.DetalleTrasladoProductos)
            .Include(a => a.ResponsableEnviado).ThenInclude(a => a.Persona)
            .Include(a => a.ResponsableRecibido).ThenInclude(a => a.Persona)
            .OrderByDescending(a => a.FechaTraslado)
            .Where(a => a.Eliminado == false
            && a.EstadoTrasladosId == (int)EstadoTrasladoEnum.Denegado), pageNumber ?? 1, pageSize);
        }
        public PaginacionList<TrasladosProductos> PaginacionConProblema(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var traslados = _context.TrasladosProductos.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                traslados = traslados.Where(s => s
                .ResponsableEnviado.Persona.Nombre.Contains(searchString)
                || s.ResponsableRecibido.Persona.Nombre.Contains(searchString)
                || s.Id.ToString().Contains(searchString));
            }

            return PaginacionList<TrasladosProductos>.CreateAsyncc(traslados
            .Include(a => a.EstadoTraslados)
            .Include(a => a.BodegaOrigen)
            .Include(a => a.BodegaDestino)
            .Include(a => a.DetalleTrasladoProductos)
            .Include(a => a.ResponsableEnviado).ThenInclude(a => a.Persona)
            .Include(a => a.ResponsableRecibido).ThenInclude(a => a.Persona)
            .OrderByDescending(a => a.FechaTraslado)
            .Where(a => a.Eliminado == false && a.EstadoTrasladosId == (int)EstadoTrasladoEnum.ConProblema), pageNumber ?? 1, pageSize);
        }
        #endregion







        //public PaginacionList<TrasladosProductos> PaginacionTrasladosBodegaAClinica(string sortOrder, string searchString, int? pageNumber, int pageSize)
        //{
        //    var traslados = _context.TrasladosProductos.AsQueryable();

        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        traslados = traslados.Where(s => s
        //        .ResponsableEnviado.Persona.Nombre.Contains(searchString)
        //        || s.ResponsableRecibido.Persona.Nombre.Contains(searchString)
        //        || s.Id.ToString().Contains(searchString));
        //    }

        //    return PaginacionList<TrasladosProductos>.CreateAsyncc(traslados
        //    .Include(a => a.EstadoTraslados)
        //    .Include(a => a.DetalleTrasladoProductos)
        //    .Include(a => a.ResponsableEnviado).ThenInclude(a => a.Persona)
        //    .Include(a => a.ResponsableRecibido).ThenInclude(a => a.Persona)
        //    .OrderByDescending(a => a.FechaTraslado)
        //    .Where(a => a.Eliminado == false && a.TipoTrasladoBodega == "BodegaAClinica"), pageNumber ?? 1, pageSize);
        //}


        //public PaginacionList<TrasladosProductos> PaginacionTrasladosBodegaAFarmacia(string sortOrder, string searchString, int? pageNumber, int pageSize)
        //{
        //    var traslados = _context.TrasladosProductos.AsQueryable();

        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        traslados = traslados.Where(s => s
        //        .ResponsableEnviado.Persona.Nombre.Contains(searchString)
        //        || s.ResponsableRecibido.Persona.Nombre.Contains(searchString)
        //        || s.Id.ToString().Contains(searchString));
        //    }

        //    return PaginacionList<TrasladosProductos>.CreateAsyncc(traslados
        //    .Include(a => a.EstadoTraslados)
        //    .Include(a => a.DetalleTrasladoProductos)
        //    .Include(a => a.ResponsableEnviado).ThenInclude(a => a.Persona)
        //    .Include(a => a.ResponsableRecibido).ThenInclude(a => a.Persona)
        //    .OrderByDescending(a => a.FechaTraslado)
        //    .Where(a => a.Eliminado == false && a.TipoTrasladoBodega == "BodegaAFarmacia"), pageNumber ?? 1, pageSize);
        //}

        //public PaginacionList<TrasladosProductos> PaginacionEnTransitoBodegaAClinica(string sortOrder, string searchString, int? pageNumber, int pageSize)
        //{
        //    var traslados = _context.TrasladosProductos.AsQueryable();

        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        traslados = traslados.Where(s => s
        //        .ResponsableEnviado.Persona.Nombre.Contains(searchString)
        //        || s.ResponsableRecibido.Persona.Nombre.Contains(searchString)
        //        || s.Id.ToString().Contains(searchString));
        //    }

        //    return PaginacionList<TrasladosProductos>.CreateAsyncc(traslados
        //    .Include(a => a.EstadoTraslados)
        //    .Include(a => a.DetalleTrasladoProductos)
        //    .Include(a => a.ResponsableEnviado).ThenInclude(a => a.Persona)
        //    .Include(a => a.ResponsableRecibido).ThenInclude(a => a.Persona)
        //    .OrderByDescending(a => a.FechaTraslado)
        //    .Where(a => a.Eliminado == false && a.EstadoTrasladosId == 5 && a.TipoTrasladoBodega == "BodegaAClinica"), pageNumber ?? 1, pageSize);
        //}

        //public PaginacionList<TrasladosProductos> PaginacionEnTransitoBodegaAFarmacia(string sortOrder, string searchString, int? pageNumber, int pageSize)
        //{
        //    var traslados = _context.TrasladosProductos.AsQueryable();

        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        traslados = traslados.Where(s => s
        //        .ResponsableEnviado.Persona.Nombre.Contains(searchString)
        //        || s.ResponsableRecibido.Persona.Nombre.Contains(searchString)
        //        || s.Id.ToString().Contains(searchString));
        //    }

        //    return PaginacionList<TrasladosProductos>.CreateAsyncc(traslados
        //    .Include(a => a.EstadoTraslados)
        //    .Include(a => a.DetalleTrasladoProductos)
        //    .Include(a => a.ResponsableEnviado).ThenInclude(a => a.Persona)
        //    .Include(a => a.ResponsableRecibido).ThenInclude(a => a.Persona)
        //    .OrderByDescending(a => a.FechaTraslado)
        //    .Where(a => a.Eliminado == false && a.EstadoTrasladosId == 5 && a.TipoTrasladoBodega == "BodegaAFarmacia"), pageNumber ?? 1, pageSize);
        //}

        //public PaginacionList<TrasladosProductos> PaginacionAceptadosBodegaAClinica(string sortOrder, string searchString, int? pageNumber, int pageSize)
        //{
        //    var traslados = _context.TrasladosProductos.AsQueryable();

        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        traslados = traslados.Where(s => s
        //        .ResponsableEnviado.Persona.Nombre.Contains(searchString)
        //        || s.ResponsableRecibido.Persona.Nombre.Contains(searchString)
        //        || s.Id.ToString().Contains(searchString));
        //    }

        //    return PaginacionList<TrasladosProductos>.CreateAsyncc(traslados
        //    .Include(a => a.EstadoTraslados)
        //    .Include(a => a.DetalleTrasladoProductos)
        //    .Include(a => a.ResponsableEnviado).ThenInclude(a => a.Persona)
        //    .Include(a => a.ResponsableRecibido).ThenInclude(a => a.Persona)
        //    .OrderByDescending(a => a.FechaTraslado)
        //    .Where(a => a.Eliminado == false && a.EstadoTrasladosId == 1 && a.TipoTrasladoBodega == "BodegaAClinica"), pageNumber ?? 1, pageSize);
        //}

        //public PaginacionList<TrasladosProductos> PaginacionAceptadosBodegaAFarmacia(string sortOrder, string searchString, int? pageNumber, int pageSize)
        //{
        //    var traslados = _context.TrasladosProductos.AsQueryable();

        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        traslados = traslados.Where(s => s
        //        .ResponsableEnviado.Persona.Nombre.Contains(searchString)
        //        || s.ResponsableRecibido.Persona.Nombre.Contains(searchString)
        //        || s.Id.ToString().Contains(searchString));
        //    }

        //    return PaginacionList<TrasladosProductos>.CreateAsyncc(traslados
        //    .Include(a => a.EstadoTraslados)
        //    .Include(a => a.DetalleTrasladoProductos)
        //    .Include(a => a.ResponsableEnviado).ThenInclude(a => a.Persona)
        //    .Include(a => a.ResponsableRecibido).ThenInclude(a => a.Persona)
        //    .OrderByDescending(a => a.FechaTraslado)
        //    .Where(a => a.Eliminado == false && a.EstadoTrasladosId == 1 && a.TipoTrasladoBodega == "BodegaAFarmacia"), pageNumber ?? 1, pageSize);
        //}




        public List<ProductoInventario> GetProductosDisponiblesTraslado(int bodegaOrigenId)
        {
            return _context.ProductosInventario
                .Include(p => p.Producto)
                .Include(p => p.UnidadMedidaVenta)
                .Where(p => p.BodegaId == bodegaOrigenId && p.Stock > 0)
                .OrderBy(p => p.Producto.NombreProducto)
                .ToList();
        }


        //public PaginacionList<TrasladosProductos> PaginacionFaltantesBodegaAClinica(string sortOrder, string searchString, int? pageNumber, int pageSize)
        //{
        //    var traslados = _context.TrasladosProductos.AsQueryable();

        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        traslados = traslados.Where(s => s
        //        .ResponsableEnviado.Persona.Nombre.Contains(searchString)
        //        || s.ResponsableRecibido.Persona.Nombre.Contains(searchString)
        //        || s.Id.ToString().Contains(searchString));
        //    }

        //    return PaginacionList<TrasladosProductos>.CreateAsyncc(traslados
        //    .Include(a => a.EstadoTraslados)
        //    .Include(a => a.DetalleTrasladoProductos)
        //    .Include(a => a.ResponsableEnviado).ThenInclude(a => a.Persona)
        //    .Include(a => a.ResponsableRecibido).ThenInclude(a => a.Persona)
        //    .OrderByDescending(a => a.FechaTraslado)
        //    .Where(a => a.Eliminado == false && a.EstadoTrasladosId == 4 && a.TipoTrasladoBodega == "BodegaAClinica"), pageNumber ?? 1, pageSize);
        //}

        //public PaginacionList<TrasladosProductos> PaginacionFaltantesBodegaAFarmacia(string sortOrder, string searchString, int? pageNumber, int pageSize)
        //{
        //    var traslados = _context.TrasladosProductos.AsQueryable();

        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        traslados = traslados.Where(s => s
        //        .ResponsableEnviado.Persona.Nombre.Contains(searchString)
        //        || s.ResponsableRecibido.Persona.Nombre.Contains(searchString)
        //        || s.Id.ToString().Contains(searchString));
        //    }

        //    return PaginacionList<TrasladosProductos>.CreateAsyncc(traslados
        //    .Include(a => a.EstadoTraslados)
        //    .Include(a => a.DetalleTrasladoProductos)
        //    .Include(a => a.ResponsableEnviado).ThenInclude(a => a.Persona)
        //    .Include(a => a.ResponsableRecibido).ThenInclude(a => a.Persona)
        //    .OrderByDescending(a => a.FechaTraslado)
        //    .Where(a => a.Eliminado == false && a.EstadoTrasladosId == 4 && a.TipoTrasladoBodega == "BodegaAFarmacia"), pageNumber ?? 1, pageSize);
        //}

        //public PaginacionList<TrasladosProductos> PaginacionConProblemaBodegaAClinica(string sortOrder, string searchString, int? pageNumber, int pageSize)
        //{
        //    var traslados = _context.TrasladosProductos.AsQueryable();

        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        traslados = traslados.Where(s => s
        //        .ResponsableEnviado.Persona.Nombre.Contains(searchString)
        //        || s.ResponsableRecibido.Persona.Nombre.Contains(searchString)
        //        || s.Id.ToString().Contains(searchString));
        //    }

        //    return PaginacionList<TrasladosProductos>.CreateAsyncc(traslados
        //    .Include(a => a.EstadoTraslados)
        //    .Include(a => a.DetalleTrasladoProductos)
        //    .Include(a => a.ResponsableEnviado).ThenInclude(a => a.Persona)
        //    .Include(a => a.ResponsableRecibido).ThenInclude(a => a.Persona)
        //    .OrderByDescending(a => a.FechaTraslado)
        //    .Where(a => a.Eliminado == false && a.EstadoTrasladosId == 3 && a.TipoTrasladoBodega == "BodegaAClinica"), pageNumber ?? 1, pageSize);
        //}


        //public PaginacionList<TrasladosProductos> PaginacionConProblemaBodegaAFarmacia(string sortOrder, string searchString, int? pageNumber, int pageSize)
        //{
        //    var traslados = _context.TrasladosProductos.AsQueryable();

        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        traslados = traslados.Where(s => s
        //        .ResponsableEnviado.Persona.Nombre.Contains(searchString)
        //        || s.ResponsableRecibido.Persona.Nombre.Contains(searchString)
        //        || s.Id.ToString().Contains(searchString));
        //    }

        //    return PaginacionList<TrasladosProductos>.CreateAsyncc(traslados
        //    .Include(a => a.EstadoTraslados)
        //    .Include(a => a.DetalleTrasladoProductos)
        //    .Include(a => a.ResponsableEnviado).ThenInclude(a => a.Persona)
        //    .Include(a => a.ResponsableRecibido).ThenInclude(a => a.Persona)
        //    .OrderByDescending(a => a.FechaTraslado)
        //    .Where(a => a.Eliminado == false && a.EstadoTrasladosId == 3 && a.TipoTrasladoBodega == "BodegaAFarmacia"), pageNumber ?? 1, pageSize);
        //}

        //public PaginacionList<TrasladosProductos> PaginacionCanceladosBodegaAClinica(string sortOrder, string searchString, int? pageNumber, int pageSize)
        //{
        //    var traslados = _context.TrasladosProductos.AsQueryable();

        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        traslados = traslados.Where(s => s
        //        .ResponsableEnviado.Persona.Nombre.Contains(searchString)
        //        || s.ResponsableRecibido.Persona.Nombre.Contains(searchString)
        //        || s.Id.ToString().Contains(searchString));
        //    }

        //    return PaginacionList<TrasladosProductos>.CreateAsyncc(traslados
        //    .Include(a => a.EstadoTraslados)
        //    .Include(a => a.DetalleTrasladoProductos)
        //    .Include(a => a.ResponsableEnviado).ThenInclude(a => a.Persona)
        //    .Include(a => a.ResponsableRecibido).ThenInclude(a => a.Persona)
        //    .OrderByDescending(a => a.FechaTraslado)
        //    .Where(a => a.Eliminado == false && a.EstadoTrasladosId == 2 && a.TipoTrasladoBodega == "BodegaAClinica"), pageNumber ?? 1, pageSize);
        //}

        //public PaginacionList<TrasladosProductos> PaginacionCanceladosBodegaAFarmacia(string sortOrder, string searchString, int? pageNumber, int pageSize)
        //{
        //    var traslados = _context.TrasladosProductos.AsQueryable();

        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        traslados = traslados.Where(s => s
        //        .ResponsableEnviado.Persona.Nombre.Contains(searchString)
        //        || s.ResponsableRecibido.Persona.Nombre.Contains(searchString)
        //        || s.Id.ToString().Contains(searchString));
        //    }

        //    return PaginacionList<TrasladosProductos>.CreateAsyncc(traslados
        //    .Include(a => a.EstadoTraslados)
        //    .Include(a => a.DetalleTrasladoProductos)
        //    .Include(a => a.ResponsableEnviado).ThenInclude(a => a.Persona)
        //    .Include(a => a.ResponsableRecibido).ThenInclude(a => a.Persona)
        //    .OrderByDescending(a => a.FechaTraslado)
        //    .Where(a => a.Eliminado == false && a.EstadoTrasladosId == 2 && a.TipoTrasladoBodega == "BodegaAFarmacia"), pageNumber ?? 1, pageSize);
        //}

        public TrasladosProductos GetTraslados(int id, bool includeRelatedEntities = true)
        {
            var traslado = _context.TrasladosProductos.AsQueryable();

            if (includeRelatedEntities == true)
            {
                traslado = traslado
                .Include(a => a.EstadoTraslados)
                .Include(a => a.BodegaOrigen)
                .Include(a => a.BodegaDestino)
                .Include(a => a.DetalleTrasladoProductos).ThenInclude(a => a.Producto).ThenInclude(a => a.PresentacionProducto)
                .Include(a => a.DetalleTrasladoProductos).ThenInclude(a => a.ProductoInventario).ThenInclude(a => a.Producto)

                .Include(a => a.DetalleTrasladoProductos).ThenInclude(a => a.ProductoInventario)
                .ThenInclude(a => a.Producto).ThenInclude(a => a.PresentacionProducto)

                .Include(a => a.DetalleTrasladoProductos).ThenInclude(a => a.ProductoInventario)
                .ThenInclude(a => a.UnidadMedidaVenta)

                .Include(a => a.DetalleTrasladoProductos).ThenInclude(a => a.ProductoInventario)
                .ThenInclude(a => a.ProductosInventarioPrecios)

                .Include(a => a.ResponsableRecibido).ThenInclude(a => a.Persona)
                .Include(a => a.ResponsableEnviado).ThenInclude(a => a.Persona);
            }

            if (traslado != null)
            {
                foreach (var item in traslado)
                {
                    item.DetalleTrasladoProductos = item.DetalleTrasladoProductos.Where(a => !a.Eliminado).ToList();
                }
            }

            return traslado
            .Where(a => a.Id == id).SingleOrDefault();
        }

        public IList<DetalleTrasladoProductos> GetListDetalleTrasladoProductos(int trasladoId)
        {
            return _context.DetalleTrasladoProductos
                .Include(a => a.ProductoInventario).ThenInclude(a => a.Producto)
                .Include(a => a.ProductoInventario).ThenInclude(a => a.UnidadMedidaVenta)
                .Where(a => a.TrasladosProductosId == trasladoId && !a.Eliminado)
                .OrderByDescending(a => a.FechaTraslado)
                .ToList();
        }


        public void DeleteDetalleTraslado(int id, bool savechanges = true)
        {
            var set = _context.Set<DetalleTrasladoProductos>();
            var entity = set.Find(id);
            set.Remove(entity);

            if (savechanges)
            {
                _context.SaveChanges();

            }
        }

        public DetalleTrasladoProductos GetDetalleTrasladoProductos(int id, bool includeRelatedEntities = true)
        {
            var detalleTrasladoProductos = _context.DetalleTrasladoProductos.AsQueryable();

            return detalleTrasladoProductos.Where(a => a.Id == id).SingleOrDefault();
        }


        public void UpdateDetalleTrasladoProductos(DetalleTrasladoProductos model, bool saveChanges = true)
        {

            _context.Entry(model).State = EntityState.Modified;

            if (saveChanges)
            {
                _context.SaveChanges();
            }

        }

        public void UpdateTraslado(TrasladosProductos model, bool saveChanges = true)
        {
            _context.Entry(model).State = EntityState.Modified;

            if (saveChanges)
            {
                _context.SaveChanges();
            }

        }


    }
}