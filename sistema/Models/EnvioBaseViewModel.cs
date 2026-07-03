using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;
using Database.Shared.Data;

namespace sistema.Models
{
    public class EnvioBaseViewModel
    {
        public Envio Envio {get;set;} = new Envio();

        public Empleado Cliente {get; set;} = new Empleado();

        public Empleado Empleado {get; set;} = new Empleado();

        //public List<Venta> ListaVentas {get; set;}

        public SelectList ListaRutas {get;set;}

        public List<Producto> ListaProductos {get; set;}
        public SelectList ListaClientes {get;set;}

        public SelectList ListaFormaPagos {get;set;}

         //public SelectList ListaUsuarios {get;set;}
       public List<User> ListaUsuarios = new List<User>();


        public bool Modificar {get;set;}

        public Producto Producto {get; set;} = new Producto();

        public void Init(ICliente clienteRepository)
        {
            ListaClientes = new SelectList(clienteRepository.GetList(), "Id", "Nombre");
           
        }

         public void Init(IEnvio envioRepository)
        {
            ListaFormaPagos = new SelectList(envioRepository.GetListPagos(), "Id", "NombreFormaPago");
           
        }

        
        public void Init(IRuta rutaRepository, IProducto productoRepository)
        {
            ListaRutas = new SelectList(rutaRepository.GetList(), "Id", "Destino");
            ListaProductos = productoRepository.GetList();
        }

        public int Id
        {
            get { return Envio.Id; }
            set { Envio.Id = value; }
        }
    }
} 