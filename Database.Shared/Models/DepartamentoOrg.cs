using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    [Table("DepartamentosOrg", Schema = "public")]
    public class DepartamentoOrg
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("Nombre")]
        public string Nombre { get; set; }

        [Column("Eliminada")]
        public bool Eliminada { get; set; } = false;

        // Navegación
        public virtual ICollection<UnidadOrg> Unidades { get; set; } = new List<UnidadOrg>();
    }
}
