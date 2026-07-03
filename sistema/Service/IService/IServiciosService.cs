using Database.Shared.Dto;
using Database.Shared.Models;
using farmamest.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace farmamest.Service.IService
{
    public interface IServiciosService
    {
        List<DtoSpGetServicios> GetServicios();
    }
}
