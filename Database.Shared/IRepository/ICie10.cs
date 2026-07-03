using System.Collections.Generic;
using Database.Shared.Models;
using System.Threading.Tasks;
namespace Database.Shared.IRepository{

    public interface ICie10{
        Task<IEnumerable<Cie10>> GetAll();
        Task<Cie10> GetByID(string codigo);
    }
}