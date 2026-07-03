//using Database.Shared.Models;
//using System.Collections.Generic;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Database.Shared.IRepository;
//using Microsoft.AspNetCore.Http;
//using Database.Shared.Data;

//namespace sistema.Models
//{
//    public class VentaServicioBaseViewModel
//    {
//        public VentaServicio VentaServicio {get;set;} = new VentaServicio();

//        public Paciente Paciente {get; set;} = new Paciente();

//        public List<VentaServicio> ListaVentasServicio = new List<VentaServicio>();
//        public SelectList ListaClientes {get;set;}

//        public SelectList ListaServicios {get;set;}
//        public SelectList ListaEmpleados {get;set;}

//         public List<Servicio> ListaServiciosCuadricula {get; set;}

//        public bool Modificar {get;set;}

//        public Producto Servicio {get; set;} = new Producto();

//        public void Init(ICliente clienteRepository)
//        {
//            ListaClientes = new SelectList(clienteRepository.GetList(), "Id", "Nombre");
//        }

//         public void Init(IEmpleado empleadoRepository)
//        {
//            ListaEmpleados = new SelectList(empleadoRepository.GetList(), "Id", "Nombre");
           
//        }

//        public void Init(IServicio servicioRepository)
//        {
//            ListaServicios = new SelectList(servicioRepository.GetList(), "Id", "NombreServicio");
//            // ListaServiciosCuadricula = servicioRepository.GetList();
//        }

//        public int Id
//        {
//            get { return VentaServicio.Id; }
//            set { VentaServicio.Id = value; }
//        }
//    }
//} 