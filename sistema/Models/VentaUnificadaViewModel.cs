using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using Database.Shared.Enumeraciones;

namespace sistema.Models
{
    public class VentaUnificadaViewModel
    {
        public string TipoVenta { get; set; }
        public bool IsClinica { get; set; }
        public bool IsFarmacia { get; set; }
        public bool IsLaboratorio { get; set; }
        public bool IsEmergencia { get; set; }
        public bool IsHospital { get; set; }

        //Consulta
        public int? ConsultaId { get; set; }

        public int? EmergenciaId { get; set; }

        public string CitaTipoAtencion { get; set; }
        public int AmbienteId { get; set; }

        //Encabezado
        public string UuidFel { get; set; }

        public string Origen { get; set; }

        public string NumeroComprobante { get; set; }
        public int? CodigoVendedor { get; set; }
        public int? CodigoMedico { get; set; }
        public string Cliente { get; set; }
        public int? ClienteId { get; set; }
        public SelectList ListaClientes { get; set; }

        public SelectList ListaPacientes { get; set; }
        public int SucursalId { get; set; }
        public SelectList ListaSucursales { get; set; }
        public string EmergenciaResponsable { get; set; }
        public string Medico { get; set; }
        public SelectList ListaMedicosDisponibles { get; set; }
        public string Clinica { get; set; }
        public SelectList ListaClinicasDisponibles { get; set; }
        public string Nit { get; set; }
        public string Direccion { get; set; }
        public string Correo { get; set; }

        //Pago
        public SelectList ListaFormaPagos { get; set; }
        public int FormaPagoId { get; set; }
        public decimal PagoMonto { get; set; }
        public decimal PagoVuelto { get; set; }

        //Detalle
        public List<VentaUnificadaProductoAgregadoViewModel> Productos { get; set; }
        public List<VentaUnificadaServicioAgregadoViewModel> Servicios { get; set; }
        public List<VentaUnificadaExamenAgregadoViewModel> Examenes { get; set; }
        public List<PagosViewModel> Pagos { get; set; }

        //Valor cubierto seguro
        public decimal ValorCubiertoSeguro { get; set; }
        public string ResponsableNit { get; set; }

        public string ResponsableNombre { get; set; }
        public string ResponsableDireccion { get; set; }
        public string ResponsableCorreo { get; set; }
        public string ResponsableDPI { get; set; }
        public DateTime PacienteFechaNacimiento { get; set; }

        public void ApplyTipoVenta(string tipoVenta = null)
        {
            var tipo = (tipoVenta ?? TipoVenta)?.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(tipo))
                return;

            TipoVenta = tipo;
            IsClinica = false;
            IsFarmacia = false;
            IsLaboratorio = false;
            IsEmergencia = false;
            IsHospital = false;

            switch (tipo)
            {
                case "clinica":
                    IsClinica = true;
                    break;
                case "farmacia":
                    IsFarmacia = true;
                    break;
                case "laboratorio":
                    IsLaboratorio = true;
                    break;
                case "emergencia":
                    IsEmergencia = true;
                    break;
                case "hospital":
                    IsHospital = true;
                    break;
            }

            SetAmbienteFromTipo();
        }

        public void SetAmbienteFromTipo()
        {
            if (IsHospital)
                AmbienteId = (int)AmbienteEnum.Hospital;
            else if (IsClinica)
                AmbienteId = (int)AmbienteEnum.Clinica;
            else if (IsFarmacia)
                AmbienteId = (int)AmbienteEnum.Farmacia;
            else if (IsLaboratorio)
                AmbienteId = (int)AmbienteEnum.Laboratorio;
            else if (IsEmergencia)
                AmbienteId = (int)AmbienteEnum.Clinica;
        }

        public string GetTituloPantalla()
        {
            switch (TipoVenta?.Trim().ToLowerInvariant())
            {
                case "clinica":
                    return "Clínica - Nueva venta";
                case "farmacia":
                    return "Farmacia - Nueva venta";
                case "laboratorio":
                    return "Laboratorio - Nueva venta";
                case "emergencia":
                    return "Emergencia - Nueva venta";
                case "hospital":
                    return "Hospital - Nueva venta";
                default:
                    if (IsClinica) return "Clínica - Nueva venta";
                    if (IsFarmacia) return "Farmacia - Nueva venta";
                    if (IsLaboratorio) return "Laboratorio - Nueva venta";
                    if (IsEmergencia) return "Emergencia - Nueva venta";
                    if (IsHospital) return "Hospital - Nueva venta";
                    return "Nueva venta";
            }
        }

        public void Init(
            IPacientes _pacientesRepository,
            ICliente _clienteRepository,
            IEnvio envioRepository,
            IEmpleado empleadoRepository,
            ISucursal _sucursalRepository
            )
        {
            ListaFormaPagos = new SelectList(envioRepository.GetListPagos(), "Id", "NombreFormaPago");
            if (IsClinica)
                ListaClientes = new SelectList(_pacientesRepository.GetList(), "Id", "Nombre");
            if (IsFarmacia)
                ListaClientes = new SelectList(_clienteRepository.GetListCombinada(), "Id", "Nombre");
            if (IsLaboratorio)
            {
                ListaMedicosDisponibles = new SelectList(empleadoRepository.GetListMedicos(), "Id", "Nombres");
                ListaClinicasDisponibles = new SelectList(empleadoRepository.GetListClinicas(), "Id", "NombreClinica");
                ListaClientes = new SelectList(_pacientesRepository.GetList(), "Id", "Nombre");
            }
            var sucursales = _sucursalRepository.GetList()?.ToList() ?? new List<Sucursal>();
            ListaSucursales = new SelectList(sucursales, "Id", "NombreSucursal");
            if (SucursalId == 0 && sucursales.Count > 0)
                SucursalId = sucursales[0].Id;
        }


        public class PagosViewModel
        {
            public int PagoId { get; set; }
            //public DateTime? FechaHora { get; set; }
            public int? VentaId { get; set; }
            public int? VentaLabId { get; set; }
            public int? CuentaPorCobrarId { get; set; }
            //public CuentaPorCobrar CuentaPorCobrar { get; set; }
            public int FormaPagoId { get; set; }

            //[Column(TypeName = "decimal(18,2)")]
            public decimal ValorTotal { get; set; }
            //public FormaPago FormaPago { get; set; }
            //public Venta Venta { get; set; }
            //public VentasLab VentaLab { get; set; }
            public int? MonedaId { get; set; }
            //public Moneda Moneda { get; set; }
            public bool Eliminado { get; set; }

            //PagoId: $("#FormaPagoId").val(),
            //        PagoNombre: $("#FormaPagoId option:selected").text(),

            //        ValorTotal: self.agregarMonto(),
        }

    }
}
