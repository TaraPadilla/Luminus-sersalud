using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class FormaPago
    {

        public FormaPago()
        {
            Pagos = new List<Pagos>();
        }
        public int Id { get; set; }
        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        public string NombreFormaPago { get; set; }
        public ICollection<Pagos> Pagos { get; set; }
        public bool Eliminada { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PorcentajeCobroAdicional { get; set; }
    }
}