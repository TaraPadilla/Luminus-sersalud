using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Database.Shared.Data
{
    public class VisitadorMedicoRepository : IVisitadorMedico
    {
        private readonly Context _context;

        public VisitadorMedicoRepository(Context context)
        {
            _context = context;
        }

        // Método para agregar un nuevo visitador médico
        public void AddVisitadorMedico(VisitadorMedico entity)
        {
            _context.VisitadorMedico.Add(entity);
            _context.SaveChanges();
        }

        // Método para obtener todos los visitadores médicos
        public List<VisitadorMedico> GetAllVisitadorMedico()
        {
            return _context.VisitadorMedico.ToList();
        }

        // Método para obtener un visitador médico por ID
        public VisitadorMedico GetVisitadorMedicoById(int id)
        {
            return _context.VisitadorMedico
                .FirstOrDefault(vm => vm.Id == id);
        }

        // Método para actualizar un visitador médico
        public void UpdateVisitadorMedico(VisitadorMedico entity)
        {
            var existingEntity = _context.VisitadorMedico.FirstOrDefault(vm => vm.Id == entity.Id);
            if (existingEntity != null)
            {
                existingEntity.NombreVisitador = entity.NombreVisitador;
                existingEntity.ContactoVisitador = entity.ContactoVisitador;
                existingEntity.NombreFarmaceutica = entity.NombreFarmaceutica;
                existingEntity.ContactoFarmaceutica = entity.ContactoFarmaceutica;
                existingEntity.Observaciones = entity.Observaciones;
                existingEntity.UrlCatalogo = entity.UrlCatalogo;

                _context.SaveChanges();
            }
        }

        // Método para eliminar un visitador médico por ID
        public void DeleteVisitadorMedico(int id)
        {
            var entity = _context.VisitadorMedico.FirstOrDefault(vm => vm.Id == id);
            if (entity != null)
            {
                _context.VisitadorMedico.Remove(entity);
                _context.SaveChanges();
            }
        }
    }
}
