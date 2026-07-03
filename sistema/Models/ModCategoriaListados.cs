using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;
using Database.Shared.Data;

namespace sistema.Models
{
    public class ModCategoriaListados
    {
        public CategoriaLabClinico CategoriaLabClinico {get;set;} 

        public List<ExamenLabClinico> NombresExamenes {get;set;}
        public string NombreExamen { get; set; }
        public decimal Precio {get;set;}
        public decimal PrecioB {get;set;}
        public decimal PrecioC {get;set;}
        public string CodigoInterno {get;set;}
        public string TipoDeExamen {get;set;}
        public string Indicaciones {get;set;}


    }
}