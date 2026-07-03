using Database.Shared.Models;
using System.Collections.Generic;
using Database.Shared.Paginacion;
using System;
using System.Linq;
using Database.Shared.DataBindings;

namespace Database.Shared.IRepository
{
    public interface IChart
    {
        // List<PorNombreMesYAnioModel> TotalIngresoVentasPorMes();
        // IEnumerable<dynamic> TotalIngresoVentasPorMes();
        // IQueryable<dynamic> TotalIngresoVentasPorMes();
        // public IEnumerable<dynamic> TotalIngresoVentasPorMes();
        IEnumerable<PorNombreMesYAnioModel> TotalIngresoVentasPorMesClinica();
        IEnumerable<PorNombreMesYAnioModel> TotalIngresoVentasPorMesFarmacia();


    }
}