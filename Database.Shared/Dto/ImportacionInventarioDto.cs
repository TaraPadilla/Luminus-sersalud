using System.Collections.Generic;

namespace Database.Shared.Dto
{
    // Objeto maestro que guardaremos en la Session temporal
    public class PrevisualizacionInventarioDto
    {
        public string SessionKey { get; set; } 
        public string HashArchivo { get; set; } 
        public int BodegaId { get; set; }
        public int TipoProductoId { get; set; }
        public bool TieneErrores { get; set; }
        public List<FilaImportacionDto> Filas { get; set; } = new List<FilaImportacionDto>();
    }

    // Representa cada fila del Excel procesada y evaluada
    public class FilaImportacionDto
    {
        public int FilaExcel { get; set; }
        public string CodigoReferencia { get; set; }
        public string NombreProducto { get; set; }
        public string Lote { get; set; }
        public decimal StockASumar { get; set; }
        
        // Precios
        public decimal PrecioCompra { get; set; }
        public decimal PrecioPublico { get; set; }
        public decimal PrecioInterno { get; set; }
        public decimal PrecioVip { get; set; }
        
        // Banderas de control y validación
        public bool ProductoExisteEnBd { get; set; }
        public bool LoteExisteEnBd { get; set; } 
        public bool EsFilaValida { get; set; } 
        public string MensajeError { get; set; } 
    }
}