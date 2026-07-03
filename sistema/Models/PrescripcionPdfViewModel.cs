using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;
using System;

namespace sistema.Models
{
    public class PrescripcionPdfViewModel
    {
        public string EstablecimientoCorreoElectronico { get; set; }
        public string EstablecimientoTelefono { get; set; }
        public string EstablecimientoDireccion { get; set; }
        public string ImagenLogoBase64 { get; set; }
        public string PacienteNombre { get; set; }
        public string PacienteEdad { get; set; } // Ya existente
        public int? PacienteSexo { get; set; }
        public string MedicoNombre { get; set; }  // Esta es la nueva propiedad para el nombre del médico

        public string FirmaEmpleado { get; set; }  // Nueva propiedad para la firma
        public string FirmaEmpleadoUrl { get; set; } // src listo para <img> (data-uri o url absoluta)


        public string PacientePeso { get; set; } // Nueva propiedad para el peso
        public string Color { get; set; }

        public List<ConsultaPrescripcionViewModel> DetallesPrescripcion { get; set; }
        public string MedicoCelular { get; internal set; }
        public DateTime? FechaProximaConsulta { get; set; }

        public List<MedicamentoOtro> MedicamentosOtros { get; set; } = new();

    }

}