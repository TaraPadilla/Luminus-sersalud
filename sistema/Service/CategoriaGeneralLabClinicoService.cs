using Database.Shared.Data;
using Database.Shared.Enumeraciones;
using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Models;
using Microsoft.AspNetCore.Identity;
using sistema.Models;
using sistema.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using Database.Shared.Paginacion;
using Microsoft.AspNetCore.Http;
namespace sistema.Service 
{ 


    public class CategoriaGeneralLabClinicoService : ICategoriaGeneralLabClinicoService
    {
        private readonly UserManager<User> _userManager = null;
        private readonly IUser _userRepository = null;
        private readonly ICategoriaGeneralLabClinico _categoriaGeneralLabClinicoRepository = null;

        public CategoriaGeneralLabClinicoService(
           ICategoriaGeneralLabClinico categoriaGeneralLabClinicoRepository,
            IUser userRepository,
            UserManager<User> userManager

        )
        {

            _userManager = userManager;
            _userRepository = userRepository;
            _categoriaGeneralLabClinicoRepository = categoriaGeneralLabClinicoRepository;
        }

        public PaginacionList<CategoriaGeneralLabClinicoViewModel> GetListCategoriasGeneralesLabClinico(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {

            var data = _categoriaGeneralLabClinicoRepository.GetListCategoriasGeneralesLabClinico();


            if (!string.IsNullOrEmpty(searchString))
            {
                data = data.Where(c => c.Nombre.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            
            var viewModel = data.Select(c => new CategoriaGeneralLabClinicoViewModel
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Estado = c.Estado,
                FechaCreacion = c.FechaCreacion,
                UltimoUsuarioModificado = c.UltimoUsuarioModificado,
                Eliminado = c.Eliminado,
                Activo = c.Activo
            })
            .ToList();

            return PaginacionList<CategoriaGeneralLabClinicoViewModel>.CreateAsynccCustom(viewModel, pageNumber ?? 1, pageSize);

        }

        public void Add(CategoriaGeneralLabClinicoViewModel categoriaViewModel) {
            var categoria = new CategoriaGeneralLabClinico
            {
                Nombre = categoriaViewModel.Nombre,
                FechaCreacion = DateTime.Now,
                Activo = true,
                UltimoUsuarioModificado = categoriaViewModel.UltimoUsuarioModificado,
                Eliminado =  false
            };



            _categoriaGeneralLabClinicoRepository.Add(categoria);

        }

        public CategoriaGeneralLabClinicoViewModel GetCategoriaGeneralLab(int id) {

            var categoria = _categoriaGeneralLabClinicoRepository.GetCategoriaGeneralLab(id);

            var categoriaViewModel = new CategoriaGeneralLabClinicoViewModel {

                Id = categoria.Id,
                Nombre = categoria.Nombre,
                Estado = categoria.Estado,
                FechaCreacion = categoria.FechaCreacion,
                UltimoUsuarioModificado = categoria.UltimoUsuarioModificado,
                Eliminado = categoria.Eliminado,
                Activo = categoria.Activo

            };

            return categoriaViewModel;

        }


        public void Update(CategoriaGeneralLabClinicoViewModel categoriaViewModel)
        {
            var categoria = new CategoriaGeneralLabClinico
            {
                Id = categoriaViewModel.Id,
                Nombre = categoriaViewModel.Nombre,
                FechaCreacion = categoriaViewModel.FechaCreacion,
                Activo = categoriaViewModel.Activo,
                UltimoUsuarioModificado = categoriaViewModel.UltimoUsuarioModificado,
                Eliminado = categoriaViewModel.Eliminado
            };

            _categoriaGeneralLabClinicoRepository.Update(categoria);

        }


    }
}

    

