using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Database.Shared.Paginacion;
using Database.Shared.Models;

namespace sistema.Models
{
    public class ExamenesLabClinicoViewModel
    { 
        public SelectList CategoriasExamenesSelectList {get;set;}
        public PaginacionList<ExamenLabClinico> nombreExamenes {get;set;}
        public string buscar {get;set;}
        public string currentFilter {get;set;}
        public int? pageNumber {get;set;}
        public int? catexamenId {get;set;}

        public void Init(ILaboratorioClinico _laboratorioClinico)
        {
            CategoriasExamenesSelectList = new SelectList(_laboratorioClinico.GetListCategoriasLab(), "Id", "Nombre");
        }
    }
}