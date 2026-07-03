using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Shared.Data
{
    public class ControlGlucometria2Repository : IControlGlucometria2
    {
        private readonly Context _context;

        public ControlGlucometria2Repository(Context context)
        {
            _context = context;
        }

        public void Add(ControlGlucometria2 entity)
        {
            _context.ControlGlucometria2.Add(entity);
            _context.SaveChanges();
        }

        public void Update(ControlGlucometria2 entity)
        {
            _context.ControlGlucometria2.Update(entity);
            _context.SaveChanges();
        }
        public void Update(DetalleControlGlucometria2 entity)
        {
            _context.DetalleControlGlucometria2.Update(entity);
            _context.SaveChanges();
        }

        public List<DetalleControlGlucometria2> GetDetalleControlGlucometria2ByHospitalizacionId(int hospitalizacionId)
        {
            return _context.DetalleControlGlucometria2
                .Include(x => x.ControlGlucometria2)
                .Include(x => x.Profesional)
                    .ThenInclude(x => x.Persona)
                .Include(x => x.User)
                    .ThenInclude(x => x.Persona)
                .Where(x => x.ControlGlucometria2.HospitalizacionId == hospitalizacionId)
                .OrderByDescending(x => x.FechaAplicacion) // Ordenar por fecha de aplicación de forma descendente
                .ToList();
        }


        public ControlGlucometria2 GetById(int id)
        {
            return _context.ControlGlucometria2.Where(x => x.Id == id).FirstOrDefault();
        }
        public DetalleControlGlucometria2 GetDetalleControlGlucometria2ById(int id)
        {
            return _context.DetalleControlGlucometria2.Where(x => x.Id == id).FirstOrDefault();
        }

    }
}

