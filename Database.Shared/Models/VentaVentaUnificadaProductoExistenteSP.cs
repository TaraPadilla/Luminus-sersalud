using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Database.Shared.Models
{
    public class VentaVentaUnificadaProductoExistenteSP
    {
        [Key]
        public int ProductoId { get; set; }
        public int ProductoInventarioId { get; set; }
        public string ProductoCodigo { get; set; }
        public string ProductoNombre { get; set; }
        public string ProductoActivoConcentracion { get; set; }
        public string ProductoPresentacion { get; set; }
        public string ProductoViaAdministracion { get; set; }
        public string ProductoGrupoTerapeutico { get; set; }
        public string ProductoLaboratorio { get; set; }
        public string ProductoImagen { get; set; }
        public int? UnidadMedidaVentaId { get; set; }
        public string UnidadMedidaVentaNombre { get; set; }
        public decimal ProductoStock { get; set; }

        //Utilizado para la consulta por nombre
        public int PrecioId { get; set; }
        public string PrecioNombre { get; set; }
        public decimal PrecioValor { get; set; }

        public string ProductoNombreMostrar
        {
            get
            {
                return $"{ProductoCodigo} - {ProductoNombre} (Stock: {ProductoStock} {UnidadMedidaVentaNombre})";
            }
        }
    }
}
