using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class Auditoria
    {
        public int Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public ICollection<AuditoriaProducto> Productos { get; set; }
        public bool ActualizoStock { get; set; }
        public bool Eliminada { get; set; }
        public string PersonaCreacionAuditoria { get; set; }
    }
}
