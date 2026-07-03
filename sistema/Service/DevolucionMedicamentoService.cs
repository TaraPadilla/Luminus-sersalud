using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Service.IService;
using System;
using System.Collections.Generic;

namespace farmamest.Service
{
    public class DevolucionMedicamentoService : IDevolucionMedicamentoService
    {
        private readonly IDevolucionMedicamentoRepository _devolucionRepository;

        public DevolucionMedicamentoService(IDevolucionMedicamentoRepository devolucionRepository)
        {
            _devolucionRepository = devolucionRepository;
        }

        // Agregar una nueva devolución de medicamento
        public void AddDevolucionMedicamento(DevolucionMedicamento devolucion)
        {
            if (devolucion == null)
                throw new ArgumentNullException(nameof(devolucion), "La devolución no puede ser nula.");

            _devolucionRepository.AddDevolucionMedicamento(devolucion);
        }

        // Obtener una devolución por ID
        public DevolucionMedicamento GetDevolucionMedicamentoById(int id)
        {
            return _devolucionRepository.GetDevolucionMedicamentoById(id);
        }

        // Obtener todas las devoluciones, con filtro opcional por hospitalización
        public List<DevolucionMedicamento> GetAllDevolucionesMedicamento(int? hospitalizacionId = null)
        {
            return _devolucionRepository.GetAllDevolucionesMedicamento(hospitalizacionId);
        }

        // Obtener devoluciones por hospitalización
        public List<DevolucionMedicamento> GetDevolucionesByHospitalizacionId(int hospitalizacionId)
        {
            return _devolucionRepository.GetDevolucionesByHospitalizacionId(hospitalizacionId);
        }

        // Actualizar una devolución de medicamento
        public void UpdateDevolucionMedicamento(DevolucionMedicamento devolucion)
        {
            if (devolucion == null)
                throw new ArgumentNullException(nameof(devolucion), "La devolución no puede ser nula.");

            _devolucionRepository.UpdateDevolucionMedicamento(devolucion);
        }

        // Eliminar una devolución de medicamento por ID
        public void DeleteDevolucionMedicamento(int id)
        {
            _devolucionRepository.DeleteDevolucionMedicamento(id);
        }
    }
}
