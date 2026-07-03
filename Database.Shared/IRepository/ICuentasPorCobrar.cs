using Database.Shared.Models;
using System.Collections.Generic;
using Database.Shared.Paginacion;
using System;
using System.Linq;
using Database.Shared.DataBindings;

namespace Database.Shared.IRepository
{
    public interface ICuentasPorCobrar
    {
        IList<CuentaPorCobrar> GetList();
        CuentaPorCobrar Add(CuentaPorCobrar model);
        void AddPago(Pagos pago);
        Pagos GetPago(int pagoId);
        void Update(CuentaPorCobrar model);
        void Update(Pagos pago);
        public void Update(DetalleCuentaPorCobrar model);
        CuentaPorCobrar Get(int cuentaId);
        CuentaPorCobrar GetCuenta(int cuentaId, bool includePaciente = false);
        List<Pagos> GetPagos(int cuentaId);
        public IList<FormaPago> GetFormasPago();
        public IList<Moneda> GetMonedas();
        public CuentaPorCobrar GetUltimaCuentaPendientePaciente(int pacienteId);

        IEnumerable<Pagos> GetPagosPorCuenta(int cuentaId);

    }
}