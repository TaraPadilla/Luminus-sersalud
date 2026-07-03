using Database.Shared.Models;
using System.Collections.Generic;

namespace Database.Shared.IRepository
{
    public interface IInfoIngesta
    {
        // Método para añadir un nuevo registro de InfoIngesta
        void AddInfoIngesta(InfoIngesta entity);

        // Método para obtener la lista de InfoIngesta asociada a un ID de IngestaExcreta2
        List<InfoIngesta> GetInfoIngestaByIngestaExcretaId(int ingestaExcretaId);
    }
}
