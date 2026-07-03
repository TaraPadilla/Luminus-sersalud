using Database.Shared.Models;
using Database.Shared.IRepository;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Paginacion;

namespace Database.Shared.Data
{
    public class LaboratorioClinico : ILaboratorioClinico
    {

        private readonly Context _context = null;

        public LaboratorioClinico(Context context)
        {
            _context = context;
        }

        public PaginacionList<CategoriaLabClinico> PaginacionCategoriasLab(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var categorias = _context.CategoriaLabClinicos.AsQueryable();


            // para hacer la busqueda, el IsNullOrEmpty verifica que no este vacia la cadena entrante
            if (!string.IsNullOrEmpty(searchString))
            {
                categorias = categorias
                .Where(s => s.Nombre.Contains(searchString));
            }

            return PaginacionList<CategoriaLabClinico>.CreateAsyncc(categorias
            .Where(x => x.Eliminado == false)
            .OrderBy(a => a.Nombre),
            pageNumber ?? 1, pageSize);
        }

        //public PaginacionList<VentasLab> PaginacionVentasLab(string sortOrder, string searchString, int? pageNumber, int pageSize)
        //{
        //    var ventas = _context.VentasLabs.AsQueryable();


        //    // para hacer la busqueda, el IsNullOrEmpty verifica que no este vacia la cadena entrante
        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        ventas = ventas
        //        .Where(s => s.Nombres.Contains(searchString));
        //    }

        //    return PaginacionList<VentasLab>.CreateAsyncc(ventas
        //    .Where(x => x.Eliminado == false)
        //    .OrderByDescending(a => a.Id),
        //    pageNumber ?? 1, pageSize);
        //}

        public PaginacionList<ExamenLabClinico> PaginacionExamenClinicoLab(string sortOrder, string searchString, int? pageNumber, int pageSize, int? catexamenid)
        {
            var examen = _context.ExamenLabClinicos
            .Include(a => a.CategoriaLabClinico)
            .Include(a => a.ExamenLabClinicosPrecios).ThenInclude(a => a.Precio)
            .Include(a => a.ExamenLabClinicoInsumo).ThenInclude(a => a.Producto)
            .AsQueryable();


            // para hacer la busqueda, el IsNullOrEmpty verifica que no este vacia la cadena entrante
            if (!string.IsNullOrEmpty(searchString))
            {
                examen = examen
                .Where(s => s.NombreExamen.Contains(searchString));
            }

            if (catexamenid != null) examen =
                 examen.Where(a => a.CategoriaLabClinicoId == catexamenid);


            return PaginacionList<ExamenLabClinico>.CreateAsyncc(examen
            .Where(x => x.Eliminado == false)
            .OrderByDescending(a => a.Id),
            pageNumber ?? 1, pageSize);
        }


        public IQueryable<Examen> ObtenerExamenesQueryable(string searchString, int? estado)
        {
            var examen = _context.Examenes
                .Include(a => a.Medicos)
                .Include(a => a.DetalleExamenes).ThenInclude(a => a.ExamenLabClinico).ThenInclude(a => a.CategoriaLabClinico)
                .Include(a => a.Paciente)
                .Include(a => a.EstadoExamen)
                .Where(a => !a.Eliminado) // Filtro base de eliminados
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                examen = examen.Where(s => s.Paciente.Nombre.Contains(searchString) || s.Id.ToString() == searchString);
            }

            if (estado != null)
            {
                examen = examen.Where(a => a.EstadoExamenId == estado);
            }

            return examen.OrderByDescending(a => a.Id);
        }

        public List<Examen> PaginacionExamenesRealizados(string sortOrder, string searchString, int? pageNumber, int pageSize, int? estado, bool solicitado = false)
        {
            var examen = _context.Examenes
            .Include(a => a.Medicos)
            // .Include(a => a.Empleado).ThenInclude(a => a.Users)
            .Include(a => a.DetalleExamenes).ThenInclude(a => a.ExamenLabClinico).ThenInclude(a => a.CategoriaLabClinico)
            .Include(a => a.Paciente)
            .Include(a => a.EstadoExamen)
            .AsQueryable();



            // para hacer la busqueda, el IsNullOrEmpty verifica que no este vacia la cadena entrante
            if (!string.IsNullOrEmpty(searchString))
            {
                examen = examen
                .Where(s => s.Paciente.Nombre.Contains(searchString));
            }

            if (estado != null)
            {
                examen = examen.Where(a => a.EstadoExamenId == estado);
            }

            if (solicitado)
            {
                examen = examen.OrderBy(a => a.Id);
            }
            else
            {
                examen = examen.OrderByDescending(a => a.Id);
            }


            return examen
                .Where(a => !a.Eliminado)
                .ToList();

        }


