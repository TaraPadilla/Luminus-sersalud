using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using sistema.Utilidades;
using Microsoft.AspNetCore.Http;
using Database.Shared.Data;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace sistema.Models
{
    public class VentaBaseViewModel
    {
        public VentaBaseViewModel(int empleadoId)
        {
            Venta.EmpleadoId = empleadoId;
        }
        // private readonly SignInManager<IdentityUser> _signInManager;
        // private readonly UserManager<IdentityUser> _userManager;
        // private readonly RoleManager<IdentityRole> _roleManager;
        public Venta Venta { get; set; } = new Venta();
        public Paciente Pacicente { get; set; } = new Paciente();
        public Envio Envio { get; set; } = new Envio();
        public List<Producto> ListaProductos { get; set; }
        public List<Venta> ListaVentas = new List<Venta>();
        public SelectList ListaClientes { get; set; }
        public SelectList ListaEmpleados { get; set; }
        public SelectList ListaRutas { get; set; }
        public SelectList ListaFormaPagos { get; set; }
        public List<User> ListaUsuarios = new List<User>();

        //public SelectList ListaUsuarios {get;set;}

        public bool Modificar { get; set; }

        public Producto Producto { get; set; } = new Producto();

        public void Init(IProducto productoRepository)
        {
            ListaProductos = productoRepository.GetList();
        }


        public void Init(IVenta ventaRepository)
        {
            ListaVentas = ventaRepository.GetListado();
        }

        public void Init(IEnvio envioRepository)
        {
            ListaFormaPagos = new SelectList(envioRepository.GetListPagos(), "Id", "NombreFormaPago");

        }
        public void Init(ICliente clienteRepository, IPacientes pacientesRepository)
        {
            //ListaClientes = new SelectList(clienteRepository.GetList(), "Id", "Nombre");
            ListaClientes = new SelectList(pacientesRepository.GetList(), "Id", "Nombre");

        }

        public void Init(IEmpleado empleadoRepository)
        {
            ListaEmpleados = EmpleadoSelectListHelper.Crear(empleadoRepository);
        }
        public void Init(IRuta rutaRepository)
        {

            ListaRutas = new SelectList(rutaRepository.GetList(), "Id", "Destino");
        }

        public int Id
        {
            get { return Venta.Id; }
            set { Venta.Id = value; }
        }
    }
}