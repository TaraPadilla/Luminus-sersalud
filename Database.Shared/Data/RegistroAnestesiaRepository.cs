using Database.Shared.IRepository;
using Database.Shared.Models;
using System.Linq;

namespace Database.Shared.Data
{
    public class RegistroAnestesiaRepository : IRegistroAnestesia
    {
        private readonly Context _context;

        public RegistroAnestesiaRepository(Context context)
        {
            _context = context;
        }

        public void Add(RegistroAnestesia entity)
        {
            _context.RegistrosAnestesia.Add(entity);
            _context.SaveChanges();
        }

        public void Update(RegistroAnestesia entity)
        {
            _context.RegistrosAnestesia.Update(entity);
            _context.SaveChanges();
        }

        public RegistroAnestesia GetByHospitalizacionId(int hospitalizacionId)
        {
            return _context.RegistrosAnestesia
                .Where(r => r.HospitalizacionId == hospitalizacionId)
                .OrderByDescending(r => r.FechaActualizacion ?? r.FechaRegistro)
                .FirstOrDefault();
        }
    }
}
