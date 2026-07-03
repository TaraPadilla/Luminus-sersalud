using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    [Table("Requision")]
    public class Requision
    {
        public Requision()
        {
            RequisionDetalles = new HashSet<RequisionDetalle>();
            RequisionHistoriales = new HashSet<RequisionHistorial>();
        }

        [Key]
        public int Id { get; set; }

        // Encabezado (maqueta)
        [MaxLength(100)]
        public string? Direccion { get; set; }

        [MaxLength(100)]
        public string? Departamento { get; set; }

        [MaxLength(60)]
        public string? UnidadSeccion { get; set; }

        public string? Otros { get; set; }

        public int? NumeroRequisicion { get; set; } // UNIQUE en BD
        public int? NumeroOrden { get; set; }       // UNIQUE en BD

        public DateTime FechaSolicitud { get; set; }

        // Ubicaciones
        public int BodegaOrigenId { get; set; }
        public int BodegaDestinoId { get; set; }

        // Observaciones
        [MaxLength(500)]
        public string? Observaciones { get; set; }

        // Firmas / nombres (fase 1 texto)
        [MaxLength(60)]
        public string? SolicitanteNombre { get; set; }

        [MaxLength(60)]
        public string? JefaturaNombre { get; set; }

        [MaxLength(60)]
        public string? GerenciaNombre { get; set; }

        [MaxLength(60)]
        public string? EncargadoAlmacenNombre { get; set; }

        [MaxLength(60)]
        public string? RecibeNombre { get; set; }

        // Flujo (int en BD; puedes mapear al enum en capa sistema.Models si quieres)
        public int Estado { get; set; } = 1;

        // Auditoría simple
        public DateTime CreadoEn { get; set; } = DateTime.Now;

        [MaxLength(60)]
        public string? CreadoPor { get; set; }

        // Navegación (si existen estas entidades en tu BD shared models)
        [ForeignKey(nameof(BodegaOrigenId))]
        public virtual Bodega? BodegaOrigen { get; set; }

        [ForeignKey(nameof(BodegaDestinoId))]
        public virtual Bodega? BodegaDestino { get; set; }


        public string? VOBOJefatura { get; set; }

        public string? AutorizacionGerencia { get; set; }

        public string? AutorizacionAlmacen { get; set; }

        public string? FirmaSolicitante { get; set; }


        public string? EntregadoNombre { get; set; }

        public virtual ICollection<RequisionDetalle> RequisionDetalles { get; set; }
        public virtual ICollection<RequisionHistorial> RequisionHistoriales { get; set; }
    }

    [Table("RequisionDetalle")]
    public class RequisionDetalle
    {
        [Key]
        public int Id { get; set; }

        public int RequisionId { get; set; }

        public int ProductoInventarioId { get; set; }

        public int CantidadSolicitada { get; set; } = 0;

        public int? CantidadDespachada { get; set; }

        public DateTime CreadoEn { get; set; } = DateTime.Now;

        [ForeignKey(nameof(RequisionId))]
        public virtual Requision? Requision { get; set; }

        [ForeignKey(nameof(ProductoInventarioId))]
        public virtual ProductoInventario? ProductoInventario { get; set; }
    }

    [Table("RequisionHistorial")]
    public class RequisionHistorial
    {
        [Key]
        public int Id { get; set; }

        public int RequisionId { get; set; }

        public int? EstadoAnterior { get; set; }

        public int EstadoNuevo { get; set; }

        public string? Observacion { get; set; }

        public DateTime CreadoEn { get; set; } = DateTime.Now;

        [MaxLength(60)]
        public string? CreadoPor { get; set; }

        [ForeignKey(nameof(RequisionId))]
        public virtual Requision? Requision { get; set; }
    }
}
