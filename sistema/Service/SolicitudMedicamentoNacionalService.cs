using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Service.IService;
using System;
using System.Collections.Generic;

namespace farmamest.Service
{
    public class SolicitudMedicamentoNacionalService : ISolicitudMedicamentoNacionalService
    {
        private readonly ISolicitudMedicamentoNacionalRepository _solicitudMedicamentoNacionalRepository;

        public SolicitudMedicamentoNacionalService(ISolicitudMedicamentoNacionalRepository solicitudMedicamentoNacionalRepository)
        {
            _solicitudMedicamentoNacionalRepository = solicitudMedicamentoNacionalRepository;
        }

        // ✅ 1. Agregar una nueva solicitud de medicamento
        public void AddSolicitudMedicamento(SolicitudMedicamentoNacional solicitud)
        {
            if (solicitud == null)
                throw new ArgumentNullException(nameof(solicitud), "La solicitud de medicamento no puede ser nula.");

            _solicitudMedicamentoNacionalRepository.AddSolicitudMedicamento(solicitud);
        }

        // ✅ 2. Obtener una solicitud de medicamento por ID
        public SolicitudMedicamentoNacional GetSolicitudMedicamentoById(int id)
        {
            return _solicitudMedicamentoNacionalRepository.GetSolicitudMedicamentoById(id);
        }

        // ✅ 3. Obtener todas las solicitudes de medicamento
        public List<SolicitudMedicamentoNacional> GetAllSolicitudesMedicamento(int? hospitalizacionId = null)
        {
            return _solicitudMedicamentoNacionalRepository.GetAllSolicitudesMedicamento(hospitalizacionId);
        }

        // ✅ 4. Obtener solicitudes por ID de hospitalización
        public List<SolicitudMedicamentoNacional> GetSolicitudesByHospitalizacionId(int hospitalizacionId)
        {
            return _solicitudMedicamentoNacionalRepository.GetSolicitudesByHospitalizacionId(hospitalizacionId);
        }

        // ✅ 5. Actualizar una solicitud de medicamento
        public void UpdateSolicitudMedicamento(SolicitudMedicamentoNacional solicitud)
        {
            if (solicitud == null)
                throw new ArgumentNullException(nameof(solicitud), "La solicitud de medicamento no puede ser nula.");

            _solicitudMedicamentoNacionalRepository.UpdateSolicitudMedicamento(solicitud);
        }

        // ✅ 6. Eliminar una solicitud de medicamento
        public void DeleteSolicitudMedicamento(int id)
        {
            _solicitudMedicamentoNacionalRepository.DeleteSolicitudMedicamento(id);
        }
        public List<CategoriaHabitacion> GetAllCategoriasHabitacion()
        {
            return _solicitudMedicamentoNacionalRepository.GetAllCategoriasHabitacion();
        }
    }
}
