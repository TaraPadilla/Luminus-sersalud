using Database.Shared.Models;
using farmamest.Models;
using sistema.Models;
using System.Collections.Generic;

namespace farmamest.Service.IService
{
    public interface IHospitalizacionService
    {
        List<PacientesBaseViewModel> GetPacientesExistentes();
        List<HospitalizacionViewModel> GetListaHospitalizaciones();
        List<HospitalizacionHabitacionViewModel> GetHabitaciones(bool disponibles, bool ocupadas);
        /// <summary>
        /// Consulta los servicios de una hospitalizacion. Consulta los servicios incluso
        /// agregados desde consulta externa en caso de que la hospitalizacion
        /// fuera realizada desde la CONSULTA
        /// </summary>
        /// <param name="hospitalizacionId"></param>
        /// <returns></returns>
        List<HospitalizacionServicioViewModel> GetServiciosHospitalizacion(int hospitalizacionId);
    }
}
