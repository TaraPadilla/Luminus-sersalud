using Database.Shared.Models;
using Database.Shared.IRepository;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Paginacion;
using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Database.Shared.Data
{
    public class EmpleadoRepository : IEmpleado
    {
        private readonly Context _context = null;

        public EmpleadoRepository(Context context)
        {
            _context = context;
        }

        public void Add(Empleado empleado, bool saveChanges = true)
        {
            _context.Empleados.Add(empleado);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        public void Add(Medicos medico, bool saveChanges = true)
        {
            _context.Medicos.Add(medico);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        public void Add(Clinica medico, bool saveChanges = true)
        {
            _context.Clinicas.Add(medico);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        public Empleado Get(int id, bool includeRelatedEntities = true)
        {
            try
            {
                var query = _context.Empleados.AsQueryable();

                if (includeRelatedEntities)
                {
                    query = query.Include(a => a.Users)
                    .Include(a => a.UnidadOrg) // agrego Unidad
                    .ThenInclude(u => u.DepartamentoOrg); // agrego Departamento de esa Unidad
                }

                var empleado = query.SingleOrDefault(a => a.Id == id);

                // Log mínimo para detectar caso "no encontrado"
                if (empleado == null)
                    Console.WriteLine($"Empleado con ID {id} no encontrado.");

                return empleado;
            }
            catch (Exception ex)
            {
                // Log esencial para capturar y registrar cualquier excepción
                Console.WriteLine($"Excepción dentro de Get(): {ex.Message}");
                throw; // Se relanza la excepción para que sea manejada externamente
            }
        }

        public Medicos GetMedicoById(int id, bool includeRelatedEntities = true)
        {
            return _context.Medicos
               .Where(a => a.Id == id)
               .SingleOrDefault();
        }

        public Medicos GetMedicoByName(string name, bool includeRelatedEntities = true)
        {
            return _context.Medicos
               .Where(a => a.Nombres == name)
               .SingleOrDefault();
        }

        public Clinica GetClinicaByName(string name, bool includeRelatedEntities = true)
        {
            return _context.Clinicas
               .Where(a => a.NombreClinica == name)
               .SingleOrDefault();
        }

        public List<Empleado> GetList() => _context.Empleados.Where(x => x.Eliminado == false).ToList();

        // public List<Empleado> GetEmpleadosConDetalles()
        // {
        //     return _context.Empleados
        //         .Include(e => e.Especialidad)
        //         .Include(e => e.UnidadOrg)
        //         .Where(x => x.Eliminado == false)
        //         .ToList();
        // }

        public List<Empleado> GetEmpleadosConDetalles()
        {
            return _context.Empleados
                .Include(e => e.Especialidad)
                .Include(e => e.UnidadOrg)
                    .ThenInclude(u => u.DepartamentoOrg) // Incluimos el departamento
                        .ThenInclude(d => d.Unidades)    // <-- CORRECCIÓN: Usar "Unidades", que es el nombre de la ICollection en el modelo DepartamentoOrg
                .Where(x => x.Eliminado == false)
                .ToList();
        }

        public List<Empleado> GetListEmpleadoTipoProfesional() =>
            _context.Empleados.Where(x => x.Eliminado == false && x.TipoEmpleado == "Profesional").ToList();

        public List<object> GetListEmpleadoTipoProfesionalColegiado()
        {
            return _context.Empleados
                .Where(x => x.Eliminado == false && x.TipoEmpleado == "Profesional")
                .Select(e => new
                {
                    Id = e.Id,
                    NombreCompleto = $"{e.Nombre} {e.Apellido} - {e.Colegiado}"
                })
                .ToList<object>();
        }

        public List<Medicos> GetListMedicos() => _context.Medicos.Where(x => x.Eliminado == false).ToList();
        public List<Clinica> GetListClinicas() => _context.Clinicas.Where(x => x.Eliminado == false).ToList();

        public PaginacionList<Empleado> PaginacionEmpleados(string sortOrder, string searchString, int? pageNumber, int pageSize, string tipoEmpleado)
        {
            var empleado = _context.Empleados
                .Include(a => a.Sucursal)
                .Include(a => a.UnidadOrg) // <-- NUEVO
                    .ThenInclude(u => u.DepartamentoOrg) // <-- NUEVO
                .AsQueryable();

            // para hacer la busqueda, el IsNullOrEmpty verifica que no este vacia la cadena entrante
            if (!string.IsNullOrEmpty(searchString))
            {
                var filtro = Normalizar(searchString);
                empleado = empleado
                    .AsEnumerable()
                    .Where(e => Normalizar(e.Nombre).Contains(filtro))
                    .AsQueryable();
            }

            switch (sortOrder)
            {
                case "Nombre_desc":
                    empleado = empleado.OrderByDescending(s => s.Nombre);
                    break;

                default:
                    empleado = empleado.OrderBy(s => s.Nombre);
                    break;
            }

            return PaginacionList<Empleado>.CreateAsyncc(
                empleado.Where(x => x.Eliminado == false && x.TipoEmpleado != "Profesional"),
                pageNumber ?? 1,
                pageSize
            );
        }

        public static string Normalizar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return "";

            return new string(texto
                .Trim()
                .Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray())
                .ToLower();
        }

        public List<MedicoSecundarioDtoHospi> GetMedicosSecundariosPorIds(List<int> ids)
        {
            // Obtiene los médicos cuyo ID está en la lista proporcionada
            var medicos = _context.Empleados
                .Where(m => ids.Contains(m.Id) && m.Eliminado == false) // Asegúrate de que no estén eliminados
                .Select(m => new MedicoSecundarioDtoHospi
                {
                    Id = m.Id,
                    NombreCompleto = $"{m.Nombre} {m.Apellido}"
                })
                .ToList();

            return medicos;
        }

        public PaginacionList<Empleado> PaginacionEmpleadosMedicos(string sortOrder, string searchString, int? pageNumber, int pageSize, string tipoEmpleado)
        {
            var empleado = _context.Empleados
                .Include(a => a.Sucursal)
                .Include(a => a.Especialidad) // <-- NUEVO
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                var filtro = Normalizar(searchString);
                empleado = empleado
                    .AsEnumerable()
                    .Where(e => Normalizar(e.Nombre).Contains(filtro))
                    .AsQueryable();
            }

            switch (sortOrder)
            {
                case "Nombre_desc":
                    empleado = empleado.OrderByDescending(s => s.Nombre);
                    break;

                default:
                    empleado = empleado.OrderBy(s => s.Nombre);
                    break;
            }

            return PaginacionList<Empleado>.CreateAsyncc(
                empleado.Where(x => x.Eliminado == false && x.TipoEmpleado == "Profesional"),
                pageNumber ?? 1,
                pageSize
            );
        }

        public void Update(Empleado model, bool saveChanges = true)
        {
            _context.Entry(model).State = EntityState.Modified;

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        // NUEVO: Método para obtener empleados por una lista de IDs
        public List<Empleado> GetByIds(List<int> ids)
        {
            return _context.Empleados
                .Where(e => ids.Contains(e.Id))
                .ToList();
        }

        // =========================================================================================
        // NUEVO: Soporte DataTables server-side (NO se elimina ni modifica la lógica existente)
        // =========================================================================================
        //
        // Este método está pensado para ser consumido por EmpleadoController.ListaDataTable
        // y evita AsEnumerable() para que todo ocurra en PostgreSQL.
        //
        // Búsqueda:
        // - Usa EF.Functions.ILike => case-insensitive en PostgreSQL.
        // - No usamos "Normalizar" aquí porque NO es traducible a SQL.
        //
        // Ordenamiento:
        // - Whitelist: solo campos conocidos.
        //
        public async Task<(int RecordsTotal, int RecordsFiltered, List<object> Data)> DataTableEmpleadosAsync(
     string tipoEmpleado,
     string searchValue,
     string orderBy,
     bool orderDescending,
     int start,
     int length
 )
        {
            if (start < 0) start = 0;
            if (length <= 0) length = 10;
            if (length > 200) length = 200;

            var esMedico = string.Equals(tipoEmpleado, "Medico", StringComparison.OrdinalIgnoreCase);

            // Base query (NO materializa)
            IQueryable<Empleado> query = _context.Empleados
                .AsNoTracking()
                .Include(e => e.Sucursal)
                .Where(e => e.Eliminado == false);

            // Total por tipo (sin search)
            IQueryable<Empleado> totalQuery = esMedico
                ? query.Where(e => e.TipoEmpleado == "Profesional")
                : query.Where(e => e.TipoEmpleado != "Profesional");

            var recordsTotal = await totalQuery.CountAsync().ConfigureAwait(false);

            // Aplicar búsqueda (filtrado)
            IQueryable<Empleado> filteredQuery = totalQuery;

            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                var pattern = $"%{searchValue.Trim()}%";

                filteredQuery = filteredQuery.Where(e =>
                    EF.Functions.ILike(e.Nombre ?? "", pattern) ||
                    EF.Functions.ILike(e.Apellido ?? "", pattern) ||
                    EF.Functions.ILike(e.Dpi ?? "", pattern) ||
                    EF.Functions.ILike(e.Nit ?? "", pattern) ||
                    EF.Functions.ILike(e.Telefono ?? "", pattern) ||
                    (e.Sucursal != null && EF.Functions.ILike(e.Sucursal.NombreSucursal ?? "", pattern))
                );
            }

            var recordsFiltered = await filteredQuery.CountAsync().ConfigureAwait(false);

            // Ordenamiento (whitelist)
            filteredQuery = ApplyOrder(filteredQuery, orderBy, orderDescending);

            // Paginación + proyección (evitar ternario con tipos anónimos diferentes)
            if (esMedico)
            {
                var dataMedicos = await filteredQuery
                    .Skip(start)
                    .Take(length)
                    .Select(e => new
                    {
                        id = e.Id,
                        nombre = e.Nombre,
                        apellido = e.Apellido,
                        telefono = e.Telefono,
                        direccion = e.Direccion,
                        dpi = e.Dpi,
                        nit = e.Nit,
                        sucursalNombre = e.Sucursal != null ? e.Sucursal.NombreSucursal : "-",
                        colorHexadecimalFondo = e.ColorHexadecimalFondo,
                        colorHexadecimalTexto = e.ColorHexadecimalTexto,

                        colegiado = e.Colegiado,
                        credenciales = e.Credenciales,
                        direccionClinica = e.DireccionClinica,
                        telefonoClinica = e.TelefonoClinica,
                        tipoRegimen = e.TipoRegimen,
                        tipoEmpleado = e.TipoEmpleado
                    })
                    .ToListAsync()
                    .ConfigureAwait(false);

                return (recordsTotal, recordsFiltered, dataMedicos.Cast<object>().ToList());
            }
            else
            {
                var dataEmpleados = await filteredQuery
                    .Skip(start)
                    .Take(length)
                    .Select(e => new
                    {
                        id = e.Id,
                        nombre = e.Nombre,
                        apellido = e.Apellido,
                        telefono = e.Telefono,
                        direccion = e.Direccion,
                        dpi = e.Dpi,
                        nit = e.Nit,
                        sucursalNombre = e.Sucursal != null ? e.Sucursal.NombreSucursal : "-",
                        colorHexadecimalFondo = e.ColorHexadecimalFondo,
                        colorHexadecimalTexto = e.ColorHexadecimalTexto,

                        salario = e.Salario,
                        estadoCivil = e.EstadoCivil
                    })
                    .ToListAsync()
                    .ConfigureAwait(false);

                return (recordsTotal, recordsFiltered, dataEmpleados.Cast<object>().ToList());
            }
        }

        private static IQueryable<Empleado> ApplyOrder(IQueryable<Empleado> query, string orderBy, bool desc)
        {
            // Default: Id desc/asc
            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return desc ? query.OrderByDescending(e => e.Id) : query.OrderBy(e => e.Id);
            }

            switch (orderBy.Trim().ToLowerInvariant())
            {
                case "id":
                    return desc ? query.OrderByDescending(e => e.Id) : query.OrderBy(e => e.Id);

                case "nombre":
                    return desc ? query.OrderByDescending(e => e.Nombre) : query.OrderBy(e => e.Nombre);

                case "apellido":
                    return desc ? query.OrderByDescending(e => e.Apellido) : query.OrderBy(e => e.Apellido);

                case "telefono":
                    return desc ? query.OrderByDescending(e => e.Telefono) : query.OrderBy(e => e.Telefono);

                case "direccion":
                    return desc ? query.OrderByDescending(e => e.Direccion) : query.OrderBy(e => e.Direccion);

                case "dpi":
                    return desc ? query.OrderByDescending(e => e.Dpi) : query.OrderBy(e => e.Dpi);

                case "nit":
                    return desc ? query.OrderByDescending(e => e.Nit) : query.OrderBy(e => e.Nit);

                case "sucursal":
                case "sucursalnombre":
                    return desc
                        ? query.OrderByDescending(e => e.Sucursal != null ? e.Sucursal.NombreSucursal : "")
                        : query.OrderBy(e => e.Sucursal != null ? e.Sucursal.NombreSucursal : "");

                case "salario":
                    // OJO: Salario es string en tu entidad. Orden será lexicográfico.
                    // Si luego quieres orden numérico real, habría que modelarlo o proyectar a decimal parseable.
                    return desc ? query.OrderByDescending(e => e.Salario) : query.OrderBy(e => e.Salario);

                case "estadocivil":
                    return desc ? query.OrderByDescending(e => e.EstadoCivil) : query.OrderBy(e => e.EstadoCivil);

                case "colegiado":
                    return desc ? query.OrderByDescending(e => e.Colegiado) : query.OrderBy(e => e.Colegiado);

                case "credenciales":
                    return desc ? query.OrderByDescending(e => e.Credenciales) : query.OrderBy(e => e.Credenciales);

                case "direccionclinica":
                    return desc ? query.OrderByDescending(e => e.DireccionClinica) : query.OrderBy(e => e.DireccionClinica);

                case "telefonoclinica":
                    return desc ? query.OrderByDescending(e => e.TelefonoClinica) : query.OrderBy(e => e.TelefonoClinica);

                case "tiporegimen":
                    return desc ? query.OrderByDescending(e => e.TipoRegimen) : query.OrderBy(e => e.TipoRegimen);

                case "tipoempleado":
                    return desc ? query.OrderByDescending(e => e.TipoEmpleado) : query.OrderBy(e => e.TipoEmpleado);

                default:
                    return desc ? query.OrderByDescending(e => e.Id) : query.OrderBy(e => e.Id);
            }
        }
    }
}

