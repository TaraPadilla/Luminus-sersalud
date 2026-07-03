using Database.Shared.Models;

namespace Database.Shared.IRepository
{
    public interface IRegistroAnestesia
    {
        void Add(RegistroAnestesia entity);
        void Update(RegistroAnestesia entity);
        RegistroAnestesia GetByHospitalizacionId(int hospitalizacionId);
    }
}
