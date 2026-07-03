using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    [Table("UnidadesOrg", Schema = "public")]
    public class UnidadOrg
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("DepartamentoOrgId")]
        public int DepartamentoOrgId { get; set; }

        [Required]
        [Column("Nombre")]
        public string Nombre { get; set; }

        [Column("Eliminada")]
        public bool Eliminada { get; set; } = false;

        // Navegación
        [ForeignKey("DepartamentoOrgId")]
        public virtual DepartamentoOrg DepartamentoOrg { get; set; }

        public virtual ICollection<SeccionOrg> Secciones { get; set; } = new List<SeccionOrg>();
    }
}
