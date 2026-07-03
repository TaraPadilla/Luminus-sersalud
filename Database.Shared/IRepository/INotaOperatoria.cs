using Database.Shared.Models;
using System.Collections.Generic;

namespace Database.Shared.IRepository
{
    public interface INotaOperatoria
    {
        public void AddNotaOperatoria(NotaOperatoria entity);
        public List<NotaOperatoria> GetNotaOperatoriaListByHospitalizacionId(int hospitalizacionId);

        void GuardarFirmaNotaOperatoria(int notaId, string firmaRuta);

        void ActualizarNotaOperatoria(NotaOperatoria entity);

    }
}