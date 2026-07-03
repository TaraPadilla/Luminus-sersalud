using Database.Shared.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Database.Shared.IRepository
{
    public interface ICategoriaCuentaContable
    {
        public void Add(CategoriasCuentaContable entity);
        public List<CategoriasCuentaContable> GetAll();
        public CategoriasCuentaContable GetById(int id);
        public void Update(CategoriasCuentaContable entity);
        public void Delete(int Id);

    }
}