// using Database.Shared.Models;
// using Database.Shared.IRepository;
// using System.Collections.Generic;
// using System.Linq;
// using Microsoft.EntityFrameworkCore;
// using Database.Shared.Paginacion;
// using System;


// namespace Database.Shared.Data
// {
//     public class EmpleadoRepository : IEmpleado
//     {
//         private readonly Context _context = null;

//         public EmpleadoRepository(Context context)
//         {
//             _context = context;
//         }

//         public void Add(Empleado empleado, bool saveChanges = true)
//         {
//             _context.Empleados.Add(empleado);

//             if (saveChanges)
//             {
//                 _context.SaveChanges();
//             }
//         }



//         public void Add(Medicos medico, bool saveChanges = true)
//         {
//             _context.Medicos.Add(medico);

//             if (saveChanges)
//             {
//                 _context.SaveChanges();
//             }
//         }

//         public void Add(Clinica medico, bool saveChanges = true)
//         {
//             _context.Clinicas.Add(medico);

//             if (saveChanges)
//             {
//                 _context.SaveChanges();
//             }
//         }

//         public Empleado Get(int id, bool includeRelatedEntities = true)
//         {
//             try
//             {
//                 var query = _context.Empleados.AsQueryable();

//                 if (includeRelatedEntities)
//                 {
//                     query = query.Include(a => a.Users);
//                 }

