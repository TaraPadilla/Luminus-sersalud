using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class HospitalizacionDetallePaqueteHospitalizacion
    {
        public int Id { get; set; }
        public DateTime FechaHora { get; set; }
        public int HospitalizacionPaqueteHospitalizacionId { get; set; }
        public HospitalizacionPaqueteHospitalizacion HospitalizacionPaqueteHospitalizacion { get; set; }
        public bool Aplicacion { get; set; }
        public string UsuarioAplicacionId { get; set; }

        public User UsuarioAplicacion { get; set; }
        public DateTime? FechaHoraAplicada { get; set; }
        public bool Eliminado { get; set; }
        public int? ServicioId { get; set; }
        public Servicio Servicio { get; set; }
        public int? ProductoId { get; set; }
        public Producto Producto { get; set; }
        public int? LaboratorioId { get; set; }
        public ExamenLabClinico Laboratorio { get; set; }
        public int? LaboratorioPrecioId { get; set; }
        public int? ExamenId { get; set; }
        public Examen Examen { get; set; }
        public ExamenLabClinicoPrecio LaboratorioPrecio { get; set; }
        public int? UnidadMedidaVentaId { get; set; }
        public UnidadMedidaVenta UnidadMedidaVenta { get; set; }
        public decimal PrecioProducto { get; set; }
        public int CantidadAplicada { get; set; }

        public int CantidadExcedida { get; set; }

        public string MotivoDevolucion { get; set; }

    }
}
