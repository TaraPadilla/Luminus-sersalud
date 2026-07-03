using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using System.Collections.Generic;
using System.Linq;

namespace sistema.Models
{
    public class CuentasPorCobrarPagarViewModel
    {
        public bool SoloVista { get; set; } = false;

        // Datos de factura
        public string NoComprobante { get; set; }
        public int EmpleadoId { get; set; }

        // Datos de cuenta
        public int CuentaId { get; set; }
        public decimal Valor { get; set; }
        public string Observaciones { get; set; }

        // Datos del paciente
        public int PacienteId { get; set; }
        public int AdmisionId { get; set; }

        public string SeguroNombre { get; set; }
        public string PacienteNombre { get; set; }
        public string PacienteTelefono { get; set; }
        public string PacienteCelular { get; set; }
        public string PacienteDireccion { get; set; }
        public string PacienteNit { get; set; }
        public string PacienteDpi { get; set; }
        public string PacienteFechaNacimiento { get; set; }
        public string PacienteEdad { get; set; }
        public string PacienteSexo { get; set; }
        public string PacienteNombreAdmision { get; set; }

        public string FechaInicioHops { get; set; }
        public string MedicoHops { get; set; }
        public string NombreDelSeguro { get; set; }

        public string ResponsableNit { get; set; }
        public string ResponsableNombre { get; set; }
        public string ResponsableDireccion { get; set; }
        public string ResponsableCorreo { get; set; }
        public string UuidFel { get; set; }

        // Datos de pago
        public SelectList DoctoresSelectListItem { get; set; }

        public int FormaPagoId { get; set; }
        public SelectList FormaPagoSelectListItems { get; set; }
        public int MonedaId { get; set; }
        public SelectList MonedaSelectListItems { get; set; }
        public List<CuentasPorCobrarPagoViewModel> Pagos { get; set; }

        // **Nuevas propiedades para almacenar los datos de la hospitalización**
        public HabitacionPagarViewModel Habitacion { get; set; }  // Datos de la habitación
        public List<ProductoPagarViewModel> Productos { get; set; }  // Lista de productos aplicados
        public List<PaquetePagarViewModel> Paquetes { get; set; }
        public List<AmbulanciaPagarViewModel> Ambulancias { get; set; }

        // =======================================================
        // SOLUCIÓN CS1061: Propiedad agregada dentro del ViewModel
        // =======================================================
        public List<HonorarioPagarViewModel> Honorarios { get; set; }

        public List<ProductoExclusionJson> Exclusiones { get; set; }

        public decimal TotalFrontend { get; set; }
        public List<DescuentoLineaViewModel> DescuentosLinea { get; set; }

        public void Init(ICuentasPorCobrar cuentasPorCobrarRepository, IEmpleado empleadoRepository)
        {
            var formasPago = cuentasPorCobrarRepository.GetFormasPago() ?? new List<Database.Shared.Models.FormaPago>();
            var monedas = cuentasPorCobrarRepository.GetMonedas() ?? new List<Database.Shared.Models.Moneda>();
            var doctores = empleadoRepository.GetListEmpleadoTipoProfesionalColegiado() ?? new List<object>();

            FormaPagoSelectListItems = new SelectList(formasPago, "Id", "NombreFormaPago");
            MonedaSelectListItems = new SelectList(monedas, "Id", "NombreMoneda");
            DoctoresSelectListItem = new SelectList(doctores, "Id", "NombreCompleto");
        }

        public decimal Descuento { get; set; } = 0m;
        public decimal PorcentajeDescuento { get; set; } = 0m;
        public decimal TotalConDescuento { get; set; } = 0m;
    }

    public class HonorarioPagarViewModel
    {
        public int EmpleadoId { get; set; }
        public decimal Monto { get; set; }
    }

    // **ViewModel para la habitación**
    public class HabitacionPagarViewModel
    {
        public int Id { get; set; }
        public int CategoriaId { get; set; }

        public string NombreHabitacion { get; set; }
        public string NombreCategoriaHabitacion { get; set; }

        public decimal CostoTotal { get; set; }
    }

    // **ViewModel para los productos**
    public class ProductoPagarViewModel
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string Nombre { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public string Tipo { get; set; }
        public string FechaAplicacion { get; set; }

        public bool EsExamen { get; set; }


    }
    public class PaquetePagarViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public string Tipo { get; set; }
    }

    public class AmbulanciaPagarViewModel
    {
        public int Id { get; set; }
        public string TipoTraslado { get; set; }
        public decimal Precio { get; set; }
    }

    public class ProductoExclusionJson
    {
        public int IdOriginal { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }

    public class DescuentoLineaViewModel
    {
        public int IdOriginal { get; set; }
        public decimal DescPct { get; set; }
        public decimal Cargo { get; set; }
        public decimal Subtotal { get; set; }
    }
}