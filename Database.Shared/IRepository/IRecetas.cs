using Database.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.IRepository
{
    public interface IRecetas
    {
        void Add(Receta receta);
        void Delete(int recetaId);
        Receta Get(int recetaId);
        IList<Receta> GetList();
        void Update(Receta receta);

        // NUEVO - Catálogos / Relación Receta-Menus
        IList<CategoriaReceta> GetCategoriasRecetas();
        CategoriaReceta GetCategoriaReceta(int categoriaId);
        void AddCategoriaReceta(CategoriaReceta categoria);
        void UpdateCategoriaReceta(CategoriaReceta categoria);
        void SoftDeleteCategoriaReceta(int categoriaId);

        IList<RecetaMenu> GetRecetaMenus();
        IList<int> GetMenuIdsByReceta(int recetaId);
        void ReplaceMenusForReceta(int recetaId, IEnumerable<int> menuIds);

        // Menús
        RecetaMenu GetRecetaMenu(int menuId);
        void AddRecetaMenu(RecetaMenu menu);
        void UpdateRecetaMenu(RecetaMenu menu);
        void SoftDeleteRecetaMenu(int menuId);
    }
}
