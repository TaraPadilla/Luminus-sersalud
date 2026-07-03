using System.Collections.Generic;
using Database.Shared.Models;
using Database.Shared.Paginacion;

namespace Database.Shared.IRepository
{
    public interface IEmergencias
    {
        void Add(Emergencia emergencia);
        List<Emergencia> GetEmergencias(bool ingresadas);
        Emergencia Get(int emergenciaId,
            bool includePaciente = false,
            bool includeElementos = false);
        void Update(Emergencia emergencia);


        List<EmergenciaDetalle> GetExamenesAgregados(int emergenciaId);

        List<EmergenciaDetalle> GetServiciosAgregados(int emergenciaId);

        Emergencia UpdateSoloEstado(int id, bool nuevoEstado);

        List<EmergenciaDetalle> GetPrescripcionEmergencia(int emergenciaId, bool includeProducto = false);


        int? GetIdByHospitalizacion(int hospitalizacionId);


        void AddDetalle(EmergenciaDetalle detalle);
        void UpdateDetalle(EmergenciaDetalle detalle);
        void DeleteDetalle(int detalleId);
        EmergenciaDetalle GetDetalle(int detalleId);

    }
}