using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Database.Shared.Data
{
    public class SolicitudMedicamentoNacionalRepository : ISolicitudMedicamentoNacionalRepository
    {
        private readonly Context _context;

        public SolicitudMedicamentoNacionalRepository(Context context)
        {
            _context = context;
        }

        // ✅ 1. Agregar una nueva solicitud de medicamento
        public void AddSolicitudMedicamento(SolicitudMedicamentoNacional solicitud)
        {
            _context.SolicitudMedicamentoNacional.Add(solicitud);
            _context.SaveChanges();
        }

        // ✅ 2. Obtener una solicitud de medicamento por ID
        public SolicitudMedicamentoNacional GetSolicitudMedicamentoById(int id)
        {
            return _context.SolicitudMedicamentoNacional
                .Include(s => s.Hospitalizacion)
                .Include(s => s.Producto)
                .FirstOrDefault(s => s.Id == id);
        }

        // ✅ 3. Obtener todas las solicitudes de medicamento
        public List<SolicitudMedicamentoNacional> GetAllSolicitudesMedicamento(int? hospitalizacionId = null)
        {
            var query = _context.SolicitudMedicamentoNacional
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
        public List<SolicitudMedicamentoNacional> GetSolicitudesByHospitalizacionId(int hospitalizacionId)
        {
            return _context.SolicitudMedicamentoNacional
                .Where(s => s.HospitalizacionId == hospitalizacionId)
                .Include(s => s.Producto)
                .OrderByDescending(s => s.FechaSolicitud) // Ordenar por fecha más reciente primero
                .ToList();
        }


        // ✅ 5. Actualizar una solicitud de medicamento
        public void UpdateSolicitudMedicamento(SolicitudMedicamentoNacional solicitud)
        {
            var existingSolicitud = _context.SolicitudMedicamentoNacional.Find(solicitud.Id);
            if (existingSolicitud != null)
            {
                _context.Entry(existingSolicitud).CurrentValues.SetValues(solicitud);
                _context.SaveChanges();
            }
        }

        // ✅ 6. Eliminar una solicitud de medicamento
        public void DeleteSolicitudMedicamento(int id)
        {
            var solicitud = _context.SolicitudMedicamentoNacional.Find(id);
            if (solicitud != null)
            {
                _context.SolicitudMedicamentoNacional.Remove(solicitud);
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


        public List<SolicitudMedicamentoNacional> GetList() =>
        _context.SolicitudMedicamentoNacional
        .OrderByDescending(a => a.Id)
        .ToList();
    }
}
