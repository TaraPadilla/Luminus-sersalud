using Database.Shared.IRepository;
using Database.Shared.Models;
using Database.Shared.Paginacion;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Shared.Data
{
    public class AlergiaRaraPacientesRepository : IAlergiaRaraPacientes
    {
        private readonly Context _context = null;

        public AlergiaRaraPacientesRepository(Context context)
        {
            _context = context;
            
        }

        //Metodo para Traer alergias
        public List<AlergiaRara> GetAlergias()
        {
            return _context.AlergiaRaras.Where(v => v.Eliminado == false).ToList();
        }
        //Metodo para Traer alergias paciente
        public List<AlergiaRaraPaciente> GetAlergiasPaciente(int idPaciente)
        {
            return _context.AlergiaRaraPacientes
            .Include(al => al.AlergiaRara)
            .Where(a => a.PacienteId == idPaciente)
            .ToList();
        }

        public void AddAlergiasPacientes(AlergiaRaraPaciente alergiaRaraPaciente, bool saveChanges = true)
        {

            _context.AlergiaRaraPacientes.Add(alergiaRaraPaciente);

            if (saveChanges)
            {
                _context.SaveChanges();
            }

        }

        public void AddAlergiaRara(AlergiaRara alergiaRara, bool saveChanges = true)
        {

            _context.AlergiaRaras.Add(alergiaRara);

            if (saveChanges)
            {
                _context.SaveChanges();
            }

        }

    }
}
