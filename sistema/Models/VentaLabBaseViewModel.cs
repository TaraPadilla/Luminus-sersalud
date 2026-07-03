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
    public class VentaLabBaseViewModel
    {
        public VentaLabBaseViewModel(int empleadoId)
        {
            // Venta.EmpleadoId = empleadoId;
        }

        // public Venta Venta {get;set;} = new Venta();
        // public List<Producto> ListaProductos {get; set;}
        // public List<Venta> ListaVentas = new List<Venta>();
        // public SelectList ListaClientes {get;set;}
        public SelectList ListaFormaPagos {get;set;}
        // public List<User> ListaUsuarios = new List<User>();

        public void Init(IEnvio envioRepository)
        {
            ListaFormaPagos = new SelectList(envioRepository.GetListPagos(), "Id", "NombreFormaPago");
        }
        
        // public int Id
        // {
        //     get { return Venta.Id; }
        //     set { Venta.Id = value; }
        // }
    }
} 