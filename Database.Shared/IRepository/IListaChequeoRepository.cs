using Database.Shared.Models;
using System.Collections.Generic;

namespace Database.Shared.IRepository
{
    public interface IListaChequeo
    {
        void Add(ListaChequeo listaChequeo);
        IEnumerable<ListaChequeo> GetByHospitalizacionId(int hospitalizacionId);
        void Actualizar(ListaChequeo listaChequeo);
    }
}