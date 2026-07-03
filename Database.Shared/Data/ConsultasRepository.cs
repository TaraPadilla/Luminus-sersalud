using Database.Shared.Models;
using Database.Shared.IRepository;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Paginacion;
using System;

namespace Database.Shared.Data
{
    public class ConsultasRepository : IConsultas
    {
        private readonly Context _context = null;

        public ConsultasRepository(Context context)
        {
            _context = context;
        }
        public void Add(Consulta consulta, bool saveChanges = true)
        {
            _context.Consultas.Add(consulta);
            if (saveChanges) _context.SaveChanges();
        }
        public int AddConsulta(Consulta consulta)
        {
            _context.Consultas.Add(consulta);
            _context.SaveChanges();
            return consulta.Id;
        }
        public void AddExamenFisico(ExamenFisico examenFisico)
        {
            _context.ExamenFisico.Add(examenFisico);
            _context.SaveChanges();

        }
        public void Update(Consulta consulta, bool saveChanges = true)
        {
            _context.Entry(consulta).State = EntityState.Modified;

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }
        public void Update(ExamenFisico examen, bool saveChanges = true)
        {
            _context.Entry(examen).State = EntityState.Modified;

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }
        public void Update(Prescripcion prescripcion)
        {
            _context.Entry(prescripcion).State = EntityState.Modified;
            _context.SaveChanges();
        }

        // public IList<Consulta> ListaConsultas()
        // {
        //     return _context.Consultas
        //         .Include(a => a.ConsultasServicios).ThenInclude(a => a.Servicio)
        //         .Include(a => a.Citas).ThenInclude(a => a.Paciente)
        //         .Include(a => a.Citas).ThenInclude(a => a.Empleado)
        //         .Include(a => a.Citas).ThenInclude(a => a.Servicio)
        //         .Include(a => a.Citas).ThenInclude(a => a.Especialidad)
        //         .Include(a => a.EstadoPagoConsulta)
        //         .Include(a => a.MedicamentosOtros)
        //         .OrderByDescending(a => a.FechaYHoraInicioConsulta).ToList();
        // }

        public IList<Consulta> ListaConsultas()
        {
            return _context.Consultas
                .AsNoTracking()
                .Include(a => a.ConsultasServicios).ThenInclude(a => a.Servicio) // USADO en tabla (Servicios)
                .Include(a => a.Citas).ThenInclude(a => a.Paciente)              // USADO en tabla (IGSS)
                .Include(a => a.Citas).ThenInclude(a => a.Empleado)              // USADO en tabla (Médico)
                                                                                 //.Include(a => a.Citas).ThenInclude(a => a.Servicio)            // NO USADO en tabla
                .Include(a => a.Citas).ThenInclude(a => a.Especialidad)          // USADO en tabla (Especialidad)
                .Include(a => a.EstadoPagoConsulta)                              // USADO en tabla (Pago)
                                                                                 //.Include(a => a.MedicamentosOtros)                              // NO USADO en tabla
                .OrderByDescending(a => a.FechaYHoraInicioConsulta)
                .ToList();
        }

        public IQueryable<Consulta> QueryConsultasParaListado()
        {
            return _context.Consultas
                .AsNoTracking()
                .Where(a => !a.Eliminado)

                // --- USADAS por la tabla ---
                .Include(a => a.Citas).ThenInclude(a => a.Paciente)
                .Include(a => a.Citas).ThenInclude(a => a.Empleado)
                .Include(a => a.Citas).ThenInclude(a => a.Especialidad)
                .Include(a => a.EstadoPagoConsulta)
                .Include(a => a.ConsultasServicios).ThenInclude(a => a.Servicio)

                // --- NO usadas en el listado (dejar comentadas) ---
                //.Include(a => a.Citas).ThenInclude(a => a.Servicio)
                //.Include(a => a.MedicamentosOtros)

                ;
        }


        public Consulta GetUltimaConsultaPaciente(int? idPaciente)
        {
            return _context.Consultas
                .Include(c => c.Citas)
                .Where(c => c.Citas.PacienteId == idPaciente)
                .OrderByDescending(c => c.Id)
                .FirstOrDefault();
        }

