using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;

namespace sistema.Models
{
    public class TrasladosBaseViewModel
    {
        public TrasladosProductos TrasladosProductos { get; set;}
        public TrasladosDto TrasladosDto {get;set;}
        public List<DetalleTrasladoProductos> DetalleTrasladoProductos {get;set;} 
        public List<DetalleTrasladoProductos> DetalleTrasladosProductosNuevos {get;set;} 
        public SelectList ProductoSelectListItems {get;set;}
        public virtual void Init(IProducto productoRepository)
        {
            ProductoSelectListItems = new SelectList(productoRepository.GetListadoProductosBodega(), "Id", "ProductoYPresentacion");
        }

        public string UsuarioCreacion {get;set;}
        public string UsuarioUltimaOperacion {get;set;}
    }

    public class TrasladosDto
    {
        public int Id {get;set;}
        public string Observaciones {get;set;} 
    }

} 