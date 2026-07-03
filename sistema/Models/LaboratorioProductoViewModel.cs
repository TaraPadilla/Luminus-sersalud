using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;

namespace sistema.Models
{
    public class LaboratorioProductoViewModel
    {
        public string NombreLaboratorio { get; set; }
        public LaboratorioProducto LaboratorioProducto { get; set; } = new LaboratorioProducto();

        // public List<Categoria> ListaCategorias = new List<Categoria>();

        public int Id
        {
            get { return LaboratorioProducto.Id; }
            set { LaboratorioProducto.Id = value; }
        }
    }
}