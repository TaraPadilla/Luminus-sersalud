using Database.Shared.Models;
using System.Collections.Generic;

namespace Database.Shared.IRepository
{
    public interface ITurnoEnfermeria
    {
        void AddTurnoEnfermeria(TurnoEnfermeria entity);
        List<TurnoEnfermeria> GetTurnoEnfermeriaList();
        List<TurnoEnfermeria> GetTurnosByHospitalizacionId(int hospitalizacionId);
        void MarkTurnoAsFirmado(int turnoId);
    }
}
