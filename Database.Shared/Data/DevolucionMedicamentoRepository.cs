using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Database.Shared.Data
{
    public class DevolucionMedicamentoRepository : IDevolucionMedicamentoRepository
    {
        private readonly Context _context;

        public DevolucionMedicamentoRepository(Context context)
        {
            _context = context;
        }

        // 1. Agregar una nueva devolución de medicamento
        public void AddDevolucionMedicamento(DevolucionMedicamento devolucion)
        {
            _context.DevolucionMedicamento.Add(devolucion);
            _context.SaveChanges();
        }

        // 2. Obtener una devolución por ID
        public DevolucionMedicamento GetDevolucionMedicamentoById(int id)
        {
            return _context.DevolucionMedicamento
                .Include(d => d.Hospitalizacion)
                    .ThenInclude(h => h.Habitacion)
                .Include(d => d.Hospitalizacion)
                    .ThenInclude(h => h.Paciente)
                // Ahora se incluye la entidad HospitalizacionProductoAplicacion en lugar de Producto
                .Include(d => d.HospitalizacionProductoAplicacion)
                .FirstOrDefault(d => d.Id == id);
        }

        // 3. Obtener todas las devoluciones (con filtro opcional por hospitalizacionId)
        public List<DevolucionMedicamento> GetAllDevolucionesMedicamento(int? hospitalizacionId = null)
        {
            var query = _context.DevolucionMedicamento
                .Include(d => d.Hospitalizacion)
                    .ThenInclude(h => h.Habitacion)
                .Include(d => d.Hospitalizacion)
                    .ThenInclude(h => h.Paciente)
                // Se cambia para incluir la nueva entidad de relación
                .Include(d => d.HospitalizacionProductoAplicacion)
                .AsQueryable();

            if (hospitalizacionId.HasValue)
            {
                query = query.Where(d => d.HospitalizacionId == hospitalizacionId.Value);
            }

            // Ordenar por FechaSolicitud descendente (las más recientes primero)
            return query.OrderByDescending(d => d.FechaSolicitud).ToList();
        }

        // 4. Obtener devoluciones por hospitalizacionId
        public List<DevolucionMedicamento> GetDevolucionesByHospitalizacionId(int hospitalizacionId)
        {
            Console.WriteLine($"[DEBUG] Iniciando GetDevolucionesByHospitalizacionId con HospitalizacionId: {hospitalizacionId}");
            
            var query = _context.DevolucionMedicamento
                .Where(d => d.HospitalizacionId == hospitalizacionId)
                // Se incluye la nueva relación en lugar de Producto
                .Include(d => d.HospitalizacionProductoAplicacion)
                .OrderByDescending(d => d.FechaSolicitud);
            Console.WriteLine($"[DEBUG] Consulta preparada: Filtrando registros con HospitalizacionId = {hospitalizacionId}");
            
            List<DevolucionMedicamento> resultados = query.ToList();
            Console.WriteLine($"[DEBUG] Consulta ejecutada. Registros encontrados: {resultados.Count}");
            
            foreach (var registro in resultados)
            {
                Console.WriteLine($"[DEBUG] Registro - Id: {registro.Id}, HospitalizacionProductoAplicacionId: {registro.HospitalizacionProductoAplicacionId}, FechaSolicitud: {registro.FechaSolicitud}");
            }
            
            return resultados;
        }

        // 5. Actualizar una devolución
        public void UpdateDevolucionMedicamento(DevolucionMedicamento devolucion)
        {
            var existingDevolucion = _context.DevolucionMedicamento.Find(devolucion.Id);
            if (existingDevolucion != null)
            {
                _context.Entry(existingDevolucion).CurrentValues.SetValues(devolucion);
                _context.SaveChanges();
            }
        }

        // 6. Eliminar una devolución
        public void DeleteDevolucionMedicamento(int id)
        {
            var devolucion = _context.DevolucionMedicamento.Find(id);
            if (devolucion != null)
            {
                _context.DevolucionMedicamento.Remove(devolucion);
                _context.SaveChanges();
            }
        }
    }
}
