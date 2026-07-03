using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Database.Shared.Data
{
    public class CategoriaCuentaContableRepository : ICategoriaCuentaContable
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;

        public CategoriaCuentaContableRepository(Context context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public void Add(CategoriasCuentaContable entity)
        {
            _context.CategoriasCuentaContables.Add(entity);
            _context.SaveChanges();
        }

        public void Delete(int Id)
        {
            var data = GetById(Id);
            data.Eliminado = true;
            Update(data);
        }

        public List<CategoriasCuentaContable> GetAll()
        {
            return _context.CategoriasCuentaContables.Where(x => x.Eliminado == false).ToList();
        }

        public CategoriasCuentaContable GetById(int id)
        {
            return _context.CategoriasCuentaContables.Where(x => x.Id == id).FirstOrDefault();
        }

        public void Update(CategoriasCuentaContable entity)
        {
            _context.CategoriasCuentaContables.Update(entity);
            _context.SaveChanges();
        }
    }
}
