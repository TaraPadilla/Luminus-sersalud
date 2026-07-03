using Database.Shared.Models;
using Database.Shared.IRepository;
using farmamest.Models;
using farmamest.Service.IService;
using System.Collections.Generic;
using static farmamest.Models.ConsentimientoHospiVM;
using System.Linq;

namespace farmamest.Service
{
    public class ConsentimientoHospiService : IConsentimientoHospiService
    {
        private readonly IConsentimientoHospi _consentimientoHospiRepository;

        public ConsentimientoHospiService(IConsentimientoHospi consentimientoHospiRepository)
        {
            _consentimientoHospiRepository = consentimientoHospiRepository;
        }

        public void AddConsentimiento(ConsentimientoHospi consentimiento)
        {
            _consentimientoHospiRepository.AddConsentimiento(consentimiento);
        }

        public ConsentimientoHospiVM GetConsentimientoByPacienteAndHabitacion(int pacienteId, int habitacionId)
        {
            var consentimiento = _consentimientoHospiRepository.GetConsentimientoByPacienteAndHabitacion(pacienteId, habitacionId);

            if (consentimiento == null) return null;

            var resultado = new ConsentimientoHospiVM
            {
                Id = consentimiento.Id,
                PacienteId = consentimiento.PacienteId,
                HabitacionId = consentimiento.HabitacionId,
                HospitalizacionId = consentimiento.HospitalizacionId,
                HoraIngreso = consentimiento.HoraIngreso,
                NumeroPaciente = consentimiento.NumeroPaciente,
                NumeroHabitacion = consentimiento.NumeroHabitacion,
                NombreCompleto = consentimiento.NombreCompleto,
                NombrePaciente = consentimiento.NombreCompleto,
                EstadoCivil = consentimiento.EstadoCivil,
                DPI = consentimiento.DPI,
                FechaNacimiento = consentimiento.FechaNacimiento,
                Edad = consentimiento.Edad,
                Nacionalidad = consentimiento.Nacionalidad,
                Direccion = consentimiento.Direccion,
                Celular = consentimiento.Celular,
                Email = consentimiento.Email,
                TipoSangre = consentimiento.TipoSangre,
                Genero = consentimiento.Genero,
                Religion = consentimiento.Religion,
                Ocupacion = consentimiento.Ocupacion,
                PoseeSeguroMedico = consentimiento.PoseeSeguroMedico,
                Aseguradora = consentimiento.Aseguradora,
                TipoPoliza = consentimiento.TipoPoliza,
                NombreEmpresa = consentimiento.NombreEmpresa,
                FormularioPreAutorizacion = consentimiento.FormularioPreAutorizacion,
                TratamientoMedico = consentimiento.TratamientoMedico,
                NombreResponsable = consentimiento.NombreResponsable,
                DPIResponsable = consentimiento.DPIResponsable,
                EdadResponsable = consentimiento.EdadResponsable,
                DireccionResponsable = consentimiento.DireccionResponsable,
                CelularResponsable = consentimiento.CelularResponsable,
                EmailResponsable = consentimiento.EmailResponsable,
                NITResponsable = consentimiento.NITResponsable,
                NombreFacturacion = consentimiento.NombreFacturacion,
                ContactosEmergencia = consentimiento.ContactosEmergencia?.Select(c => new ContactoEmergenciaVM
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Telefono = c.Telefono,
                    Parentesco = c.Parentesco
                }).ToList() ?? new List<ContactoEmergenciaVM>(),
                HospitalProporcionoMedico = consentimiento.HospitalProporcionoMedico,
                MedicoAfiliado = consentimiento.MedicoAfiliado,
                NombreMedicoTratante = consentimiento.NombreMedicoTratante,
                RecetaMedica = consentimiento.RecetaMedica,
                URLFirmaPaciente = consentimiento.URLFirmaPaciente,
                URLFirmaResponsable = consentimiento.URLFirmaResponsable,
                NombreNotaria = consentimiento.NombreNotaria,
                NombreRepresentanteNarajo = consentimiento.NombreRepresentanteNarajo,
                URLFirmaNotaria = consentimiento.URLFirmaNotaria,
                URLFirmaRepresentanteNaranjo = consentimiento.URLFirmaRepresentanteNaranjo,
                NacionalidadResponsable = consentimiento.NacionalidadResponsable,
                OcupacionResponsable = consentimiento.OcupacionResponsable,
                CitaId = consentimiento.CitaId,
                ConsultaId = consentimiento.ConsultaId
            };

