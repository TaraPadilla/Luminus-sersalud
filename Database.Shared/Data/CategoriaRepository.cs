using System.Collections.Generic;
using System.Linq;
using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Paginacion;

namespace Database.Shared.Data
{
    public class CategoriaRepository : IDespegablesProducto
    {
        private readonly Context _context = null;
        public CategoriaRepository(Context context)
        {
            _context = context;
        }

        // via de administracion
        public List<Viadmin> ListarCategorias()
        {
            return _context.viadmins
            .Include(a => a.Productos)
            .OrderBy(a => a.NombreViadmin).Where(x => x.Eliminado == false)
            .ToList();
        }

        public List<Viadmin> GetViadmins()
        {
            return _context.viadmins
                .Where(a => !a.Eliminado)
                .ToList();
        }
        public void Add(Viadmin viadmin, bool saveChanges = true)
        {
            _context.viadmins.Add(viadmin);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        public void Update(Viadmin viadmin, bool saveChanges = true)
        {

            _context.Entry(viadmin).State = EntityState.Modified;

            if (saveChanges)
            {
                _context.SaveChanges();
            }

        }

        public PaginacionList<Viadmin> PaginacionCategoria(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var viadmin = _context.viadmins.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                viadmin = viadmin.Where(s => s.NombreViadmin.Contains(searchString));
            }


            return PaginacionList<Viadmin>.CreateAsyncc(viadmin.Where(s => s.Eliminado == false), pageNumber ?? 1, pageSize);
        }

        public Viadmin Get(int id, bool includeRelatedEntities = true)
        {
            return _context.viadmins.Where(a => a.Id == id).SingleOrDefault();
        }


        public Viadmin GetViadminByname(string name, bool includeRelatedEntities = true)
        {
            return _context.viadmins.Where(a => a.NombreViadmin == name).SingleOrDefault();
        }

        // Tipo de Producto 
        public List<TipoProducto> ListarTipoProductos()
        {
            return _context.TipoProductos
            .Include(a => a.Productos)
            .OrderBy(a => a.NombreTipoProducto).Where(x => x.Eliminado == false)
            .ToList();
        }

        public void Add(TipoProducto tipoproducto, bool saveChanges = true)
        {
            _context.TipoProductos.Add(tipoproducto);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        public void Update(TipoProducto tipoProducto, bool saveChanges = true)
        {

            _context.Entry(tipoProducto).State = EntityState.Modified;

            if (saveChanges)
            {
                _context.SaveChanges();
            }

        }

        public PaginacionList<TipoProducto> PaginacionTipoProducto(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var viadmin = _context.TipoProductos.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                viadmin = viadmin.Where(s => s.NombreTipoProducto.Contains(searchString));
            }


            return PaginacionList<TipoProducto>.CreateAsyncc(viadmin.Where(s => s.Eliminado == false), pageNumber ?? 1, pageSize);
        }

        public TipoProducto GetTipoProducto(int id, bool includeRelatedEntities = true)
        {
            return _context.TipoProductos.Where(a => a.Id == id).SingleOrDefault();
        }


        // Presentacion Producto 
        public List<PresentacionProducto> ListarPresentacion(bool includeRelatedEntities = true)
        {
            if (includeRelatedEntities)
            {
                return _context.PresentacionProductos
                .Include(a => a.Productos)
                .OrderBy(a => a.PresentProducto)
                .Where(x => x.Eliminado == false)
                .ToList();
            }
            else
            {
                return _context.PresentacionProductos
                .OrderBy(a => a.PresentProducto)
                .Where(x => x.Eliminado == false)
                .ToList();
            }
        }