        public List<ConsultaCaracteristicaDental> GetCaracteristicasDentales(int? idConsulta)
        {
            return _context.ConsultasCaracteristicasDentales
            .Where(c => c.ConsultaId == (int)idConsulta)
            .ToList();
        }
        public void DeleteArchivoConsulta(int archivoId)
        {
            var archivo = _context.Archivos.Where(a => a.Id == archivoId).FirstOrDefault();
            if (archivo != null)
            {
                archivo.Eliminado = true;
                _context.Entry(archivo).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }
        public List<ConsultaServicio> GetServiciosAgregados(int consultaId)
        {
            return _context.ConsultasServicios
            .Include(c => c.Servicio)
            .Include(c => c.Precio)
            .Where(c => c.ConsultaId == consultaId)
            .ToList();
        }

        public List<ConsultaExamenLabClinico> GetExamenesAgregadosConsulta(int consultaId)
        {
            return _context.ConsultasExamenLabClinicos
            .Include(c => c.ExamenLabClinico)
            .Include(c => c.Precio)
            .Where(c => c.ConsultaId == consultaId)
            .ToList();
        }

        public Consulta GetConsultaPorHospitalizacion(int hospitalizacionId)
        {
            var consulta = _context.Consultas
                .Include(a => a.Historia)
                .Include(a => a.HistoriaPediatria)
                .Include(a => a.Prescripciones)
                    .ThenInclude(a => a.DetallePrescripcion)
                .Include(t => t.ExamenFisico)
                .Include(t => t.ExamenFisicoPediatria)
                .Include(t => t.ConsultaAntPatologicosGinecologia)
                .Include(t => t.ConsultaAntNoPatologicosGinecologia)
                .Include(t => t.ConsultaAntNoPatologicosObstetricia)
                .Include(a => a.ConsultasCaracteristicasDentales)
                .Include(a => a.ConsultaExamenFisicoGinecologia)
                .Include(a => a.ConsultasServicios).ThenInclude(a => a.Servicio)
                .Include(a => a.EstadoPagoConsulta)
                .Include(a => a.Citas).ThenInclude(a => a.Empleado)
                .Include(a => a.Citas).ThenInclude(a => a.Paciente)
                .Include(a => a.Citas).ThenInclude(a => a.Especialidad)
                .Include(a => a.Citas).ThenInclude(a => a.Servicio)
                .Where(c => c.HospitalizacionId == hospitalizacionId)
                .FirstOrDefault();

            if (consulta != null)
            {
                // Cargar prescripción no eliminada
                var prescripcion = consulta.Prescripciones
                    .Where(a => !a.Eliminada)
                    .FirstOrDefault();
                consulta.Prescripciones = new List<Prescripcion>();
                if (prescripcion != null)
                    consulta.Prescripciones.Add(prescripcion);

                // Cargar hospitalización
                if (consulta.Hospitalizado && consulta.HospitalizacionId != null)
                {
                    consulta.Hospitalizacion = _context.Hospitalizaciones
                        .Include(a => a.Habitacion)
                        .Where(a => a.Id == consulta.HospitalizacionId)
                        .FirstOrDefault();
                }

                // Cargar cita con todas las entidades relacionadas
                consulta.Citas = _context.Citass
                    .Include(a => a.Especialidad)
                    .Include(a => a.Servicio)
                    .Include(a => a.Empleado)
                    .Include(b => b.Paciente)
                    .Include(a => a.Paciente).ThenInclude(a => a.PacienteApnp)
                    .Include(a => a.Paciente).ThenInclude(a => a.PacientePediatricoApnp)
                    .Include(a => a.Paciente).ThenInclude(a => a.Sexo)
                    .Include(a => a.Paciente).ThenInclude(a => a.PacientesSeguimientosNutricionales)
                    .Include(a => a.Paciente).ThenInclude(a => a.VacunasPaciente)
                    .Include(a => a.Paciente).ThenInclude(a => a.VacunasPaciente).ThenInclude(a => a.Vacuna)
                    .Include(a => a.Paciente).ThenInclude(a => a.PatologiasPaciente)
                    .Include(a => a.Paciente).ThenInclude(a => a.PatologiasPaciente).ThenInclude(a => a.TipoPatologia)
                    .Where(a => a.Id == consulta.CitasId)
                    .FirstOrDefault();

                // Cargar exámenes agregados
                consulta.ConsultaExamenesAgregados = _context.ConsultasExamenLabClinicos
                    .Where(a => a.ConsultaId == consulta.Id)
                    .ToList();

                // Cargar revisión de sistemas
                consulta.ConsultaRevisionSistemas = _context.ConsultaRevisionSistemas
                    .Where(a => a.Id == consulta.ConsultaRevisionSistemasId)
                    .FirstOrDefault();

                consulta.ConsultaRevisionSistemasPediatria = _context.ConsultaRevisionSistemasPediatria
                    .Where(a => a.Id == consulta.ConsultaRevisionSistemasPediatriaId)
                    .FirstOrDefault();

                consulta.ConsultaAntNoPatologicosGinecologia = _context.ConsultaAntNoPatologicosGinecologia
                    .Where(a => a.Id == consulta.ConsultaAntNoPatologicosGinecologiaId)
                    .FirstOrDefault();

                consulta.ConsultaAntNoPatologicosObstetricia = _context.ConsultaAntNoPatologicosObstetricia
                    .Where(a => a.Id == consulta.ConsultaAntNoPatologicosObstetriciaId)
                    .FirstOrDefault();

                consulta.ConsultaAntPatologicosGinecologia = _context.ConsultaAntPatologicosGinecologia
                    .Where(a => a.Id == consulta.ConsultaAntPatologicosGinecologiaId)
                    .FirstOrDefault();

                consulta.ConsultaExamenFisicoGinecologia = _context.ConsultaExamenFisicoGinecologia
                    .Where(a => a.Id == consulta.ConsultaExamenFisicoGinecologiaId)
                    .FirstOrDefault();
            }

            return consulta;
        }

        public Consulta GetConsulta(int id, bool relatedEntities = true)
        {
            if (relatedEntities)
            {
                var consulta = _context.Consultas
                    .Include(a => a.Historia)
                    .Include(a => a.HistoriaPediatria)
                    .Include(a => a.Prescripciones)
                        .ThenInclude(a => a.DetallePrescripcion)
                    .Include(t => t.ExamenFisico)
                    .Include(t => t.ExamenFisicoPediatria)
                    .Include(t => t.ConsultaAntPatologicosGinecologia)
                    .Include(t => t.ConsultaAntNoPatologicosGinecologia)
                    .Include(t => t.ConsultaAntNoPatologicosObstetricia)
                    .Include(a => a.ConsultasCaracteristicasDentales)
                    .Include(a => a.ConsultaExamenFisicoGinecologia)
                    .Include(a => a.ConsultasServicios).ThenInclude(a => a.Servicio)
                    .Include(a => a.EstadoPagoConsulta)
                    .Include(a => a.Citas).ThenInclude(a => a.Empleado)
                    .Include(a => a.MedicamentosOtros)
                    .Where(a => a.Id == id)
                    .FirstOrDefault();

                var prescripcion = consulta.Prescripciones
                    .Where(a => !a.Eliminada)
                    .FirstOrDefault();
                consulta.Prescripciones = new List<Prescripcion>();
                if (prescripcion != null)
                    consulta.Prescripciones.Add(prescripcion);

                if (consulta.Hospitalizado && consulta.HospitalizacionId != null)
                {
                    consulta.Hospitalizacion = _context.Hospitalizaciones
                        .Include(a => a.Habitacion)
                        .Where(a => a.Id == consulta.HospitalizacionId)
                        .FirstOrDefault();
                }

                consulta.Citas = _context.Citass
                    .Include(a => a.Especialidad)
                    .Include(a => a.Servicio)
                    .Include(a => a.Empleado)
                    .Include(b => b.Paciente)
                    .Include(a => a.Paciente).ThenInclude(a => a.PacienteApnp)
                    .Include(a => a.Paciente).ThenInclude(a => a.PacientePediatricoApnp)
                    .Include(a => a.Paciente).ThenInclude(a => a.Sexo)
                    .Include(a => a.Paciente).ThenInclude(a => a.PacientesSeguimientosNutricionales)
                    .Include(a => a.Paciente).ThenInclude(a => a.VacunasPaciente)
                    .Include(a => a.Paciente).ThenInclude(a => a.VacunasPaciente).ThenInclude(a => a.Vacuna)
                    .Include(a => a.Paciente).ThenInclude(a => a.PatologiasPaciente)
                    .Include(a => a.Paciente).ThenInclude(a => a.PatologiasPaciente).ThenInclude(a => a.TipoPatologia)
                    .Where(a => a.Id == consulta.CitasId)
                    .FirstOrDefault();



                consulta.ConsultaExamenesAgregados = _context.ConsultasExamenLabClinicos
                    .Where(a => a.ConsultaId == consulta.Id)
                    .ToList();
                consulta.ConsultaRevisionSistemas = _context.ConsultaRevisionSistemas
                    .Where(a => a.Id == consulta.ConsultaRevisionSistemasId)
                    .FirstOrDefault();
                consulta.ConsultaRevisionSistemasPediatria = _context.ConsultaRevisionSistemasPediatria
                    .Where(a => a.Id == consulta.ConsultaRevisionSistemasPediatriaId)
                    .FirstOrDefault();

                consulta.ConsultaAntNoPatologicosGinecologia = _context.ConsultaAntNoPatologicosGinecologia
                    .Where(a => a.Id == consulta.ConsultaAntNoPatologicosGinecologiaId)
                    .FirstOrDefault();

                consulta.ConsultaAntNoPatologicosObstetricia = _context.ConsultaAntNoPatologicosObstetricia
                    .Where(a => a.Id == consulta.ConsultaAntNoPatologicosObstetriciaId)
                    .FirstOrDefault();

                consulta.ConsultaAntPatologicosGinecologia = _context.ConsultaAntPatologicosGinecologia
                    .Where(a => a.Id == consulta.ConsultaAntPatologicosGinecologiaId)
                    .FirstOrDefault();

                consulta.ConsultaExamenFisicoGinecologia = _context.ConsultaExamenFisicoGinecologia
                    .Where(a => a.Id == consulta.ConsultaExamenFisicoGinecologiaId)
                    .FirstOrDefault();

                return consulta;
            }
            else
            {
                return _context.Consultas.Where(a => a.Id == id).SingleOrDefault();
            }
        }
        public void AddPrescipcion(Prescripcion prescripcion)
        {
            _context.Prescripciones.Add(prescripcion);
            _context.SaveChanges();
        }

        public void AddDetallePrescipcion(DetallePrescripcion detallePrescripcion)
        {
            _context.DetallePrescripcion.Add(detallePrescripcion);
            _context.SaveChanges();
        }
        public Prescripcion GetPrescripcion(int prescripcionId)
        {
            return _context.Prescripciones
            .Include(x => x.Citas).ThenInclude(x => x.Paciente)
            .Include(x => x.Citas).ThenInclude(c => c.Empleado)
            .Include(x => x.Consulta).ThenInclude(x => x.Citas).ThenInclude(x => x.Paciente)
            .Include(x => x.DetallePrescripcion)
                .ThenInclude(a => a.Producto)
            .Include(x => x.DetallePrescripcion)
                .ThenInclude(a => a.UnidadMedidaVenta)
            .FirstOrDefault(x => x.Id == prescripcionId);
        }


        public Prescripcion GetPrescripcionConsulta(int consultaId, bool includeProducto = false)
        {
            var prescripcion = _context.Prescripciones
                .Include(a => a.DetallePrescripcion)
                .Where(a => a.ConsultaId == consultaId)
                .OrderByDescending(a => a.Id)
                .FirstOrDefault();
            if (includeProducto)
            {
                if (prescripcion != null && prescripcion.DetallePrescripcion != null)
                {
                    foreach (var detalle in prescripcion.DetallePrescripcion)
                    {
                        if (detalle.ProductoId != null)
                        {
                            detalle.Producto = _context.Productos
                                .Where(a => a.Id == detalle.ProductoId)
                                .FirstOrDefault();
                        }
                        if (detalle.PrecioId != null)
                        {
                            detalle.Precio = _context.Precios
                                .Where(a => a.Id == detalle.PrecioId)
                                .FirstOrDefault();
                        }
                        if (detalle.UnidadMedidaVentaId != null)
                        {
                            detalle.UnidadMedidaVenta = _context.UnidadesMedidaVenta
                                .Where(a => a.Id == detalle.UnidadMedidaVentaId)
                                .FirstOrDefault();
                        }
                    }
                }
            }
            return prescripcion;
        }
        public List<Prescripcion> GetPrescripcionesCita(int citaId)
        {
            return _context.Prescripciones
                .Include(a => a.DetallePrescripcion)
                    .ThenInclude(a => a.Precio)
                .Include(a => a.DetallePrescripcion)
                    .ThenInclude(a => a.UnidadMedidaVenta)
                .Include(a => a.DetallePrescripcion)
                    .ThenInclude(a => a.Producto)
                .Where(a => a.CitasId == citaId)
                .OrderByDescending(a => a.Id)
                .ToList();
        }
        public List<Prescripcion> GetPrescripcionesPaciente(int pacienteId)
        {
            return _context.Prescripciones
                .Include(p => p.DetallePrescripcion)
                    .ThenInclude(d => d.Precio)
                .Include(p => p.DetallePrescripcion)
                    .ThenInclude(d => d.UnidadMedidaVenta)
                .Include(p => p.DetallePrescripcion)
                    .ThenInclude(d => d.Producto)
                .Include(p => p.Consulta)
                    .ThenInclude(c => c.Citas)
                .Where(p =>
                    p.Consulta != null &&
                    p.Consulta.Citas != null &&
                    p.Consulta.Citas.PacienteId == pacienteId
                )
                .Where(p => !p.Eliminada)
                .OrderByDescending(p => p.Id)
                .ToList();
        }

        public void UpdateTablePrescription()
        {
            _context.Database.ExecuteSqlRaw("UPDATE public.\"Prescripcion\" SET \"ConsultaId1\" = \"ConsultaId\";");
            _context.SaveChanges();
        }

        public ExamenLabClinico GetExamenLabClincos(string codigo)
        {
            return _context.ExamenLabClinicos
                .Include(a => a.CategoriaLabClinico)
                .Where(a => a.CodigoInterno == codigo).FirstOrDefault();
        }

        public List<Archivo> GetArchivos(int consultaId)
        {
            return _context.Archivos
                .Where(a => a.ConsultaId == consultaId
                && !a.Eliminado)
                .ToList();
        }
        public List<Archivo> GetArchivosNew(int pacienteId)
        {
            return _context.Archivos
                .Where(a =>
                    !a.Eliminado &&
                    a.Consulta != null &&
                    a.Consulta.Citas != null &&
                    a.Consulta.Citas.PacienteId == pacienteId)
                .Include(a => a.Consulta)
                    .ThenInclude(c => c.Citas)
                .ToList();
        }


        public List<ConsultaExamenArchivo> GetExamenArchivo(int consultaId)
        {
            return _context.ConsultaExamenArchivo
            .Where(a => a.ConsultaId == consultaId)
            .ToList();
        }

        public void AddExamnenArchivo(ConsultaExamenArchivo examenArchivo, bool saveChanges = true)
        {
            _context.ConsultaExamenArchivo.Add(examenArchivo);
            if (saveChanges) _context.SaveChanges();
        }


        public void UpdateExamnenArchivo(ConsultaExamenArchivo examenArchivo, bool saveChanges = true)
        {
            _context.Entry(examenArchivo).State = EntityState.Modified;

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        /*      public List<ConsultaExamenLabClinico> GetHistorialExamenesPaciente(int pacienteId)
             {
                 return _context.ConsultasExamenLabClinicos
                     .Include(e => e.ExamenLabClinico)
                     .Include(e => e.Precio)
                     .Include(e => e.Consulta)
                         .ThenInclude(c => c.Citas)
                     .Where(e =>
                         e.Consulta != null &&
                         e.Consulta.Citas != null &&
                         e.Consulta.Citas.PacienteId == pacienteId)
                     .ToList();
             }

             public List<ConsultaServicio> GetHistorialServiciosPaciente(int pacienteId)
             {
                 return _context.ConsultasServicios
                     .Include(s => s.Servicio)
                     .Include(s => s.Precio)
                     .Include(s => s.Consulta)
                         .ThenInclude(c => c.Citas)
                     .Where(s =>
                         s.Consulta != null &&
                         s.Consulta.Citas != null &&
                         s.Consulta.Citas.PacienteId == pacienteId)
                     .ToList();
             } */


    }
}