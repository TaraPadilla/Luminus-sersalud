using Database.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.IRepository
{
    public interface IDetallePaqueteHospitalizacion
    {
        public List<DetallePaqueteHospitalizacion> GetByIdPaqueteHospitalizacion(int id);
        public void Update(DetallePaqueteHospitalizacion entity);
        public DetallePaqueteHospitalizacion GetById(int Id);
    }
}
