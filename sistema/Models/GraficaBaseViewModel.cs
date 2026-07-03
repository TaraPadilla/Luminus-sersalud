using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.Models;
using Database.Shared.IRepository;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;


namespace sistema.Models
{
    public class GraficaBaseViewModel
    {
        //Filtro de año para ventas
        public int TabGeneralVentasComprasGananciasAnnio { get; set; }


        public List<string> Meses { get; set; }
        public SelectList Annios { get; set; }
        public List<VentaTotalViewModel> VentasTotales { get; set; }
        public List<VentaClinicaViewModel> VentasClinica { get; set; }
        public List<VentaFarmaciaViewModel> VentasFarmacia { get; set; }
        public List<VentaLaboratorioViewModel> VentasLaboratorio { get; set; }

        public List<GraficaVentasMesViewModel> VentaMeses { get; set; }

        public List<decimal> MontoTotalxMes { get; set; }

        public GraficaBaseViewModel()
        {
            Meses = new List<string>
        {
            "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
            "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
        };
            var listaAnnios = new List<int>();
            var annio = DateTime.Today.Year;
            for (int i = 0; i < 50; i++)
            {
                listaAnnios.Add(annio - i);
            }
            Annios = new SelectList(listaAnnios);
        }
    }

    public class VentaClinicaViewModel
    {
        public int Mes { get; set; }
        public decimal MontoTotal { get; set; }
    }

    public class VentaFarmaciaViewModel
    {
        public int Mes { get; set; }
        public decimal MontoTotal { get; set; }
    }

    public class VentaLaboratorioViewModel
    {
        public int Mes { get; set; }
        public decimal MontoTotal { get; set; }
    }

    public class VentaTotalViewModel
    {
        public int Mes { get; set; }
        public decimal MontoTotal { get; set; }
    }

    public class MontoViewModel
    {
        public decimal Monto { get; set; }
    }

    public class GraficaVentasMesViewModel
    {
        public string Name { get; set; }
        public decimal y { get; set; }

        public string drilldown { get; set; }

    }

    public class GraficaVentasDiaViewModel
    {
        public string Name { get; set; }
        public string Id { get; set; }

        public List<DataPoint> Data { get; set; }

    }

    public class DataPoint
    {
        public int X { get; set; }
        public decimal Y { get; set; }
    }

    public class GraficaPagosMesViewModel
    {
        public string Name { get; set; }
        public decimal y { get; set; }

        public string drilldown { get; set; }

    }

    public class GraficaIngresosMesViewModel
    {
        public string Name { get; set; }
        public decimal y { get; set; }

        public string drilldown { get; set; }

    }


    public class GraficaGastosMesViewModel
    {
        public string Name { get; set; }
        public decimal y { get; set; }

        public string drilldown { get; set; }

    }

    public class GraficaRangoFechaAñoViewModel
    {
        public string Name { get; set; }
        public List<decimal> Data { get; set; }



    }
}
