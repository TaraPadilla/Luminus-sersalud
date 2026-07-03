using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Database.Shared.Models
{
    public class Role : IdentityRole
    {
        public Role()
        {
        }
        
        public static object Identity {get;set;}

       
    }
}