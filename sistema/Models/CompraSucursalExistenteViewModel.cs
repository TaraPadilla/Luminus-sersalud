using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;
using Database.Shared.Data;
using System;

namespace sistema.Models
{
    public class CompraSucursalExistenteViewModel
    {
        public int SucursalId { get; set; }
        public string SucursalNombre { get; set; }
    }
}