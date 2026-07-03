using System.Collections.Generic;
using farmamest.Models;

namespace sistema.Models
{
    public class InformeGlucometriaPdfRow
    {
        public string FechaHora { get; set; }
        public string GMT { get; set; }
        public string InsulinaNombre { get; set; }
        public string Unidades { get; set; }
        public string FirmaTexto { get; set; }
        public string FirmaBase64 { get; set; }
        public bool Aplicado { get; set; }
        public string FechaHoraAplicacion { get; set; }
        public string NombrePersonaAplica { get; set; }
        public string NombreProfesional { get; set; }
        public bool Autorizado { get; set; }
        public string AutorizadoPor { get; set; }
    }

    public class InformeAplicacionPdfRow
    {
        public string TipoElemento { get; set; }
        public string Nombre { get; set; }
        public string Indicaciones { get; set; }
        public decimal Cantidad { get; set; }
        public string UnidadMedidaVentaNombre { get; set; }
        public string OrdenMedicaNumero { get; set; }
        public string PersonaCrea { get; set; }
        public bool Aplicado { get; set; }
        public string FechaHoraAplicacion { get; set; }
        public string PersonaAplica { get; set; }
    }

    public class InformeNotaSimplePdfRow
    {
        public string FechaRegistro { get; set; }
        public string Profesional { get; set; }
        public string Descripcion { get; set; }
        public bool Firmado { get; set; }
        public string FirmadoPor { get; set; }
        public string FirmaBase64 { get; set; }
    }

    public class InformeExamenPdfRow
    {
        public string FechaHora { get; set; }
        public string Nombre { get; set; }
    }

    public class HospitalizacionInformePdfSectionsVm
    {
        public List<SignosVitalesHospPdfRow> SignosVitales { get; set; } = new();
        public List<InformeGlucometriaPdfRow> Glucometria { get; set; } = new();
        public List<IngestaExcretaViewModel> IngestasExcretas { get; set; } = new();
        public List<InformeNotaSimplePdfRow> NotasEvolucion { get; set; } = new();
        public List<InformeAplicacionPdfRow> Aplicaciones { get; set; } = new();
        public List<InformeNotaSimplePdfRow> NotasEnfermeria { get; set; } = new();
        public List<InformeExamenPdfRow> Examenes { get; set; } = new();
    }
}
