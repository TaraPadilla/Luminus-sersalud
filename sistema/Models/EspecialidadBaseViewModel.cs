using Database.Shared.Models;
using Microsoft.AspNetCore.Http;

namespace sistema.Models
{
    public class EspecialidadBaseViewModel
    {
        public Especialidad Especialidad { get; set; } = new Especialidad();
        public IFormFile Imagen { get; set; }

        public bool Modificar { get; set; }
        public int Id
        {
            get { return Especialidad.Id; }
            set { Especialidad.Id = value; }
        }
    }
}
