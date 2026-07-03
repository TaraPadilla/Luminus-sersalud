using System.Collections.Generic;             
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
namespace sistema.Models
{
    public class ReportesIneViewModel
    {
        public int? SelectedDepartamentoId { get; set; }
        public string SelectedDepartamentoCodigo { get; set; }
        public IEnumerable<SelectListItem> Departamentos { get; set; }

        public int? SelectedMunicipioId { get; set; }
        public string SelectedMunicipioCodigo { get; set; }
        public IEnumerable<SelectListItem> Municipios { get; set; }

        public string MesNombre { get; set; }
        public int Año { get; set; }
        // …las demás propiedades que necesites
          public string RangoFechas { get; set; }
    public List<ConsultaIneViewModel> Registros { get; set; } = new();
    }
    public class ConsultaIneViewModel
    {
        public int NoOrden { get; set; }
        public string TieneIgss { get; set; }
        public DateTime FechaConsulta { get; set; }
        public string HistoriaClinica { get; set; }
        public string Sexo { get; set; }
        public string Etnia { get; set; }
        public int DiasEdad { get; set; }
        public int MesesEdad { get; set; }
        public int AniosEdad { get; set; }
        public string Direccion { get; set; }
        public string Ubicacion { get; set; }
        public string CodigoCartografico { get; set; }
        public string TipoConsulta { get; set; }
        public string ControlMaternoInfantil { get; set; }
        public string Observaciones { get; set; }
        public string ImpresionClinica { get; set; }
        public string cie { get; set; }
      public string TratamientoRecibidoEn { get; set; }
}
    
}