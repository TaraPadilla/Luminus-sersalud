using Database.Shared.Models;
using Microsoft.AspNetCore.Identity;
using sistema.Models;
using System.Collections.Generic;

namespace sistema.Service.IService
{
    public interface IEmergenciaService
    {
        void RegistrarEmergencia(EmergenciaViewModel model);
        List<EmergenciaViewModel> GetEmergencias(bool ingresadas);
        EmergenciaViewModel Get(int emergenciaId, bool includePaciente = false, bool includeElementos = false);
        void EditarEmergencia(EmergenciaViewModel model);


        int GetEmergenciaIdByHospitalizacion(int hospitalizacionId);


        void AgregarProducto(DetalleEmergenciaViewModel detalle);
        void AgregarServicio(DetalleEmergenciaViewModel detalle);
        void AgregarExamen(DetalleEmergenciaViewModel detalle);

        void EliminarDetalle(int detalleId);


    }
}
