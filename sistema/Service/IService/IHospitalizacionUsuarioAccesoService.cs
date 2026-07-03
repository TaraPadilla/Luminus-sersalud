using Database.Shared.Models;
using farmamest.Models;
using System.Collections.Generic;

namespace farmamest.Service.IService
{
    public interface IHospitalizacionUsuarioAccesoService
    {
        public void Add(HospitalizacionUsuarioAcceso entity);
        public List<HospitalizacionUsuarioAccesoViewModels> GetAllByHospitalizacionId(int hospitalizacionId);
        public void Delete(int id);
        //public bool ValidarVisualizacionHospitalizacionUsuarioAcceso(int hospitalizacionId, string usuarioId);
        void UpdateUsuarioAcceso(HospitalizacionUsuarioAccesoViewModels model);
    }
}
