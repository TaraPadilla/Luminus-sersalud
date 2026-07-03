using Database.Shared.Models;
using System.Collections.Generic;
using Database.Shared.Paginacion;
using Database.Shared.Data;

namespace Database.Shared.IRepository
{
    public interface IConfiguracionSistema
    {
        public string GetNombreEstablecimiento();
        public bool GetProrrateoHabilitado();
        //public void Add(Servicio servicio, bool saveChanges = true);
        //public List<ListadoServiciosIdYNombres> GetList();
        //public List<ServicioPrecio> GetPreciosServicio(int servicioId);
        //public List<SucursalServicio> GetSucursalesServicio(int servicioId);
        //public List<ServicioInsumo> GetInsumosServicio(int servicioId);
        //public Servicio GetNombre(string nombre, bool includeRelatedEntities = true);
        //public IList<Servicio> BuscarPorNombreBusquedaAjax(string searchString);
        //public Servicio Get(int id, bool includeRelatedEntities = true);
        //public void Update(Servicio servicio, bool saveChanges = true);
        //public void UpdatePrecio(ServicioPrecio servicioPrecio, bool saveChanges = true);
        //public void UpdateInsumo(ServicioInsumo servicioPrecio, bool saveChanges = true);
        //public IList<Servicio> GetListaServicios();
        //public void ActualizarInventarioVentaServicio(int servicioId);
        //public void SaveChanges();

        //#region Categorias
        //public void AddCategoria(CategoriaServicio categoria);
        //public List<CategoriaServicio> GetListaCategorias();
        //public CategoriaServicio GetCategoria(int categoriaId);
        //public void UpdateCategoria(CategoriaServicio categoria);
        //#endregion
    }
}