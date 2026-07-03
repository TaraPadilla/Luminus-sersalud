using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class Bodega
    {
        public Bodega()
        {
            ProductosInventario = new List<ProductoInventario>();
        }

        public int Id { get; set; }
        public string NombreBodega { get; set; }
        public int? TipoBodegaId { get; set; }
        public TipoBodega TipoBodega { get; set; }
        public int? SucursalId { get; set; }
        public Sucursal Sucursal { get; set; }
        public int? AmbienteId { get; set; }
        public Ambiente Ambiente { get; set; }
        public bool Eliminada { get; set; }
        public ICollection<ProductoInventario> ProductosInventario { get; set; }
        [InverseProperty("BodegaOrigen")]
        public ICollection<TrasladosProductos> TrasladosOrigen { get; set; }
        [InverseProperty("BodegaDestino")]
        public ICollection<TrasladosProductos> TrasladosDestino { get; set; }
        public string BodegaSucursalText
        {
            get
            {
                var sucursalNombre = Sucursal != null ? Sucursal.NombreSucursal : "N/A";
                var ambienteNombre = Ambiente != null ? Ambiente.NombreAmbiente : "N/A";
                return $"Sucursal: {sucursalNombre} - Ambiente: {ambienteNombre} - Bodega: {NombreBodega}";
            }
        }
    }
}