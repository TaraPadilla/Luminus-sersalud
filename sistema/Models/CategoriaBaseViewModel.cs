using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;

namespace sistema.Models
{
    public class CategoriaBaseViewModel 
    {
        public Viadmin Categoria {get;set;} = new Viadmin();

        // public List<Categoria> ListaCategorias = new List<Categoria>();
    
      
        public int Id
        {
            get { return Categoria.Id; }
            set { Categoria.Id = value; }
        }
    }
} 