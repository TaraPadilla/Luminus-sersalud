using Database.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.IRepository
{
    public interface IHospitalizacionDetallePaqueteHospitalizacion
    {
        public void Update(HospitalizacionDetallePaqueteHospitalizacion entity);
        public HospitalizacionDetallePaqueteHospitalizacion GetById(int id);
            IEnumerable<HospitalizacionDetallePaqueteHospitalizacion> GetAll();

    }
}
