using System;

namespace Database.Shared.Models
{
    public class Notificacion
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Leida { get; set; }
    }
}