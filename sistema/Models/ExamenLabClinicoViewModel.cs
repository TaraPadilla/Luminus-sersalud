using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;
using System;
using DocumentFormat.OpenXml.Drawing;

namespace sistema.Models
{
    public class ExamenLabClinicoViewModel
    {
        public ExamenLabClinico ExamenLabClinico { get; set; } = new ExamenLabClinico();
        public List<DatosExamenesLabClinico> DatosExamenes { get; set; }
        public List<ExamenLabClinicoInsumoViewModel> InsumosUtilizados { get; set; }
        public string Tipo { get; set; }
        public string Campos { get; set; }
        public string Resultado { get; set; }
        public string ValorReferencia { get; set; }
        public SelectList ListTiposDatos { get; set; }
        public int? IdExamen { get; set; }
        public string CodigoInterno { get; set; }
        public string NombreExamen { get; set; }
        public string TipoExamen { get; set; }
        public SelectList ListCategorias { get; set; }
        public int CategoriaLabClinicoId { get; set; }
        public string Indicaciones { get; set; }
        public string Instrucciones { get; set; }
        public string Advertencias { get; set; }
        public string DuracionHoras { get; set; }
        public string DuracionMinutos { get; set; }
        public string TipoPDF { get; set; }
        public string DeclaracionConsentimiento { get; set; }
        public List<ExamenLabClinicoPrecioViewModel> Precios { get; set; }
        public List<ExamenLabClinicoPreguntasViewModel> Preguntas { get; set; }
        public Paciente Paciente { get; set; }
        public void Init(ILaboratorioClinico categoriaRepository)
        {
            ListCategorias = new SelectList(categoriaRepository.GetListCategoriasLab(), "Id", "Nombre");
            ListTiposDatos = new SelectList(categoriaRepository.GetListTipo(), "Tipo", "Tipo");
        }

        public int Id
        {
            get { return ExamenLabClinico.Id; }
            set { ExamenLabClinico.Id = value; }
        }
    }
}