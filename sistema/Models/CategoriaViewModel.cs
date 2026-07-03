using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;

namespace sistema.Models
{
    public class CategoriaViewModel 
    {
        public string NombreCategoria { get; set; }
        public Categoria Categoria {get;set;} = new Categoria();
      
        public int Id
        {
            get { return Categoria.Id; }
            set { Categoria.Id = value; }
        }
    }
} 