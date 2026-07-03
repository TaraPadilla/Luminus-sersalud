using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Database.Shared.Models
{
    public class TipoBodega
    {
        public TipoBodega()
        {
            Bodegas = new List<Bodega>();
            Productos = new List<Producto>();
            Cajas = new List<Caja>();
            Compras = new List<Compra>();
        }
        public int Id { get; set; }
        public string DescripcionBodega { get; set; }
        public ICollection<Bodega> Bodegas { get; set; }
        public ICollection<Producto> Productos { get; set; }
        public ICollection<Caja> Cajas { get; set; }
        public ICollection<Compra> Compras { get; set; }
    }
}