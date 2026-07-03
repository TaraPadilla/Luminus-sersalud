using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sistema.Models
{
    public class VentaUnificadaViewModel
    {
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
            ListaSucursales = new SelectList(_sucursalRepository.GetList(), "Id", "NombreSucursal");
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
