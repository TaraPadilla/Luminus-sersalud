using Database.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace sistema.Models
{
    public class CategoriaGeneralLabClinicoViewModel
    {

        public CategoriaGeneralLabClinicoViewModel()
        {
            ExamenLabClinicos = new List<ExamenLabClinico>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        public string Nombre { get; set; }
        public string Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string UltimoUsuarioModificado { get; set; }
        public bool Eliminado { get; set; }
        public bool Activo { get; set; }

        public ICollection<ExamenLabClinico> ExamenLabClinicos { get; set; }
    }
}
