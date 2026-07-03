using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class Gasto
    {

        public int Id { get; set; }

        public int? CategoriaGastoId { get; set; }

        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        public string NombreGasto { get; set; }

        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Costo { get; set; }
        public string Descripcion { get; set; }

        public DateTime Fecha { get; set; }

        public bool Eliminado { get; set; }

        public CategoriaGasto CategoriaGasto { get; set; }

    }
}