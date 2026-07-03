using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;

namespace sistema.Models
{
    public class PresentacionProductoViewModel 
    {
        public string NombrePresentacion { get; set; }
        public PresentacionProducto PresentacionProducto {get;set;} = new PresentacionProducto();

        // public List<Categoria> ListaCategorias = new List<Categoria>();
    
      
        public int Id
        {
            get { return PresentacionProducto.Id; }
            set { PresentacionProducto.Id = value; }
        }
    }
} 