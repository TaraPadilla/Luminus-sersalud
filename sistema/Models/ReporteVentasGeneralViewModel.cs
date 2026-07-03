// =============================================================================
// ARCHIVO: Models/ReporteVentasGeneralViewModel.cs
// =============================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace sistema.Models
{
    public class ReporteVentasGeneralViewModel
    {
        public decimal TotalGlobalCobrado { get; set; }

        public bool MostrarConsolidadoMedicos { get; set; }

        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public string UsuarioGenera { get; set; } = "Sistema";
        public DateTime FechaGeneracion { get; set; } = DateTime.Now;

        public int? AmbienteIdFiltro { get; set; }
        public int? EmpleadoIdFiltro { get; set; }
        public string EmpleadoNombre { get; set; }

        public List<ReporteVentasAmbienteViewModel> Ambientes { get; set; } = new();

        // Totales globales
        public decimal TotalGlobalVentas => Ambientes.Sum(a => a.TotalBruto);
        public decimal TotalGlobalDescuentos => Ambientes.Sum(a => a.TotalDescuentos);
        public decimal TotalGlobalNeto => Ambientes.Sum(a => a.TotalNeto);
        public int TotalGlobalTransacciones => Ambientes.Sum(a => a.CantidadVentas);

        // Totales globales por categoría
        public decimal TotalGlobalProductos => Ambientes.Sum(a => a.TotalProductos);
        public decimal TotalGlobalServicios => Ambientes.Sum(a => a.TotalServicios);
        public decimal TotalGlobalExamenes => Ambientes.Sum(a => a.TotalExamenes);
        public decimal TotalGlobalHospital => Ambientes.Sum(a => a.TotalHospital);

        public string RangoFechasTexto =>
            $"{Desde:dd/MM/yyyy} al {Hasta:dd/MM/yyyy}";

        public bool MostrarDetalle { get; set; } = false;
        public List<ResumenFormaPagoViewModel> TotalGlobalFormasPago { get; set; }
            = new List<ResumenFormaPagoViewModel>();

        public List<ConsolidadoMedicoViewModel> ConsolidadoMedicos { get; set; } = new();

        public List<ItemConsolidadoViewModel> ItemsConsolidados { get; set; } = new();

    }

    // ── Bloque por ambiente ────────────────────────────────────────────────────
    public class ReporteVentasAmbienteViewModel
    {
        public decimal TotalCobrado { get; set; }

        public int AmbienteId { get; set; }
        public string AmbienteNombre { get; set; } = "";

        public List<ReporteVentaItemViewModel> Ventas { get; set; } = new();

        // Subtotales del ambiente
        public decimal TotalBruto => Ventas.Sum(v => v.MontoTotal);
        public decimal TotalDescuentos => Ventas.Sum(v => v.TotalDescuento);
        public decimal TotalNeto => TotalBruto - TotalDescuentos;
        public int CantidadVentas => Ventas.Count;

        // Subtotales por categoría dentro del ambiente
        public decimal TotalProductos => Ventas.Sum(v => v.SubtotalProductos);
        public decimal TotalServicios => Ventas.Sum(v => v.SubtotalServicios);
        public decimal TotalExamenes => Ventas.Sum(v => v.SubtotalExamenes);
        public decimal TotalHospital => Ventas.Sum(v => v.SubtotalHospital);

        public List<ResumenFormaPagoViewModel> ResumenFormasPago { get; set; } = new();

        public List<ReporteVentaDetalleItemViewModel> DetalleItems { get; set; }
    = new List<ReporteVentaDetalleItemViewModel>();
    }

    // ── Fila individual de venta ───────────────────────────────────────────────
    public class ReporteVentaItemViewModel
    {
        public int VentaId { get; set; }
        public DateTime FechaVenta { get; set; }
        public string NumeroComprobante { get; set; } = "";
        public string ClienteNombre { get; set; } = "Consumidor Final";
        public string ClienteNit { get; set; } = "CF";
        public string EmpleadoNombre { get; set; } = "";
        public string TipoVenta { get; set; } = "";
        public string Origen { get; set; } = "";

        public decimal MontoTotal { get; set; }
        public decimal TotalDescuento { get; set; }
        public decimal MontoNeto => MontoTotal - TotalDescuento;

        // Desglose por categoría de ítem
        // Productos = BienOServicio "B" (medicamentos, insumos, farmacia)
        public decimal SubtotalProductos { get; set; }
        // Servicios = BienOServicio "S" con ServicioId (consultas, procedimientos)
        public decimal SubtotalServicios { get; set; }
        // Exámenes  = BienOServicio "S" con ExamenLabClinicoId
        public decimal SubtotalExamenes { get; set; }
        // Hospital  = TipoVenta "Hospitalizacion" (estadia, paquetes, cargos hospi)
        public decimal SubtotalHospital { get; set; }

        public List<string> FormasPagoTexto { get; set; } = new();
        public string FormasPagoResumen => string.Join(" / ", FormasPagoTexto.Distinct());
        public List<ReporteVentaDetalleItemViewModel> DetalleItems { get; set; }
       = new List<ReporteVentaDetalleItemViewModel>();

        public decimal CantidadItemsProductos { get; set; }
        public decimal CantidadItemsServicios { get; set; }
        public decimal CantidadItemsExamenes { get; set; }
        public decimal CantidadItemsHospital { get; set; }

        public bool EsSalaOperaciones { get; set; }
        public List<ReporteMedicoViewModel> MedicosAsignados { get; set; } = new();
    }

    // ── Resumen por forma de pago ──────────────────────────────────────────────
    public class ResumenFormaPagoViewModel
    {
        public string FormaPago { get; set; } = "";
        public decimal Total { get; set; }
    }

    public class ReporteVentaDetalleItemViewModel
    {
        /// <summary>Nombre / descripción del producto, servicio, examen, paquete o dieta.</summary>
        public string Descripcion { get; set; } = string.Empty;

        /// <summary>Categoría calculada: "Producto", "Servicio", "Examen/Lab" o "Hospital".</summary>
        public string Categoria { get; set; } = string.Empty;

        /// <summary>Cantidad vendida.</summary>
        public decimal Cantidad { get; set; }

        /// <summary>Precio unitario neto (subtotal / cantidad).</summary>
        public decimal PrecioUnit { get; set; }

        /// <summary>Descuento aplicado a esta línea.</summary>
        public decimal Descuento { get; set; }

        /// <summary>Subtotal neto de la línea.</summary>
        public decimal Subtotal { get; set; }
    }

    public class ReporteMedicoViewModel
    {
        /// <summary>Rol: "Tratante" | "Ingreso" | "Secundario"</summary>
        public string Rol { get; set; }
        public string Nombre { get; set; }
        public string Especialidad { get; set; }   // puede ser null/vacío
    }

    public class ConsolidadoMedicoViewModel
    {
        public string MedicoNombre { get; set; }
        public int CantidadVentas { get; set; }

        // Totales monetarios
        public decimal TotalProductos { get; set; }
        public decimal TotalServicios { get; set; }
        public decimal TotalExamenes { get; set; }
        public decimal TotalHospital { get; set; }
        public decimal TotalDescuentos { get; set; }
        public decimal TotalNeto { get; set; }

        // Cantidades de ítems (unidades/piezas)
        public decimal CantidadProductos { get; set; }
        public decimal CantidadServicios { get; set; }
        public decimal CantidadExamenes { get; set; }
        public decimal CantidadHospital { get; set; }
public int CantidadCitas { get; set; }
public int CantidadHospitalizaciones { get; set; }
public int CantidadConsultaExterna { get; set; }
public int CantidadSalaOperaciones { get; set; }


    }

    public class ItemConsolidadoViewModel
    {
        public string Categoria { get; set; } = "";
        public string Nombre { get; set; } = "";
        public decimal Cantidad { get; set; }
        public decimal Total { get; set; }

        public decimal? CostoUnitario { get; set; }   // Precio costo unitario
        public decimal CostoTotal { get; set; }
    }
}

