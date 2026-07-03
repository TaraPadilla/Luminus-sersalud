using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class BancoTrazabilidad
    {
        public int Id { get; set; }
        public int BancoId { get; set; }
        public Banco Banco { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public DateTime FechaEliminacion { get; set; }
    }
}
