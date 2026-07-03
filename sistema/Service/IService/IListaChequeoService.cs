using Database.Shared.Models;
using System.Collections.Generic;

namespace farmamest.Service.IService
{
    public interface IListaChequeoService
    {
        void Add(ListaChequeo listaChequeo);
        IEnumerable<ListaChequeo> GetByHospitalizacionId(int hospitalizacionId);
        void Actualizar(ListaChequeo listaChequeo);
    }
}