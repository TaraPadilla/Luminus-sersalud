using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace Database.Shared.Models
{
    public class VentaServicio
    {

        public VentaServicio()
        {
            DetalleServicio = new List<DetalleServicio>();
        }
        public int Id { get; set; }
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        public int EmpleadoId { get; set; }
        public string NoComprobante { get; set; }
        public string Nit { get; set; }
        public string Nombres { get; set; }
        public string Direccion { get; set; }
        public DateTime FechaVenta { get; set; }
        public string FormaPago { get; set; }
        public bool Eliminado { get; set; }
        public ICollection<DetalleServicio> DetalleServicio { get; set; }
        public Paciente Paciente { get; set; }
        public Empleado Empleado { get; set; }


    }
}