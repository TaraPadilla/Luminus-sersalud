using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Database.Shared;
using Database.Shared.IRepository;
using Database.Shared.Models;

namespace sistema.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Administrador, Supervisor, Desarrollador")]
    public class NuevoUsuarioModel : RegisterModel
    {
        public NuevoUsuarioModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IEmpleado empleadoRepository,
            RoleManager<IdentityRole> roleManager,
            IUser userRepository,
            Context context)
            : base(userManager, signInManager, logger, emailSender, empleadoRepository, roleManager, userRepository, context)
        {
        }
    }
}
