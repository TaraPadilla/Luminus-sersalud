using Database.Shared.Models;

namespace farmamest.Service.IService
{
    public interface IRegistroAnestesiaService
    {
        RegistroAnestesia GetByHospitalizacionId(int hospitalizacionId);
        RegistroAnestesia Guardar(int hospitalizacionId, string userId, string datosJson);
    }
}
