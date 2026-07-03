using System;
using System.Globalization;

namespace Database.Shared.Models
{
    public class DevolucionMedicamento
    {
        public int Id { get; set; }
        
        // Información del producto devuelto
        public int HospitalizacionId { get; set; }
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; }
        public int CantidadDevuelta { get; set; }
        public string MotivoDevolucion { get; set; }
        
        // Información del estado de la devolución
        // Estados posibles: "En espera", "Aprobada", "Rechazada"
        public string Estado { get; set; } = "En espera";
        public string TipoProducto { get; set; }  // Medicamento, Insumo, Equipo Médico

        // Información del usuario que realiza la devolución
        public string UsuarioSolicitanteId { get; set; }
        public string UsuarioSolicitanteNombre { get; set; }
        public DateTime FechaSolicitud { get; set; } = DateTime.Now;
        
        // Información del proceso de aprobación/rechazo de la devolución
        public string UsuarioAprobadorId { get; set; }  // Usuario que procesa la devolución
        public string UsuarioAprobadorNombre { get; set; }  // Nombre completo del usuario aprobador
        public DateTime? FechaAprobacion { get; set; }    // Fecha cuando se aprueba o rechaza la devolución

        // Nueva propiedad y FK para la relación con HospitalizacionProductoAplicacion
        public int HospitalizacionProductoAplicacionId { get; set; } // Nueva Propiedad 
        public HospitalizacionProductoAplicacion HospitalizacionProductoAplicacion { get; set; } // Nueva FK

        // Relaciones (Opcionales, si se usan Foreign Keys en la base de datos)
        public Hospitalizacion Hospitalizacion { get; set; }
        public Producto Producto { get; set; }

        // Propiedades calculadas para formatear las fechas de solicitud y aprobación
        public string FechaSolicitudFormatted => FechaSolicitud.ToString("dddd, dd MMMM yyyy - hh:mm tt", new CultureInfo("es-ES"));
        public string FechaAprobacionFormatted => FechaAprobacion?.ToString("dddd, dd MMMM yyyy - hh:mm tt", new CultureInfo("es-ES")) ?? "No procesada";
    }

    public class CheckReturnRequest
    {
        public int HospitalizacionId { get; set; }
        public int ProductoId { get; set; }
    }
}
