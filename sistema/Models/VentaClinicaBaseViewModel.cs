using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;
using Database.Shared.Data;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace sistema.Models
{
    public class VentaClinicaBaseViewModel
    {
        public VentaClinicaBaseViewModel()
        {
            // Venta.EmpleadoId = empleadoId;
        }
        public Venta Venta {get;set;} = new Venta();
        public List<Producto> ListaProductos {get; set;}
        public List<Venta> ListaVentas = new List<Venta>();
        public SelectList ListaClientes {get;set;}
        public SelectList ListaFormaPagos {get;set;}
        public List<User> ListaUsuarios = new List<User>();
        public int EmpleadoId {get;set;}

        public void Init(IProducto productoRepository, IVenta ventaRepository, IPacientes pacientesRepository, IEnvio envioRepository)
        {
            ListaProductos = productoRepository.GetList();
            ListaVentas = ventaRepository.GetListado();
            ListaClientes = new SelectList(pacientesRepository.GetList(), "Id", "Nombre");
            ListaFormaPagos = new SelectList(envioRepository.GetListPagos(), "Id", "NombreFormaPago");
        }
        
        public int Id
        {
            get { return Venta.Id; }
            set { Venta.Id = value; }
        }
    }
} 