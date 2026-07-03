using System.Collections.Generic;
using farmamest.Models;

public class NotaMedica2ListaViewModel
{
    public string PacienteNombre { get; set; }
    public string PacienteEdad { get; set; }
    public string PacienteSexoText { get; set; }
    public string EmpleadoText { get; set; }
    public string ColegioEmpleado { get; set; }
    public string EmpleadoEspecialidad { get; set; }
    public List<NotaMedica2ViewModel> NotasMedicas { get; set; }

}