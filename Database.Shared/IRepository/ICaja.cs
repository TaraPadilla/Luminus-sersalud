using System.Collections.Generic;
using Database.Shared.Models;
using Database.Shared.Paginacion;
using System;

namespace Database.Shared.IRepository
{
    public interface ICaja
    {
        public IList<Caja> ListarCajas();
        public void Add(Caja caja, bool saveChanges = true);
        public void Add(DetalleCaja caja, bool saveChanges = true);
        List<DetalleCaja> GetDetallesCaja(int annio);
        public Caja GetCajaAbierta();
        Caja GetCajaAbierta(int ambienteId);
        public Caja GetCajaAbiertaById(int Id);
        public List<Caja> GetListadoFecha(DateTime inicio, DateTime final, int ambienteId);
        public void Update(Caja caja, bool saveChanges = true);
        public PaginacionList<Caja> PaginacionCategoria(string sortOrder, string searchString, int? pageNumber, int pageSize);
        public Caja GetCaja(int id, bool includeRelatedEntities = true);

        /// <summary>
        /// Consulta las subcajas que funcionaron durante el rango de fechas recibido.
        /// Generalmente corresponden a las fechas de apertura y cierre de una
        /// caja global
        /// </summary>
        /// <param name="fechaApertura"></param>
        /// <param name="fechaCierre"></param>
        /// <param name="sucursalId"></param>
        public List<Caja> GetSubcajas(DateTime fechaApertura, DateTime? fechaCierre, int? sucursalId, int cajaGlobalId);

        public void GetDetalleCajaPorVentaId(int id, bool savechanges = true);
        public DetalleCaja GetDetalleCaja(int id);
        //public DetalleCaja GetDetalleCajaVentaServicio(int id);

        void DeleteDetalleCaja(int id, bool saveChanges = true);
        public void ReabrirCaja(int cajaId);
    }
}
