using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Service.IService;
using System;
using System.Collections.Generic;

namespace farmamest.Service
{
    public class SolicitudMedicamentoService : ISolicitudMedicamentoService
    {
        private readonly ISolicitudMedicamentoRepository _solicitudMedicamentoRepository;

        public SolicitudMedicamentoService(ISolicitudMedicamentoRepository solicitudMedicamentoRepository)
        {
            _solicitudMedicamentoRepository = solicitudMedicamentoRepository;
        }

        // ✅ 1. Agregar una nueva solicitud de medicamento
        public void AddSolicitudMedicamento(SolicitudMedicamento solicitud)
        {
            if (solicitud == null)
                throw new ArgumentNullException(nameof(solicitud), "La solicitud de medicamento no puede ser nula.");

            _solicitudMedicamentoRepository.AddSolicitudMedicamento(solicitud);
        }

        // ✅ 2. Obtener una solicitud de medicamento por ID
        public SolicitudMedicamento GetSolicitudMedicamentoById(int id)
        {
            return _solicitudMedicamentoRepository.GetSolicitudMedicamentoById(id);
        }

        // ✅ 3. Obtener todas las solicitudes de medicamento
        public List<SolicitudMedicamento> GetAllSolicitudesMedicamento(int? hospitalizacionId = null)
        {
            return _solicitudMedicamentoRepository.GetAllSolicitudesMedicamento(hospitalizacionId);
        }

        // ✅ 4. Obtener solicitudes por ID de hospitalización
        public List<SolicitudMedicamento> GetSolicitudesByHospitalizacionId(int hospitalizacionId)
        {
            return _solicitudMedicamentoRepository.GetSolicitudesByHospitalizacionId(hospitalizacionId);
        }

        // ✅ 5. Actualizar una solicitud de medicamento
        public void UpdateSolicitudMedicamento(SolicitudMedicamento solicitud)
        {
            if (solicitud == null)
                throw new ArgumentNullException(nameof(solicitud), "La solicitud de medicamento no puede ser nula.");

            _solicitudMedicamentoRepository.UpdateSolicitudMedicamento(solicitud);
        }

        // ✅ 6. Eliminar una solicitud de medicamento
        public void DeleteSolicitudMedicamento(int id)
        {
            _solicitudMedicamentoRepository.DeleteSolicitudMedicamento(id);
        }
        public List<CategoriaHabitacion> GetAllCategoriasHabitacion()
        {
            return _solicitudMedicamentoRepository.GetAllCategoriasHabitacion();
        }
    }
}
