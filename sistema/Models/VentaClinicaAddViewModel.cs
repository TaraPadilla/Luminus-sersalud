using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;

namespace sistema.Models
{
    public class VentaClinicaAddViewModel
    {
        public EncabezadoVentaClinicaDto encabezado {get;set;}
        public IList<DetalleVentaClinicaDto> detalle {get;set;}
       
    }

    public class DetalleVentaClinicaDto
    {
            public int ProductoId {get;set;}
            // public int ServicioId {get;set;}
            public int Cantidad {get;set;}
            public decimal Precio {get;set;}
            public decimal Descuento {get;set;}
            public decimal Subtotal {get;set;}
            public decimal Total {get;set;} 
            public string BienOServicio {get;set;}
            public string UsuarioAutorizaModificacion {get;set;}

    }

    public class EncabezadoVentaClinicaDto
    {
        public string Nombres {get;set;}
        public string NoComprobante {get;set;}
        public string Nit {get;set;}
        public string Direccion {get;set;}
        public int EmpleadoId {get;set;}
        public int FormaPago {get;set;}
        public decimal Monto {get;set;}
        public decimal Vuelto {get;set;}
        public int ClienteId {get;set;}
        public string MedicoReferido {get;set;}
        public string ClinicaReferida {get;set;}
        public string Medico {get;set;}
        public string Clinica {get;set;}

    }
} 