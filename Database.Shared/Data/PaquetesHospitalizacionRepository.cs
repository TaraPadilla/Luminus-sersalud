using Database.Shared.Models;
using Database.Shared.IRepository;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Paginacion;


namespace Database.Shared.Data
{
    public class PaquetesHospitalizacionRepository : IPaquetesHospitalizacion
    {
        //private readonly Context _context = null;

        //public EmpleadoRepository(Context context)
        //{
        //    _context = context;
        //}

        //public void Add(Empleado empleado, bool saveChanges = true)
        //{
        //    _context.Empleados.Add(empleado);

        //    if (saveChanges)
        //    {
        //        _context.SaveChanges();
        //    }
        //}



        //public void Add(Medicos medico, bool saveChanges = true)
        //{
        //    _context.Medicos.Add(medico);

        //    if (saveChanges)
        //    {
        //        _context.SaveChanges();
        //    }
        //}

        //public void Add(Clinica medico, bool saveChanges = true)
        //{
        //    _context.Clinicas.Add(medico);

        //    if (saveChanges)
        //    {
        //        _context.SaveChanges();
        //    }
        //}

        //public Empleado Get(int id, bool includeRelatedEntities = true)
        //{
        //    return _context.Empleados
        //    .Include(a => a.Users)
        //       .Where(a => a.Id == id)
        //       .SingleOrDefault();
        //}

        //public Medicos GetMedicoById(int id, bool includeRelatedEntities = true)
        //{
        //    return _context.Medicos
        //       .Where(a => a.Id == id)
        //       .SingleOrDefault();
        //}

        //public Medicos GetMedicoByName(string name, bool includeRelatedEntities = true)
        //{
        //    return _context.Medicos
        //       .Where(a => a.Nombres == name)
        //       .SingleOrDefault();
        //}

        //public Clinica GetClinicaByName(string name, bool includeRelatedEntities = true)
        //{
        //    return _context.Clinicas
        //       .Where(a => a.NombreClinica == name)
        //       .SingleOrDefault();
        //}
        //public List<Empleado> GetList() => _context.Empleados.Where(x => x.Eliminado == false).ToList();

        //public List<Medicos> GetListMedicos() => _context.Medicos.Where(x => x.Eliminado == false).ToList();
        //public List<Clinica> GetListClinicas() => _context.Clinicas.Where(x => x.Eliminado == false).ToList();

        //public PaginacionList<Empleado> PaginacionEmpleados(string sortOrder, string searchString, int? pageNumber, int pageSize)
        //{
        //    var empleado = _context.Empleados.AsQueryable();


        //    // para hacer la busqueda, el IsNullOrEmpty verifica que no este vacia la cadena entrante
        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        empleado = empleado.Where(s => s.Nombre.Contains(searchString));
        //    }

        //    // esto que hice no es buena practica, lo ideal seria hacer un patron por estrategia
        //    // visitar : https://refactoring.guru/es/design-patterns/strategy
        //    // asi como lo tengo funciona pero no es tan tan tan recomendado
        //    // quizas mas adelante lo mejoremos con un patron de estrategia.

        //    switch (sortOrder)
        //    {
        //        case "Nombre_desc":
        //            empleado = empleado.OrderByDescending(s => s.Nombre);
        //            break;

        //        default:
        //            empleado = empleado.OrderBy(s => s.Nombre);
        //            break;
        //    }

        //    return PaginacionList<Empleado>.CreateAsyncc(empleado.Where(x => x.Eliminado == false), pageNumber ?? 1, pageSize);
        //}

        //public void Update(Empleado model, bool saveChanges = true)
        //{

        //    _context.Entry(model).State = EntityState.Modified;

        //    if (saveChanges)
        //    {
        //        _context.SaveChanges();
        //    }
        //}
    }
}