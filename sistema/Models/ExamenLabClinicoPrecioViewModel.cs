using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Database.Shared.Paginacion;
using Database.Shared.Models;

namespace sistema.Models
{

    public class ExamenLabClinicoPrecioViewModel
    {
        public int? Id { get; set; }
        public bool Activar { get; set; }
        public int PrecioId { get; set; }
        public string PrecioNombre { get; set; }
        public decimal PrecioValor { get; set; }
    }
}