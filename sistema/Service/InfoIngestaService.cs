using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Models;
using farmamest.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;

namespace farmamest.Service
{
    public class InfoIngestaService : IInfoIngestaService // En esta linea surge el error: 'InfoIngestaService' no implementa el miembro de interfaz 'IInfoIngestaService.GetInfoIngestaByIngestaExcretaId(int)'. 'InfoIngestaService.GetInfoIngestaByIngestaExcretaId(int)' no puede implementar 'IInfoIngestaService.GetInfoIngestaByIngestaExcretaId(int)' porque no tiene el tipo de valor devuelto coincidente de 'List<InfoIngestaVM>'.CS0738
    {
        private readonly IInfoIngesta _infoIngesta;

        public InfoIngestaService(IInfoIngesta infoIngesta)
        {
            _infoIngesta = infoIngesta;
        }

        // Método para añadir un nuevo registro de InfoIngesta
        public void AddInfoIngesta(InfoIngesta entity)
        {
            _infoIngesta.AddInfoIngesta(entity);
        }

        // Método para obtener la lista de InfoIngesta por el ID de IngestaExcreta2
        public List<InfoIngestaVM> GetInfoIngestaByIngestaExcretaId(int ingestaExcretaId) // En esta linea surge el error: El nombre del tipo o del espacio de nombres 'InfoIngestaVM' no se encontró (¿falta una directiva using o una referencia de ensamblado?)CS0246
        {
            var data = _infoIngesta.GetInfoIngestaByIngestaExcretaId(ingestaExcretaId);

            var result = data.Select(info => new InfoIngestaVM // En esta linea surge el error: El nombre del tipo o del espacio de nombres 'InfoIngestaVM' no se encontró (¿falta una directiva using o una referencia de ensamblado?)CS0246
            {
                Id = info.Id,
                InfoIngestaIV1 = info.InfoIngestaIV1,
                InfoIngestaIV2 = info.InfoIngestaIV2,
                InfoIngestaIV3 = info.InfoIngestaIV3,
                InfoIngestaIV4 = info.InfoIngestaIV4,
                InfoIngestaIV5 = info.InfoIngestaIV5,
                InfoIngestaIV6 = info.InfoIngestaIV6,
                InfoIngestaPO = info.InfoIngestaPO,
                FechaRegistro = info.FechaRegistro.ToString("yyyy-MM-dd HH:mm:ss"), // Formato de fecha con hora
                UserId = info.UserId,
                Profesional = info.User?.Persona != null ? $"{info.User.Persona.NombreYApellidos}" : "-"
            }).ToList();

            return result;
        }
    }
}
