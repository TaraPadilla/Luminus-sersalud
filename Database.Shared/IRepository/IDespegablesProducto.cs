using System.Collections.Generic;
using Database.Shared.Models;
using Database.Shared.Paginacion;

namespace Database.Shared.IRepository
{
    public interface IDespegablesProducto
    {
        // via de administracion
        public List<Viadmin> ListarCategorias();
        public List<Viadmin> GetViadmins();
        public void Add(Viadmin viadmin, bool saveChanges = true);

        public Viadmin Get(int id, bool includeRelatedEntities = true);
        public Viadmin GetViadminByname(string name, bool includeRelatedEntities = true);


        public void Update(Viadmin viadmin, bool saveChanges = true);

        public PaginacionList<Viadmin> PaginacionCategoria(string sortOrder, string searchString, int? pageNumber, int pageSize);


        // tipo de producto

        public List<TipoProducto> ListarTipoProductos();

        public void Add(TipoProducto viadmin, bool saveChanges = true);

        public TipoProducto GetTipoProducto(int id, bool includeRelatedEntities = true);

        public void Update(TipoProducto viadmin, bool saveChanges = true);

        public PaginacionList<TipoProducto> PaginacionTipoProducto(string sortOrder, string searchString, int? pageNumber, int pageSize);



        // Presentacion
        public List<PresentacionProducto> ListarPresentacion(bool includeRelatedEntities = true);

        public void Add(PresentacionProducto viadmin, bool saveChanges = true);

        public PresentacionProducto GetPresentacionProducto(int id, bool includeRelatedEntities = true);
        public PresentacionProducto GetPresentacionProductoByName(string name, bool includeRelatedEntities = true);

        public void Update(PresentacionProducto presentacionProducto, bool saveChanges = true);

        public PaginacionList<PresentacionProducto> PaginacionPresentacionProducto(string sortOrder, string searchString, int? pageNumber, int pageSize);



        // GrupoT
        public List<GrupoTProducto> ListarGrupoT(bool includeRelatedEntities = true);

        public void Add(GrupoTProducto grupoTProducto, bool saveChanges = true);

        public GrupoTProducto GetGrupoTProducto(int id, bool includeRelatedEntities = true);
        public GrupoTProducto GetGrupoTProductoByName(string name, bool includeRelatedEntities = true);


        public void Update(GrupoTProducto grupoTProducto, bool saveChanges = true);

        public PaginacionList<GrupoTProducto> PaginacionGrupoTProducto(string sortOrder, string searchString, int? pageNumber, int pageSize);



        // Laboratorio 
        public List<LaboratorioProducto> ListaLaboratorioProducto();
        public void Add(LaboratorioProducto laboratorio, bool saveChanges = true);
        public LaboratorioProducto GetLaboratorioProducto(int id, bool includeRelatedEntities = true);
        public LaboratorioProducto GetLaboratorioProductoByName(string name, bool includeRelatedEntities = true);

        public void Update(LaboratorioProducto laboratorio, bool saveChanges = true);
        public PaginacionList<LaboratorioProducto> PaginacionLaboratorioProducto(string sortOrder, string searchString, int? pageNumber, int pageSize);
        LaboratorioProducto GetLaboratorioPorNombre(string laboratorio);




        // categorias (para clinica)
        public IList<Categoria> ListaCategorias();
        public PaginacionList<Categoria> PaginacionCategorias(string sortOrder, string searchString, int? pageNumber, int pageSize);
        public void Add(Categoria categoria, bool saveChanges = true);
        public void Update(Categoria categoria, bool saveChanges = true);


        // marcas ( solo clinica)
        public IList<Marca> ListaMarcas();
        public PaginacionList<Marca> PaginacionMarcas(string sortOrder, string searchString, int? pageNumber, int pageSize);
        public void Add(Marca marca, bool saveChanges = true);
        public void Update(Marca marca, bool saveChanges = true);

        public Categoria GetCategoria(int id);
        public Categoria GetCategoriaByName(string name);
        public Marca GetMarca(int id);
        public Marca GetMarcaByName(string name);


        public IList<Grupo> ListaGrupos();
        public PaginacionList<Grupo> PaginacionGrupos(string sortOrder, string searchString, int? pageNumber, int pageSize);
        public void Add(Grupo grupo, bool saveChanges = true);
        public void Update(Grupo grupo, bool saveChanges = true);
        public Grupo GetGrupo(int id);
        public Grupo GetGrupoByName(string name);


    }
}
