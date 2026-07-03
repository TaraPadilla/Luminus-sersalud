// using System;
// using System.Collections.Generic;
// using System.ComponentModel.DataAnnotations;
// using System.Linq;
// using System.Text.Encodings.Web;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Authentication;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Identity.UI.Services;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.RazorPages;
// using Microsoft.Extensions.Logging;
// using Database.Shared.Models;
// using Database.Shared.IRepository;
// using Database.Shared;

// namespace sistema.Areas.Identity.Pages.Account
// {
//     [AllowAnonymous]
//     public class LoginModel : PageModel
//     {
//         private readonly UserManager<User> _userManager;
//         private readonly SignInManager<User> _signInManager;

//         // private readonly IUser _userRepository = null;

//         private readonly Context _context = null;

//         private readonly ILogger<LoginModel> _logger;

//         public LoginModel(SignInManager<User> signInManager,
//             ILogger<LoginModel> logger,
//             UserManager<User> userManager, Context context)
//         {
//             _userManager = userManager;
//             _signInManager = signInManager;
//             _logger = logger;
//             _context = context;
//         }

//         [BindProperty]
//         public InputModel Input { get; set; }

//         public IList<AuthenticationScheme> ExternalLogins { get; set; }

//         public string ReturnUrl { get; set; }

//         [TempData]
//         public string ErrorMessage { get; set; }

//         public class InputModel
//         {
//             [Required]
//             [EmailAddress]
//             public string Email { get; set; }

//             [Required]
//             [DataType(DataType.Password)]
//             public string Password { get; set; }

//             [Display(Name = "Recuerdame")]
//             public bool RememberMe { get; set; }
//         }

//         public async Task OnGetAsync(string returnUrl = null)
//         {
//             if (!string.IsNullOrEmpty(ErrorMessage))
//             {
//                 ModelState.AddModelError(string.Empty, ErrorMessage);
//             }

//             returnUrl ??= Url.Content("/Home/Index");

//             // Clear the existing external cookie to ensure a clean login process
//             await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

//             ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

//             ReturnUrl = returnUrl;
//         }

//         // public async Task<IActionResult> OnPostAsync(string returnUrl = null)
//         // {
//         //     returnUrl ??= Url.Content("/Home/Index");

//         //     ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

//         //     if (ModelState.IsValid)
//         //     {

//         //         var email = Input.Email;
//         //         var pass = Input.Password;
//         //         // This doesn't count login failures towards account lockout
//         //         // To enable password failures to trigger account lockout, set lockoutOnFailure: true

//         //         var user = await _userManager.FindByEmailAsync(Input.Email);

//         //         if (user == null)
//         //         {
//         //             ModelState.AddModelError(string.Empty, "Usuario No Existe.");
//         //             return Page();
//         //         }

//         //         if (user.LockoutEnabled != true)
//         //         {
//         //             ModelState.AddModelError(string.Empty, "Usuario Inactivo.");
//         //             return Page();
//         //         }

//         //         var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);


//         //         if (result.Succeeded)
//         //         {
//         //             // var user = await _signInManager.UserManager.FindByEmailAsync(Input.Email);
//         //             // var userPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
//         //             // var identity = userPrincipal.Identity;
//         //             _logger.LogInformation("User logged in.");
//         //             return LocalRedirect("/Home/Index");
//         //         }
//         //         if (result.RequiresTwoFactor)
//         //         {
//         //             return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
//         //         }
//         //         if (result.IsLockedOut)
//         //         {
//         //             _logger.LogWarning("User account locked out.");
//         //             return RedirectToPage("./Lockout");
//         //         }
//         //         else
//         //         {
//         //             ModelState.AddModelError(string.Empty, "Invalid login attempt.");
//         //             return Page();
//         //         }
//         //     }

//         //     // If we got this far, something failed, redisplay form
//         //     return Page();

//         //     //return LocalRedirect(returnUrl);
//         // }

//         public async Task<IActionResult> OnPostAsync(string returnUrl = null)
//         {
//             // 1. Asignamos la ruta por defecto si returnUrl viene nulo
//             returnUrl ??= Url.Content("/Home/Index");

//             ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

//             if (ModelState.IsValid)
//             {
//                 // 2. Buscamos el usuario en la base de datos
//                 var user = await _userManager.FindByEmailAsync(Input.Email);

//                 if (user == null)
//                 {
//                     // Recordatorio de seguridad: Es mejor usar un mensaje genérico
//                     ModelState.AddModelError(string.Empty, "Intento de inicio de sesión no válido.");
//                     return Page();
//                 }

//                 if (user.LockoutEnabled != true)
//                 {
//                     ModelState.AddModelError(string.Empty, "Usuario Inactivo.");
//                     return Page();
//                 }

//                 // 3. Intentamos iniciar sesión
//                 var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

//                 if (result.Succeeded)
//                 {
//                     _logger.LogInformation("User logged in.");

//                     // 4. NUEVA LÓGICA: Validamos el rol usando UserManager, no HttpContext.User
//                     bool esRegistroHuella = await _userManager.IsInRoleAsync(user, "RegistroHuellaMedica");

//                     if (esRegistroHuella)
//                     {
//                         // Redirigimos a la URL específica y pasamos el Id del usuario como parámetro en la URL
//                         // Ajusta "/RegistrarHuella" a la ruta real de tu controlador/página
//                         return LocalRedirect($"/WebAuthnVista/AgregarHuella?userId={user.Id}");
//                     }

//                     // 5. Si no tiene el rol, lo enviamos al flujo normal (Home o la URL que intentaba visitar)
//                     return LocalRedirect(returnUrl);
//                 }

//                 if (result.RequiresTwoFactor)
//                 {
//                     return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
//                 }

//                 if (result.IsLockedOut)
//                 {
//                     _logger.LogWarning("User account locked out.");
//                     return RedirectToPage("./Lockout");
//                 }
//                 else
//                 {
//                     ModelState.AddModelError(string.Empty, "Intento de inicio de sesión no válido.");
//                     return Page();
//                 }
//             }

//             // If we got this far, something failed, redisplay form
//             return Page();
//         }
//     }
// }
