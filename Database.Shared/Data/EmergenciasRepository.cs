using System.Collections.Generic;
using System.Linq;
using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Paginacion;

namespace Database.Shared.Data
{
    public class EmergenciasRepository : IEmergencias
    {
        private readonly Context _context = null;
        public EmergenciasRepository(Context context)
        {
            _context = context;
        }
        public void Add(Emergencia emergencia)
        {
            _context.Emergencias.Add(emergencia);
            _context.SaveChanges();
        }
        public Emergencia Get(int emergenciaId,
            bool includePaciente = false,
            bool includeElementos = false)
        {
            var emergenciaBd = _context.Emergencias
                .Include(a => a.Hospitalizacion)
                    .ThenInclude(a => a.Habitacion)
                .Where(a => a.Id == emergenciaId)
                .FirstOrDefault();

            if (emergenciaBd == null)
            {
                return null;
            }

            if (includePaciente)
            {
                emergenciaBd.Paciente = _context.Pacientes
                    .Where(a => a.Id == emergenciaBd.PacienteId)
                    .FirstOrDefault();
            }

            if (includeElementos)
            {
                emergenciaBd.EmergenciaDetalles = _context.EmergenciaDetalles
                    .Include(a => a.Producto)
                    .Include(a => a.Servicio)
                    .Include(a => a.ExamenLabClinico)
                    .Include(a => a.UnidadMedidaVenta)
                    .Include(a => a.Precio)
                    .Where(a => a.EmergenciaId == emergenciaBd.Id
                    && !a.Eliminado)
                    .ToList();
            }
            return emergenciaBd;
        }
        public List<Emergencia> GetEmergencias(bool ingresadas)
        {
            var emergenciasBd = _context.Emergencias
                .Include(a => a.Paciente)
                .Include(a => a.EmergenciaDetalles)
                    .ThenInclude(a => a.Servicio)
                .Include(a => a.EmergenciaDetalles)
                    .ThenInclude(a => a.Producto)
                .Include(a => a.EmergenciaDetalles)
                    .ThenInclude(a => a.ExamenLabClinico)
                .Include(a => a.EmergenciaDetalles)
                    .ThenInclude(a => a.Precio)
                .Include(a => a.EmergenciaDetalles)
                    .ThenInclude(a => a.UnidadMedidaVenta)
                .Where(a => a.Ingresada == ingresadas
                && !a.Eliminado)
                .ToList();
            if (emergenciasBd != null)
            {
                foreach (var emergencia in emergenciasBd)
                {
                    emergencia.EmergenciaDetalles = emergencia.EmergenciaDetalles
                        .Where(a => !a.Eliminado)
                        .ToList();
                }
            }
            return emergenciasBd;
        }
        public void Update(Emergencia emergencia)
        {
            _context.Entry(emergencia).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public Emergencia UpdateSoloEstado(int id, bool nuevoEstado)
        {
            var emergencia = _context.Emergencias.Find(id);

            if (emergencia != null)
            {
                emergencia.Pagado = nuevoEstado;
                _context.Entry(emergencia).Property(x => x.Pagado).IsModified = true;
                _context.SaveChanges();
            }

            return emergencia;
        }


        public List<EmergenciaDetalle> GetExamenesAgregados(int emergenciaId)
        {
            return _context.EmergenciaDetalles
            .Include(c => c.ExamenLabClinico)
            .Include(c => c.Precio)
            .Where(c => c.EmergenciaId == emergenciaId)
            .ToList();
        }

        public List<EmergenciaDetalle> GetServiciosAgregados(int emergenciaId)
        {
            return _context.EmergenciaDetalles
            .Include(c => c.Servicio)
            .Include(c => c.Precio)
            .Where(c => c.EmergenciaId == emergenciaId)
            .ToList();
        }



        public List<EmergenciaDetalle> GetPrescripcionEmergencia(int emergenciaId, bool includeProducto = false)
        {
            var query = _context.EmergenciaDetalles
                .Where(a => a.EmergenciaId == emergenciaId);

            if (includeProducto)
            {
                query = query
                    .Include(d => d.Producto)
                    .Include(d => d.Precio)
                    .Include(d => d.UnidadMedidaVenta);
            }

            return query.ToList();
        }


        public int? GetIdByHospitalizacion(int hospitalizacionId)
        {
            return _context.Emergencias
                .Where(e => e.HospitalizacionId == hospitalizacionId)
                .Select(e => e.Id)
                .FirstOrDefault();
        }



        public void AddDetalle(EmergenciaDetalle detalle)
        {
            _context.EmergenciaDetalles.Add(detalle);
            _context.SaveChanges();
        }

        public void UpdateDetalle(EmergenciaDetalle detalle)
        {
            _context.Entry(detalle).State = EntityState.Modified;
            _context.SaveChanges();
        }



        public void DeleteDetalle(int detalleId)
        {
            var detalle = _context.EmergenciaDetalles.Find(detalleId);
            if (detalle != null)
            {
                detalle.Eliminado = true; 
                _context.Entry(detalle).Property(x => x.Eliminado).IsModified = true;
                _context.SaveChanges();
            }
        }

        public EmergenciaDetalle GetDetalle(int detalleId)
        {
            return _context.EmergenciaDetalles
                .Include(d => d.Producto)
                .Include(d => d.Servicio)
                .Include(d => d.ExamenLabClinico)
                .Include(d => d.Precio)
                .Include(d => d.UnidadMedidaVenta)
                .FirstOrDefault(d => d.Id == detalleId);
        }

    }
}