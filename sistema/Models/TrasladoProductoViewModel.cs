using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;
using System;

namespace sistema.Models
{

    public class TrasladoProductoViewModel
    {
        public int? DetalleTrasladoId { get; set; }
        public int? ProductoInventarioId { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public string ProductoCodigo { get; set; }
        public string UnidadMedidaVentaNombre { get; set; }
        public int UnidadMedidaVentaId { get; set; }
        public decimal CantidadExistente { get; set; }
        public decimal CantidadTrasladada { get; set; }
        public bool Nuevo { get; set; }
        public DateTime FechaTraslado { get; set; }
        public decimal? CantidadDespachada { get; set; }
        public decimal? Diferencia
        {
            get
            {
                // Diferencia = Solicitado - Despachado
                // Si no hay despachado aún, se devuelve null para indicar "pendiente".
                if (!CantidadDespachada.HasValue)
                    return null;

                return CantidadTrasladada - CantidadDespachada.Value;
            }
        }

        // Para RFID / Etiqueta (la vista intenta leer TagRfid/Etiqueta).
        // Lo dejamos listo en el VM para que, cuando el backend lo envíe, el UI lo muestre sin hacks.
        public string TagRfid { get; set; }
        public string Etiqueta { get; set; }

    }

}