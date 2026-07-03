using Database.Shared.Models;
using System.Collections.Generic;

namespace farmamest.Service.IService
{
    public interface ICuestionarioPreAnestesicoService
    {
        void Add(CuestionarioPreAnestesico cuestionario);
        IEnumerable<CuestionarioPreAnestesico> GetByHospitalizacionId(int hospitalizacionId);
        void Actualizar(CuestionarioPreAnestesico cuestionario);
    }
}