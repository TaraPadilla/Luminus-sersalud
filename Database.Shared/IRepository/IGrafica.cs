using Database.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static Database.Shared.Data.GraficaRepository;

namespace Database.Shared.IRepository
{
    public interface IGrafica
    {
        List<Venta> GetVentasClinica();
        List<Venta> GetVentasFarmacia();
        //List<VentasLab> GetVentaLaboratorio();

        public List<decimal> GetVentasGenerales();
        public List<VentasDia> GetVentasGeneralesxDia();

        public List<decimal> GetPagosGenerales();

        public List<PagosDia> GetPagosGeneralesxDia();

        public List<decimal> GetIngresosGenerales();

        public List<IngresosDia> GetIngresosGeneralesxDia();

        public List<decimal> GetGastosGenerales();

        public List<decimal> GetIngresosVentasGenerales();

        public List<IngresosVentasDia> GetIngresosVentasGeneralesxDia();

        public List<GastosDia> GetgastosGeneralesxDia();

        public List<decimal> GetGastosComprasGenerales();

        public List<GastosComprasDia> GetGastosComprasGeneralesxDia();
 

    }
        
}
