using System.Reflection.Metadata;
using System.ComponentModel;
using Database.Shared.Models;
using Database.Shared.IRepository;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Paginacion;

namespace Database.Shared.Data
{
    public class PersonasRepository : IPersonas
    {

        private readonly Context _context = null;

        public PersonasRepository(Context context)
        {
            _context = context;
        }
        public List<Sexo> GetSexosList()
        {
            return _context.Sexo.ToList();
        }

        public Persona Add(Persona model)
        {
            _context.Personas.Add(model);
            _context.SaveChanges();
            return model;
        }
        public Persona Get(int personaId)
        {
            return _context.Personas
                .Include(p => p.Sexo)
                .Where(p => p.Id == personaId)
                .FirstOrDefault();
        }
        public void Update(Persona persona)
        {
            _context.Entry(persona).State = EntityState.Modified;
            _context.SaveChanges();
        }
        public List<Persona> GetPersonas()
        {
            return _context.Personas
                .Include(p => p.Sexo)
                .Include(p => p.TipoRedSocial)
                .Include(p => p.TipificacionComunicacion)
                .Where(p => !p.Eliminada)
                .ToList();
        }
        public IList<TipificacionComunicacion> GetTipificacionesComunicacion()
        {
            return _context.TipificacionesComunicacion
                .ToList();
        }
        public IList<TipoRedSocial> GetTiposRedSocial()
        {
            return _context.TiposRedSocial
                .ToList();
        }
    }
}