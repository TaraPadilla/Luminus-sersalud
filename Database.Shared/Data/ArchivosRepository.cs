
using Database.Shared.Models;
using Database.Shared.IRepository;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Paginacion;

namespace Database.Shared.Data
{
    public class ArchivosRepository : IArchivos
    {
        private readonly Context _context = null;
        public ArchivosRepository(Context context)
        {
            _context = context;
        }
        public void Add(Archivo archivo)
        {
            _context.Archivos.Add(archivo);
            _context.SaveChanges();
        }
    }
}