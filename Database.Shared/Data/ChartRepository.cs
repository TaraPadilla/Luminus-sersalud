using Database.Shared.Models;
using Database.Shared.IRepository;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Paginacion;
using System;
using Database.Shared.DataBindings;


namespace Database.Shared.Data
{
    public class ChartRepository : IChart
    {

        private readonly Context _context = null;

        public ChartRepository(Context context)
        {
            _context = context;
        }

        public IEnumerable<PorNombreMesYAnioModel> TotalIngresoVentasPorMesClinica()
        {
            var ventas = _context.Ventas
            .Where(a => a.TipoVenta == "Clinica")
            .Include(a => a.DetalleVenta).AsEnumerable();
            
            return ventas.Where(a => a.Eliminado == false)
            .GroupBy(a => new {
                Month = a.FechaVenta.Month,
                Year = a.FechaVenta.Year,
            })
            .Select(q => new PorNombreMesYAnioModel{
                Month = q.Key.Month,
                Year = q.Key.Year,
                // Total = q.SelectMany(a=>a.DetalleVenta).Sum(a => a.Total)
                Total = q.Sum(a => a.MontoPago)

            }).OrderBy(a => a.Month)
            .AsEnumerable();
                
        }

        public IEnumerable<PorNombreMesYAnioModel> TotalIngresoVentasPorMesFarmacia()
        {
            var ventas = _context.Ventas
            .Where(a => a.TipoVenta == "Farmacia")
            .Include(a => a.DetalleVenta).AsEnumerable();
            
            return ventas.Where(a => a.Eliminado == false)
            .GroupBy(a => new {
                Month = a.FechaVenta.Month,
                Year = a.FechaVenta.Year,
            })
            .Select(q => new PorNombreMesYAnioModel{
                Month = q.Key.Month,
                Year = q.Key.Year,
                // Total = q.SelectMany(a=>a.DetalleVenta).Sum(a => a.Total)
                Total = q.Sum(a => a.MontoPago)

            }).OrderBy(a => a.Month)
            .AsEnumerable();
                
        }
        
    }
}