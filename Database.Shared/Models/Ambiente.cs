using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Database.Shared.Models
{
    public class Ambiente
    {
        public Ambiente()
        {
            Bodegas = new List<Bodega>();
            Ventas = new List<Venta>();
        }
        public int Id { get; set; }
        public string NombreAmbiente { get; set; }
        public ICollection<Bodega> Bodegas { get; set; }
        public ICollection<Venta> Ventas { get; set; }
    }
}