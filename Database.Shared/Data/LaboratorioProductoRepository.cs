using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Shared.Data
{
    public class LaboratorioProductoRepository : ILaboratorioProducto
    {
        private readonly Context _context;

        public LaboratorioProductoRepository(Context context)
        {
            _context = context;
        }

        public List<LaboratorioProducto> GetAll()
        {
            return _context.LaboratorioProductos
                .Where(x => x.Eliminado == false).ToList();
        }
    }
}
