using System;
using System.Collections.Generic;

namespace farmamest.Models
{
    public class EstadoCuentaViewModel
    {
        public ResponsableViewModel Responsable { get; set; }
        public HabitacionViewModel Habitacion { get; set; }
        public List<ProductoViewModel> Productos { get; set; }
        public List<ExclusionViewModel> Exclusiones { get; set; }
        public List<HonorarioViewModel> Honorarios { get; set; }
        public PagosViewModel Pagos { get; set; }
        public PacienteEstadoCuentaViewModel Paciente { get; set; }
        public HospitalizacionEstadoCuentaViewModel Hospitalizacion { get; set; }

        public List<PaqueteViewModel> Paquetes { get; set; }
        public List<GastoAdministrativoViewModel> GastosAdministrativos { get; set; }
            = new List<GastoAdministrativoViewModel>();

        public decimal DescuentoGlobal { get; set; }

    }

    public class ResponsableViewModel
    {
        public string Nombre { get; set; }
        public string Nit { get; set; }
        public string Direccion { get; set; }
        public string Correo { get; set; }
    }

    public class HabitacionViewModel
    {
        public string Numero { get; set; }
        public string Categoria { get; set; }
        public decimal Tarifa { get; set; }
        public string Seguro { get; set; }
    }

    public class ProductoViewModel
    {
        public string Fecha { get; set; }
        public string Item { get; set; }
        public string Tipo { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }

        public decimal DescPct { get; set; }
        public decimal Cargo { get; set; }
    }

    public class ExclusionViewModel
    {
        public string Item { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class HonorarioViewModel
    {
        public string Medico { get; set; }
        public decimal Monto { get; set; }
    }

    public class PagosViewModel
    {
        public decimal TotalCuenta { get; set; }
        public decimal TotalAseguradora { get; set; }
        public decimal TotalNoElegibles { get; set; }
        public decimal Deducibles { get; set; }
        public decimal Coaseguro { get; set; }
        public decimal Copago { get; set; }
        public decimal IVA { get; set; }
        public decimal PagoPaciente { get; set; }
        public decimal PagoSeguro { get; set; }
    }

    // Modelo de datos para el paciente
    public class PacienteEstadoCuentaViewModel
    {
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Celular { get; set; }
        public string Direccion { get; set; }
        public string FechaNacimiento { get; set; }
        public int Edad { get; set; }
        public string Sexo { get; set; }
        public string Nit { get; set; }
        public string Dpi { get; set; }
    }

    // Modelo de datos para la Hospitalización (Hops)
    public class HospitalizacionEstadoCuentaViewModel
    {
        public string FechaInicioHospitalizacion { get; set; }
        public string MedicoResponsable { get; set; }
        public string NombreSeguro { get; set; }
        public int HospitalizacionId { get; set; }

    }

    public class PaqueteViewModel
    {
        public DateTime Fecha { get; set; }

        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}