        public IList<CategoriaLabClinico> GetListCategoriasLab()
        {
            return _context.CategoriaLabClinicos
            .Where(x => x.Eliminado == false).ToList();
        }

        public IList<ExamenLabClinico> GetListExamenesLaboratorio()
        {
            return _context.ExamenLabClinicos
            .Where(x => x.Eliminado == false).ToList();
        }
        public List<ExamenLabClinicosSP> GetListExamenesLaboratorioSP()
        {
            return _context.ExamenLabClinicos
                .Where(x => !x.Eliminado)
                .OrderBy(x => x.CodigoInterno)
                .Select(x => new ExamenLabClinicosSP
                {
                    ExamenId = x.Id,
                    ExamenNombre = x.NombreExamen,
                    ExamenCodigo = x.CodigoInterno,
                    ExamenNombreMostrar = (x.CodigoInterno ?? "") + " - " + (x.NombreExamen ?? "")
                })
                .ToList();
        }

        public IList<Examen> GetListExamenesRealizado()
        {
            return _context.Examenes
            // .Include(a => a.Empleado).ThenInclude(a => a.Users)
            .Include(a => a.Paciente)
            .Include(a => a.DetalleExamenes).ThenInclude(a => a.ExamenLabClinico)
            .Where(x => x.Eliminado == false).ToList();
        }



