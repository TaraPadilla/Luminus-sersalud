using Database.Shared.Models;
using farmamest.Models;
using System.Collections.Generic;

namespace farmamest.Service.IService
{
    public interface IInfoIngestaService
    {
        // Método para obtener la lista de InfoIngesta por IngestaExcretaId
        List<InfoIngestaVM> GetInfoIngestaByIngestaExcretaId(int ingestaExcretaId);

        // Método para agregar un nuevo registro de InfoIngesta
        void AddInfoIngesta(InfoIngesta entity);
    }
}
