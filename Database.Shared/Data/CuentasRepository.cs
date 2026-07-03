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
    public class CuentasRepository : ICuentas
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;

        public CuentasRepository(Context context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public void Add(Cuentas entity)
        {
            if (entity == null)
            {
                throw new Exception("La entidad no puede ser nula");
            }
            _context.Cuentas.Add(entity);
            _context.SaveChanges();
        }

        public void Delete(int Id, ClaimsPrincipal UserId)
        {
            var data = _context.Cuentas.Where(x => x.Id == Id).FirstOrDefault();
            if (data.Id != 0)
            {
                data.Eliminado = true;
                _context.Cuentas.Update(data);
            }
            else
            {
                throw new Exception("La cuenta con ese ID no existe");
            }

            _context.CuentasTrazabilidad.Add(new CuentasTrazabilidad
            {
                CuentaId = data.Id,
                UserId = _userManager.GetUserId(UserId),
                FechaEliminacion = DateTime.Now
            });

            _context.SaveChanges();

        }

        public List<Cuentas> GetAll()
        {
            return _context.Cuentas
                .Include(x => x.Banco)
                .Include(x => x.TipoCuenta)
                .Where(x => x.Eliminado == false).ToList();
        }

        public Cuentas GetById(int id)
        {
            return _context.Cuentas.Where(x => x.Eliminado == false && x.Id == id).FirstOrDefault();

        }

        public void Update(Cuentas entity)
        {
            _context.Cuentas.Update(entity);
            _context.SaveChanges();
        }
    }
}
