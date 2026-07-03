using Database.Shared.Models;
using System.Collections.Generic;

namespace Database.Shared.IRepository
{
    public interface IHospitalizacionExamenPdf
    {
        void Add(HospitalizacionExamenPdf entity);
        void Update(HospitalizacionExamenPdf entity);
        void Delete(int id);
        HospitalizacionExamenPdf GetById(int id);
        List<HospitalizacionExamenPdf> GetAll();
        List<HospitalizacionExamenPdf> GetByHospitalizacionAndExamen(int hospitalizacionId, int examenId);
        List<HospitalizacionExamenPdf> GetByHospitalizacionId(int hospitalizacionId);
        void MarkAsDeleted(int id);
    }
}
