using Database.Shared.IRepository;
using Database.Shared.Models;
using System.Collections.Generic;

namespace farmamest.Service
{
    public class CuestionarioPreAnestesicoService : IService.ICuestionarioPreAnestesicoService
    {
        private readonly ICuestionarioPreAnestesico _repository;

        public CuestionarioPreAnestesicoService(ICuestionarioPreAnestesico repository)
        {
            _repository = repository;
        }

        public void Add(CuestionarioPreAnestesico cuestionario)
        {
            _repository.Add(cuestionario);
        }

        public IEnumerable<CuestionarioPreAnestesico> GetByHospitalizacionId(int hospitalizacionId)
        {
            return _repository.GetByHospitalizacionId(hospitalizacionId);
        }

        public void Actualizar(CuestionarioPreAnestesico cuestionario)
        {
            _repository.Actualizar(cuestionario);
        }
    }
}