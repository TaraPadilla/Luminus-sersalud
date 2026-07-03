using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;

namespace sistema.Models
{
    public class GrupoTViewModel 
    {
        public string NombreGrupoT { get; set; }
        public GrupoTProducto GrupoTProducto {get;set;} = new GrupoTProducto();

        // public List<Categoria> ListaCategorias = new List<Categoria>();
    
      
        public int Id
        {
            get { return GrupoTProducto.Id; }
            set { GrupoTProducto.Id = value; }
        }
    }
} 