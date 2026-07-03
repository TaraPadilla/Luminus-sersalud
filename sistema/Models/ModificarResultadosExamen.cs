using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;
using Database.Shared.Data;
using Microsoft.AspNetCore.Mvc;

namespace sistema.Models
{
    public class ModificarResultadosExamen
    {
        public DetalleExamen DetalleExamen { get; set; }
        public int? ExamenId { get; set; }
        public SelectList ListaReferencias { get; set; }

        public int ExamenLabClinicoId { get; set; }

        [BindProperty]
        public List<Resultados> DatosResultados { get; set; }


        public void Init(ILaboratorioClinico laboratorioClinico)
        {
            ListaReferencias = new SelectList(laboratorioClinico.DatosLabList((int)ExamenLabClinicoId), "Id", "Campos");
        }

        public int Id
        {
            get { return DetalleExamen.Id; }
            set { DetalleExamen.Id = value; }
        }

    }
}