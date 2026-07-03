using Database.Shared.Models;
using Database.Shared.IRepository;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Paginacion;
using System;
using Database.Shared.Enumeraciones;

namespace Database.Shared.Data
{
    public class CitasRepository : ICitas
    {
        private readonly Context _context = null;

        public CitasRepository(Context context)
        {
            _context = context;
        }

        public IList<Citas> GetList()
        {
            return _context.Citass
            .Include(a => a.Especialidad)
            .Include(a => a.Paciente)
            .Include(a => a.Habitacion)
            .Where(a => a.Eliminado == false && a.Finalizada == false)
            .ToList();
        }

        public IList<Citas> GetAll()
        {
            return _context.Citass
                    .Include(a => a.Paciente)
                    .ToList();
        }

        public IList<Servicio> GetServiciosList()
        {
            return _context.Servicios
                .Where(s => s.Eliminado == false)
                .OrderBy(s => s.NombreServicio)
                .ToList();
        }


        public string ObtenerSiguienteDpiFicticio()
        {
            var ultimoDpiAsignado = _context.Pacientes
                .Where(p => p.Dpi.StartsWith("000")) // Filtrar solo DPI ficticios
                .OrderByDescending(p => p.Dpi) // Obtener el mayor valor
                .Select(p => p.Dpi)
                .FirstOrDefault();

            long nuevoDpi = 1; // Comenzamos en 1 si no hay registros

            if (!string.IsNullOrEmpty(ultimoDpiAsignado) && long.TryParse(ultimoDpiAsignado, out long dpiActual))
            {
                nuevoDpi = dpiActual + 1; // Incrementamos correctamente
            }
            else if (!string.IsNullOrEmpty(ultimoDpiAsignado)) // Manejo si TryParse falla
            {
                nuevoDpi = long.Parse(ultimoDpiAsignado.TrimStart('0')) + 1;
            }

            return nuevoDpi.ToString("D6"); // Devuelve siempre en formato de 6 dígitos (000001, 000002...)
        }
        public IList<Especialidad> GetEspecialidadesList()
        {
            return _context.Especialidad
            .OrderBy(a => a.NombreEspecialidad).ToList();
        }


        public IList<CategoriaHabitacion> GetHabitacionesCategoriasList()
        {
            return _context.CategoriasHabitaciones
                .Where(c => c.Eliminada == false)
                .OrderBy(c => c.NombreCategoria)
                .ToList();
        }

        // Método para obtener habitaciones por categoría de habitación
        public IList<Habitacion> GetHabitacionesList(int categoriaHabitacionId)
        {
            var habitaciones = _context.Habitaciones
                .Where(a => a.Eliminada == false
                            && a.EstadoHabitacionId == (int)EstadoHabitacionEnum.Disponible
                            && a.CategoriaHabitacionId == categoriaHabitacionId)
                .OrderBy(a => a.NombreNumeroHabitacion)
                .ToList();
            return habitaciones;
        }



