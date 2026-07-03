using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Shared.Data
{
    public class RecetaRepository : IRecetas
    {
        private readonly Context _context = null;

        public RecetaRepository(Context context)
        {
            _context = context;
        }

        public void Add(Receta receta)
        {
            _context.Recetas.Add(receta);
            _context.SaveChanges();
        }

        public void Delete(int recetaId)
        {
            var receta = _context.Recetas
                            .Where(g => g.Id == recetaId)
                            .FirstOrDefault();

            _context.Recetas.Remove(receta);
            _context.SaveChanges();
        }

        public Receta Get(int recetaId)
        {
            return _context.Recetas
                        .Where(a => a.Id == recetaId)
                        .SingleOrDefault();
        }

        public IList<Receta> GetList()
        {
            return _context.Recetas
                .ToList();
        }

        public void Update(Receta receta)
        {
            _context.Entry(receta).State = EntityState.Modified;
            _context.SaveChanges();
        }

        // -----------------------------
        // Categorías
        // -----------------------------
        public IList<CategoriaReceta> GetCategoriasRecetas()
        {
            return _context.CategoriasRecetas
                .Where(x => !x.Eliminado)
                .OrderBy(x => x.Nombre)
                .ToList();
        }

        public CategoriaReceta GetCategoriaReceta(int categoriaId)
        {
            return _context.CategoriasRecetas
                .FirstOrDefault(x => x.Id == categoriaId);
        }

        public void AddCategoriaReceta(CategoriaReceta categoria)
        {
            _context.CategoriasRecetas.Add(categoria);
            _context.SaveChanges();
        }

        public void UpdateCategoriaReceta(CategoriaReceta categoria)
        {
            _context.CategoriasRecetas.Update(categoria);
            _context.SaveChanges();
        }

        public void SoftDeleteCategoriaReceta(int categoriaId)
        {
            var categoria = _context.CategoriasRecetas.FirstOrDefault(x => x.Id == categoriaId);
            if (categoria == null) return;

            categoria.Eliminado = true;
            _context.CategoriasRecetas.Update(categoria);
            _context.SaveChanges();
        }

        // -----------------------------
        // Menús
        // -----------------------------
        public IList<RecetaMenu> GetRecetaMenus()
        {
            return _context.RecetasMenu
                .Where(x => !x.Eliminado)
                .OrderBy(x => x.Nombre)
                .ToList();
        }

        public RecetaMenu GetRecetaMenu(int menuId)
        {
            return _context.RecetasMenu.FirstOrDefault(x => x.Id == menuId);
        }

        public void AddRecetaMenu(RecetaMenu menu)
        {
            _context.RecetasMenu.Add(menu);
            _context.SaveChanges();
        }

        public void UpdateRecetaMenu(RecetaMenu menu)
        {
            _context.RecetasMenu.Update(menu);
            _context.SaveChanges();
        }

        public void SoftDeleteRecetaMenu(int menuId)
        {
            var menu = _context.RecetasMenu.FirstOrDefault(x => x.Id == menuId);
            if (menu == null) return;

            menu.Eliminado = true;
            _context.RecetasMenu.Update(menu);
            _context.SaveChanges();
        }

        // -----------------------------
        // Relación Receta <-> Menús
        // -----------------------------
        public IList<int> GetMenuIdsByReceta(int recetaId)
        {
            return _context.RecetasMenuRelacion
                .Where(x => x.RecetaId == recetaId)
                .Select(x => x.MenuId)
                .ToList();
        }

        public void ReplaceMenusForReceta(int recetaId, IEnumerable<int> menuIds)
        {
            var ids = (menuIds ?? Enumerable.Empty<int>())
                .Distinct()
                .ToList();

            var actuales = _context.RecetasMenuRelacion.Where(x => x.RecetaId == recetaId);
            _context.RecetasMenuRelacion.RemoveRange(actuales);

            foreach (var menuId in ids)
            {
                _context.RecetasMenuRelacion.Add(new RecetaMenuRelacion
                {
                    RecetaId = recetaId,
                    MenuId = menuId,
                    FechaHoraCreada = System.DateTime.Now
                });
            }

            _context.SaveChanges();
        }
    }
}
