using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Service.IService;
using System;

namespace farmamest.Service
{
    public class RegistroAnestesiaService : IRegistroAnestesiaService
    {
        private readonly IRegistroAnestesia _repository;

        public RegistroAnestesiaService(IRegistroAnestesia repository)
        {
            _repository = repository;
        }

        public RegistroAnestesia GetByHospitalizacionId(int hospitalizacionId)
        {
            return _repository.GetByHospitalizacionId(hospitalizacionId);
        }

        public RegistroAnestesia Guardar(int hospitalizacionId, string userId, string datosJson)
        {
            var existente = _repository.GetByHospitalizacionId(hospitalizacionId);
            if (existente != null)
            {
                existente.DatosJson = datosJson;
                existente.FechaActualizacion = DateTime.Now;
                existente.UserId = userId ?? existente.UserId;
                _repository.Update(existente);
                return existente;
            }

            var nuevo = new RegistroAnestesia
            {
                HospitalizacionId = hospitalizacionId,
                FechaRegistro = DateTime.Now,
                FechaActualizacion = DateTime.Now,
                UserId = userId,
                DatosJson = datosJson
            };
            _repository.Add(nuevo);
            return nuevo;
        }
    }
}
