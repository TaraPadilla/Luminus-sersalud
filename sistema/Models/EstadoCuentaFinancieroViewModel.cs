using System;
namespace sistema.Models
{
    public class EstadoCuentaFinancieroViewModel
    {
        // Totales de Cargos (Brutos)
        public decimal TotalHabitacion { get; set; }
        public decimal TotalProductos { get; set; }
        public decimal TotalServicios { get; set; }
        public decimal TotalExamenes { get; set; } // Recordar: lógica de 100% descuento si aplica
        public decimal TotalDietas { get; set; }
        public decimal TotalPaquetes { get; set; }
        public decimal TotalAmbulancias { get; set; }
        public decimal TotalEmergencias { get; set; }
        public decimal TotalInsumosDirectos { get; set; }
        public decimal TotalHonorarios { get; set; }

        // Gran Total de la Cuenta (Suma de todo lo anterior)
        public decimal TotalBruto { get; set; }

        // Sección de Seguros y Exclusiones
        public decimal MontoNoElegible { get; set; } // Suma de lo que el usuario excluyó
        public decimal SubtotalElegible => Math.Max(TotalBruto - MontoNoElegible, 0);

        // Cálculos de Responsabilidad
        public decimal PorcentajeCoaseguro { get; set; }
        public decimal MontoCoaseguro { get; set; }
        public decimal MontoDeducible { get; set; }
        public decimal MontoCopago { get; set; }
        public decimal MontoIva { get; set; }

        // Descuentos
        public decimal PorcentajeDescuentoGlobal { get; set; }
        public decimal MontoDescuentoGlobal { get; set; }

        // Totales Finales
        public decimal TotalResponsabilidadSeguro { get; set; }
        public decimal TotalResponsabilidadPaciente { get; set; }

        // Estado de Pagos
        public decimal TotalPagadoAnteriormente { get; set; }
        public decimal TotalPagosNuevos { get; set; }
        
        // El número que debe llegar a 0
        public decimal SaldoPendienteActual { get; set; }
        public bool EsCuentaLiquidada => SaldoPendienteActual <= 0.005m;
    }
}