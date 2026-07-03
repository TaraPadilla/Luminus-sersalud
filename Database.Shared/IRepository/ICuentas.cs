using Database.Shared.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Database.Shared.IRepository
{
    public interface ICuentas
    {

        public void Add(Cuentas entity);
        public List<Cuentas> GetAll();
        public Cuentas GetById(int id);
        public void Update(Cuentas entity);
        public void Delete(int Id, ClaimsPrincipal UserId);

    }
}