        public void Add(CategoriaLabClinico categoriaLabClinico, bool saveChanges = true)
        {
            _context.CategoriaLabClinicos.Add(categoriaLabClinico);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        public void Add(Resultados resultado, bool saveChanges = true)
        {
            _context.Resultados.Add(resultado);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        public void Add(Examen examen, bool saveChanges = true)
        {
            _context.Examenes.Add(examen);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        public void Add(ExamenLabClinico examenLabClinico, bool saveChanges = true)
        {
            _context.ExamenLabClinicos.Add(examenLabClinico);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        public void Add(DatosExamenesLabClinico datos, bool saveChanges = true)
        {
            _context.DatosExamenesLabClinicos.Add(datos);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }
        //Traer los tipo de campos de exman
        public List<DatoTipoExamenLabClinico> GetListTipo()
        {
            return _context.DatoTipoExamenLabClinico
                .OrderByDescending(a => a.Id).ToList();
        }

        // public IList<Sexo> GetSexosList()
        // {
        //     return _context.Sexo.ToList();
        // }

        public CategoriaLabClinico GetCategoriaLab(int id, bool includeRelatedEntities = true)
        {
            return _context.CategoriaLabClinicos.Where(a => a.Id == id).FirstOrDefault();
        }

        //Obtiene los datos de un examen clinico pormedio de in id
        public DatosExamenesLabClinico GetDatosExamenLab(int id, bool includeRelatedEntities = true)
        {
            return _context.DatosExamenesLabClinicos
                .Include(a => a.ExamenLabClinico)
                .Where(a => a.Id == id).FirstOrDefault();
        }


        public List<ExamenLabClinico> ExamenesLabList(int idCategoria, bool includeRelatedEntities = true)
        {
            return _context.ExamenLabClinicos
                .Where(a => a.CategoriaLabClinicoId == idCategoria && a.Eliminado == false)
                    .ToList();
        }

        public List<Resultados> ResultadosListByDetalleId(int DetalleId, bool includeRelatedEntities = true)
        {
            return _context.Resultados
                .Where(a => a.DetalleExamenId == DetalleId)
                    .ToList();
        }

        public List<Resultados> ResultadosListByExamenId(int Id, bool includeRelatedEntities = true)
        {
            return _context.Resultados
                .Where(a => a.DetalleExamen.ExamenId == Id)
                    .ToList();
        }

        public List<ExamenLabClinico> ExamenesLabListTodos(bool includeRelatedEntities = true)
        {
            return _context.ExamenLabClinicos
            .Include(a => a.CategoriaLabClinico)
                .Where(a => a.Eliminado == false)
                    .ToList();
        }

        public List<DatosExamenesLabClinico> DatosLabList(int idExamenLab, bool includeRelatedEntities = true)
        {
            return _context.DatosExamenesLabClinicos
                .Where(a => a.ExamenLabClinicoId == idExamenLab
                        && a.Eliminado == false)
                    .ToList();
        }
        public ExamenLabClinico GetByCodigo(string codigoInterno)
        {
            return _context.ExamenLabClinicos
                .Include(e => e.ExamenLabClinicosPrecios)
                .FirstOrDefault(e => e.CodigoInterno == codigoInterno && !e.Eliminado);
        }

        public ExamenLabClinico GetExamenLab(int id, bool includeRelatedEntities = true)
        {
            return _context.ExamenLabClinicos
                .Include(a => a.ExamenLabClinicoInsumo)
                .Include(a => a.ExamenLabClinicosPrecios)
                    .ThenInclude(p => p.Precio)
                .Include(a => a.ExamenLabClinicosPreguntas)
                .Where(a => a.Id == id)
                .FirstOrDefault();
        }

        public List<ExamenLabClinicoPrecio> GetPreciosExamen(int examenLabClinicoId)
        {
            return _context.ExamenLabClinicosPrecios
                .Include(a => a.Precio)
                .Where(a => a.ExamenLabClinicoId == examenLabClinicoId
                && !a.Precio.Eliminado)
                .ToList();
        }
        public DetalleExamen GetDetalleExamenLab(int id, bool includeRelatedEntities = true)
        {
            return _context.DetalleExamenes.AsNoTracking()
            .Where(a => a.Id == id)
            .FirstOrDefault();
        }

        // En tu LaboratorioRepository
        public Examen GetExamenByConsulta(int consultaId)
        {
            return _context.Examenes
                .Include(e => e.DetalleExamenes)
                .FirstOrDefault(e => e.ConsultaId == consultaId);
        }

        public Resultados GetResultadoById(int id, bool includeRelatedEntities = true)
        {
            return _context.Resultados.AsNoTracking()
            .Where(a => a.Id == id)
            .FirstOrDefault();
        }

        public void Update(Resultados resultado, bool saveChanges = true)
        {
            EfUpdateHelper.UpdateEntity(_context, resultado, saveChanges);
        }

        public Examen GetExamenRealizado(int id, bool includeRelatedEntities = true)
        {
            return _context.Examenes
                .Include(a => a.Empleado).ThenInclude(a => a.Users)
                .Include(a => a.Medicos)
                .Include(a => a.Clinicas)
                .Include(a => a.Paciente).ThenInclude(a => a.Sexo)
                //.Include(a => a.VentasLabs)
                .Include(a => a.DetalleExamenes).ThenInclude(a => a.ExamenLabClinico).ThenInclude(a => a.DatosExamenesLabClinicos)
                .Include(a => a.DetalleExamenes).ThenInclude(a => a.ExamenLabClinico).ThenInclude(a => a.CategoriaLabClinico)
                .Include(a => a.DetalleExamenes).ThenInclude(a => a.Resultados).ThenInclude(a => a.DatosExamenesLabClinico)
                .Include(a => a.HospitalizacionesExamenes)
                .Where(a => a.Id == id)
                .FirstOrDefault();
        }

        //Trae solo los exmanes que tienen un resultado
        public Examen GetExamenResultados(int id, bool includeRelatedEntities = true)
        {
            var examen = _context.Examenes
                .Include(a => a.Empleado).ThenInclude(a => a.Users)
                .Include(a => a.Medicos)
                .Include(a => a.Clinicas)
                .Include(a => a.Paciente).ThenInclude(a => a.Sexo)
                .Include(a => a.Ventas)
                .Include(a => a.DetalleExamenes)
                .Include(a => a.DetalleExamenes).ThenInclude(a => a.ExamenLabClinico).ThenInclude(a => a.DatosExamenesLabClinicos)
                .Include(a => a.DetalleExamenes).ThenInclude(a => a.ExamenLabClinico).ThenInclude(a => a.CategoriaLabClinico)
                .Include(a => a.DetalleExamenes).ThenInclude(a => a.Resultados)
                .Include(a => a.DetalleExamenes).ThenInclude(a => a.Resultados).ThenInclude(a => a.DatosExamenesLabClinico)
                .Where(a => a.Id == id)
                .FirstOrDefault();

            //if (examen != null && includeRelatedEntities)
            //{
            //    examen.DetalleExamenes = examen.DetalleExamenes
            //        .Where(de => de.Resultados.Any(r => !string.IsNullOrEmpty(r.ValorResultado)))
            //        .ToList();
            //}

            return examen;
        }

        public void Update(CategoriaLabClinico categoriaLabClinico, bool saveChanges = true)
        {
            EfUpdateHelper.UpdateEntity(_context, categoriaLabClinico, saveChanges);
        }

        public void Update(DatosExamenesLabClinico datos, bool saveChanges = true)
        {
            EfUpdateHelper.UpdateEntity(_context, datos, saveChanges);
        }

        public void Update(ExamenLabClinico examen, bool saveChanges = true)
        {
            EfUpdateHelper.UpdateEntity(_context, examen, saveChanges);
        }

        public void Update(Examen examen, bool saveChanges = true)
        {
            EfUpdateHelper.UpdateEntity(_context, examen, saveChanges);
        }

        public void Update(DetalleExamen examen, bool saveChanges = true)
        {
            EfUpdateHelper.UpdateEntity(_context, examen, saveChanges);
        }

        //public void Update(VentasLab examen, bool saveChanges = true)
        //{
        //    _context.Entry(examen).State = EntityState.Modified;

        //    if (saveChanges)
        //    {
        //        _context.SaveChanges();
        //    }
        //}


        //public void Update(CajaLab caja, bool saveChanges = true)
        //{
        //    _context.Entry(caja).State = EntityState.Modified;

        //    if (saveChanges)
        //    {
        //        _context.SaveChanges();
        //    }
        //}

        // public Paciente GetPacientePorNombre(string nombre)
        // {
        //     return _context.Pacientes.Where(a => a.Nombre == nombre && a.Eliminado == false).SingleOrDefault();
        // }

        // public Paciente GetPacientePorId(int id)
        // {
        //     return _context.Pacientes.Where(a => a.Id == id && a.Eliminado == false).SingleOrDefault();
        // }

        // para ventas

        public void Add(DetalleExamen detalle, bool saveChanges = true)
        {
            _context.DetalleExamenes.Add(detalle);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        //public void Add(DetalleCajaLab detalle, bool saveChanges = true)
        //{
        //    _context.DetalleCajaLab.Add(detalle);

        //    if (saveChanges)
        //    {
        //        _context.SaveChanges();
        //    }
        //}


        //public void Add(CajaLab detalle, bool saveChanges = true)
        //{
        //    _context.CajaLab.Add(detalle);

        //    if (saveChanges)
        //    {
        //        _context.SaveChanges();
        //    }
        //}

        public void saveChanges()
        {
            _context.SaveChanges();
        }


        //public List<CajaLab> ListarCajas()
        //{
        //    return _context.CajaLab
        //        .Include(x => x.DetalleCajaLabs)
        //        .Include(a => a.ResponsableAperturaLab).ThenInclude(a => a.Persona)
        //        .Include(a => a.ResponsableCierreLab).ThenInclude(a => a.Persona)
        //        .OrderByDescending(a => a.FechaApertura).ToList();
        //}


        //public CajaLab GetCajaAbierta()
        //{
        //    return _context.CajaLab.Where(a => a.EstadoCaja == true).FirstOrDefault();
        //}

        //public CajaLab GetCaja(int id, bool includeRelatedEntities = true)
        //{
        //    var caja = _context.CajaLab.AsQueryable();

        //    if (includeRelatedEntities)
        //    {
        //        caja = caja
        //        .Include(a => a.DetalleCajaLabs).ThenInclude(a => a.VentasLab).ThenInclude(a => a.Examen).ThenInclude(a => a.Paciente)
        //        // .Include(a => a.DetalleCajas).ThenInclude(a => a.Venta).ThenInclude(a => a.Empleado)
        //        // .Include(a => a.DetalleCajas).ThenInclude(a => a.Compra).ThenInclude(a => a.Proveedor)
        //        // .Include(a => a.DetalleCajas).ThenInclude(a => a.Compra).ThenInclude(a => a.Empleado)
        //        // .Include(a => a.DetalleCajas).ThenInclude(a => a.VentaServicio).ThenInclude(a => a.Paciente)
        //        // .Include(a => a.DetalleCajas).ThenInclude(a => a.VentaServicio).ThenInclude(a => a.Empleado)
        //        .Include(a => a.ResponsableAperturaLab).ThenInclude(a => a.Persona)
        //        .Include(a => a.ResponsableCierreLab).ThenInclude(a => a.Persona);
        //    }

        //    return caja.Where(a => a.Id == id).SingleOrDefault();
        //}

        public void ActualizarInventarioInsumoVentaExamenesLaboratorio(int examenId)
        {
            var insumos = _context.ExamenLabClinicoInsumo
                .Where(s => s.ExamenLabClinicoId == examenId)
                .ToList();
            if (insumos != null && insumos.Count > 0)
            {
                foreach (var insumo in insumos)
                {
                    var inventarioProducto = _context.ProductosInventario
                        .Where(p => p.ProductoId == insumo.ProductoId
                        && p.UnidadMedidaVentaId == insumo.UnidadMedidaVentaId
                            && p.Stock > 0)
                        .FirstOrDefault();
                    if (inventarioProducto != null)
                    {
                        inventarioProducto.Stock -= insumo.CantidadUtilizada;

                        _context.Entry(inventarioProducto).State = EntityState.Modified;
                        _context.SaveChanges();
                    }

                }
            }
        }
        public List<Examen> GetAllExamenesRealizados()
        {
            var examen = _context.Examenes
            .Include(a => a.Medicos)
            // .Include(a => a.Empleado).ThenInclude(a => a.Users)
            .Include(a => a.DetalleExamenes).ThenInclude(a => a.ExamenLabClinico).ThenInclude(a => a.CategoriaLabClinico)
            .Include(a => a.Paciente)
            .Include(a => a.EstadoExamen)
            .ToList();
            return examen;
        }


        //Metodo para traer las preguntas de un examen de una cita especifica
        public IList<ExamenLabClinicoPregunta> GetPreguntasExamenLabClinicoCita(int citaId)
        {
            return _context.Citass
                .Where(c => c.Id == citaId)
                .SelectMany(c => c.CitasExamenes.SelectMany(ce => ce.ExamenLabClinico.ExamenLabClinicosPreguntas))
                 .Where(ce => ce.Eliminado == false)
                .ToList();
        }
        //Metodo para traer las preguntas de un examen especifico
        public IList<ExamenLabClinicoPregunta> GetPreguntasExamenLabClinico(int examenLabClinicoId)
        {
            return _context.ExamenLabClinicosPreguntas
                .Where(e => e.ExamenLabClinico.Id == examenLabClinicoId)
                .Where(e => e.Eliminado == false)
                .ToList();
        }


        //Metodo creado para obtener una pregunta por su id
        public ExamenLabClinicoPregunta GetPregunta(int id, bool includeRelatedEntities = true)
        {
            return _context.ExamenLabClinicosPreguntas
                .Where(a => a.Id == id).FirstOrDefault();
        }


        public void UpdatePregunta(ExamenLabClinicoPregunta examen, bool saveChanges = true)
        {
            EfUpdateHelper.UpdateEntity(_context, examen, saveChanges);
        }

        public Examen GetExamenPaciente(int id, bool includeRelatedEntities = true)
        {
            return _context.Examenes
            .Include(a => a.Empleado).ThenInclude(a => a.Users)
            .Include(a => a.Medicos)
            .Include(a => a.Paciente).ThenInclude(a => a.Sexo)
            //.Include(a => a.VentasLabs)
            .Include(a => a.DetalleExamenes).ThenInclude(a => a.ExamenLabClinico).ThenInclude(a => a.DatosExamenesLabClinicos)
            .Include(a => a.DetalleExamenes).ThenInclude(a => a.ExamenLabClinico).ThenInclude(a => a.CategoriaLabClinico)
            .Include(a => a.DetalleExamenes).ThenInclude(a => a.Resultados).ThenInclude(a => a.DatosExamenesLabClinico)
            .Where(a => a.PacienteId == id)
            .OrderByDescending(a => a.FechaRealizacion)
            .FirstOrDefault();
        }

        public Examen GetInfoRequeridaEditarDetalleExamen(int id)
        {
            // Se asume que _context es el DbContext y que la entidad Examen está configurada en él.
            return _context.Examenes
                .Include(e => e.Paciente)
                    .ThenInclude(p => p.Citas)
                        .ThenInclude(c => c.Empleado) // Incluye el empleado en cada cita (para el nombre del médico)
                .Include(e => e.HospitalizacionesExamenes)
                    .ThenInclude(he => he.Hospitalizacion)
                        .ThenInclude(hos => hos.Habitacion) // Incluye la habitación asociada a la hospitalización
                .FirstOrDefault(e => e.Id == id);
        }
    }
}