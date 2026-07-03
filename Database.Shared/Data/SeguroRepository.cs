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
    public class SeguroRepository : ISeguro
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;

        public SeguroRepository(Context context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public void AddSeguro(Seguro seguro)
        {
            _context.Seguros.Add(seguro);
            _context.SaveChanges();
        }

        public void DeleteSeguro(int Id, ClaimsPrincipal UserId)
        {
            var data = _context.Seguros.Where(x => x.Id == Id).FirstOrDefault();
            if (data.Id != 0)
            {
                data.Eliminado = true;
                _context.Seguros.Update(data);
            }
            else
            {
                throw new Exception("El seguro con ese ID no existe");
            }

            // _context.SeguroTrazabilidad.Add(new SeguroTrazabilidad
            // {
            //     SeguroId = data.Id,
            //     UserId = _userManager.GetUserId(UserId),
            //     FechaEliminacion = DateTime.Now
            // });

            _context.SaveChanges();

        }


        public List<Seguro> GetList()
        {
            var seguros = _context.Seguros
                .Where(x => x.Eliminado == false)
                .Select(x => new Seguro
                {
                    Id = x.Id,
                    Nombre = x.Nombre
                })
                .ToList();

            return seguros;
        }

        public List<Seguro> GetAll()
        {
            var seguros = _context.Seguros.Where(x => x.Eliminado == false).ToList();
            // Console.WriteLine("SeguroRepository - Seguros Count: " + seguros.Count());
            return seguros;
        }

        public Seguro GetSeguro(int id)
        {
            return _context.Seguros.Where(x => x.Id == id && x.Eliminado == false).FirstOrDefault();
        }

        public void UpdateSeguro(Seguro seguro)
        {
            _context.Seguros.Update(seguro);
            _context.SaveChanges();

        }
    }
}
