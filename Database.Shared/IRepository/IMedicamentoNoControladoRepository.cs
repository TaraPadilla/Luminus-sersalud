using System.Collections.Generic;
using Database.Shared.Models;

namespace Database.Shared.IRepository
{
    public interface IMedicamentoNoControladoRepository
    {
        void Add(MedicamentoNoControlado registro);
        List<MedicamentoNoControlado> GetHistorialByHospitalizacion(int hospitalizacionId);

        IEnumerable<MedicamentoNoControlado> GetByHospitalizacionId(int hospitalizacionId);

    }
}