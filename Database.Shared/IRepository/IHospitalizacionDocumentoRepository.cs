using Database.Shared.Models;
using System.Collections.Generic;

namespace Database.Shared.IRepository
{
    public interface IHospitalizacionDocumentoRepository
    {
        DocumentosHospitalizacion GetById(int id);
        IEnumerable<DocumentosHospitalizacion> GetByHospitalizacionId(int hospitalizacionId);
        IEnumerable<DocumentosHospitalizacion> GetByPacienteId(int pacienteId);
        void Add(DocumentosHospitalizacion documento);
        void Update(DocumentosHospitalizacion documento);
        void Delete(int id);
        void SaveChanges();
    }
}