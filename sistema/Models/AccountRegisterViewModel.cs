using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace sistema.Models
{
    public class AccountRegisterViewModel 
    {
         [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "El nombre de usuario debe tener al menos {2} caracteres.")]
        public string Name { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos {2} dígitos.")]
        public string Password { get; set; }

        [Display(Name = "Confirmar contraseña")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "*La contraseña de confirmación no es igual a la ingresada.")]
        public string ConfirmPassword { get; set; }

        public IEnumerable<SelectListItem> RoleList { get; set; }
        //public List<Role> TempRolesList { get; set; }

        public string Role { get; set; }

        public List<User> ListaUsers = new List<User>();

  
        public AccountRegisterViewModel()
        {

        }
    }

}