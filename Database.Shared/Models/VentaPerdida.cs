using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace Database.Shared.Models
{
    /// <summary>
    /// Ventas que se perdieron por no tener stock o
    /// por no manejar el producto
    /// </summary>
    public class VentaPerdida
    {
        public int Id { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int? ProductoId { get; set; }
        public Producto Producto { get; set; }
        /// <summary>
        /// Este campo se utiliza para cuando la perdida
        /// fue por un producto que no se maneja o manejaba en ese momento. 
        /// Por tanto no se le puede relacionar con un producto de inventario
        /// </summary>
        public string ProductoNombre { get; set; }
        public string Descripcion { get; set; }
        public bool Eliminada { get; set; }
    }
}