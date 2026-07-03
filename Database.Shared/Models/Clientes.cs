using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Database.Shared.Models
{
    public class Clientes
    {
        public Clientes()
        {
            Ventas = new List<Venta>();
        }

    public int Id {get;set;}

    [Required(ErrorMessage = "* Este campo es obligatorio.")]
    public string Nombre {get;set;}
    public string Telefono {get;set;}
    public string Celular {get;set;}
    public string Nit {get;set;}
    public string Direccion {get;set;}
    public bool Eliminado {get; set;}

    public ICollection<Venta> Ventas {get;set;}

    }
}