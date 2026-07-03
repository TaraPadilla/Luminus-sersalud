using Database.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.IRepository
{
    public interface IHospitalizacionUsuarioAcceso
    {
        public void Add(HospitalizacionUsuarioAcceso entity);
        public List<HospitalizacionUsuarioAcceso> GetHospitalizacionUsuarioAccesosByIdHospitalizacion(int hospitalizacionId);
        public void Delete(HospitalizacionUsuarioAcceso entity);
        public HospitalizacionUsuarioAcceso GetById(int id);
        void Update(HospitalizacionUsuarioAcceso usuarioAcceso);
    }
}
