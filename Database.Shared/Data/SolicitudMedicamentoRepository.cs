using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Database.Shared.Data
{
    public class SolicitudMedicamentoRepository : ISolicitudMedicamentoRepository
    {
        private readonly Context _context;

        public SolicitudMedicamentoRepository(Context context)
        {
            _context = context;
        }

        // ✅ 1. Agregar una nueva solicitud de medicamento
        public void AddSolicitudMedicamento(SolicitudMedicamento solicitud)
        {
            _context.SolicitudMedicamento.Add(solicitud);
            _context.SaveChanges();
        }

        // ✅ 2. Obtener una solicitud de medicamento por ID
        public SolicitudMedicamento GetSolicitudMedicamentoById(int id)
        {
            return _context.SolicitudMedicamento
                .Include(s => s.Hospitalizacion)
                .Include(s => s.Producto)
                .FirstOrDefault(s => s.Id == id);
        }

        // ✅ 3. Obtener todas las solicitudes de medicamento
        public List<SolicitudMedicamento> GetAllSolicitudesMedicamento(int? hospitalizacionId = null)
        {
            var query = _context.SolicitudMedicamento
                .Include(s => s.Hospitalizacion)
                    .ThenInclude(h => h.Habitacion)
                .Include(s => s.Hospitalizacion)
                    .ThenInclude(h => h.Paciente)
                .Include(s => s.Producto)
                .Where(s => s.Estado == "En espera")
                .AsQueryable();

            if (hospitalizacionId.HasValue)
            {
                query = query.Where(s => s.HospitalizacionId == hospitalizacionId.Value);
            }

            return query.OrderByDescending(s => s.FechaSolicitud).ToList();
        }




        // ✅ 4. Obtener solicitudes por ID de hospitalización
        public List<SolicitudMedicamento> GetSolicitudesByHospitalizacionId(int hospitalizacionId)
        {
            return _context.SolicitudMedicamento
                .Where(s => s.HospitalizacionId == hospitalizacionId)
                .Include(s => s.Producto)
                .OrderByDescending(s => s.FechaSolicitud) // Ordenar por fecha más reciente primero
                .ToList();
        }


        // ✅ 5. Actualizar una solicitud de medicamento
        public void UpdateSolicitudMedicamento(SolicitudMedicamento solicitud)
        {
            var existingSolicitud = _context.SolicitudMedicamento.Find(solicitud.Id);
            if (existingSolicitud != null)
            {
                _context.Entry(existingSolicitud).CurrentValues.SetValues(solicitud);
                _context.SaveChanges();
            }
        }

        // ✅ 6. Eliminar una solicitud de medicamento
        public void DeleteSolicitudMedicamento(int id)
        {
            var solicitud = _context.SolicitudMedicamento.Find(id);
            if (solicitud != null)
            {
                _context.SolicitudMedicamento.Remove(solicitud);
                _context.SaveChanges();
            }
        }
        public List<CategoriaHabitacion> GetAllCategoriasHabitacion()
        {
            return _context.CategoriasHabitaciones
                .Where(c => !c.Eliminada)
                .Select(c => new CategoriaHabitacion
                {
                    Id = c.Id,
                    NombreCategoria = c.NombreCategoria
                })
                .ToList();
        }


        public List<SolicitudMedicamento> GetList() =>
        _context.SolicitudMedicamento
        .OrderByDescending(a => a.Id)
        .ToList();
    }
}
