using System;
using System.Globalization;

namespace Database.Shared.Models
{
    public class SolicitudMedicamento
    {
        public int Id { get; set; }

        // Información del medicamento solicitado
        public int HospitalizacionId { get; set; }
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; }
        public int UnidadMedidaVentaId { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public string Indicaciones { get; set; }
        public string ViaAdministracion { get; set; }
        public string FrecuenciaAdministracion { get; set; }
        public int IdProductoPrecioInventario { get; set; }
        public int PrecioId { get; set; }
        public string FechaHoraAplicacionManual { get; set; }

        // Información del estado de la solicitud
        public string Estado { get; set; } = "En espera";  // 'En espera', 'Despachado', 'Rechazado'
        public string TipoProducto { get; set; }  // Medicamento, Insumo, Equipo Médico

        // Información del proceso de solicitud y despacho
        public string UsuarioSolicitanteId { get; set; }  // Usuario que hace la solicitud
        public string UsuarioSolicitanteNombre { get; set; }  // Usuario que hace la solicitud
        public DateTime FechaSolicitud { get; set; } = DateTime.Now;
        public string UsuarioDespachanteId { get; set; }  // Usuario que despacha la solicitud
        public DateTime? FechaDespacho { get; set; }  // Fecha cuando se despacha

        // Relaciones (Opcionales si no hay Foreign Keys)
        public Hospitalizacion Hospitalizacion { get; set; }
        public Producto Producto { get; set; }

        // Indicador de registro en la hospitalizacion
        public bool EsRegistroHospitalizacion { get; set; } = false;

        // ✅ Propiedad Calculada para Formatear la Fecha de Solicitud
        public string FechaSolicitudFormatted => FechaSolicitud.ToString("dddd, dd MMMM yyyy - hh:mm tt", new CultureInfo("es-ES"));

        // ✅ Propiedad Calculada para Formatear la Fecha de Despacho (si existe)
        public string FechaDespachoFormatted => FechaDespacho?.ToString("dddd, dd MMMM yyyy - hh:mm tt", new CultureInfo("es-ES")) ?? "No despachado";
        public bool Directa { get; set; } = false;

    }
}
