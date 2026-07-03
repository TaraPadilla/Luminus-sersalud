using Microsoft.EntityFrameworkCore;
using Database.Shared.Models;
using System.Linq;
using Database.Shared.IRepository;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;


namespace Database.Shared.Data
{

    public class UserRepository : IUser
    {

        private readonly Context _context = null;
        // private readonly RoleManager<IdentityRole> _roleManager;

        public UserRepository(Context context)
        {
            _context = context;

        }
        private List<User> users = new List<User>
        {
            new User { Id = "1", UserName = "admin", PasswordHash = "farmamest123" }
        };

        public void Add(User user, bool saveChanges = true)
        {
            _context.Users.Add(user);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public List<User> GetList() => _context.Usuarios.Include(a => a.Persona).ToList();

        // public List<User> GetListRoles() => _context.Users.ToList();

        public List<IdentityRole> GetRolesList() => _context.Roles.ToList();

        public IdentityUser GetByUsernameAndPassword(string username, string password)
        {
            var user = users.SingleOrDefault(u => u.UserName == username &&
                u.PasswordHash == password);//.Sha256());
            return user;
        }

        public User Get(string username, bool includeRelatedEntities = true)
        {

            return _context.Usuarios
            .Where(a => a.UserName == username)
            .SingleOrDefault();
        }

        public IdentityRole GetRole(string name, bool includeRelatedEntities = true)
        {

            return _context.Roles
               .Where(a => a.Name == name)
               .SingleOrDefault();


        }

        public User GetbyId(string id, bool includeRelatedEntities = true)
        {
            return _context.Usuarios
            .Include(a => a.Persona)
            .Where(a => a.Id == id).SingleOrDefault();
        }



        public void Update(User model, bool saveChanges = true)
        {

            _context.Entry(model).State = EntityState.Modified;

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }
        public bool ValidateRolAdmin(string userId)
        {
            var rolesUsuario = _context.UserRoles
                .Where(x => x.UserId == userId)
                .ToList();

            var rol = _context.Roles
                .FirstOrDefault(x => x.NormalizedName.ToLower().Trim() == "administrador".ToLower().Trim());

            // Verificar si el rol de administrador existe en la lista de roles del usuario
            if (rol != null && rolesUsuario.Any(r => r.RoleId == rol.Id))
            {
                return true;
            }

            return false;
        }

        // NUEVO: Método para obtener usuarios por una lista de IDs.
        public List<User> GetByIds(List<string> ids)
        {
            return _context.Usuarios
                .Include(u => u.Persona)
                .Where(u => ids.Contains(u.Id))
                .ToList();
        }
    }


}