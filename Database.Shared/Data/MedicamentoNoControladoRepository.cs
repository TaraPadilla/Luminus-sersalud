using System.Collections.Generic;
using System.Linq;
using Database.Shared.Data;
using Database.Shared.IRepository;
using Database.Shared.Models;

namespace Database.Shared.Repository
{
    public class MedicamentoNoControladoRepository : IMedicamentoNoControladoRepository
    {
        private readonly Context _context;

        public MedicamentoNoControladoRepository(Context context)
        {
            _context = context;
        }

        public void Add(MedicamentoNoControlado registro)
        {
            _context.MedicamentosNoControlado.Add(registro);
            _context.SaveChanges();
        }

        public List<MedicamentoNoControlado> GetHistorialByHospitalizacion(int hospitalizacionId)
        {
            return _context.MedicamentosNoControlado
                .Where(m => m.HospitalizacionId == hospitalizacionId && !m.Eliminado)
                .OrderByDescending(m => m.FechaRegistro)
                .ToList();
        }

        public IEnumerable<MedicamentoNoControlado> GetByHospitalizacionId(int hospitalizacionId)
        {
            return _context.MedicamentosNoControlado
                .Where(m => m.HospitalizacionId == hospitalizacionId)
                .OrderBy(m => m.ProductoNombre)
                .ToList();
        }
    }
}