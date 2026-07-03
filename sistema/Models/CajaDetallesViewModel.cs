using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Database.Shared.Paginacion;
using System.ComponentModel.DataAnnotations;
using System;

namespace sistema.Models
{
    public class CajaDetallesViewModel
    {
        public DateTime FechaApertura { get; set; }
        public string ResponsableApertura { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string ResponsableCierre { get; set; }
        public decimal MontoApertura { get; set; }
        public string NombrePersonalizado { get; set; }
        public List<Sucursal> ListaSucursales { get; set; }
        public int Id { get; set; }
        public bool EstadoCaja { get; set; }
        public int? SucursalId { get; set; }
        public string SucursalNombre { get; set; }
        public int AmbienteId { get; set; }
        public string AmbienteNombre { get; set; }
        public List<CajaDetallesVentaViewModel> Ventas { get; set; }
        public List<CajaDetallesMontoViewModel> Montos { get; set; }
        public List<CajaDetallesIngresoViewModel> Ingresos { get; set; }
        public List<CajaDetallesEgresoViewModel> Egresos { get; set; }
        public List<CajaDetallesCompraViewModel> Compras { get; set; }
        public List<CajaDetallesSubcajaViewModel> Subcajas { get; set; }

        //Totales de caja
        //TotalIngresos incluye INGRESOS y VENTAS
        public decimal TotalIngresosCaja { get; set; }
        //TotalEgresos incluye EGRESOS y COMPRAS
        public decimal TotalEgresosCaja { get; set; }
        public decimal TotalComprasCredito { get; set; }

        public string Origen { get; set; } // Agregamos el origen como string
        public decimal TotalCierreCaja
        {
            get
            {
                return MontoApertura + TotalIngresosCaja - TotalEgresosCaja;
            }
        }
    }
}