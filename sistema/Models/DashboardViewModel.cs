using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;
using Database.Shared.Data;
using Database.Shared.DataBindings;


namespace sistema.Models
{
    public class DashboardViewModel
    {
        public List<CuentaPorCobrar> CuentasProximasCobrar { get; set; }
        // public void Init(IProducto productoRepository)
        // {
        //     // ProductoSelectListItems = new SelectList(productoRepository.GetListadoProductos(), "Id", "ProductoYPresentacion");
        // }
    }
}