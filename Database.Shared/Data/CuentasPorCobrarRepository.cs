using Database.Shared.Models;
using Database.Shared.IRepository;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Paginacion;
using System;

namespace Database.Shared.Data
{
    public class CuentasPorCobrarRepository : ICuentasPorCobrar
    {
        private readonly Context _context = null;

        public CuentasPorCobrarRepository(Context context)
        {
            _context = context;
        }

        public IList<CuentaPorCobrar> GetList()
        {
            return _context.CuentasPorCobrar
                .Include(a => a.Paciente)
                .Where(a => a.Eliminada == false)
                .ToList();
        }

        public CuentaPorCobrar Add(CuentaPorCobrar model)
        {
            _context.CuentasPorCobrar.Add(model);
            _context.SaveChanges();
            return model;
        }

        public void AddPago(Pagos pago)
        {
            _context.Pagos.Add(pago);
            _context.SaveChanges();
        }

        public Pagos GetPago(int pagoId)
        {
            return _context.Pagos.Find(pagoId);
        }

        public CuentaPorCobrar Get(int cuentaId)
        {
            return _context.CuentasPorCobrar
                .Include(a => a.Paciente).ThenInclude(a => a.Sexo)
                .Include(a => a.DetallesCuentaPorCobrar)
                .Where(a => a.Id == cuentaId).SingleOrDefault();
        }

        public CuentaPorCobrar GetCuenta(int cuentaId, bool includePaciente = false)
        {
            // ✅ FIX: si se requiere paciente, usar Include real
            IQueryable<CuentaPorCobrar> query = _context.CuentasPorCobrar;

            if (includePaciente)
            {
                query = query.Include(c => c.Paciente);
            }

            return query
                .Where(a => a.Id == cuentaId)
                .FirstOrDefault();
        }

        public List<Pagos> GetPagos(int cuentaId)
        {
            return _context.Pagos
                .Include(a => a.FormaPago)
                .Include(a => a.Moneda)
                .Where(a => a.CuentaPorCobrarId == cuentaId)
                .ToList();
        }

        public void Update(CuentaPorCobrar model)
        {
            _context.Entry(model).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Update(DetalleCuentaPorCobrar model)
        {
            _context.Entry(model).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Update(Pagos pago)
        {
            _context.Entry(pago).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public IList<FormaPago> GetFormasPago()
        {
            return _context.FormaPagos.ToList();
        }

        public IList<Moneda> GetMonedas()
        {
            return _context.Monedas.ToList();
        }

        public CuentaPorCobrar GetUltimaCuentaPendientePaciente(int pacienteId)
        {
            return _context.CuentasPorCobrar
                .Include(a => a.DetallesCuentaPorCobrar)
                .Where(c => c.PacienteId == pacienteId && !c.Eliminada && !c.Pagada)
                .FirstOrDefault();
        }

        public IEnumerable<Pagos> GetPagosPorCuenta(int cuentaId)
        {
            return _context.Pagos
                .Where(p => p.CuentaPorCobrarId == cuentaId)
                .OrderByDescending(p => p.FechaHora)
                .ToList();
        }
    }
}