        public void Add(PresentacionProducto presentacionProducto, bool saveChanges = true)
        {
            _context.PresentacionProductos.Add(presentacionProducto);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        public void Update(PresentacionProducto presentacionProducto, bool saveChanges = true)
        {

            _context.Entry(presentacionProducto).State = EntityState.Modified;

            if (saveChanges)
            {
                _context.SaveChanges();
            }

        }

        public PaginacionList<PresentacionProducto> PaginacionPresentacionProducto(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var presentacion = _context.PresentacionProductos.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                presentacion = presentacion.Where(s => s.PresentProducto.Contains(searchString));
            }


            return PaginacionList<PresentacionProducto>.CreateAsyncc(presentacion.Where(s => s.Eliminado == false), pageNumber ?? 1, pageSize);
        }

        public PresentacionProducto GetPresentacionProducto(int id, bool includeRelatedEntities = true)
        {
            return _context.PresentacionProductos.Where(a => a.Id == id).SingleOrDefault();
        }

        public PresentacionProducto GetPresentacionProductoByName(string name, bool includeRelatedEntities = true)
        {
            return _context.PresentacionProductos.Where(a => a.PresentProducto == name).SingleOrDefault();
        }



        // GrupoT Producto 
        public List<GrupoTProducto> ListarGrupoT(bool includeRelatedEntities = true)
        {
            if (includeRelatedEntities)
            {
                return _context.GrupoTProductos
                .Include(a => a.Productos)
                .OrderBy(a => a.NombreGrupoT).Where(x => x.Eliminado == false)
                .ToList();
            }
            else
            {
                return _context.GrupoTProductos
                .OrderBy(a => a.NombreGrupoT)
                .Where(x => x.Eliminado == false)
                .ToList();
            }
        }

        public void Add(GrupoTProducto grupoTProducto, bool saveChanges = true)
        {
            _context.GrupoTProductos.Add(grupoTProducto);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        public void Update(GrupoTProducto grupoTProducto, bool saveChanges = true)
        {

            _context.Entry(grupoTProducto).State = EntityState.Modified;

            if (saveChanges)
            {
                _context.SaveChanges();
            }

        }

        public PaginacionList<GrupoTProducto> PaginacionGrupoTProducto(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var presentacion = _context.GrupoTProductos.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                presentacion = presentacion.Where(s => s.NombreGrupoT.Contains(searchString));
            }


            return PaginacionList<GrupoTProducto>.CreateAsyncc(presentacion.Where(s => s.Eliminado == false), pageNumber ?? 1, pageSize);
        }

        public GrupoTProducto GetGrupoTProducto(int id, bool includeRelatedEntities = true)
        {
            return _context.GrupoTProductos.Where(a => a.Id == id).SingleOrDefault();
        }


        public GrupoTProducto GetGrupoTProductoByName(string name, bool includeRelatedEntities = true)
        {
            return _context.GrupoTProductos.Where(a => a.NombreGrupoT == name).SingleOrDefault();
        }

        // Laboratorio Producto 
        public List<LaboratorioProducto> ListaLaboratorioProducto()
        {
            return _context.LaboratorioProductos
            .Include(a => a.Productos)
            .OrderBy(a => a.NombreLaboratorioProducto)
            .Where(x => x.Eliminado == false)
            .ToList();
        }

        public void Add(LaboratorioProducto laboratorioProducto, bool saveChanges = true)
        {
            _context.LaboratorioProductos.Add(laboratorioProducto);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        public LaboratorioProducto GetLaboratorioPorNombre(string laboratorio)
        {
            return _context.LaboratorioProductos.Where(a => a.NombreLaboratorioProducto.ToLower() == laboratorio.ToLower())
                .SingleOrDefault();
        }

        public void Update(LaboratorioProducto laboratorioProducto, bool saveChanges = true)
        {

            _context.Entry(laboratorioProducto).State = EntityState.Modified;

            if (saveChanges)
            {
                _context.SaveChanges();
            }

        }

        public PaginacionList<LaboratorioProducto> PaginacionLaboratorioProducto(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var presentacion = _context.LaboratorioProductos.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                presentacion = presentacion.Where(s => s.NombreLaboratorioProducto.Contains(searchString));
            }


            return PaginacionList<LaboratorioProducto>.CreateAsyncc(presentacion.Where(s => s.Eliminado == false), pageNumber ?? 1, pageSize);
        }

