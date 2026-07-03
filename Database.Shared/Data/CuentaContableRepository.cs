using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Database.Shared.Data
{
    public class CuentaContableRepository : ICuentaContable
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;

        public CuentaContableRepository(Context context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public void Add(CuentaContable entity)
        {
            _context.CuentaContable.Add(entity);
            _context.SaveChanges();
        }

        public void Delete(int Id, ClaimsPrincipal UserId)
        {
            var data = _context.CuentaContable.Where(x => x.Id == Id).FirstOrDefault();
            if (data.Id != 0)
            {
                data.Eliminado = true;
                _context.CuentaContable.Update(data);
            }
            else
            {
                throw new Exception("La cuenta contable con ese ID no existe");
            }

            _context.CuentaContableTrazabilidad.Add(new CuentaContableTrazabilidad
            {
                CuentaContableId = data.Id,
                UserId = _userManager.GetUserId(UserId),
                FechaEliminacion = DateTime.Now
            });

            _context.SaveChanges();
        }

        public List<CuentaContable> GetAll()
        {
            return _context.CuentaContable
                .Include(x => x.Banco)
                .Include(x => x.CategoriaCuenta)
                .Include(x => x.Nomenclatura)
                    .ThenInclude(p => p.Nomenclatura)
                .Include(x => x.Cuenta)
                .Where(x => x.Eliminado == false).ToList();
        }

        public CuentaContable GetById(int id)
        {
            return _context.CuentaContable.Where(x => x.Id == id && x.Eliminado == false).FirstOrDefault();
        }

        public void Update(CuentaContable entity)
        {
            var data = _context.CuentaContableNomenclaturas.Where(x => x.CuentaContableId == entity.Id);
            _context.CuentaContableNomenclaturas.RemoveRange(data);
            _context.CuentaContable.Update(entity);
            _context.SaveChanges();
        }
    }
}
