using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Database.Shared.Data
{
    public class DevolucionNacionalRepository : IDevolucionNacional
    {
        private readonly Context _context;

        public DevolucionNacionalRepository(Context context)
        {
            _context = context;
        }

        public async Task<DevolucionNacional> CrearAsync(DevolucionNacional devolucion, string? usuario)
        {
            if (devolucion == null) throw new ArgumentNullException(nameof(devolucion));

            await using var tx = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                devolucion.NumeroDevolucion ??= await SiguienteNumeroDevolucionAsync();
                devolucion.FechaSolicitud      = DateTime.Now;
                devolucion.CreadoEn            = DateTime.Now;
                devolucion.CreadoPor           = usuario;

                var detalles = devolucion.Detalles.ToList();
                devolucion.Detalles.Clear();

                _context.DevolucionNacional.Add(devolucion);
                await _context.SaveChangesAsync();

                foreach (var detalle in detalles)
                {
                    detalle.Id                   = 0;
                    detalle.DevolucionNacionalId = devolucion.Id;
                    _context.DevolucionNacionalDetalle.Add(detalle);
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();
                return devolucion;
            }
            catch { await tx.RollbackAsync(); throw; }
        }

        public async Task<DevolucionNacional?> GetByIdAsync(int id)
        {
            return await _context.DevolucionNacional
                .Include(d => d.BodegaOrigen)
                .Include(d => d.Proveedor)         
                .Include(d => d.Detalles)
                    .ThenInclude(det => det.ProductoInventario)
                        .ThenInclude(pi => pi.Producto)
                .Include(d => d.Detalles)
                    .ThenInclude(det => det.ProductoInventario)
                        .ThenInclude(pi => pi.UnidadMedidaVenta)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<bool> CambiarEstadoAsync(int devolucionId, int nuevoEstado, string? usuario, string? observacion)
        {
            var dev = await _context.DevolucionNacional.FirstOrDefaultAsync(x => x.Id == devolucionId);
            if (dev == null) return false;
            dev.Estado = nuevoEstado;
            await _context.SaveChangesAsync();
            return true;
        }

        public int? ObtenerUltimoRegistro()
        {
            return _context.DevolucionNacional
                .OrderByDescending(x => x.Id)
                .Select(x => x.NumeroDevolucion)
                .FirstOrDefault();
        }

        public async Task ActualizarFirmaAsync(DevolucionNacional devolucion)
        {
            _context.DevolucionNacional.Update(devolucion);
            await _context.SaveChangesAsync();
        }

        public async Task<List<DevolucionListaDTO>> GetAllAsync()
        {
            return await _context.DevolucionNacional
                .Include(x => x.BodegaOrigen)
                .OrderByDescending(x => x.Id)
                .Select(x => new DevolucionListaDTO
                {
                    Id                 = x.Id,
                    NumeroDevolucion   = x.NumeroDevolucion  ?? 0,
                    FechaSolicitud     = x.FechaSolicitud    ?? DateTime.Now,
                    Estado             = x.Estado,
                    DepartamentoNombre = x.Departamento      ?? "N/A",
                    UnidadNombre       = x.UnidadSeccion     ?? "N/A",
                    SolicitanteNombre  = x.SolicitanteNombre ?? "N/A",
                    BodegaOrigenNombre = x.BodegaOrigen != null ? x.BodegaOrigen.NombreBodega : "N/A",
                    AutorizadoPor      = x.AutorizadoPor     ?? ""

                })
                .ToListAsync();
        }

        private async Task<int> SiguienteNumeroDevolucionAsync() =>
            (await _context.DevolucionNacional.MaxAsync(x => (int?)x.NumeroDevolucion) ?? 0) + 1;
    }
}