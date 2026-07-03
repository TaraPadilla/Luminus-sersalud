using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class AuditoriaProducto
    {
        public int Id { get; set; }
        public int Stock { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        public int AuditoriaId { get; set; }
        public virtual Auditoria Auditoria { get; set; }
    }
}
