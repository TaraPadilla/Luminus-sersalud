using System;
using System.Collections.Generic;

namespace sistema.Models
{
    public class EstadoDeCuentaViewModel
    {
        public int CuentaId { get; set; }
        public int? HospitalizacionId { get; set; }
        public DateTime FechaGeneracion { get; set; } = DateTime.Now;
        public List<ProductoViewModel> Productos { get; set; } = new List<ProductoViewModel>();
        public List<ServicioViewModel> Servicios { get; set; } = new List<ServicioViewModel>();
        public List<ExamenViewModel> Examenes { get; set; } = new List<ExamenViewModel>();
        public decimal TotalProductos => CalcularTotal(Productos);
        public decimal TotalServicios => CalcularTotal(Servicios);
        public decimal TotalExamenes => CalcularTotal(Examenes);
        public decimal TotalGeneral => TotalProductos + TotalServicios + TotalExamenes;

        private decimal CalcularTotal<T>(List<T> items) where T : ITotalizable
        {
            decimal total = 0;
            foreach (var item in items)
            {
                total += item.Subtotal;
            }
            return total;
        }
    }

    public class ProductoViewModel : ITotalizable
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }

    public class ServicioViewModel : ITotalizable
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }

    public class ExamenViewModel : ITotalizable
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => PrecioUnitario;
    }

    public interface ITotalizable
    {
        decimal Subtotal { get; }
    }
}
