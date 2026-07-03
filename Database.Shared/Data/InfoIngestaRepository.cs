using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Shared.Data
{
    public class InfoIngestaRepository : IInfoIngesta
    {
        private readonly Context _context;

        public InfoIngestaRepository(Context context)
        {
            _context = context;
        }

        // Método para obtener el listado de InfoIngesta por el ID de IngestaExcreta2
        public List<InfoIngesta> GetInfoIngestaByIngestaExcretaId(int ingestaExcretaId)
        {
            return _context.InfoIngesta
                .Where(x => x.IngestaExcreta2Id == ingestaExcretaId) // Filtrar por IngestaExcreta2Id
                .Include(x => x.IngestaExcreta2) // Incluir los detalles de IngestaExcreta2
                .Include(x => x.User) // Incluir los detalles del usuario
                .ToList(); // Retornar el listado de InfoIngesta
        }

        // Método para crear un nuevo registro de InfoIngesta
        public void AddInfoIngesta(InfoIngesta entity)
        {
            _context.InfoIngesta.Add(entity); // Añadir el nuevo registro de InfoIngesta
            _context.SaveChanges(); // Guardar cambios en la base de datos
        }
    }
}