            return resultado;
        }

        public ConsentimientoHospiVM GetConsentimientoByPacienteHabitacionAndHospitalizacion(int pacienteId, int habitacionId, string hospitalizacionId)
        {
            var consentimiento = _consentimientoHospiRepository.GetConsentimientoByPacienteHabitacionAndHospitalizacion(pacienteId, habitacionId, hospitalizacionId);

            if (consentimiento == null) return null;

            var resultado = new ConsentimientoHospiVM
            {
                Id = consentimiento.Id,
                PacienteId = consentimiento.PacienteId,
                HabitacionId = consentimiento.HabitacionId,
                HospitalizacionId = consentimiento.HospitalizacionId,
                HoraIngreso = consentimiento.HoraIngreso,
                NumeroPaciente = consentimiento.NumeroPaciente,
                NumeroHabitacion = consentimiento.NumeroHabitacion,
                NombreCompleto = consentimiento.NombreCompleto,
                NombrePaciente = consentimiento.NombreCompleto,
                EstadoCivil = consentimiento.EstadoCivil,
                DPI = consentimiento.DPI,
                FechaNacimiento = consentimiento.FechaNacimiento,
                Edad = consentimiento.Edad,
                Nacionalidad = consentimiento.Nacionalidad,
                Direccion = consentimiento.Direccion,
                Celular = consentimiento.Celular,
                Email = consentimiento.Email,
                TipoSangre = consentimiento.TipoSangre,
                Genero = consentimiento.Genero,
                Religion = consentimiento.Religion,
                Ocupacion = consentimiento.Ocupacion,
                PoseeSeguroMedico = consentimiento.PoseeSeguroMedico,
                Aseguradora = consentimiento.Aseguradora,
                TipoPoliza = consentimiento.TipoPoliza,
                NombreEmpresa = consentimiento.NombreEmpresa,
                FormularioPreAutorizacion = consentimiento.FormularioPreAutorizacion,
                TratamientoMedico = consentimiento.TratamientoMedico,
                NombreResponsable = consentimiento.NombreResponsable,
                DPIResponsable = consentimiento.DPIResponsable,
                EdadResponsable = consentimiento.EdadResponsable,
                DireccionResponsable = consentimiento.DireccionResponsable,
                CelularResponsable = consentimiento.CelularResponsable,
                EmailResponsable = consentimiento.EmailResponsable,
                NITResponsable = consentimiento.NITResponsable,
                NombreFacturacion = consentimiento.NombreFacturacion,
                ContactosEmergencia = consentimiento.ContactosEmergencia?.Select(c => new ContactoEmergenciaVM
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Telefono = c.Telefono,
                    Parentesco = c.Parentesco
                }).ToList() ?? new List<ContactoEmergenciaVM>(),
                HospitalProporcionoMedico = consentimiento.HospitalProporcionoMedico,
                MedicoAfiliado = consentimiento.MedicoAfiliado,
                NombreMedicoTratante = consentimiento.NombreMedicoTratante,
                RecetaMedica = consentimiento.RecetaMedica,
                URLFirmaPaciente = consentimiento.URLFirmaPaciente,
                URLFirmaResponsable = consentimiento.URLFirmaResponsable,
                NombreNotaria = consentimiento.NombreNotaria,
                NombreRepresentanteNarajo = consentimiento.NombreRepresentanteNarajo,
                URLFirmaNotaria = consentimiento.URLFirmaNotaria,
                URLFirmaRepresentanteNaranjo = consentimiento.URLFirmaRepresentanteNaranjo,
                NacionalidadResponsable = consentimiento.NacionalidadResponsable,
                OcupacionResponsable = consentimiento.OcupacionResponsable,
                CitaId = consentimiento.CitaId,
                ConsultaId = consentimiento.ConsultaId
            };

