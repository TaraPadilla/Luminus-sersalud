using System.Collections.Generic;

namespace TuProyecto.Models.ViewModels
{
    public class OrganizacionVM
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public List<UnidadVM> Unidades { get; set; } = new List<UnidadVM>();
    }

    public class UnidadVM
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public List<SeccionVM> Secciones { get; set; } = new List<SeccionVM>();
    }

    public class SeccionVM
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}