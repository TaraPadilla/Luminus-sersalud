using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Service.IService;
using System.Collections.Generic;

namespace farmamest.Service
{
    public class HospitalizacionExamenPdfService : IHospitalizacionExamenPdfService
    {
        private readonly IHospitalizacionExamenPdf _repository;

        public HospitalizacionExamenPdfService(IHospitalizacionExamenPdf repository)
        {
            _repository = repository;
        }

        public void Add(HospitalizacionExamenPdf entity)
        {
            _repository.Add(entity);
        }

        public void Update(HospitalizacionExamenPdf entity)
        {
            _repository.Update(entity);
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }

        public HospitalizacionExamenPdf GetById(int id)
        {
            return _repository.GetById(id);
        }

        public List<HospitalizacionExamenPdf> GetAll()
        {
            return _repository.GetAll();
        }

        public List<HospitalizacionExamenPdf> GetByHospitalizacionAndExamen(int hospitalizacionId, int examenId)
        {
            return _repository.GetByHospitalizacionAndExamen(hospitalizacionId, examenId);
        }

        public List<HospitalizacionExamenPdf> GetByHospitalizacionId(int hospitalizacionId)
        {
            return _repository.GetByHospitalizacionId(hospitalizacionId);
        }

        public void MarkAsDeleted(int id)
        {
            _repository.MarkAsDeleted(id);
        }
    }
}
