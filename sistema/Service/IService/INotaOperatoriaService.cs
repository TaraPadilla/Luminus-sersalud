using Database.Shared.Models;
using farmamest.Models;
using System.Collections.Generic;

namespace farmamest.Service.IService
{
    public interface INotaOperatoriaService
    {
        public void Add(NotaOperatoria entity);
        public List<NotaOperatoriaVM> GetByHospitalizacionId(int hospitalizacionId);

        void GuardarFirma(int notaId, string firmaRuta);



        public void AddNotaOperatoria(NotaOperatoria entity);
        public List<NotaOperatoria> GetNotaOperatoriaListByHospitalizacionId(int hospitalizacionId);

        void GuardarFirmaNotaOperatoria(int notaId, string firmaRuta);
        void ActualizarNotaOperatoria(NotaOperatoria entity);


    }
}