        public LaboratorioProducto GetLaboratorioProducto(int id, bool includeRelatedEntities = true)
        {
            return _context.LaboratorioProductos.Where(a => a.Id == id).SingleOrDefault();
        }

        public LaboratorioProducto GetLaboratorioProductoByName(string name, bool includeRelatedEntities = true)
        {
            return _context.LaboratorioProductos.Where(a => a.NombreLaboratorioProducto == name).SingleOrDefault();
        }

        // area de insumos

        public IList<Categoria> ListaCategorias()
        {
            return _context.Categorias
            .OrderBy(a => a.NombreCategoria).Where(x => x.Eliminado == false)
            .ToList();
        }

        public PaginacionList<Categoria> PaginacionCategorias(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var categorias = _context.Categorias.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                categorias = categorias.Where(s => s.NombreCategoria.Contains(searchString));
            }

            return PaginacionList<Categoria>.CreateAsyncc(categorias
            .Where(s => s.Eliminado == false),
            pageNumber ?? 1, pageSize);
        }

        public void Add(Categoria categoria, bool saveChanges = true)
        {
            _context.Categorias.Add(categoria);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        public void Update(Categoria categoria, bool saveChanges = true)
        {
            _context.Entry(categoria).State = EntityState.Modified;

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        public Categoria GetCategoria(int id)
        {
            return _context.Categorias.Where(a => a.Id == id).SingleOrDefault();
        }

        public Categoria GetCategoriaByName(string name)
        {
            return _context.Categorias.Where(a => a.NombreCategoria == name).SingleOrDefault();
        }



        public Marca GetMarca(int id)
        {
            return _context.Marcas.Where(a => a.Id == id).SingleOrDefault();
        }

        public Marca GetMarcaByName(string name)
        {
            return _context.Marcas.Where(a => a.NombreMarca == name).SingleOrDefault();
        }

        public IList<Marca> ListaMarcas()
        {
            return _context.Marcas
            .OrderBy(a => a.NombreMarca).Where(x => x.Eliminado == false)
            .ToList();
        }

        public PaginacionList<Marca> PaginacionMarcas(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var marcas = _context.Marcas.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                marcas = marcas.Where(s => s.NombreMarca.Contains(searchString));
            }

            return PaginacionList<Marca>.CreateAsyncc(marcas
            .Where(s => s.Eliminado == false),
            pageNumber ?? 1, pageSize);
        }

        public void Add(Marca marca, bool saveChanges = true)
        {
            _context.Marcas.Add(marca);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        public void Update(Marca marca, bool saveChanges = true)
        {
            _context.Entry(marca).State = EntityState.Modified;

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }



        public IList<Grupo> ListaGrupos()
        {
            return _context.Grupos
            .OrderBy(a => a.NombreGrupo).Where(x => x.Eliminado == false)
            .ToList();
        }

        public PaginacionList<Grupo> PaginacionGrupos(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var grupos = _context.Grupos.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                grupos = grupos.Where(s => s.NombreGrupo.Contains(searchString));
            }

            return PaginacionList<Grupo>.CreateAsyncc(grupos
            .Where(s => s.Eliminado == false),
            pageNumber ?? 1, pageSize);
        }

        public void Add(Grupo grupo, bool saveChanges = true)
        {
            _context.Grupos.Add(grupo);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        public void Update(Grupo grupo, bool saveChanges = true)
        {
            _context.Entry(grupo).State = EntityState.Modified;

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        public Grupo GetGrupo(int id)
        {
            return _context.Grupos.Where(a => a.Id == id && a.Eliminado == false).SingleOrDefault();
        }

        public Grupo GetGrupoByName(string name)
        {
            return _context.Grupos.Where(a => a.NombreGrupo == name && a.Eliminado == false).FirstOrDefault();
        }

    }
}