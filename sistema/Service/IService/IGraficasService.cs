using Database.Shared.Models;
using Microsoft.AspNetCore.Identity;
using sistema.Models;
using sistema.Models.Graficas;
using System.Collections.Generic;

namespace sistema.Service.IService
{
    public interface IGraficasService
    {
        (List<GraficaColumnSeries> series, List<GraficaColumnSeries> drilldown) TabProductosVentasMensuales(int año, int productoId);
        (List<GraficaColumnSeries> series, List<GraficaColumnSeries> drilldown) TabProductosComparacionPreciosProveedor(int productoId);
        (List<GraficaColumnSeries> series, List<GraficaColumnSeries> drilldown) TabProductosMasVendidos();
        (List<GraficaColumnSeries> series, List<GraficaColumnSeries> drilldown) TabProductosMenosVendidos();
        (List<GraficaColumnSeries> series, List<GraficaColumnSeries> drilldown) TabGeneralGetVentasComprasGanancias(int año);

        (List<GraficaColumnSeries> series, List<GraficaColumnSeries> drilldown) TabGeneralGetCompras(int año);

        (List<GraficaColumnSeries> series, List<GraficaColumnSeries> drilldown) TabTendenciasVentas(int año);

        (List<GraficaColumnSeries> series, List<GraficaColumnSeries> drilldown) TabVentasAnuales(int año);

        (List<GraficaColumnSeries> series, List<GraficaColumnSeries> drilldown) TabVentasPorRango(string fechaRango);

        (List<GraficaColumnSeries> series, List<GraficaColumnSeries> drilldown) TabProductosMasVendidosServiciosSolicitudes(string fechaRango);

        (List<GraficaColumnSeries> series, List<GraficaColumnSeries> drilldown) TabVentasPorMedico();

        (List<GraficaColumnSeries> series, List<GraficaColumnSeries> drilldown) PacientesAtendidosPorServicio();

        (List<GraficaColumnSeries> series, List<GraficaColumnSeries> drilldown) ComprasProveedor();


    }
}
