using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Database.Shared.Models
{
    public class SucursalServicio
    {
        public int Id { get; set; }
        public int SucursalId { get; set; }
        public Sucursal Sucursal { get; set; }
        public int ServicioId { get; set; }
        public Servicio Servicio { get; set; }
        public bool Activar { get; set; }

    }
}