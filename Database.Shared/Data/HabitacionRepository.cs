using Database.Shared.IRepository;
using Database.Shared.Models;
using Database.Shared.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Paginacion;
using System;

namespace Database.Shared.Data
{
    public class HabitacionRepository : IHabitacion
    {
        private readonly Context _context = null;

        public HabitacionRepository(Context context)
        {
            _context = context;
        }

        public Habitacion Add(Habitacion habitacion)
        {
            _context.Habitaciones.Add(habitacion);
            _context.SaveChanges();
            return habitacion;
        }
        public Habitacion Update(Habitacion habitacion)
        {
            _context.Entry(habitacion).State = EntityState.Modified;
            _context.SaveChanges();
            return habitacion;
        }
        public Habitacion Get(int habitacionId)
        {
            return _context.Habitaciones
                .Include(a => a.CategoriaHabitacion)
                .Include(a => a.EstadoHabitacion)
                .Where(a => a.Id == habitacionId && !a.Eliminada)
                .SingleOrDefault();
        }
        public IList<Habitacion> GetHabitaciones()
        {
            return _context.Habitaciones
                .Include(a => a.EstadoHabitacion)
                .Include(a => a.CategoriaHabitacion).ThenInclude(a => a.CategoriaHabitacionTarifas)
                .Where(a => !a.Eliminada)
                .ToList();
        }
        public Paciente GetPacienteOcupante(int habitacionId)
        {
            var hospitalizacion = _context.Hospitalizaciones
                .Include(p => p.Paciente)
                .Where(a => !a.Finalizada && a.HabitacionId == habitacionId)
                .FirstOrDefault();

            if (hospitalizacion == null)
            {
                return null;
            }

            return hospitalizacion.Paciente;
        }


        public (Paciente, Empleado, string) GetPacienteOcupanteConMedicoYCita(int habitacionId)
        {
            var hospitalizacion = _context.Hospitalizaciones
                .Include(h => h.Paciente)
                .Include(h => h.Consultas)
                    .ThenInclude(c => c.Citas)
                .ThenInclude(c => c.Empleado)
                .Where(h => !h.Finalizada && h.HabitacionId == habitacionId)
                .FirstOrDefault();

            if (hospitalizacion == null)
                return (null, null, "-");

            var consulta = hospitalizacion.Consultas
                .OrderByDescending(c => c.FechaYHoraInicioConsulta)
                .FirstOrDefault();

            var cita = consulta?.Citas;
            var medico = cita?.Empleado;
            var codigoCita = cita?.CodigoDeCita ?? "-";

            return (hospitalizacion.Paciente, medico, codigoCita);
        }

        public void Delete(int habitacionId)
        {
            var habitacion = _context.Habitaciones
                .Where(a => a.Id == habitacionId)
                .SingleOrDefault();

            habitacion.Eliminada = true;

            _context.SaveChanges();
        }
        public CategoriaHabitacion AddCategoria(CategoriaHabitacion categoria)
        {
            _context.CategoriasHabitaciones.Add(categoria);
            _context.SaveChanges();
            return categoria;
        }
        public CategoriaHabitacion GetCategoria(int categoriaId)
        {
            return _context.CategoriasHabitaciones
                .Include(c => c.CategoriaHabitacionTarifas)
                .Where(c => c.Id == categoriaId)
                .SingleOrDefault();
        }
        public void UpdateCategoria(CategoriaHabitacion categoria)
        {
            _context.Entry(categoria).State = EntityState.Modified;
            _context.SaveChanges();
        }
        public IList<CategoriaHabitacion> GetCategorias(bool includeTarifas = true)
        {
            if (includeTarifas)
            {
                return _context.CategoriasHabitaciones
                    .Include(c => c.CategoriaHabitacionTarifas
                        .Where(c => !c.Eliminada))
                    .Where(c => !c.Eliminada)
                    .ToList();
            }
            return _context.CategoriasHabitaciones
                    .Where(c => !c.Eliminada)
                    .ToList();
        }

        public Hospitalizacion GetHospitalizacionActual(int habitacionId)
        {
            return GetOcupacionActual(habitacionId).HospitalizacionId is int id
                ? _context.Hospitalizaciones.FirstOrDefault(h => h.Id == id)
                : null;
        }

        public (int? HospitalizacionId, int? CitaId, Paciente Paciente) GetOcupacionActual(int habitacionId)
        {
            var hospitalizacion = _context.Hospitalizaciones
                .Include(h => h.Paciente)
                .Include(h => h.Consultas)
                .Where(a => !a.Eliminada
                    && a.HabitacionId == habitacionId
                    && !a.Finalizada)
                .OrderByDescending(a => a.Id)
                .FirstOrDefault();

            if (hospitalizacion == null)
            {
                hospitalizacion = _context.Hospitalizaciones
                    .Include(h => h.Paciente)
                    .Include(h => h.Consultas)
                    .Where(a => !a.Finalizada && a.HabitacionId == habitacionId)
                    .OrderByDescending(a => a.Id)
                    .FirstOrDefault();
            }

            if (hospitalizacion == null)
                return (null, null, null);

            int? citaId = hospitalizacion.Consultas?
                .OrderByDescending(c => c.FechaYHoraInicioConsulta)
                .Select(c => c.CitasId)
                .FirstOrDefault(id => id.HasValue && id.Value > 0);

            if (!citaId.HasValue || citaId.Value <= 0)
            {
                citaId = _context.Citass
                    .Where(c => c.HabitacionId == habitacionId && !c.Eliminado && !c.Finalizada)
                    .OrderByDescending(c => c.FechaInicio)
                    .Select(c => (int?)c.Id)
                    .FirstOrDefault();
            }

            return (hospitalizacion.Id, citaId, hospitalizacion.Paciente);
        }
        public void DeleteCategoria(int categoriaId)
        {
            var categoria = _context.CategoriasHabitaciones
                .Where(c => c.Id == categoriaId)
                .SingleOrDefault();
            categoria.Eliminada = true;
            UpdateCategoria(categoria);
        }
        public IList<EstadoHabitacion> GetEstados()
        {
            return _context.EstadosHabitacion
                .ToList();
        }

        public IList<Habitacion> GetHabitacionesPorCategoriaParaAgenda(int categoriaId)
        {
            return _context.Habitaciones
                .Where(a => !a.Eliminada && a.CategoriaHabitacionId == categoriaId)
                .OrderBy(a => a.NombreNumeroHabitacion)
                .ToList();
        }
        public CategoriaHabitacionTarifa GetTarifaById(int tarifaId)
        {
            return _context.CategoriaHabitacionTarifas
                .FirstOrDefault(t => t.Id == tarifaId);
        }

        public IList<CategoriaHabitacionTarifa> GetTarifasHabitacion(int habitacionId)
        {
            if (habitacionId <= 0)
                return new List<CategoriaHabitacionTarifa>();

            var categoriaId = _context.Habitaciones
                .Where(h => h.Id == habitacionId && !h.Eliminada)
                .Select(h => h.CategoriaHabitacionId)
                .FirstOrDefault();

            if (categoriaId <= 0)
                return new List<CategoriaHabitacionTarifa>();

            return GetTarifasCategoria(categoriaId);
        }

        public IList<CategoriaHabitacionTarifa> GetTarifasCategoria(int categoriaId)
        {
            if (categoriaId <= 0)
                return new List<CategoriaHabitacionTarifa>();

            return _context.CategoriaHabitacionTarifas
                .Where(t => t.CategoriaHabitacionId == categoriaId && !t.Eliminada)
                .OrderBy(t => t.NombreTarifa)
                .ToList();
        }
    }

}
