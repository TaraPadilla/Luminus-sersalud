using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    [Table("SeccionesOrg", Schema = "public")]
    public class SeccionOrg
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("UnidadOrgId")]
        public int UnidadOrgId { get; set; }

        [Required]
        [Column("Nombre")]
        public string Nombre { get; set; }

        [Column("Eliminada")]
        public bool Eliminada { get; set; } = false;

        // Navegación
        [ForeignKey(nameof(UnidadOrgId))]
        public virtual UnidadOrg Unidad { get; set; }
    }
}
