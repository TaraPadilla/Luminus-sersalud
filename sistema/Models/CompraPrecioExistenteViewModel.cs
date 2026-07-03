using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;
using Database.Shared.Data;
using System;

namespace sistema.Models
{
    public class CompraPrecioExistenteViewModel
    {
        public int PrecioId { get; set; }
        public string PrecioNombre { get; set; }
    }
}