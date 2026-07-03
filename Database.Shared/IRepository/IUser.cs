using Microsoft.EntityFrameworkCore;
using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Database.Shared.IRepository
{

    public interface IUser
    {
        public IdentityUser GetByUsernameAndPassword(string username, string password);

        public void Add(User user, bool saveChanges = true);

        public List<User> GetList();

        public void SaveChanges();

        public List<IdentityRole> GetRolesList();

        public User Get(string username, bool includeRelatedEntities = true);

        public IdentityRole GetRole(string name, bool includeRelatedEntities = true);

        public User GetbyId(string id, bool includeRelatedEntities = true);
       

        public void Update(User model, bool saveChanges = true);

        public bool ValidateRolAdmin(string userId);
        public List<User> GetByIds(List<string> ids);

    }


}