using System.Collections.Generic;
using System.Threading.Tasks;
using Database.Shared.Models;

namespace farmamest.Service.IService
{
    public interface IMedicamentoNotificacionService
    {
        Task ProgramarNotificacionesAsync(
            int hospitalizacionId,
            string nombreProducto,
            decimal cantidad,
            string indicaciones,
            string viaAdministracion,
            string frecuenciaAdministracion,
            string fechaHoraAplicacionManual,
            string baseUrl,
            string usuarioSolicitanteId = null);
    }
}