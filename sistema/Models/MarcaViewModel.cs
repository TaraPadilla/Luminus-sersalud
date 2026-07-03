using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;

namespace sistema.Models
{
    public class MarcaViewModel
    {
        public string NombreMarca { get; set; }
        public Marca Marca { get; set; } = new Marca();

        public int Id
        {
            get { return Marca.Id; }
            set { Marca.Id = value; }
        }
    }
}