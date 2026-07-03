using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class Servicio
    {
        public Servicio()
        {
            //DetalleServicio = new List<DetalleServicio>();
            DetalleCotizaciones = new List<DetalleCotizacion>();
            EmergenciaDetalles = new List<EmergenciaDetalle>();
            DetalleVentas = new List<DetalleVenta>();
            ServiciosInsumos = new List<ServicioInsumo>();
            HospitalizacionesServicios = new List<HospitalizacionServicio>();
            ServiciosPrecios = new List<ServicioPrecio>();
            CitasServicios = new List<CitasServicio>();
            SucursalServicios = new List<SucursalServicio>();
        }

        public int Id { get; set; }
        public string CodigoInterno { get; set; }
        public int? CategoriaServicioId { get; set; }
        public CategoriaServicio CategoriaServicio { get; set; }

        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        public string NombreServicio { get; set; }

        //[Required(ErrorMessage = "* Este campo es obligatorio.")]
        //[Column(TypeName = "decimal(18,2)")]
        //public decimal Precio { get; set; }

        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio_2 { get; set; }

        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio_3 { get; set; }

        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio_4 { get; set; }
        public string Descripcion { get; set; }
        public int? DuracionHoras { get; set; }
        public int? DuracionMinutos { get; set; }
        public int? PrecioId { get; set; }//Precio para mostrar al publico en reservas
        public Precio Precio { get; set; }
        public bool Eliminado { get; set; }
        //public ICollection<DetalleServicio> DetalleServicio { get; set; }
        public ICollection<DetalleCotizacion> DetalleCotizaciones { get; set; }
        public ICollection<EmergenciaDetalle> EmergenciaDetalles { get; set; }
        public ICollection<DetalleVenta> DetalleVentas { get; set; }
        public ICollection<ServicioInsumo> ServiciosInsumos { get; set; }
        public ICollection<HospitalizacionServicio> HospitalizacionesServicios { get; set; }
        public ICollection<ServicioPrecio> ServiciosPrecios { get; set; }
        public ICollection<CitasServicio> CitasServicios { get; set; }
        public ICollection<SucursalServicio> SucursalServicios { get; set; }

    }
}