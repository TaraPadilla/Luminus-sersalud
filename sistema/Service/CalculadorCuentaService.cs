using sistema.Models;
using sistema.Service.IService;
using System;
using System.Linq;

namespace sistema.Service
{
    public class CalculadorCuentaService : ICalculadorCuentaService
    {
        public EstadoCuentaFinancieroViewModel Calcular(CuentasPorCobrarPagarViewModel model)
        {
            var vm = new EstadoCuentaFinancieroViewModel();

            // 1. Sumatorias Base (Extraídas de lo que ya tienes en el Controller)
            vm.TotalHabitacion = model.Habitacion?.CostoTotal ?? 0;
            vm.TotalPaquetes = model.Paquetes?.Sum(p => p.Precio) ?? 0;
            vm.TotalAmbulancias = model.Ambulancias?.Sum(a => a.Precio) ?? 0;
            vm.TotalHonorarios = model.Honorarios?.Sum(h => h.Monto) ?? 0;

            // 2. Lógica de Productos (Separar por tipo y aplicar regla de exámenes)
            if (model.Productos != null)
            {
                // Los exámenes tienen descuento del 100% según tu lógica actual
                vm.TotalExamenes = model.Productos
                    .Where(p => p.EsExamen)
                    .Sum(p => p.Subtotal);

                vm.TotalProductos = model.Productos
                    .Where(p => !p.EsExamen && p.Tipo != "Servicio" && p.Tipo != "Dietas")
                    .Sum(p => p.Subtotal);

                vm.TotalServicios = model.Productos
                    .Where(p => p.Tipo == "Servicio")
                    .Sum(p => p.Subtotal);

                vm.TotalDietas = model.Productos
                    .Where(p => p.Tipo == "Dietas")
                    .Sum(p => p.Subtotal);
            }

            // 3. Gran Total Bruto
            vm.TotalBruto = vm.TotalHabitacion + vm.TotalPaquetes + vm.TotalAmbulancias + 
                            vm.TotalHonorarios + vm.TotalProductos + vm.TotalServicios + 
                            vm.TotalDietas; // Nota: No sumamos Exámenes porque son 100% descuento

            // 4. Lógica de Seguros y Exclusiones (Inputs del usuario)
            vm.MontoNoElegible = model.Descuento; // O la lógica que definamos para ítems excluidos
            vm.PorcentajeCoaseguro = model.PorcentajeDescuento; // Ejemplo de mapeo

            // 5. Cálculos Finales
            vm.MontoCoaseguro = Math.Round(vm.SubtotalElegible * (vm.PorcentajeCoaseguro / 100), 2);
            
            // Aquí sumamos la responsabilidad del paciente según tus campos actuales
            vm.TotalResponsabilidadPaciente = vm.MontoNoElegible + vm.MontoCoaseguro; 

            // 6. Pagos
            vm.TotalPagadoAnteriormente = 0; // Esto se debe consultar de BD
            vm.TotalPagosNuevos = model.Pagos?.Sum(p => p.Monto) ?? 0;

            vm.SaldoPendienteActual = Math.Round(vm.TotalResponsabilidadPaciente - vm.TotalPagosNuevos, 2);

            return vm;
        }
    }
}