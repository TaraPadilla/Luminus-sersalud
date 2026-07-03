using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Database.Shared.Data
{
    public class KitIngresoRepository : IKitIngreso
    {
        private readonly Context _context;

        public KitIngresoRepository(Context context)
        {
            _context = context;
        }

        // ---------- Kits ----------
        public void Add(KitIngreso kitIngreso)
        {
            _context.KitsIngreso.Add(kitIngreso);
            _context.SaveChanges();
        }

        public KitIngreso GetById(int id)
        {
            return _context.KitsIngreso
                .Include(k => k.Detalles)
                .FirstOrDefault(k => k.Id == id);
        }
        public IEnumerable<KitIngreso> GetGlobalKits()
        {
            return _context.KitsIngreso
                .Include(k => k.Detalles.Where(d => !d.Eliminado)) // Solo detalles no eliminados
                .Where(k => k.HospitalizacionId == null)
                .OrderByDescending(k => k.FechaRegistro)
                .ToList();
        }

        public IEnumerable<KitIngreso> GetByHospitalizacionId(int hospitalizacionId)
        {
            return _context.KitsIngreso
                .Include(k => k.Detalles)
                .Where(k => k.HospitalizacionId == hospitalizacionId)
                .OrderByDescending(k => k.FechaRegistro)
                .ToList();
        }

        public void UpdateKit(KitIngreso kit)
        {
            _context.KitsIngreso.Update(kit);
            _context.SaveChanges();
        }

        // ---------- Detalles ----------
        public void AddDetalle(KitIngresoDetalle detalle)
        {
            _context.KitsIngresoDetalles.Add(detalle);
            _context.SaveChanges();
        }

        public KitIngresoDetalle GetDetalleById(int detalleId)
        {
            return _context.KitsIngresoDetalles.Find(detalleId);
        }

        public void UpdateDetalle(KitIngresoDetalle detalle)
        {
            _context.KitsIngresoDetalles.Update(detalle);
            _context.SaveChanges();
        }

        public void EliminarDetalle(int detalleId)
        {
            var detalle = _context.KitsIngresoDetalles.Find(detalleId);
            if (detalle != null)
            {
                detalle.Eliminado = true;
                _context.SaveChanges();
            }
        }

        public IEnumerable<KitIngresoDetalle> GetDetallesByKitId(int kitIngresoId)
        {
            return _context.KitsIngresoDetalles
                .Where(d => d.KitIngresoId == kitIngresoId)
                .ToList();
        }

        // ---------- Consumos ----------
        public async Task<decimal> ObtenerUtilizadoPorDetalleYHospitalizacionAsync(int detalleId, int hospitalizacionId)
        {
            var consumo = await _context.Set<HospitalizacionKitConsumo>()
                .FirstOrDefaultAsync(c => c.KitIngresoDetalleId == detalleId && c.HospitalizacionId == hospitalizacionId);
            return consumo?.Utilizado ?? 0;
        }

        public async Task GuardarConsumoAsync(int detalleId, int hospitalizacionId, decimal utilizado)
        {
            var consumo = await _context.Set<HospitalizacionKitConsumo>()
                .FirstOrDefaultAsync(c => c.KitIngresoDetalleId == detalleId && c.HospitalizacionId == hospitalizacionId);
            if (consumo == null)
            {
                consumo = new HospitalizacionKitConsumo
                {
                    KitIngresoDetalleId = detalleId,
                    HospitalizacionId = hospitalizacionId,
                    Utilizado = utilizado,
                    FechaRegistro = DateTime.Now
                };
                _context.Set<HospitalizacionKitConsumo>().Add(consumo);
            }
            else
            {
                consumo.Utilizado = utilizado;
                consumo.FechaRegistro = DateTime.Now;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<HospitalizacionKitConsumo>> ObtenerConsumosPorHospitalizacionAsync(int hospitalizacionId)
        {
            return await _context.Set<HospitalizacionKitConsumo>()
                .Where(c => c.HospitalizacionId == hospitalizacionId)
                .ToListAsync();
        }

        // Implementación de métodos faltantes de la interfaz
        public IEnumerable<KitIngreso> GetGlobalesYPorHospitalizacion(int hospitalizacionId)
        {
            return _context.KitsIngreso
                .Include(k => k.Detalles.Where(d => !d.Eliminado))
                .Where(k => k.HospitalizacionId == null || k.HospitalizacionId == hospitalizacionId)
                .OrderByDescending(k => k.FechaRegistro)
                .ToList();
        }

        public IEnumerable<KitIngreso> GetLocalKits(int hospitalizacionId)
        {
            // Devuelve solo los kits asociados a la hospitalización específica
            return _context.KitsIngreso
                .Include(k => k.Detalles)
                .Where(k => k.HospitalizacionId == hospitalizacionId)
                .OrderByDescending(k => k.FechaRegistro)
                .ToList();
        }
    }
}