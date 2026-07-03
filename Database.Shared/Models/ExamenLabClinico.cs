using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Database.Shared.Models
{
    public class ExamenLabClinico
    {

        public ExamenLabClinico()
        {
            EmergenciaDetalles = new List<EmergenciaDetalle>();
            DetalleExamens = new List<DetalleExamen>();
            DatosExamenesLabClinicos = new List<DatosExamenesLabClinico>();
            ExamenLabClinicosPrecios = new List<ExamenLabClinicoPrecio>();
            ExamenLabClinicosPreguntas = new List<ExamenLabClinicoPregunta>();
            DetalleVentas = new List<DetalleVenta>();
        }
        public int Id { get; set; }
        public int CategoriaLabClinicoId { get; set; }
        public int? CategoriaGeneralLabClinicoId { get; set;}

        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        public string NombreExamen { get; set; }
        public string CodigoInterno { get; set; }
        public decimal Precio { get; set; }
        public decimal PrecioB { get; set; }
        public decimal PrecioC { get; set; }
        public string Indicaciones { get; set; }
        public DateTime FechaCreacion { get; set; }
        public String UltimaModificacion { get; set; }
        public bool Eliminado { get; set; }
        public decimal PrecioCosto { get; set; }
        public string TipoDeExamen { get; set; }
        public bool Activo { get; set; } = true;
        public string Instrucciones { get; set; }
        public string Advertencias { get; set; }
        public string DuracionHoras { get; set; }
        public string DuracionMinutos { get; set; }
        public string DeclaracionConsentimiento { get; set; }

        public CategoriaLabClinico CategoriaLabClinico { get; set; }

        public CategoriaGeneralLabClinico CategoriaGeneralLabClinico { get; set; }
        public ICollection<DetalleExamen> DetalleExamens { get; set; }
        public ICollection<EmergenciaDetalle> EmergenciaDetalles { get; set; }
        public ICollection<DetalleVenta> DetalleVentas { get; set; }
        public ICollection<DatosExamenesLabClinico> DatosExamenesLabClinicos { get; set; }
        public ICollection<ExamenLabClinicoPrecio> ExamenLabClinicosPrecios { get; set; }
        public ICollection<ExamenLabClinicoInsumo> ExamenLabClinicoInsumo { get; set; }
        public ICollection<ExamenLabClinicoPregunta> ExamenLabClinicosPreguntas { get; set; }
    }
}