using System.Collections.Generic;
using farmamest.Models;

public class OrdenMedicaViewModel
{
    public string FechaHora { get; set; }
    public string Profesional { get; set; }
    public string Descripcion { get; set; }
    public string PacienteNombre { get; set; }
    public string PacienteEdad { get; set; }
    public string PacienteSexoText { get; set; }
    public string EmpleadoText { get; set; }
    public string ColegioEmpleado { get; set; }
    public string Realizada { get; set; }
    public bool Autorizado { get; set; }
    public string FirmaBase64 { get; set; }
    public string NombreFirmante { get; set; }
    public string AutorizadoPor { get; set; }
}

public class OrdenesMedicasListaPdfViewModel
{
    public string PacienteNombre { get; set; }
    public string PacienteEdad { get; set; }
    public string PacienteSexoText { get; set; }
    public string EmpleadoText { get; set; }
    public string ColegioEmpleado { get; set; }
    public List<OrdenMedicaViewModel> Ordenes { get; set; } = new();
}

public class DocumentosCargadosPdfViewModel
{
    public string PacienteNombre { get; set; }
    public int HospitalizacionId { get; set; }
    public List<DocumentoEmbebidoVm> DocumentosEmbebidos { get; set; } = new();
    public DocumentoEmbebidoVm DocumentoConsentimientoEmbebido { get; set; }
}
