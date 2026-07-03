using Database.Shared.Models;
using farmamest.Models;
using System.Collections.Generic;

namespace farmamest.Service.IService
{
    public interface ITurnoEnfermeriaService
    {
        // Método para obtener la lista de turnos de enfermería
        List<TurnoEnfermeriaVM> GetTurnoEnfermeriaList();

        // Método para obtener los turnos de enfermería por hospitalización
        List<TurnoEnfermeriaVM> GetTurnosByHospitalizacionId(int hospitalizacionId);

        // Método para agregar un nuevo turno de enfermería
        void AddTurnoEnfermeria(TurnoEnfermeria entity);

        // Método para marcar un turno como firmado
        void MarkTurnoAsFirmado(int turnoId);
    }
}
