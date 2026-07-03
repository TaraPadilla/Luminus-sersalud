using Database.Shared.Models;
using Microsoft.AspNetCore.Identity;
using sistema.Models;

namespace sistema.Service.IService
{
    public interface ICajaService
    {
        public CajaDetallesViewModel GetDetallesCaja(int id);
        public void ReabrirCaja(int cajaId);
        public bool VerificarEmpleado(string guidUsuario, int empleadoId);
        public void CerrarCaja(User usuario, int cajaId);
    }
}
