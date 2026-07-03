using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Models;
using farmamest.Service.IService;
using System.Collections.Generic;
using System.Linq;

namespace farmamest.Service
{
    public class HospitalizacionProductoObservacionService : IHospitalizacionProductoObservacionService
    {
        private readonly IHospitalizacionProductoObservacion _repository;

        public HospitalizacionProductoObservacionService(IHospitalizacionProductoObservacion repository)
        {
            _repository = repository;
        }

        // Añadir una nueva observación
        public void Add(HospitalizacionProductoObservacion entity)
        {
            _repository.Add(entity);
        }

        // Modificar una observación existente
        public void Update(HospitalizacionProductoObservacion entity)
        {
            _repository.Update(entity);
        }

        // Eliminar (lógicamente) una observación
        public void Delete(int id)
        {
            _repository.Delete(id);
        }

        // Obtener observaciones por el ID de HospitalizacionProductoAplicacion
        public List<HospitalizacionProductoObservacionVM> GetByHospitalizacionProductoAplicacionId(int hospitalizacionProductoAplicacionId)
        {
            var data = _repository.GetByHospitalizacionProductoAplicacionId(hospitalizacionProductoAplicacionId);

            // Transformar los datos al ViewModel
            return data.Select(o => new HospitalizacionProductoObservacionVM
            {
                Id = o.Id,
                Observacion = o.Observacion,
                FechaCreacion = o.FechaCreacion.ToString("yyyy-MM-dd HH:mm:ss"),
                UsuarioCreaId = o.UsuarioCreaId
            }).ToList();
        }
    }
}
