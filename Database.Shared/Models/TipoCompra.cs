using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Database.Shared.Models
{
    public class TipoCompra
    {
        public TipoCompra()
        {
            Compras = new List<Compra>();
        }
        public int Id { get; set; }
        public string Tipo {get;set;}
        public ICollection<Compra> Compras {get;set;}
        public ICollection<TipoCompraProveedor> TipoCompraProveedores { get; set; }

    }
}