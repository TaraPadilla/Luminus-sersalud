using Database.Shared.Dto;
using Database.Shared.Models;
using farmamest.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sistema.Service.IService
{
    public interface IComprasService
    {
        public void AgregarProductosAInventario(Compra compra, string userId);
    }
}