//                 var empleado = query.SingleOrDefault(a => a.Id == id);

//                 // Log mínimo para detectar caso "no encontrado"
//                 if (empleado == null)
//                     Console.WriteLine($"Empleado con ID {id} no encontrado.");

//                 return empleado;
//             }
//             catch (Exception ex)
//             {
//                 // Log esencial para capturar y registrar cualquier excepción
//                 Console.WriteLine($"Excepción dentro de Get(): {ex.Message}");
//                 throw; // Se relanza la excepción para que sea manejada externamente
//             }
//         }

//         public Medicos GetMedicoById(int id, bool includeRelatedEntities = true)
//         {
//             return _context.Medicos
//                .Where(a => a.Id == id)
//                .SingleOrDefault();
//         }

//         public Medicos GetMedicoByName(string name, bool includeRelatedEntities = true)
//         {
//             return _context.Medicos
//                .Where(a => a.Nombres == name)
//                .SingleOrDefault();
//         }

//         public Clinica GetClinicaByName(string name, bool includeRelatedEntities = true)
//         {
//             return _context.Clinicas
//                .Where(a => a.NombreClinica == name)
//                .SingleOrDefault();
//         }
//         public List<Empleado> GetList() => _context.Empleados.Where(x => x.Eliminado == false).ToList();
//         public List<Empleado> GetListEmpleadoTipoProfesional() => _context.Empleados.Where(x => x.Eliminado == false && x.TipoEmpleado == "Profesional").ToList();
//         public List<object> GetListEmpleadoTipoProfesionalColegiado()
//         {
//             return _context.Empleados
//                 .Where(x => x.Eliminado == false && x.TipoEmpleado == "Profesional")
//                 .Select(e => new
//                 {
//                     Id = e.Id,
//                     NombreCompleto = $"{e.Nombre} {e.Apellido} - {e.Colegiado}"
//                 })
//                 .ToList<object>();
//         }


