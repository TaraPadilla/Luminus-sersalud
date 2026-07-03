using Database.Shared.IRepository;
using Database.Shared.Models;
using System.Collections.Generic;

namespace farmamest.Service
{
    public class ListaChequeoService : IService.IListaChequeoService
    {
        private readonly IListaChequeo _listaChequeoRepository;

        public ListaChequeoService(IListaChequeo listaChequeoRepository)
        {
            _listaChequeoRepository = listaChequeoRepository;
        }

        public void Add(ListaChequeo listaChequeo)
        {
            _listaChequeoRepository.Add(listaChequeo);
        }

        public IEnumerable<ListaChequeo> GetByHospitalizacionId(int hospitalizacionId)
        {
            return _listaChequeoRepository.GetByHospitalizacionId(hospitalizacionId);
        }

        public void Actualizar(ListaChequeo listaChequeo)
        {
            _listaChequeoRepository.Actualizar(listaChequeo);
        }
    }
}