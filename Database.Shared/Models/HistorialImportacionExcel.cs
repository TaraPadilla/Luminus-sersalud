using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class HistorialImportacionExcel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El hash del archivo es obligatorio.")]
        [MaxLength(64)] // Un hash SHA-256 en formato hexadecimal tiene exactamente 64 caracteres.
        public string HashArchivo { get; set; }

        [Required(ErrorMessage = "El nombre del archivo es obligatorio.")]
        [MaxLength(255)]
        public string NombreArchivo { get; set; }

        [Required]
        public DateTime FechaImportacion { get; set; }
    }
}