using Database.Shared.IRepository;
using Database.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Database.Shared.Paginacion;

namespace Database.Shared.Data
{
    public class EspecialidadRepository : IEspecialidad
    {
        private readonly Context _context;

        public EspecialidadRepository(Context context)
        {
            _context = context;
        }

        public List<Especialidad> GetAll()
        {
            // Filtrar por las especialidades no eliminadas
            return _context.Especialidad.Where(e => e.Eliminado == false).ToList();
        }

        public Especialidad Add(Especialidad especialidad)
        {
            _context.Especialidad.Add(especialidad);
            _context.SaveChanges();
            return especialidad;
        }

        public void Update(Especialidad especialidad)
        {
            var existingEspecialidad = _context.Especialidad.Find(especialidad.Id);
            if (existingEspecialidad != null)
            {
                // Actualizar las propiedades necesarias
                existingEspecialidad.NombreEspecialidad = especialidad.NombreEspecialidad;
                existingEspecialidad.Codigo = especialidad.Codigo;
                existingEspecialidad.Descripcion = especialidad.Descripcion;

                _context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var especialidad = _context.Especialidad.Find(id);
            if (especialidad != null)
            {
                // Marcar como eliminada en lugar de eliminar físicamente
                especialidad.Eliminado = true;
                _context.SaveChanges();
            }
        }

        // Método adicional para obtener una especialidad por ID
        public Especialidad Get(int id, bool includeRelatedEntities = true)
        {
            return _context.Especialidad
                .Where(e => e.Id == id && e.Eliminado == false)
                .SingleOrDefault();
        }

        public static string Normalizar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return "";

            return new string(texto
                .Trim() 
                .Normalize(System.Text.NormalizationForm.FormD)
                .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                .ToArray())
                .ToLower();
        }

        public PaginacionList<Especialidad> PaginacionEspecialidades(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var especialidades = _context.Especialidad
                .Where(e => e.Eliminado == false)  // Filtrar por no eliminadas
                .AsQueryable();

            // Filtros de búsqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                var filtro = Normalizar(searchString);
                especialidades = especialidades
                    .AsEnumerable()
                    .Where(e => Normalizar(e.NombreEspecialidad).Contains(filtro))
                    .AsQueryable();
            }


            // Ordenamiento
            switch (sortOrder)
            {
                case "Nombre_desc":
                    especialidades = especialidades.OrderByDescending(e => e.NombreEspecialidad);
                    break;
                default:
                    especialidades = especialidades.OrderBy(e => e.NombreEspecialidad);
                    break;
            }

            // Paginar
            return PaginacionList<Especialidad>.CreateAsyncc(especialidades, pageNumber ?? 1, pageSize);
        }
    }

}