        public void Add(Citas cita, bool saveChanges = true)
        {
            _context.Citass.Add(cita);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }
        public void Add(CitasServicio citaServicio, bool saveChanges = true)
        {
            _context.CitasServicios.Add(citaServicio);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        public Citas GetCita(int id)
        {
            return _context.Citass
            .Include(a => a.CitasServicios).ThenInclude(a => a.Servicio)
            .Include(a => a.CitasServicios).ThenInclude(a => a.Precio)
            .Include(a => a.Paciente).ThenInclude(a => a.Sexo)
            .Include(a => a.Habitacion)

            .Include(a => a.Paciente).ThenInclude(a => a.SeguroEpss)
            .Include(a => a.Paciente).ThenInclude(a => a.PacientesSeguimientosNutricionales)
            .Include(a => a.Paciente).ThenInclude(a => a.VacunasPaciente).ThenInclude(a => a.Vacuna)
            .Include(a => a.Paciente).ThenInclude(a => a.Departamento)
            .Include(a => a.Paciente).ThenInclude(a => a.Municipio)
            .Include(a => a.Servicio)
            .Include(a => a.Empleado)
            .Include(a => a.Especialidad)
            .Include(a => a.CitasExamenes).ThenInclude(a => a.ExamenLabClinico)
            .Include(a => a.CitasExamenes).ThenInclude(a => a.ExamenLabClinico.ExamenLabClinicosPreguntas)
            .Include(a => a.Paciente)
            .Where(a => a.Id == id).SingleOrDefault();
        }
        public ExamenFisico GetExamenFisico(int id)
        {
            return _context.ExamenFisico
              .Where(a => a.Id == id).SingleOrDefault();
        }
        public Especialidad GetEspecilidad(int id)
        {
            return _context.Especialidad
                .Where(a => a.Id == id).SingleOrDefault();
        }
        public List<CitasServicio> GetServiciosCita(int citaId)
        {
            return _context.CitasServicios
                .Include(a => a.Servicio)
                .Include(a => a.Precio)
                .Where(a => a.CitasId == citaId
                && !a.Eliminado)
                .ToList();
        }
        public List<Citas> GetTurnoEspecialidadCita(int Id)
        {
            DateTime fechaHoy = DateTime.Now.Date;
            return _context.Citass
                //.Include(a => a.Especialidad).ThenInclude(a => a.NombreEspecialidad)

                .Where(a => a.EspecialidadId == Id
                 && (a.FechaInicio.HasValue && a.FechaInicio.Value.Date == fechaHoy))
                .ToList();
        }

        public Especialidad GetTurnoId(int id)
        {
            return _context.Especialidad
                .Include(a => a.Citas)
                .Where(a => a.Id == id).SingleOrDefault();
        }

        public void Update(Citas model, bool saveChanges = true)
        {
            _context.Entry(model).State = EntityState.Modified;

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }
        public void Update(CitasServicio citaServicio, bool saveChanges = true)
        {
            _context.Entry(citaServicio).State = EntityState.Modified;

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        public PaginacionList<Citas> PaginacionCitas(string sortOrder, string searchString, int? pageNumber, int pageSize, int? empleadoId)
        {
            var cita = _context.Citass.AsQueryable();

            //is
            if (!string.IsNullOrEmpty(searchString))
            {
                cita = cita.Where(s => s.Paciente.Nombre
                .Contains(searchString) || s.Especialidad.NombreEspecialidad.Contains(searchString));
            }
            if ((empleadoId != null))
            {
                cita = cita.Where(s => s.EspecialidadId == empleadoId);
            }
            var resultado = PaginacionList<Citas>.CreateAsyncc(cita
             .Include(a => a.Especialidad)
             .Include(a => a.Sucursal)
             .Include(a => a.Paciente).ThenInclude(a => a.Sexo)
             .Include(a => a.Empleado)
             .Include(a => a.User).ThenInclude(a => a.Persona)
             .Include(a => a.CitasServicios).ThenInclude(a => a.Servicio)
             .Include(a => a.Servicio)
             .OrderByDescending(a => a.FechaInicio)
             .Where(a => a.Eliminado == false
             && a.Finalizada == false
             && a.EstadoCita == "normal"),
             pageNumber ?? 1, pageSize);
            return resultado;
        }

        public PaginacionList<Citas> PaginacionTurnos(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {

            DateTime fechaHoy = DateTime.Now.Date;
            //return _context.Citass
            //    .Where(a => a.EspecialidadId == Id
            //     && (a.FechaInicio.HasValue && a.FechaInicio.Value.Date == fechaHoy))

            var cita = _context.Citass.AsQueryable();

            //is
            if (!string.IsNullOrEmpty(searchString))
            {
                cita = cita.Where(s => s.Paciente.Nombre
                .Contains(searchString) || s.Especialidad.NombreEspecialidad.Contains(searchString));
            }
            var resultado = PaginacionList<Citas>.CreateAsyncc(cita
             .Include(a => a.Especialidad)
             .Include(a => a.Sucursal)
             .Include(a => a.Paciente).ThenInclude(a => a.Sexo)
             .Include(a => a.Empleado)
             .Include(a => a.User).ThenInclude(a => a.Persona)
             .Include(a => a.CitasServicios).ThenInclude(a => a.Servicio)
             .Include(a => a.Servicio)
             .OrderBy(a => a.FechaInicio)
             .Where(
                a => a.Eliminado == false

             && a.EstadoCita == "normal"
             && (a.EstadoTurno == "ACTIVO" || a.EstadoTurno == "PENDIENTE" || a.EstadoTurno == "ENCURSO")
             && (a.FechaHoraInicioTurno.HasValue && a.FechaHoraInicioTurno.Value.Date == fechaHoy)),
             pageNumber ?? 1, pageSize);
            return resultado;
        }

        public IList<Citas> CitasNormales(string searchString)
        {
            var citas = _context.Citass.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                citas = citas.Where(s =>
                s.Paciente.Nombre.Contains(searchString) || s.Especialidad.NombreEspecialidad.Contains(searchString));
            }

            return citas
            .Include(a => a.Especialidad)
            .Include(a => a.Paciente).ThenInclude(a => a.Sexo)
            .Include(a => a.Empleado)
                        .Include(a => a.Habitacion)

            .Include(a => a.User).ThenInclude(a => a.Persona)
            .Include(a => a.Servicio)
            .OrderByDescending(a => a.FechaInicio)
            .Where(a => a.Eliminado == false && a.Finalizada == false && a.EstadoCita == "normal").ToList();
        }

        public PaginacionList<Citas> PaginacionCitasNoAsistidas(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var cita = _context.Citass.AsQueryable();

            //is
            if (!string.IsNullOrEmpty(searchString))
            {
                cita = cita.Where(s => s.Paciente.Nombre
                .Contains(searchString) || s.Especialidad.NombreEspecialidad.Contains(searchString));
            }

            return PaginacionList<Citas>.CreateAsyncc(cita
            .Include(a => a.Especialidad)
            .Include(a => a.Paciente).ThenInclude(a => a.Sexo)
            .Include(a => a.Empleado)
            .Include(a => a.User).ThenInclude(a => a.Persona)
            .Include(a => a.Servicio)
            .OrderByDescending(a => a.FechaInicio)
            .Where(a => a.Eliminado == false && a.Finalizada == false && a.EstadoCita == "No asistida"),
            pageNumber ?? 1, pageSize);
        }



        public PaginacionList<Citas> PaginacionCitasFinalizadas(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var cita = _context.Citass.AsQueryable();

            // if (!string.IsNullOrEmpty(searchString))
            // {
            //     cita = cita.Where(s => s.Paciente.Nombre
            //     .Contains(searchString) || s.Motivo.Contains(searchString));
            // }


            if (!string.IsNullOrEmpty(searchString))
            {
                var filtro = searchString.Trim().ToLower();

                cita = cita.Where(e =>
                    e.Paciente != null &&
                    e.Paciente.Nombre != null &&
                    e.Paciente.Nombre.ToLower().Contains(filtro));
            }


            return PaginacionList<Citas>.CreateAsyncc(cita
              .Include(a => a.Especialidad)
            .Include(a => a.Paciente).ThenInclude(a => a.Sexo)
            .Include(a => a.Empleado)
            .Include(a => a.User).ThenInclude(a => a.Persona)
            .Include(a => a.Servicio)
            .Include(a => a.CitasServicios).ThenInclude(a => a.Servicio)
            .OrderByDescending(a => a.FechaInicio)
            .Where(a => a.Eliminado == false && a.Finalizada == true),
            pageNumber ?? 1, pageSize);
        }


        public IList<Citas> CitasPorFecha(DateTime fecha)
        {
            var citas = _context.Citass
                .Include(a => a.Especialidad)
                .Include(a => a.Paciente).ThenInclude(a => a.Sexo)
                .Include(a => a.Empleado)
                .Include(a => a.Habitacion)
                .Include(a => a.User).ThenInclude(a => a.Persona)
                .Include(a => a.CitasServicios).ThenInclude(a => a.Servicio)
                .Include(a => a.Sucursal)
                .Include(a => a.Servicio)
                .Where(a => a.Eliminado == false)
                .Where(a => a.FechaInicio >= fecha && a.FechaInicio < fecha.AddDays(1))
                .ToList();

            foreach (var cita in citas)
            {
                // Forzar que FechaInicio y FechaFinal se interpreten como UTC
                if (cita.FechaInicio.HasValue)
                    cita.FechaInicio = cita.FechaInicio.Value.ToUniversalTime();

                if (cita.FechaFinal.HasValue)
                    cita.FechaFinal = cita.FechaFinal.Value.ToUniversalTime();

                // Limpieza de servicios eliminados
                if (cita.CitasServicios != null)
                {
                    cita.CitasServicios = cita.CitasServicios
                        .Where(a => !a.Eliminado)
                        .ToList();
                }
            }

            return citas;
        }


        public IList<Citas> CitasListaPorFecha(DateTime fecha)
        {
            var citas = _context.Citass
            .Include(a => a.Especialidad)
            .Include(a => a.Paciente).ThenInclude(a => a.Sexo)
            .Include(a => a.Empleado)
            .Include(a => a.User).ThenInclude(a => a.Persona)
            .Include(a => a.CitasServicios).ThenInclude(a => a.Servicio)
            .Include(a => a.Sucursal)
                        .Include(a => a.Habitacion)

            .Include(a => a.CitasServicios).ThenInclude(a => a.Servicio)
            .Where(a => a.Eliminado == false)
            .Where(a => a.Eliminado == false
             && a.Finalizada == false
             && a.EstadoCita == "normal"
             && a.FechaInicio >= fecha && a.FechaInicio < fecha.AddDays(1)).ToList();

            if (citas != null)
            {
                foreach (var cita in citas)
                {
                    cita.CitasServicios = cita.CitasServicios
                        .Where(a => !a.Eliminado)
                        .ToList();
                }
            }

            return citas;
        }

        public IList<Citas> CitasListado()
        {
            return _context.Citass
            .Include(a => a.Especialidad)
            .Include(a => a.Paciente).ThenInclude(a => a.Sexo)
            .Include(a => a.Empleado)
                        .Include(a => a.Habitacion)

            .Include(a => a.User).ThenInclude(a => a.Persona)
            .Include(a => a.CitasServicios).ThenInclude(a => a.Servicio)
            .Include(a => a.Sucursal)
            .Include(a => a.CitasServicios).ThenInclude(a => a.Servicio)
            .OrderByDescending(a => a.FechaInicio)
            .Where(a => a.Eliminado == false)
            .Where(a => a.Eliminado == false
             && a.Finalizada == false
             && a.EstadoCita == "normal").ToList();




        }
        public IList<EstadoPagoConsulta> EstadoPagosConsultasLista()
        {
            return _context.EstadoPagoConsultas.OrderBy(a => a.Estado).ToList();
        }

        //Bloqueo de dias
        public void AddFechaBloqueada(CalendarioFechaBloqueada fechaBloqueada)
        {
            _context.CalendarioFechasBloqueadas.Add(fechaBloqueada);
            _context.SaveChanges();
        }
        public CalendarioFechaBloqueada GetFechaBloqueada(DateTime fecha)
        {
            return _context.CalendarioFechasBloqueadas
                .Where(a => a.Fecha.Date == fecha.Date
                && !a.Eliminada)
                .FirstOrDefault();
        }



        public void DeleteFechaBloqueada(DateTime dia)
        {
            var fecha = _context.CalendarioFechasBloqueadas
                .Where(a => a.Fecha.Date == dia.Date)
                .FirstOrDefault();
            fecha.Eliminada = true;
            _context.Entry(fecha).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public IList<Citas> CitasPorFechas(DateTime fechaInicial, DateTime fechaFinal, int? id, int? id1, int? id2)
        {
            return _context.Citass

             .Include(a => a.CitasServicios).ThenInclude(a => a.Servicio)
             .Include(a => a.Paciente).ThenInclude(a => a.Sexo)
             .Include(a => a.Paciente).ThenInclude(a => a.SeguroEpss)
             .Include(a => a.Paciente).ThenInclude(a => a.PacientesSeguimientosNutricionales)
             .Include(a => a.Paciente).ThenInclude(a => a.VacunasPaciente).ThenInclude(a => a.Vacuna)
             .Include(a => a.Servicio)
             .Include(a => a.Empleado)
             .Include(a => a.Habitacion)

             .Include(a => a.Especialidad)
             .Where(a => a.FechaInicio >= fechaInicial && a.FechaInicio < fechaFinal.AddDays(1)).ToList();
        }

        public IList<Citas> CitasCalendarioLineal(
    DateTime? fechaInicial,
    DateTime? fechaFinal,
    int? sucursalId,
    int? empleadoId,
    int? especialidadId,
    int? servicioId,
    string pacienteNombre)
        {
            // Base query
            IQueryable<Citas> query = _context.Citass
                .AsNoTracking()
                .Include(a => a.User).ThenInclude(u => u.Persona)
                .Include(a => a.CitasServicios).ThenInclude(a => a.Servicio)
                .Include(a => a.Paciente).ThenInclude(a => a.Sexo)
                .Include(a => a.Paciente).ThenInclude(a => a.SeguroEpss)
                .Include(a => a.Paciente).ThenInclude(a => a.PacientesSeguimientosNutricionales)
                .Include(a => a.Paciente).ThenInclude(a => a.VacunasPaciente).ThenInclude(a => a.Vacuna)
                .Include(a => a.Servicio)
                .Include(a => a.Empleado)
                .Include(a => a.Habitacion)
                .Include(a => a.Especialidad);

            // Fechas (opcionales)
            if (fechaInicial.HasValue && fechaFinal.HasValue)
            {
                var ini = fechaInicial.Value.Date;
                var finExclusive = fechaFinal.Value.Date.AddDays(1);
                query = query.Where(a => a.FechaInicio >= ini && a.FechaInicio < finExclusive);
            }
            else if (fechaInicial.HasValue)
            {
                var ini = fechaInicial.Value.Date;
                var finExclusive = ini.AddDays(1);
                query = query.Where(a => a.FechaInicio >= ini && a.FechaInicio < finExclusive);
            }
            else if (fechaFinal.HasValue)
            {
                var ini = fechaFinal.Value.Date;
                var finExclusive = ini.AddDays(1);
                query = query.Where(a => a.FechaInicio >= ini && a.FechaInicio < finExclusive);
            }

            // Filtros opcionales
            if (sucursalId.HasValue)
                query = query.Where(a => a.SucursalId == sucursalId.Value);

            if (empleadoId.HasValue)
                query = query.Where(a => a.EmpleadoId == empleadoId.Value);

            if (especialidadId.HasValue)
                query = query.Where(a => a.EspecialidadId == especialidadId.Value);

            if (servicioId.HasValue)
                query = query.Where(a => a.CitasServicios.Any(s => s.ServicioId == servicioId.Value && !s.Eliminado));

            // PacienteNombre LIKE '%term%' (opcional)
            if (!string.IsNullOrWhiteSpace(pacienteNombre))
            {
                var term = pacienteNombre.Trim();
                query = query.Where(a =>
                    a.Paciente != null &&
                    a.Paciente.Nombre != null &&
                    EF.Functions.Like(a.Paciente.Nombre, $"%{term}%"));
            }

            return query.ToList();
        }

        public IList<Citas> CitasPorFechaHora(DateTime fecha)
        {
            var citas = _context.Citass
            .Include(a => a.Especialidad)
            .Include(a => a.Paciente).ThenInclude(a => a.Sexo)
            .Include(a => a.Empleado)
            .Include(a => a.Habitacion).ThenInclude(a => a.CategoriaHabitacion)
            .Include(a => a.User).ThenInclude(a => a.Persona)
            .Include(a => a.CitasServicios).ThenInclude(a => a.Servicio)
            .Include(a => a.Sucursal)
            .Include(a => a.CitasServicios).ThenInclude(a => a.Servicio)
            .Include(a => a.Servicio)
            .Where(a => a.Eliminado == false)
            .Where(a => a.FechaInicio == fecha).ToList();

            if (citas != null)
            {
                foreach (var cita in citas)
                {
                    cita.CitasServicios = cita.CitasServicios
                        .Where(a => !a.Eliminado)
                        .ToList();
                }
            }

            return citas;
        }
        //Imprime y muestra las citas de 3 dias despues
        public IList<Citas> GetListCitasProximas()
        {
            DateTime fechaHoy = DateTime.Today.AddDays(1);
            DateTime fechaTesDiasDespues = fechaHoy.AddDays(3);
            var citas = _context.Citass
            .Include(a => a.Especialidad)
            .Include(a => a.Paciente)
                        .Include(a => a.Habitacion)

            .Include(a => a.CitasServicios)
            .Where(a => a.FechaInicio > fechaHoy && a.FechaInicio <= fechaTesDiasDespues)
            .Where(a => a.Eliminado == false && a.Finalizada == false)
            .ToList();

            return citas;
        }

        public bool ExisteConflictoEmpleado(int empleadoId, DateTime inicio, DateTime fin, int? citaIdExcluir = null)
        {
            // Regla de traslape (intervalos):
            // Existe conflicto si una cita existente cumple:
            //   cita.FechaInicio < fin  &&  cita.FechaFinal > inicio
            // Adicional defensivo:
            //   Si existe una cita con FechaFinal == FechaInicio (duración 0), se considera conflicto
            //   cuando cae dentro del rango [inicio, fin).
            // Se excluye la misma cita (edición) cuando aplica.

            var query = _context.Citass
                .Where(c => !c.Eliminado)
                .Where(c => c.EmpleadoId != null && c.EmpleadoId == empleadoId)
                .Where(c => c.FechaInicio.HasValue && c.FechaFinal.HasValue)
                .Where(c =>
                    // traslape normal
                    (c.FechaInicio.Value < fin && c.FechaFinal.Value > inicio)
                    ||
                    // defensivo: cita con duración 0 dentro del rango
                    (c.FechaFinal.Value == c.FechaInicio.Value &&
                     c.FechaInicio.Value >= inicio &&
                     c.FechaInicio.Value < fin)
                );

            if (citaIdExcluir.HasValue)
            {
                query = query.Where(c => c.Id != citaIdExcluir.Value);
            }

            return query.Any();
        }



        public Citas GetCitaActivaPorHabitacion(int habitacionId)
        {
            return _context.Citass
                .Where(c => c.HabitacionId == habitacionId
                         && !c.Eliminado
                         && c.Finalizada == false)
                .OrderByDescending(c => c.FechaInicio)
                .FirstOrDefault();
        }

    }
}