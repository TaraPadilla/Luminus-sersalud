using Database.Shared.Models;
using Database.Shared.IRepository;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Paginacion;


namespace Database.Shared.Data
{
    public class ConfiguracionSistemaRepository : IConfiguracionSistema
    {
        private readonly Context _context = null;
        private readonly string NombreEstablecimiento = "";
        private readonly bool ProrrateoHabilitado = false;

        public ConfiguracionSistemaRepository(Context context)
        {
            _context = context;

            //
            var configuraciones = _context.ConfiguracionesSistema
                .FirstOrDefault();
            if (configuraciones != null)
            {
                NombreEstablecimiento = configuraciones.NombreEstablecimiento
                    ?? "AVM";
                ProrrateoHabilitado = configuraciones.ProrrateoHabilitado;
            }
        }

        public string GetNombreEstablecimiento()
        {
            return NombreEstablecimiento;
        }
        public bool GetProrrateoHabilitado()
        {
            return ProrrateoHabilitado;
        }

        //public void Add(Servicio servicio, bool saveChanges = true)
        //{
        //    _context.Servicios.Add(servicio);

        //    if (saveChanges)
        //    {
        //        _context.SaveChanges();
        //    }
        //}

        //public IList<Servicio> GetListaServicios()
        //{
        //    return _context.Servicios
        //        .Include(a => a.SucursalServicios).ThenInclude(a => a.Sucursal)
        //        .Include(a => a.ServiciosPrecios).ThenInclude(a => a.Precio)
        //        .Where(a => a.Eliminado == false)
        //        .OrderBy(a => a.NombreServicio)
        //        .ToList();
        //}

        //public List<ListadoServiciosIdYNombres> GetList()
        //{
        //    return _context.Servicios
        //        .Where(a => a.Eliminado == false)
        //        .Select(a => new ListadoServiciosIdYNombres
        //        {
        //            Id = a.Id,
        //            NombreServicio = a.NombreServicio
        //        })
        //        .OrderBy(a => a.NombreServicio)
        //        .ToList();
        //}
        //public List<ServicioPrecio> GetPreciosServicio(int servicioId)
        //{
        //    return _context.ServiciosPrecios
        //        .Include(a => a.Precio)
        //        .Where(a => a.ServicioId == servicioId
        //        && !a.Precio.Eliminado)
        //        .ToList();
        //}
        //public List<SucursalServicio> GetSucursalesServicio(int servicioId)
        //{
        //    return _context.SucursalesServicios
        //        .Where(a => a.ServicioId == servicioId)
        //        .ToList();
        //}
        //public List<ServicioInsumo> GetInsumosServicio(int servicioId)
        //{
        //    return _context.ServiciosInsumos
        //        .Include(a => a.Producto)
        //        .Include(a => a.UnidadMedidaVenta)
        //        .Where(a => a.ServicioId == servicioId
        //        && !a.Producto.Eliminado)
        //        .ToList();
        //}

        //public IList<Servicio> BuscarPorNombreBusquedaAjax(string searchString)
        //{
        //    return _context.Servicios.Where(s => s.NombreServicio.Contains(searchString))
        //                    .Where(a => a.Eliminado == false)
        //                    .ToList();
        //}

        //public Servicio Get(int id, bool includeRelatedEntities = true)
        //{
        //    var servicios = _context.Servicios
        //        .Include(a => a.ServiciosPrecios).ThenInclude(a => a.Precio)
        //        .Include(a => a.ServiciosInsumos).ThenInclude(a => a.Producto)
        //        .AsQueryable();

        //    return servicios
        //       .Where(a => a.Id == id)
        //       .SingleOrDefault();
        //}

        //public Servicio GetNombre(string nombre, bool includeRelatedEntities = true)
        //{
        //    var servicios = _context.Servicios.AsQueryable();


        //    return servicios
        //       .Where(a => a.NombreServicio == nombre)
        //       .SingleOrDefault();

        //}
        //public void Update(Servicio model, bool saveChanges = true)
        //{

        //    _context.Entry(model).State = EntityState.Modified;

        //    if (saveChanges)
        //    {
        //        _context.SaveChanges();
        //    }

        //}
        //public void UpdatePrecio(ServicioPrecio servicioPrecio, bool saveChanges = true)
        //{

        //    _context.Entry(servicioPrecio).State = EntityState.Modified;

        //    if (saveChanges)
        //    {
        //        _context.SaveChanges();
        //    }

        //}
        //public void UpdateInsumo(ServicioInsumo servicioInsumo, bool saveChanges = true)
        //{

        //    _context.Entry(servicioInsumo).State = EntityState.Modified;

        //    if (saveChanges)
        //    {
        //        _context.SaveChanges();
        //    }

        //}
        //public void ActualizarInventarioVentaServicio(int servicioId)
        //{
        //    var insumos = _context.ServiciosInsumos
        //        .Where(s => s.ServicioId == servicioId)
        //        .ToList();
        //    if (insumos != null && insumos.Count > 0)
        //    {
        //        foreach (var insumo in insumos)
        //        {
        //            var inventarioProducto = _context.ProductosInventario
        //                .Where(p => p.ProductoId == insumo.ProductoId
        //                && p.UnidadMedidaVentaId == insumo.UnidadMedidaVentaId
        //                    && p.Stock > 0)
        //                .FirstOrDefault();
        //            if (inventarioProducto != null)
        //            {
        //                inventarioProducto.Stock -= insumo.CantidadUtilizada;

        //                _context.Entry(inventarioProducto).State = EntityState.Modified;
        //                _context.SaveChanges();
        //            }

        //        }
        //    }
        //}
        //public void SaveChanges()
        //{
        //    _context.SaveChanges();
        //}

        //#region Categorias
        //public void AddCategoria(CategoriaServicio categoria)
        //{
        //    _context.CategoriasServicios.Add(categoria);
        //    _context.SaveChanges();
        //}
        //public List<CategoriaServicio> GetListaCategorias()
        //{
        //    return _context.CategoriasServicios
        //        .Where(a => !a.Eliminada)
        //        .ToList();
        //}
        //public CategoriaServicio GetCategoria(int categoriaId)
        //{
        //    return _context.CategoriasServicios
        //        .Where(a => a.Id == categoriaId)
        //        .FirstOrDefault();
        //}
        //public void UpdateCategoria(CategoriaServicio categoria)
        //{
        //    _context.Entry(categoria).State = EntityState.Modified;
        //    _context.SaveChanges();
        //}
        //#endregion
    }
}