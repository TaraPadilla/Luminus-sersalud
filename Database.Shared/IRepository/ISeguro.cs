using Database.Shared.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Database.Shared.IRepository
{
    public interface ISeguro
    {
        public void AddSeguro(Seguro seguro);
        public List<Seguro> GetAll();
        public Seguro GetSeguro(int id);
        public void UpdateSeguro(Seguro seguro);
        public void DeleteSeguro(int Id, ClaimsPrincipal UserId);
        public List<Seguro> GetList();

    }
}
