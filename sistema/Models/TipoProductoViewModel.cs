using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;

namespace sistema.Models
{
    public class TipoProductoViewModel 
    {
        public TipoProducto TipoProducto {get;set;} = new TipoProducto();

        // public List<Categoria> ListaCategorias = new List<Categoria>();
    
      
        public int Id
        {
            get { return TipoProducto.Id; }
            set { TipoProducto.Id = value; }
        }
    }
} 