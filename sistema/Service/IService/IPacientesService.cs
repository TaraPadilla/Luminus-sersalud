using Database.Shared.Models;
using sistema.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sistema.Service.IService
{
    public interface IPacientesService
    {
        public List<Paciente> Buscar(string terminoBusquedaNombre, string terminoBusquedaDpi);
        public List<PacienteArchivoVM> GetArchivos(int pacienteId);
        public List<ConsultasViewModel> GetHistorialConsultas(int pacienteId);
        List<PacienteHistoricoHospitalizacionVM> GetHistorialHospitalizaciones(int pacienteId);
        List<PacientesBaseViewModel> GetPacientesExistentes();
        PacientesBaseViewModel ValidarExistenciaPaciente(PacientesBaseViewModel model);
    }
}

