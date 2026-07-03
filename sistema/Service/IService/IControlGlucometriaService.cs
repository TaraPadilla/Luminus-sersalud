using Database.Shared.Models;
using farmamest.Models;
using System.Collections.Generic;

namespace farmamest.Service.IService
{
    public interface IControlGlucometria2Service
    {

        public void Add(ControlGlucometria2 entity);
        public List<DetalleControlGlucometria2ViewModel> GetByHospitalizacionId(int hospitalizacionId);
        public void AplicacionDetalleControlGlucometria2ById(int id, string personaAplica);

        ControlGlucometria2 GetById(int id);
        void Update(ControlGlucometria2 entity);
    }
}
