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
    public class HospitalizacionRepository : IHospitalizacion
    {
        private readonly Context _context = null;

        public HospitalizacionRepository(Context context)
        {
            _context = context;
        }

        public Hospitalizacion Add(Hospitalizacion hospitalizacion, bool saveChanges = true)
        {
            _context.Hospitalizaciones.Add(hospitalizacion);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
            return hospitalizacion;
        }
        public Hospitalizacion Get(
            int hospitalizacionId,
            bool includeMedicamentos = true,
            bool includeServicios = true,
            bool includeExamenes = true,
            bool includePaquetes = true,
            bool includeConsultas = true,
            bool includeOrdenesMedicas = true
            )
        {
            var hospitalizacion = _context.Hospitalizaciones
              .Include(a => a.HospitalizacionUsuariosAcceso)
              .Include(a => a.CategoriaHabitacionTarifa)
              .Include(a => a.Habitacion)
                  .ThenInclude(h => h.CategoriaHabitacion)
              .Include(a => a.Habitacion)
                  .ThenInclude(h => h.EstadoHabitacion)
              .Include(a => a.Paciente)
                  .ThenInclude(p => p.TipoPaciente)
              .Include(a => a.Paciente)
                  .ThenInclude(p => p.EstadoPaciente)
              .Include(a => a.Paciente)
                  .ThenInclude(p => p.Sexo)
              .Include(x => x.Especialidad)
              .Include(a => a.Consultas)
                  .ThenInclude(c => c.Citas)
                      .ThenInclude(ci => ci.Empleado)
                          .ThenInclude(e => e.Especialidad)
              .Where(a => a.Id == hospitalizacionId && !a.Eliminada)
              .FirstOrDefault();




            if (hospitalizacion == null)
            {
                return null;
            }
            //Filtrar hospitalizaciones usuarios acceso y traer los NO Eliminados
            if (hospitalizacion.HospitalizacionUsuariosAcceso != null)
            {
                hospitalizacion.HospitalizacionUsuariosAcceso =
                    hospitalizacion.HospitalizacionUsuariosAcceso
                    .Where(a => !a.Eliminado).ToList();
            }

            if (includeMedicamentos)
            {
                hospitalizacion.HospitalizacionesProductos = _context.HospitalizacionesProductos
                    .Include(a => a.HospitalizacionesProductosAplicaciones)
                    .Include(a => a.Producto)
                    .Where(a => a.HospitalizacionId == hospitalizacion.Id
                    && !a.Eliminado)
                    .ToList();
            }
            if (includeServicios)
            {
                hospitalizacion.HospitalizacionesServicios = _context.HospitalizacionesServicios
                    .Include(a => a.PrecioServicio)
                    .Include(a => a.Servicio)
                    .Where(a => a.HospitalizacionId == hospitalizacion.Id
                    && !a.Eliminado)
                    .ToList();
            }
            if (includeConsultas)
            {
                hospitalizacion.Consultas = _context.Consultas
                    .Include(a => a.ConsultasServicios)
                        .ThenInclude(a => a.Servicio)
                    .Include(a => a.ConsultasServicios)
                        .ThenInclude(a => a.Precio)
                    .Where(a => a.HospitalizacionId == hospitalizacion.Id)
                    .ToList();
            }
            if (includeExamenes)
            {
                hospitalizacion.HospitalizacionesExamenes = _context.HospitalizacionesExamenes
                    .Include(a => a.Examen).ThenInclude(a => a.DetalleExamenes).ThenInclude(a => a.ExamenLabClinico)
                    .Where(a => a.HospitalizacionId == hospitalizacion.Id
                    && !a.Eliminado)
                    .ToList();
            }
            if (includePaquetes)
            {
                hospitalizacion.HospitalizacionesPaquetesHospitalizacion = _context.HospitalizacionesPaquetesHospitalizacion
                    .Include(a => a.PaqueteHospitalizacion)
                    .Where(a => a.HospitalizacionId == hospitalizacion.Id && !a.Eliminado)
                    .ToList();
            }
            // Incluir las ordenes médicas
            if (includeOrdenesMedicas)
            {
                hospitalizacion.OrdenesMedicas = _context.OrdenesMedicas
                    .Where(a => a.HospitalizacionId == hospitalizacion.Id)
                    .ToList();
            }

            return hospitalizacion;
        }
        public List<HospitalizacionCambioHabitacion> GetCambiosHabitacion(int hospitalizacionId)
        {
            return _context.HospitalizacionCambiosHabitacion
                .Where(c => c.HospitalizacionId == hospitalizacionId)
                .OrderByDescending(c => c.FechaCambio)
                .ToList();
        }
        public void AddCambioHabitacion(HospitalizacionCambioHabitacion cambio)
        {
            _context.HospitalizacionCambiosHabitacion.Add(cambio);
            _context.SaveChanges();
        }
        public HospitalizacionCambioHabitacion GetCambioHabitacion(int cambioHabitacionId)
        {
            return _context.HospitalizacionCambiosHabitacion
                .FirstOrDefault(c => c.Id == cambioHabitacionId);
        }
        public void DeleteCambioHabitacion(int cambioHabitacionId)
        {
            var cambio = _context.HospitalizacionCambiosHabitacion
                .FirstOrDefault(c => c.Id == cambioHabitacionId);
            if (cambio != null)
            {
                _context.HospitalizacionCambiosHabitacion.Remove(cambio);
                _context.SaveChanges();
            }
        }
        public void UpdateCambioHabitacion(HospitalizacionCambioHabitacion cambio)
        {
            _context.HospitalizacionCambiosHabitacion.Update(cambio);
            _context.SaveChanges();
        }

        public Hospitalizacion GetHospitalizacionById(int hospitalizacionId)
        {
            return _context.Hospitalizaciones
                .FirstOrDefault(h => h.Id == hospitalizacionId && !h.Eliminada);
        }
        public void AddServicio(HospitalizacionServicio hospitalizacionServicio)
        {
            _context.HospitalizacionesServicios.Add(hospitalizacionServicio);
            SaveChanges();
        }
        public HospitalizacionServicio GetHospitalizacionServicio(int hospitalizacionServicioId)
        {
            return _context.HospitalizacionesServicios.Find(hospitalizacionServicioId);
        }
        public HospitalizacionProducto AddMedicamento(HospitalizacionProducto hospitalizacionProducto)
        {
            _context.HospitalizacionesProductos.Add(hospitalizacionProducto);
            SaveChanges();
            return hospitalizacionProducto;
        }
        public HospitalizacionProducto GetHospitalizacionMedicamento(int hospitalizacionMedicamentoId)
        {
            return _context.HospitalizacionesProductos.Find(hospitalizacionMedicamentoId);
        }
        public void AddProductoAplicacion(HospitalizacionProductoAplicacion hospitalizacionProductoAplicacion)
        {
            _context.HospitalizacionesProductosAplicaciones.Add(hospitalizacionProductoAplicacion);
            SaveChanges();
        }
        public void AddExamen(HospitalizacionExamen hospitalizacionExamen)
        {
            _context.HospitalizacionesExamenes.Add(hospitalizacionExamen);
            SaveChanges();
        }
        public HospitalizacionExamen GetHospitalizacionExamen(int hospitalizacionExamenId)
        {
            return _context.HospitalizacionesExamenes.Find(hospitalizacionExamenId);
        }
        public List<HospitalizacionServicio> GetServicios(int hospitalizacionId)
        {
            return _context.HospitalizacionesServicios
                .Include(s => s.Servicio)
                    .ThenInclude(x => x.ServiciosPrecios)
                .Where(s => s.HospitalizacionId == hospitalizacionId
                && !s.Eliminado)
                .ToList();
        }
        public List<HospitalizacionProducto> GetProductos(int hospitalizacionId)
        {
            return _context.HospitalizacionesProductos
                .Include(s => s.Producto)
                .Include(s => s.HospitalizacionesProductosAplicaciones)
                .Where(s => s.HospitalizacionId == hospitalizacionId
                && !s.Eliminado)
                .ToList();
        }
        public List<HospitalizacionProductoAplicacion> GetProductosAplicacion(int hospitalizacionId)
        {

            var productosAplicacion = _context.HospitalizacionesProductosAplicaciones
                .Include(s => s.HospitalizacionProducto).ThenInclude(s => s.Producto)
                .Where(s => s.HospitalizacionProducto.HospitalizacionId == hospitalizacionId)
                .OrderBy(s => s.Aplicado)
                .ThenByDescending(s => s.FechaHoraAplicacion.HasValue)
                .ThenByDescending(s => s.FechaHoraAplicacion)
                .ToList();



            return productosAplicacion;
        }


        public HospitalizacionProductoAplicacion GetProductoAplicacion(int hospitalizacionProductoAplicacionId)
        {
            return _context.HospitalizacionesProductosAplicaciones
                .Include(a => a.HospitalizacionProducto)
                .Where(s => s.Id == hospitalizacionProductoAplicacionId)
                .FirstOrDefault();
        }
        public HospitalizacionServicio GetServicioHospitalizacion(int hospitalizacionServicioId)
        {
            return _context.HospitalizacionesServicios
                .Include(a => a.Servicio)
                .Where(s => s.Id == hospitalizacionServicioId)
                .FirstOrDefault();
        }
        public List<NotaMedica2> GetNotasMedicasByHospitalizacion(int hospitalizacionId)
        {
            return _context.NotaMedica2
                .AsNoTracking()
                .Include(nm => nm.Profesional)
                    .ThenInclude(p => p.Persona)
                .Include(nm => nm.Hospitalizacion)
                    .ThenInclude(h => h.Paciente)
                        .ThenInclude(p => p.Sexo)
                .Include(nm => nm.Hospitalizacion)
                    .ThenInclude(h => h.Consultas)
                        .ThenInclude(c => c.Citas)
                            .ThenInclude(cita => cita.Empleado)
                                .ThenInclude(e => e.Especialidad)
                .Where(nm => nm.HospitalizacionId == hospitalizacionId)
                .OrderByDescending(nm => nm.FechaRegistro)
                .ToList();
        }


        public NotaMedica2 GetNotaMedica2(int notaMedicaId)
        {
            return _context.NotaMedica2
                .Include(nm => nm.Profesional)
                .Include(nm => nm.Hospitalizacion)
                    .ThenInclude(h => h.Paciente)
                        .ThenInclude(p => p.Sexo)
                .Include(nm => nm.Hospitalizacion)
                    .ThenInclude(h => h.Consultas)
                        .ThenInclude(c => c.Citas)
                            .ThenInclude(cita => cita.Empleado)
                .Where(nm => nm.Id == notaMedicaId)
                .FirstOrDefault();
        }

        public OrdenesMedicas GetOrdenMedica(int ordenMedicaId)
        {
            return _context.OrdenesMedicas
                .Include(nm => nm.Hospitalizacion)
                    .ThenInclude(h => h.Paciente)
                        .ThenInclude(p => p.Sexo)
                .Include(o => o.Hospitalizacion)
                    .ThenInclude(h => h.Consultas)
                        .ThenInclude(c => c.Citas)
                            .ThenInclude(cita => cita.Empleado)
                .FirstOrDefault(o => o.Id == ordenMedicaId);
        }

        public void UpdateProductoAplicacion(HospitalizacionProductoAplicacion hospitalizacionProductoAplicacion)
        {
            _context.Entry(hospitalizacionProductoAplicacion).State = EntityState.Modified;
            _context.SaveChanges();
        }
        public List<HospitalizacionExamen> GetExamenes(int hospitalizacionId)
        {
            return _context.HospitalizacionesExamenes
                .Include(s => s.Examen)
                    .ThenInclude(a => a.DetalleExamenes)
                        .ThenInclude(a => a.ExamenLabClinico)
                            .ThenInclude(a => a.CategoriaLabClinico) // Incluir la categoría
                .Include(s => s.ExamenLabClinicoPrecio)
                .Where(s => s.HospitalizacionId == hospitalizacionId && !s.Eliminado)
                .ToList();
        }
        public void AddExamenFisicoHosp(ExamenFisicoHosp examen)
        {
            _context.ExamenesFisicosHosp
                .Add(examen);
            _context.SaveChanges();
        }
        public List<DatoExamenFisicoHosp> GetDatosExamenFisicoHosp()
        {
            return _context.DatosExamenFisicoHosp
                .ToList();
        }
        public List<ExamenFisicoHosp> GetExamenesFisicosHosp(int hospitalizacionId)
        {
            return _context.ExamenesFisicosHosp
                .Include(a => a.ExamenesFisicosHospDatos)
                .ThenInclude(a => a.DatoExamenFisicoHosp)
                .Where(a => a.HospitalizacionId == hospitalizacionId)
                .ToList();
        }
        public void Update(Hospitalizacion hospitalizacion)
        {
            _context.Entry(hospitalizacion).State = EntityState.Modified;
            SaveChanges();
        }
        public void Update(HospitalizacionServicio hospitalizacionServicio)
        {
            _context.Entry(hospitalizacionServicio).State = EntityState.Modified;
            SaveChanges();
        }
        public void Update(HospitalizacionProducto hospitalizacionProducto)
        {
            _context.Entry(hospitalizacionProducto).State = EntityState.Modified;
            SaveChanges();
        }
        public void Update(HospitalizacionExamen hospitalizacionExamen)
        {
            _context.Entry(hospitalizacionExamen).State = EntityState.Modified;
            SaveChanges();
        }
        public void Update(HospitalizacionReceta hospReceta)
        {
            _context.Entry(hospReceta).State = EntityState.Modified;
            SaveChanges();
        }
        public void Update(HospitalizacionPaqueteHospitalizacion hospitalizacionPaqueteHospitalizacion)
        {
            _context.Entry(hospitalizacionPaqueteHospitalizacion).State = EntityState.Modified;
            SaveChanges();
        }
        public PaqueteHospitalizacion AddPaqueteHospitalizacion(PaqueteHospitalizacion paquete, bool saveChanges = true)
        {
            _context.PaquetesHospitalizacion.Add(paquete);
            if (saveChanges)
            {
                _context.SaveChanges();
            }
            return paquete;
        }
        public PaqueteHospitalizacion GetPaqueteHospitalizacion(int paqueteId)
        {
            var paquete = _context.PaquetesHospitalizacion
                .Include(a => a.DetallePaquetesHospitalizacion).ThenInclude(a => a.Servicio)
                .Include(a => a.DetallePaquetesHospitalizacion).ThenInclude(a => a.ServicioPrecio).ThenInclude(a => a.Precio)
                .Include(a => a.DetallePaquetesHospitalizacion).ThenInclude(a => a.Producto)
                .Include(a => a.DetallePaquetesHospitalizacion).ThenInclude(a => a.Precio)
                .Include(a => a.DetallePaquetesHospitalizacion).ThenInclude(a => a.UnidadMedidaVenta)
                .Include(a => a.DetallePaquetesHospitalizacion).ThenInclude(a => a.Laboratorio)
                .Include(a => a.DetallePaquetesHospitalizacion).ThenInclude(a => a.LaboratorioPrecio).ThenInclude(a => a.Precio)
                .Where(a => a.Id == paqueteId)
                .FirstOrDefault();

            return paquete;
        }
        public List<PaqueteHospitalizacion> GetPaquetesHospitalizacion()
        {
            return _context.PaquetesHospitalizacion
                .Include(a => a.DetallePaquetesHospitalizacion).ThenInclude(a => a.Servicio)
                .Include(a => a.DetallePaquetesHospitalizacion).ThenInclude(a => a.Producto)
                .Include(a => a.DetallePaquetesHospitalizacion).ThenInclude(a => a.UnidadMedidaVenta)
                .Include(a => a.DetallePaquetesHospitalizacion).ThenInclude(a => a.Laboratorio)
                .Where(a => !a.Eliminado)
                .ToList();
        }
        public void AddHospitalizacionPaqueteHospitalizacion(HospitalizacionPaqueteHospitalizacion paquete)
        {
            _context.HospitalizacionesPaquetesHospitalizacion.Add(paquete);
            SaveChanges();
        }
        public HospitalizacionPaqueteHospitalizacion GetHospitalizacionPaqueteHospitalizacion(int hospitalizacionPaqueteHospitalizacionId)
        {
            return _context.HospitalizacionesPaquetesHospitalizacion.Find(hospitalizacionPaqueteHospitalizacionId);

        }
        public List<HospitalizacionPaqueteHospitalizacion> GetPaquetesAgregados(int hospitalizacionId)
        {
            return _context.HospitalizacionesPaquetesHospitalizacion
                .Include(a => a.PaqueteHospitalizacion)
                    .ThenInclude(b => b.DetallePaquetesHospitalizacion)
                        .ThenInclude(c => c.Servicio)
                            .ThenInclude(c => c.ServiciosPrecios)
.Include(a => a.PaqueteHospitalizacion)
                    .ThenInclude(b => b.DetallePaquetesHospitalizacion)
                        .ThenInclude(c => c.Laboratorio)
                            .ThenInclude(c => c.ExamenLabClinicosPrecios)
.Include(a => a.PaqueteHospitalizacion)
                    .ThenInclude(b => b.DetallePaquetesHospitalizacion)
                        .ThenInclude(c => c.Producto)

                .Include(a => a.PaqueteHospitalizacion)
                    .ThenInclude(b => b.DetallePaquetesHospitalizacion)
                        .ThenInclude(c => c.ProductoInventarioPrecio)

                .Include(a => a.PaqueteHospitalizacion)
                    .ThenInclude(b => b.DetallePaquetesHospitalizacion)
                        .ThenInclude(c => c.ServicioPrecio)

                .Where(a => a.HospitalizacionId == hospitalizacionId
                && !a.Eliminado)
                .ToList();
        }
        public ServicioPrecio GetServicioPrecioById(int id)
        {
            return _context.ServiciosPrecios.Where(x => x.Id == id).FirstOrDefault();
        }
        public void UpdatePaqueteHospitalizacion(PaqueteHospitalizacion paquete)
        {
            _context.Entry(paquete).State = EntityState.Modified;
            SaveChanges();
        }
        public void DeletePaqueteHospitalizacion(int paqueteId)
        {
            var paquete = _context.PaquetesHospitalizacion
                .Where(a => a.Id == paqueteId).FirstOrDefault();
            paquete.Eliminado = true;
            _context.Entry(paquete).State = EntityState.Modified;
            SaveChanges();
        }


        public void AddHospitalizacionReceta(HospitalizacionReceta entity)
        {
            _context.HospitalizacionesReceta.Add(entity);
            SaveChanges();
        }

        public List<HospitalizacionReceta> GetHospitalizacionRecetaByIdHospitalizacion(int hospitalizacionId)
        {
            var recetas = _context.HospitalizacionesReceta
                .Include(a => a.Receta)
                .Where(x => x.HospitalizacionId == hospitalizacionId
                && x.Eliminado == false)
                .Include(x => x.Receta).ToList();

            return recetas;

        }

        //public List<HospitalizacionRecetaDetalle> GetHospitalizacionRecetaDetalleByIdHospitalizacion(int hospitalizacionId)
        //{
        //    var recetas = _context.HospitalizacionRecetaDetalle
        //        .Where(x => x.HospitalizacionReceta.HospitalizacionId == hospitalizacionId
        //        && x.Eliminado == false)
        //        .Include(x => x.HospitalizacionReceta)
        //            .ThenInclude(x => x.Receta)
        //        .ToList();

        //    return recetas;

        //}



        public List<HospitalizacionRecetaDetalle> GetHospitalizacionRecetaDetalleByIdHospitalizacion(int hospitalizacionId)
        {
            var recetas = _context.HospitalizacionRecetaDetalle
                .Include(x => x.HospitalizacionReceta)
                    .ThenInclude(x => x.Receta)

                .Where(x => x.HospitalizacionReceta.HospitalizacionId == hospitalizacionId
                && x.Eliminado == false).ToList();

            return recetas;

        }
        public DetalleCuentaPorCobrar GetDetalleCuenta(int hospitalizacionId)
        {
            return _context.DetallesCuentaPorCobrar
                .Include(a => a.CuentaPorCobrar)
                .Where(a => a.HospitalizacionId == hospitalizacionId)
                .FirstOrDefault();
        }
        public void DeleteHospitalizacionReceta(int IdHospitalizacionReceta)
        {
            var receta = _context.HospitalizacionesReceta
                .Where(x => x.Id == IdHospitalizacionReceta).FirstOrDefault();

            receta.Eliminado = true;
            _context.Update(receta);
            SaveChanges();

        }

        public HospitalizacionReceta GetHospitalizacionRecetaById(int IdHospitalizacionReceta)
        {
            var receta = _context.HospitalizacionesReceta
                .Include(x => x.Receta)
                .Where(x => x.Id == IdHospitalizacionReceta)
                .FirstOrDefault();

            return receta;

        }
        public HospitalizacionRecetaDetalle GetHospitalizacionRecetaDetalleById(int Id)
        {
            var receta = _context.HospitalizacionRecetaDetalle
                .Where(x => x.Id == Id)
                .Include(x => x.HospitalizacionReceta)
                    .ThenInclude(x => x.Receta)
                .FirstOrDefault();

            return receta;

        }


        public void UpdateHospitalicacionReceta(HospitalizacionReceta receta)
        {
            _context.HospitalizacionesReceta.Update(receta);
            SaveChanges();
        }
        public void UpdateHospitalicacionRecetaDetalle(HospitalizacionRecetaDetalle receta)
        {
            _context.HospitalizacionRecetaDetalle.Update(receta);
            SaveChanges();
        }


        public void SaveChanges()
        {
            _context.SaveChanges();
        }
        public List<Hospitalizacion> GetHospitalizaciones()
        {
            return _context.Hospitalizaciones
                .Include(a => a.Especialidad)
                .Include(a => a.Consultas)
                .Include(a => a.Paciente)
                .Include(a => a.Habitacion)
                .ToList();
        }
        public List<HospitalizacionPaqueteHospitalizacion> GetHospitalizacionPaqueteByIdHospitalizacion(int HospitalizacionId)
        {
            return _context.HospitalizacionesPaquetesHospitalizacion.Where(x => x.HospitalizacionId == HospitalizacionId)
                .Include(x => x.PaqueteHospitalizacion)
                .Include(x => x.HospitalizacionDetallePaqueteHospitalizacion)
                    .ThenInclude(x => x.Laboratorio)
                .Include(x => x.HospitalizacionDetallePaqueteHospitalizacion)
                    .ThenInclude(x => x.Examen)
                    .ThenInclude(x => x.DetalleExamenes)
                .Include(x => x.HospitalizacionDetallePaqueteHospitalizacion)
                    .ThenInclude(x => x.Producto)
                .Include(x => x.HospitalizacionDetallePaqueteHospitalizacion)
                    .ThenInclude(x => x.Servicio)
                .Include(x => x.HospitalizacionDetallePaqueteHospitalizacion)
                    .ThenInclude(x => x.LaboratorioPrecio)
                .ToList();
        }
        public HospitalizacionPaqueteHospitalizacion GetHospitalizacionPaqueteHospitalizacionById(int Id)
        {
            return _context.HospitalizacionesPaquetesHospitalizacion.Where(x => x.Id == Id).FirstOrDefault();
        }
        public void UpdateHospitalizacionPaqueteHospitalizacion(HospitalizacionPaqueteHospitalizacion entity)
        {
            _context.HospitalizacionesPaquetesHospitalizacion.Update(entity);
            SaveChanges();
        }


        public NotaOperatoria GetNotaOperatoria(int notaOperatoriaId)
        {
            return _context.NotaOperatoria
                .Include(n => n.User)
                    .ThenInclude(u => u.Persona)
                .Include(n => n.Hospitalizacion)
                    .ThenInclude(h => h.Paciente)
                .Include(n => n.Hospitalizacion)
                    .ThenInclude(h => h.Consultas)
                        .ThenInclude(c => c.Citas)
                            .ThenInclude(ci => ci.Empleado)
                                .ThenInclude(e => e.Especialidad)
                .FirstOrDefault(n => n.Id == notaOperatoriaId);
        }

        public IEnumerable<NotaOperatoria> GetNotasOperatoriasByHospitalizacion(int hospitalizacionId)
        {
            return _context.NotaOperatoria
                .Include(n => n.User)
                    .ThenInclude(u => u.Persona)
                .Include(n => n.Hospitalizacion)
                    .ThenInclude(h => h.Paciente)
                .Include(n => n.Hospitalizacion)
                    .ThenInclude(h => h.Consultas)
                        .ThenInclude(c => c.Citas)
                            .ThenInclude(ci => ci.Empleado)
                                .ThenInclude(e => e.Especialidad)
                .Where(n => n.HospitalizacionId == hospitalizacionId)
                .ToList();
        }

        public List<HospitalizacionGastoAdministrativo> GetGastosAdministrativos(int hospitalizacionId)
        {
            return _context.HospitalizacionGastosAdministrativos
                .Where(g => g.HospitalizacionId == hospitalizacionId)
                .OrderBy(g => g.FechaHora)
                .ToList();
        }

    }

}