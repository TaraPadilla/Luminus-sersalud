using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Database.Shared.IRepository;
using Database.Shared.Models;
using Database.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sistema.Models;

namespace sistema.Areas.Identity.Pages.Account
{

    public class RegisterModel : PageModel
    {
        //private readonly IUser _userRepository;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        // private readonly RoleStoreBase<IdentityRole> _roleStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        private readonly IEmpleado _empleadoRepository;

        private readonly IUser _userRepository;

        private readonly Context _context;

        public RegisterModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender, IEmpleado empleadoRepository, RoleManager<IdentityRole> roleManager, IUser userRepository, Context context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _empleadoRepository = empleadoRepository;
            _roleManager = roleManager;
            _userRepository = userRepository;
            _context = context;
        }


        //public AccountLoginViewModel model {get; set;} = new AccountLoginViewModel();

        //public Cliente Cliente {get; set;} = new Cliente();
        public SelectList ListaEmpleados { get; set; }

        public SelectList ListaRoles { get; set; }
        public void Init(IEmpleado empleadoRepository)//,IUser userRepository)
        {
            ListaEmpleados = new SelectList(empleadoRepository.GetList(), "Id", "Nombre");
            //ListaRoles = new SelectList(userRepository.GetRolesList(),"Name");

        }

        public void Init(IUser userRepository)
        {

            ListaRoles = new SelectList(userRepository.GetRolesList(), "Id", "Name");

        }



        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public bool Desactivado = true;

            public string Id { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [Display(Name = "NewPassword")]
            public string NewPassword { get; set; }


            [Display(Name = "Confirm password")]
            [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmedPassword { get; set; }

            public IdentityUser User { get; set; }

            public User User2 { get; set; }

            public Role Role { get; set; }
            public string OldRoleName { get; set; }

            public int? EmpleadoId { get; set; }

            public string RolesId { get; set; }


            public SelectList ListaRoles { get; set; }

            public SelectList ListaEmpleados { get; set; }

            public List<Empleado> ListaEmpleados2 { get; set; }

            public void Init(IUser userRepository)
            {

                ListaRoles = new SelectList(userRepository.GetRolesList(), "Id", "Name");

            }

            public void Init(IEmpleado empleadoRepository)
            {

                ListaEmpleados = new SelectList(empleadoRepository.GetList(), "Id", "Nombre");
                ListaEmpleados2 = empleadoRepository.GetList();

            }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();


            this.Init(_empleadoRepository);//,_userRepository);
            this.Init(_userRepository);
        }


        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("/Account/Lista");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // 1. Validación de empleado duplicado
            if (Input.EmpleadoId != null)
            {
                if (_context.Usuarios.Include(a => a.Persona).Any(a => a.EmpleadoId == Input.EmpleadoId))
                {
                    ModelState.AddModelError(string.Empty, "Este empleado ya tiene un usuario asignado.");
                    this.Init(_empleadoRepository);
                    this.Init(_userRepository);
                    return Page();
                }
            }

            // 2. Buscar el rol de forma asíncrona (QUITAMOS EL .Result)
            var role = await _roleManager.FindByIdAsync(Input.RolesId);

            var user = new User
            {
                UserName = Input.Email,
                Email = Input.Email,
                EmpleadoId = Input.EmpleadoId,
                LockoutEnabled = Input.Desactivado
            };

            // 3. INTENTAR CREAR EL USUARIO Y CAPTURAR EL RESULTADO
            var resultCreate = await _userManager.CreateAsync(user, Input.Password);

            if (resultCreate.Succeeded)
            {
                // 4. SOLO SI SE CREÓ EL USUARIO, ASIGNAMOS EL ROL
                if (role != null)
                {
                    var resultRole = await _userManager.AddToRoleAsync(user, role.Name);

                    if (!resultRole.Succeeded)
                    {
                        // Si falla el rol, agregamos errores y devolvemos la página
                        foreach (var error in resultRole.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        this.Init(_empleadoRepository);
                        this.Init(_userRepository);
                        return Page();
                    }
                }

                TempData["Message"] = "¡El Usuario se ha guardado con éxito!";
                return LocalRedirect(returnUrl);
            }
            else
            {
                // 5. SI FALLÓ LA CREACIÓN DEL USUARIO (ej. contraseña débil)
                foreach (var error in resultCreate.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Si falló algo, recargamos la página con los errores en el ValidationSummary
            this.Init(_empleadoRepository);
            this.Init(_userRepository);
            return Page();
        }
    }
}
