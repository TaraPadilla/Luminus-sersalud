// Nuevo archivo: HospitalizacionHonorario.cs
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class HospitalizacionHonorario
    {
        public int Id { get; set; }
        public int HospitalizacionId { get; set; }
        
        // Relacionamos con el empleado (médico)
        public int EmpleadoId { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }

        // Propiedades de navegación (Opcional)
        public virtual Hospitalizacion Hospitalizacion { get; set; }
        public virtual Empleado Empleado { get; set; }
    }
}