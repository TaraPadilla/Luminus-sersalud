using System;

namespace Database.Shared.Models
{
    public class MedicamentoNoControlado
    {
        public int Id { get; set; }
        public int HospitalizacionId { get; set; }
        public DateTime? FechaProcedimiento { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public decimal UnidadesIniciales { get; set; }
        public decimal UnidadesExtra { get; set; }
        public decimal Utilizado { get; set; }
        public decimal Descartado { get; set; }
        public decimal Retornadas { get; set; }
        public string UsuarioRegistroId { get; set; }
        public bool Eliminado { get; set; }

        // Navegación (opcional)
        public virtual Hospitalizacion Hospitalizacion { get; set; }
        public virtual Producto Producto { get; set; }
        public virtual User UsuarioRegistro { get; set; }
    }
}