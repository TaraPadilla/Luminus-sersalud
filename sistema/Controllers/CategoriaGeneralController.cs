using Database.Shared.Data;
using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using sistema.Service;
using sistema.Service.IService;

namespace farmamest.Controllers
{
    public class CategoriaGeneralController : Controller
    {
        private readonly UserManager<User> _userManager = null;
        private readonly IUser _usuarioRepository = null;
        private readonly ICategoriaGeneralLabClinicoService _categoriaGeneralLabClinicoService = null;

        public CategoriaGeneralController(
         UserManager<User> userManager,
         IUser usuarioRepository,
         ICategoriaGeneralLabClinicoService categoriaGeneralLabClinicoService)
        {
            _userManager = userManager;
            _usuarioRepository = usuarioRepository;
            _categoriaGeneralLabClinicoService = categoriaGeneralLabClinicoService;

        }






        

    }

    

}
