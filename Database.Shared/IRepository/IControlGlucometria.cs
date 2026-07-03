using Database.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.IRepository
{
    public interface IControlGlucometria2
    {
        public void Add(ControlGlucometria2 entity);
        public List<DetalleControlGlucometria2> GetDetalleControlGlucometria2ByHospitalizacionId(int hospitalizacionId);
        public ControlGlucometria2 GetById(int id);
        public void Update(ControlGlucometria2 entity);
        public DetalleControlGlucometria2 GetDetalleControlGlucometria2ById(int id);
        public void Update(DetalleControlGlucometria2 entity);

    }
}
