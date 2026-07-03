using Database.Shared.Models;
using System.Collections.Generic;
using Database.Shared.Paginacion;

namespace Database.Shared.IRepository
{
    public interface ITraslados
    {
        void Add(TrasladosProductos traslados, bool saveChanges = true);
        void Add(DetalleTrasladoProductos detalleTrasladoProductos, bool saveChanges = true);
        #region Paginacion nuevos normalizados
        PaginacionList<TrasladosProductos> PaginacionTraslados(string sortOrder, string searchString, int? pageNumber, int pageSize);
        PaginacionList<TrasladosProductos> PaginacionEnTransito(string sortOrder, string searchString, int? pageNumber, int pageSize);
        PaginacionList<TrasladosProductos> PaginacionAceptados(string sortOrder, string searchString, int? pageNumber, int pageSize);
        PaginacionList<TrasladosProductos> PaginacionFaltantes(string sortOrder, string searchString, int? pageNumber, int pageSize);
        PaginacionList<TrasladosProductos> PaginacionCancelados(string sortOrder, string searchString, int? pageNumber, int pageSize);
        PaginacionList<TrasladosProductos> PaginacionConProblema(string sortOrder, string searchString, int? pageNumber, int pageSize);

        #endregion



        //PaginacionList<TrasladosProductos> PaginacionTrasladosBodegaAClinica(string sortOrder, string searchString, int? pageNumber, int pageSize);
        //PaginacionList<TrasladosProductos> PaginacionAceptadosBodegaAClinica(string sortOrder, string searchString, int? pageNumber, int pageSize);
        //PaginacionList<TrasladosProductos> PaginacionEnTransitoBodegaAClinica(string sortOrder, string searchString, int? pageNumber, int pageSize);
        //PaginacionList<TrasladosProductos> PaginacionFaltantesBodegaAClinica(string sortOrder, string searchString, int? pageNumber, int pageSize);
        //PaginacionList<TrasladosProductos> PaginacionConProblemaBodegaAClinica(string sortOrder, string searchString, int? pageNumber, int pageSize);
        //PaginacionList<TrasladosProductos> PaginacionCanceladosBodegaAClinica(string sortOrder, string searchString, int? pageNumber, int pageSize);

        List<ProductoInventario> GetProductosDisponiblesTraslado(int bodegaOrigenId);

        //PaginacionList<TrasladosProductos> PaginacionTrasladosBodegaAFarmacia(string sortOrder, string searchString, int? pageNumber, int pageSize);
        //PaginacionList<TrasladosProductos> PaginacionAceptadosBodegaAFarmacia(string sortOrder, string searchString, int? pageNumber, int pageSize);
        //PaginacionList<TrasladosProductos> PaginacionEnTransitoBodegaAFarmacia(string sortOrder, string searchString, int? pageNumber, int pageSize);
        //PaginacionList<TrasladosProductos> PaginacionFaltantesBodegaAFarmacia(string sortOrder, string searchString, int? pageNumber, int pageSize);
        //PaginacionList<TrasladosProductos> PaginacionConProblemaBodegaAFarmacia(string sortOrder, string searchString, int? pageNumber, int pageSize);
        //PaginacionList<TrasladosProductos> PaginacionCanceladosBodegaAFarmacia(string sortOrder, string searchString, int? pageNumber, int pageSize);


        TrasladosProductos GetTraslados(int id, bool includeRelatedEntities = true);
        void SaveChanges();
        IList<DetalleTrasladoProductos> GetListDetalleTrasladoProductos(int trasladoId);
        void DeleteDetalleTraslado(int id, bool savechanges = true);
        DetalleTrasladoProductos GetDetalleTrasladoProductos(int id, bool includeRelatedEntities = true);
        void UpdateDetalleTrasladoProductos(DetalleTrasladoProductos model, bool saveChanges = true);
        void UpdateTraslado(TrasladosProductos model, bool saveChanges = true);
    }
}