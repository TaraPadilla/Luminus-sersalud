using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Database.Shared.Data
{
    public class BancoRepository : IBanco
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;

        public BancoRepository(Context context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public void AddBanco(Banco banco)
        {
            _context.Bancos.Add(banco);
            _context.SaveChanges();
        }

        public void DeleteBanco(int Id, ClaimsPrincipal UserId)
        {
            var data = _context.Bancos.Where(x => x.Id == Id).FirstOrDefault();
            if (data.Id != 0)
            {
                data.Eliminado = true;
                _context.Bancos.Update(data);
            }
            else
            {
                throw new Exception("El banco con ese ID no existe");
            }

            _context.BancoTrazabilidad.Add(new BancoTrazabilidad
            {
                BancoId = data.Id,
                UserId = _userManager.GetUserId(UserId),
                FechaEliminacion = DateTime.Now
            });

            _context.SaveChanges();

        }

        public List<Banco> GetAll()
        {
            return _context.Bancos.Where(x => x.Eliminado == false).ToList();
        }

        public Banco GetBanco(int id)
        {
            return _context.Bancos.Where(x => x.Id == id && x.Eliminado == false).FirstOrDefault();
        }

        public void UpdateBanco(Banco banco)
        {
            _context.Bancos.Update(banco);
            _context.SaveChanges();
                
        }
    }
}
