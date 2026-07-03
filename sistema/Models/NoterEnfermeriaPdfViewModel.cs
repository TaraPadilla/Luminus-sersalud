using System;
using System.Collections.Generic;   // ← esta línea es necesaria
using Microsoft.AspNetCore.Mvc;
using Database.Shared.IRepository;
// ... resto de using

public class NotaEnfermeriaPdfViewModel
{
    public int Id { get; set; }
    public string Diagnostico { get; set; }
    public string FechaRegistro { get; set; }
    public string Profesional { get; set; }
    public bool Firmado { get; set; }
    public string FirmaBase64 { get; set; }
    public string FechaFirma { get; set; }
    public string FirmadoPor { get; set; }
    public string PacienteNombre { get; set; }
    public int HospitalizacionId { get; set; }
    public string TipoNota { get; set; }

    // Datos del establecimiento
    public string EstablecimientoDireccion { get; set; }
    public string EstablecimientoTelefono { get; set; }
    public string EstablecimientoCorreo { get; set; }
    public string ImagenLogoBase64 { get; set; }
}

public class NotasEnfermeriaAllPdfViewModel
{
    public int HospitalizacionId { get; set; }
    public string PacienteNombre { get; set; }
    public string FechaGeneracion { get; set; }
    public List<NotaEnfermeriaPdfViewModel> Notas { get; set; }

    public string EstablecimientoDireccion { get; set; }
    public string EstablecimientoTelefono { get; set; }
    public string EstablecimientoCorreo { get; set; }
    public string ImagenLogoBase64 { get; set; }
}