//         public List<Medicos> GetListMedicos() => _context.Medicos.Where(x => x.Eliminado == false).ToList();
//         public List<Clinica> GetListClinicas() => _context.Clinicas.Where(x => x.Eliminado == false).ToList();

//         public PaginacionList<Empleado> PaginacionEmpleados(string sortOrder, string searchString, int? pageNumber, int pageSize, string tipoEmpleado)
//         {
//             var empleado = _context.Empleados
//                 .Include(a => a.Sucursal)
//                 .AsQueryable();


//             // para hacer la busqueda, el IsNullOrEmpty verifica que no este vacia la cadena entrante
//             if (!string.IsNullOrEmpty(searchString))
//             {
//                 var filtro = Normalizar(searchString);
//                 empleado = empleado
//                     .AsEnumerable()
//                     .Where(e => Normalizar(e.Nombre).Contains(filtro))
//                     .AsQueryable();
//             }
//             // esto que hice no es buena practica, lo ideal seria hacer un patron por estrategia
//             // visitar : https://refactoring.guru/es/design-patterns/strategy
//             // asi como lo tengo funciona pero no es tan tan tan recomendado
//             // quizas mas adelante lo mejoremos con un patron de estrategia.

//             switch (sortOrder)
//             {
//                 case "Nombre_desc":
//                     empleado = empleado.OrderByDescending(s => s.Nombre);
//                     break;

