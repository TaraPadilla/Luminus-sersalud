using Database.Shared.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Database.Shared.IRepository
{
    public interface IBanco
    {
        public void AddBanco(Banco banco);
        public List<Banco> GetAll();
        public Banco GetBanco(int id);
        public void UpdateBanco(Banco banco);
        public void DeleteBanco(int Id, ClaimsPrincipal UserId);

    }
}
