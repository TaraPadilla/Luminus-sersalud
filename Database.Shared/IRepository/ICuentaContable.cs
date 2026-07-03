using Database.Shared.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Database.Shared.IRepository
{
    public interface ICuentaContable
    {
        public void Add(CuentaContable entity);
        public List<CuentaContable> GetAll();
        public CuentaContable GetById(int id);
        public void Update(CuentaContable entity);
        public void Delete(int Id, ClaimsPrincipal UserId);
    }
}
