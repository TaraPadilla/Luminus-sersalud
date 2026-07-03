using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;

namespace sistema.Models
{
    public class GastoBaseViewModel
    {
        public Gasto Gasto {get;set;} = new Gasto();

        public List<CategoriaGasto> ListaCategorias = new List<CategoriaGasto>();

        public SelectList ListCategorias {get;set;}

        public bool Modificar {get;set;}

        public void Init(ICategoriaGasto categoriaRepository)
        {
            ListCategorias = new SelectList(categoriaRepository.ListarCategorias(), "Id", "NombreCategoria");
        }
        public int Id
        {
            get { return Gasto.Id; }
            set { Gasto.Id = value; }
        }
    }
} 