            return resultado;
        }

        public bool UpdateHospitalizacionId(int pacienteId, int habitacionId, string newHospitalizacionId)
        {
            return _consentimientoHospiRepository.UpdateHospitalizacionId(pacienteId, habitacionId, newHospitalizacionId);
        }

        public void UpdateConsentimiento(ConsentimientoHospi consentimiento)
        {
            _consentimientoHospiRepository.UpdateConsentimiento(consentimiento);
        }

        public void UpdateFirmas(int pacienteId, int habitacionId, string urlFirmaPaciente, string urlFirmaResponsable)
        {
            _consentimientoHospiRepository.UpdateFirmas(pacienteId, habitacionId, urlFirmaPaciente, urlFirmaResponsable);
        }

        public void UpsertConsentimiento(ConsentimientoHospi consentimiento)
        {
            _consentimientoHospiRepository.UpsertConsentimiento(consentimiento);
        }

        public ConsentimientoHospiVM GetConsentimientoByPacienteAndHospitalizacion(int pacienteId, string hospitalizacionId)
        {
            var consentimiento = _consentimientoHospiRepository.GetConsentimientoByPacienteAndHospitalizacion(pacienteId, hospitalizacionId);
            return consentimiento == null ? null : MapToVm(consentimiento);
        }

        public ConsentimientoHospiVM GetLatestConsentimientoByPaciente(int pacienteId)
        {
            var consentimiento = _consentimientoHospiRepository.GetLatestConsentimientoByPaciente(pacienteId);
            return consentimiento == null ? null : MapToVm(consentimiento);
        }

        private static ConsentimientoHospiVM MapToVm(ConsentimientoHospi consentimiento)
        {
            return new ConsentimientoHospiVM
            {
                Id = consentimiento.Id,
                PacienteId = consentimiento.PacienteId,
                HabitacionId = consentimiento.HabitacionId,
                HospitalizacionId = consentimiento.HospitalizacionId,
                HoraIngreso = consentimiento.HoraIngreso,
                NumeroPaciente = consentimiento.NumeroPaciente,
                NumeroHabitacion = consentimiento.NumeroHabitacion,
                NombreCompleto = consentimiento.NombreCompleto,
                NombrePaciente = consentimiento.NombreCompleto,
                EstadoCivil = consentimiento.EstadoCivil,
                DPI = consentimiento.DPI,
                FechaNacimiento = consentimiento.FechaNacimiento,
                Edad = consentimiento.Edad,
                Nacionalidad = consentimiento.Nacionalidad,
                Direccion = consentimiento.Direccion,
                Celular = consentimiento.Celular,
                Email = consentimiento.Email,
                TipoSangre = consentimiento.TipoSangre,
                Genero = consentimiento.Genero,
                Religion = consentimiento.Religion,
                Ocupacion = consentimiento.Ocupacion,
                PoseeSeguroMedico = consentimiento.PoseeSeguroMedico,
                Aseguradora = consentimiento.Aseguradora,
                TipoPoliza = consentimiento.TipoPoliza,
                NombreEmpresa = consentimiento.NombreEmpresa,
                FormularioPreAutorizacion = consentimiento.FormularioPreAutorizacion,
                TratamientoMedico = consentimiento.TratamientoMedico,
                NombreResponsable = consentimiento.NombreResponsable,
                DPIResponsable = consentimiento.DPIResponsable,
                EdadResponsable = consentimiento.EdadResponsable,
                DireccionResponsable = consentimiento.DireccionResponsable,
                CelularResponsable = consentimiento.CelularResponsable,
                EmailResponsable = consentimiento.EmailResponsable,
                NITResponsable = consentimiento.NITResponsable,
                NombreFacturacion = consentimiento.NombreFacturacion,
                ContactosEmergencia = consentimiento.ContactosEmergencia?.Select(c => new ContactoEmergenciaVM
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Telefono = c.Telefono,
                    Parentesco = c.Parentesco
                }).ToList() ?? new List<ContactoEmergenciaVM>(),
                HospitalProporcionoMedico = consentimiento.HospitalProporcionoMedico,
                MedicoAfiliado = consentimiento.MedicoAfiliado,
                NombreMedicoTratante = consentimiento.NombreMedicoTratante,
                RecetaMedica = consentimiento.RecetaMedica,
                URLFirmaPaciente = consentimiento.URLFirmaPaciente,
                URLFirmaResponsable = consentimiento.URLFirmaResponsable,
                NombreNotaria = consentimiento.NombreNotaria,
                NombreRepresentanteNarajo = consentimiento.NombreRepresentanteNarajo,
                URLFirmaNotaria = consentimiento.URLFirmaNotaria,
                URLFirmaRepresentanteNaranjo = consentimiento.URLFirmaRepresentanteNaranjo,
                NacionalidadResponsable = consentimiento.NacionalidadResponsable,
                OcupacionResponsable = consentimiento.OcupacionResponsable,
                CitaId = consentimiento.CitaId,
                ConsultaId = consentimiento.ConsultaId
            };
        }
    }
}