//                 default:
//                     empleado = empleado.OrderBy(s => s.Nombre);
//                     break;
//             }

//             return PaginacionList<Empleado>.CreateAsyncc(empleado.Where(x => x.Eliminado == false && x.TipoEmpleado != "Profesional"), pageNumber ?? 1, pageSize);
//         }

//         public static string Normalizar(string texto)
//         {
//             if (string.IsNullOrWhiteSpace(texto)) return "";

//             return new string(texto
//                 .Trim()
//                 .Normalize(System.Text.NormalizationForm.FormD)
//                 .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
//                 .ToArray())
//                 .ToLower();
//         }
//         public List<MedicoSecundarioDtoHospi> GetMedicosSecundariosPorIds(List<int> ids)
//         {
//             // Obtiene los médicos cuyo ID está en la lista proporcionada
//             var medicos = _context.Empleados
//                 .Where(m => ids.Contains(m.Id) && m.Eliminado == false) // Asegúrate de que no estén eliminados
//                 .Select(m => new MedicoSecundarioDtoHospi
//                 {
//                     Id = m.Id,
//                     NombreCompleto = $"{m.Nombre} {m.Apellido}"
//                 })
//                 .ToList();

//             return medicos;
//         }
//         public PaginacionList<Empleado> PaginacionEmpleadosMedicos(string sortOrder, string searchString, int? pageNumber, int pageSize, string tipoEmpleado)
//         {
//             var empleado = _context.Empleados
//                 .Include(a => a.Sucursal)
//                 .AsQueryable();


//             if (!string.IsNullOrEmpty(searchString))
//             {
//                 var filtro = Normalizar(searchString);
//                 empleado = empleado
//                     .AsEnumerable()
//                     .Where(e => Normalizar(e.Nombre).Contains(filtro))
//                     .AsQueryable();
//             }

//             // esto que hice no es buena practica, lo ideal seria hacer un patron por estrategia
//             // visitar : https://refactoring.guru/es/design-patterns/strategy
//             // asi como lo tengo funciona pero no es tan tan tan recomendado
//             // quizas mas adelante lo mejoremos con un patron de estrategia.

//             switch (sortOrder)
//             {
//                 case "Nombre_desc":
//                     empleado = empleado.OrderByDescending(s => s.Nombre);
//                     break;

//                 default:
//                     empleado = empleado.OrderBy(s => s.Nombre);
//                     break;
//             }

//             return PaginacionList<Empleado>.CreateAsyncc(empleado.Where(x => x.Eliminado == false && x.TipoEmpleado == "Profesional"), pageNumber ?? 1, pageSize);
//         }

//         public void Update(Empleado model, bool saveChanges = true)
//         {

//             _context.Entry(model).State = EntityState.Modified;

//             if (saveChanges)
//             {
//                 _context.SaveChanges();
//             }
//         }

//         // NUEVO: Método para obtener empleados por una lista de IDs
//         public List<Empleado> GetByIds(List<int> ids)
//         {
//             return _context.Empleados
//                 .Where(e => ids.Contains(e.Id))
//                 .ToList();
//         }
//     }

// }