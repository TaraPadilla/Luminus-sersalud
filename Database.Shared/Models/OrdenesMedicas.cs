using System;
using System.Collections.Generic;
using Database.Shared.Models;

public class OrdenesMedicas
{
    public int Id { get; set; }
    public DateTime FechaHora { get; set; }
    public string Profesional { get; set; }
    public string Descripcion { get; set; }
    public int HospitalizacionId { get; set; }
    public Hospitalizacion Hospitalizacion { get; set; }
    public bool Realizada { get; set; }

    public DateTime FechaRealizacion { get; set; }
    public string ProfesionalRealiza { get; set; }
    public List<string> Examenes { get; set; } = new List<string>();
    public List<string> Observaciones { get; set; } = new List<string>();
    public List<string> Dietas { get; set; }

    public bool Autorizado { get; set; }
    public string? UsuarioAutoriza { get; set; }
    public DateTime? FechaAutorizacion { get; set; }

}
