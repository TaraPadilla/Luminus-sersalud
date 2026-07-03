using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class CompraTipoDocumento
    {
        public CompraTipoDocumento()
        {
            Compras = new List<Compra>();
        }
        public int Id { get; set; }
        public string NombreTipoDocumento { get; set; }
        public bool Eliminado { get; set; }
        public ICollection<Compra> Compras { get; set; }
    }
}