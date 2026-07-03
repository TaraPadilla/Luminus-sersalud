using Database.Shared.Data;
using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace sistema.Models
{
    public class HospitalizacionHospitalizarViewModel
    {
        public Consulta Consulta { get; set; }

        public int HabitacionId { get; set; }
        public string HabitacionNumeroNombre { get; set; }
        public int HabitacionCategoriaId { get; set; }
        public string HabitacionCategoria { get; set; }
        public int TarifaId { get; set; }
        public decimal TarifaValor { get; set; }
        public int HabitacionEstadoId { get; set; }
        public string HabitacionEstado { get; set; }
        public string CodigoSeguro { get; set; }
        public int? PacienteId { get; set; }
        public string PacienteNombre { get; set; }
        public string PacienteDpi { get; set; }
        public string PacienteTelefono { get; set; }
        public DateTime? PacienteFechaNacimiento { get; set; }
        public SelectList PacientesList { get; set; }
        public string Periodo { get; set; }
        public int? EspecialidadId { get; set; }
        public string EspecialidadNombre { get; set; }
        public string Observaciones { get; set; }
        public int? ConsultaId { get; set; }
        public int? EmergenciaId { get; set; }
        //CONSENTIMIENTO
        public string UrlArchivoConsentimiento { get; set; }
        public string Base64FirmaConsentimiento { get; set; }
        public int? CitaId { get; set; }

        public void Init(IPacientes pacientesRepository)
        {
            PacientesList = new SelectList(pacientesRepository.GetList(), "Id", "Nombre");
        }
    }
}
