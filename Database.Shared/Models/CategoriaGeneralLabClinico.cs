using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Database.Shared.Models
{
    public class CategoriaGeneralLabClinico
    {
        public CategoriaGeneralLabClinico()
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
