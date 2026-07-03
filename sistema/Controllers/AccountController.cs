using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Database.Shared.IRepository;
using sistema.Models;
using Database.Shared.Paginacion;
using System.Linq;
using Database.Shared.Models;
using sistema.Areas.Identity.Pages.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Database.Shared;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace sistema.Controllers
{
    //[Authorize(Roles = "Administrador, Supervisor, Vendedor, Mensajero,Desarrollador,Farmacia,Laboratorio,Hospital,Clinica,Urologia,Algologia")]
    [Authorize]
    public class AccountController : Controller
    {

        private readonly IUser _userRepository = null;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly ILogger<RegisterModel> _loggerR;
        private readonly IEmailSender _emailSender;
        private readonly IEmpleado _empleadoRepository;
        private readonly Context _context;



        public AccountController(IUser userRepository, UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> loggerR, ILogger<LogoutModel> logger,
            IEmailSender emailSender, IEmpleado empleadoRepository, Context context, RoleManager<IdentityRole> roleManager)
        {

            _userRepository = userRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _loggerR = loggerR;
            _logger = logger;
            _emailSender = emailSender;
            _empleadoRepository = empleadoRepository;
            _context = context;
            _roleManager = roleManager;

        }

        public IActionResult Lista()
        {

            var model = new LogoutModel(_signInManager, _logger) { };

            model.Init(_userRepository);


            return View(model);
        }

        public async Task<ActionResult> Modificar(string id)
        {
            if (id == null)
            {
                return BadRequest("request is incorrect");
            }

            var user = await _userManager.FindByIdAsync(id);

            var user2 = _userRepository.GetbyId(id);

            if (user == null)
            {
                return StatusCode(404);
            }

            var oldRole = _userManager.GetRolesAsync(user).Result;

            if (oldRole.Count == 0)
            {
                var input = new RegisterModel.InputModel()
                {
                    User = user,
                    User2 = user2,
                    Id = id
                };

                input.Init(_userRepository);

                input.Init(_empleadoRepository);

                return View(input);

            }
            else
            {
                var input = new RegisterModel.InputModel()
                {
                    User = user,
                    User2 = user2,
                    OldRoleName = oldRole[0],
                    Id = id,
                    RolesId = _userRepository.GetRole(oldRole.FirstOrDefault()).Id
                };

                input.Init(_userRepository);
                input.Init(_empleadoRepository);

                return View(input);

            }




        }

        [HttpPost]
        public async Task<ActionResult> ModificarUser(RegisterModel.InputModel modelo)
        {

            var user = await _userManager.FindByIdAsync(modelo.Id);


            if (user == null)
            {
                return StatusCode(404);
            }

            user.Email = modelo.User.Email;
            user.UserName = modelo.User.Email;
            user.NormalizedEmail = modelo.User.Email.ToUpper();



            var result = await _userManager.UpdateAsync(user);


            if (result.Succeeded)
            {
                TempData["Message"] = "EL NOMBRE SE HA CAMBIADO CON ÉXITO.";
                var nuevomodelo = new RegisterModel.InputModel() { };
                nuevomodelo.Email = user.Email;

                return RedirectToAction("Modificar", new { id = modelo.Id });
                //return RedirectToAction("Lista");
            }


            return RedirectToAction("Modificar", new { id = modelo.Id });
        }



        public async Task<ActionResult> ModificarPass(RegisterModel.InputModel modelo)
        {

            var user = await _userManager.FindByIdAsync(modelo.Id);



            if (user == null)
            {
                return StatusCode(404);
            }

            if (modelo.NewPassword != null)
            {


                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, modelo.NewPassword);


            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Message"] = "LA CONTRASEÑA SE HA CAMBIADO CON ÉXITO.";
                return RedirectToAction("Modificar", new { id = modelo.Id });
            }



            return View(modelo);
        }

        public async Task<ActionResult> ModificarRole(RegisterModel.InputModel modelo)
        {

            var user = await _userManager.FindByIdAsync(modelo.Id);
            var oldRole = _userManager.GetRolesAsync(user).Result;


            if (user == null)
            {
                return StatusCode(404);
            }


            if (modelo.RolesId != null)
            {
                // var oldRole =  _userManager.GetRolesAsync(user).Result;

                if (oldRole.Count == 0)
                {

                    var role = _roleManager.FindByIdAsync(modelo.RolesId).Result;
                    var result = await _userManager.AddToRoleAsync(user, role.Name);

                    if (result.Succeeded)
                    {
                        TempData["Message"] = "EL ROL SE HA CAMBIADO CON ÉXITO.";
                        return RedirectToAction("Modificar", new { id = modelo.Id });
                    }

                }
                else
                {

                    await _userManager.RemoveFromRoleAsync(user, oldRole[0]);

                    var role = _roleManager.FindByIdAsync(modelo.RolesId).Result;
                    var result = await _userManager.AddToRoleAsync(user, role.Name);

                    if (result.Succeeded)
                    {
                        TempData["Message"] = "EL ROL SE HA CAMBIADO CON ÉXITO.";
                        return RedirectToAction("Modificar", new { id = modelo.Id });
                    }
                }
            }


            return View(modelo);
        }

        public IActionResult ModificarEmpleado(RegisterModel.InputModel modelo)
        {

            if (modelo == null)
            {
                return StatusCode(404);
            }

            var user = _userRepository.GetbyId(modelo.Id);

            user.EmpleadoId = modelo.User2.EmpleadoId;

            _userRepository.Update(user);

            TempData["Message"] = "EL EMPLEADO SE HA MODIFICADO CON EXITO.";
            return RedirectToAction("Lista");





        }

        public IActionResult Desactivar(string username)
        {
            if (username == null)
            {
                return BadRequest("request is incorrect");
            }

            var model = _userRepository.Get(username);

            if (model == null)
            {
                return StatusCode(404);
            }

            if (model.LockoutEnabled == false)
            {
                model.LockoutEnabled = true;

            }
            else
            {
                model.LockoutEnabled = false;
            }



            _userRepository.Update(model);
            TempData["Message"] = "¡El empleado se ha modificado con exito.!";

            return RedirectToAction("Lista");
        }

        public async Task<IActionResult> EliminarUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return View();
            }

            return View();
        }


        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var getRolesList = _roleManager.Roles.Select(r => r.Name).ToList();

            var model = new RegisterViewModel()
            {
                RoleList = new SelectList(getRolesList)
            };

            return View(model);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var role = model.Role;
                var user = new User { Id = Guid.NewGuid().ToString(), UserName = model.Email, Email = model.Email };
                await _userManager.CreateAsync(user, model.Password);

                var result = await _userManager.AddToRoleAsync(user, model.Role);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation(3, "Usuario creado con éxito.");
                    return RedirectToAction("ListaUsuarios");
                }
                AddErrors(result);
            }

            var getRolesList = _roleManager.Roles.Select(r => r.Name).ToList();

            var viewModel = new RegisterViewModel()
            {
                RoleList = new SelectList(getRolesList)
            };
            // If we got this far, something failed, redisplay form
            return View(viewModel);
        }


        public IActionResult ListaUsuarios()
        {
            var lista = _userRepository.GetList();
            return View(lista);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

    }
}