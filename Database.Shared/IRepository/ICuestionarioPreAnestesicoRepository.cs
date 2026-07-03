using Database.Shared.Models;
using System.Collections.Generic;

namespace Database.Shared.IRepository
{
    public interface ICuestionarioPreAnestesico
    {
        void Add(CuestionarioPreAnestesico cuestionario);
        IEnumerable<CuestionarioPreAnestesico> GetByHospitalizacionId(int hospitalizacionId);
        void Actualizar(CuestionarioPreAnestesico cuestionario);
    }
}