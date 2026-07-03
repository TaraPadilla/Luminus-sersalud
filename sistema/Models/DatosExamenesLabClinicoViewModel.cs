using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace sistema.Models
{
    public class DatosExamenesLabClinicoViewModel
    {

        public DatosExamenesLabClinicoViewModel()
        {
            Resultados = new List<Resultados>();
        }

        public int Id { get; set; }
        public int ExamenLabClinicoId { get; set; }
        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        public string Campos { get; set; }
        public string Resultado { get; set; }
        public string ValorReferencia { get; set; }
        public bool Activo { get; set; } = true;
        public string Indicaciones { get; set; }
        public string Unidad { get; set; }

        public string Tipo { get; set; }
        public bool Eliminado { get; set; }
        public SelectList ListTiposDatos { get; set; }
        public ExamenLabClinico ExamenLabClinico { get; set; }

        public ICollection<Resultados> Resultados { get; set; }

        public void Init(ILaboratorioClinico laboratorioRepository)
        {
            
            ListTiposDatos = new SelectList(laboratorioRepository.GetListTipo(), "Tipo", "Tipo");

        }
    }
}
