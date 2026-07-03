using Database.Shared.Models;
using System.Collections.Generic;
using Database.Shared.Paginacion;
using System.Linq; 
namespace Database.Shared.IRepository
{
    public interface ILaboratorioClinico
    {
        PaginacionList<CategoriaLabClinico> PaginacionCategoriasLab(string sortOrder, string searchString, int? pageNumber, int pageSize);
        void Add(CategoriaLabClinico categoriaLabClinico, bool saveChanges = true);
        void Add(DatosExamenesLabClinico datos, bool saveChanges = true);
        void Update(DatosExamenesLabClinico datos, bool saveChanges = true);
        DatosExamenesLabClinico GetDatosExamenLab(int id, bool includeRelatedEntities = true);
        void Update(CategoriaLabClinico categoriaLabClinico, bool saveChanges = true);
        List<DatosExamenesLabClinico> DatosLabList(int idExamenLab, bool includeRelatedEntities = true);
        void Update(Examen examen, bool saveChanges = true);
        ExamenLabClinico GetByCodigo(string codigoInterno);
        CategoriaLabClinico GetCategoriaLab(int id, bool includeRelatedEntities = true);
        IList<CategoriaLabClinico> GetListCategoriasLab();
        void Add(DetalleExamen detalle, bool saveChanges = true);
        public Examen GetExamenByConsulta(int consultaId);
        void Add(ExamenLabClinico examenLabClinico, bool saveChanges = true);
        List<ExamenLabClinico> ExamenesLabListTodos(bool includeRelatedEntities = true);
        PaginacionList<ExamenLabClinico> PaginacionExamenClinicoLab(string sortOrder, string searchString, int? pageNumber, int pageSize, int? catexamenid);
        // PaginacionList<ExamenLabClinico> PaginacionExamenClinicoLab(string sortOrder, string searchString, int? pageNumber, int pageSize);
        void Update(ExamenLabClinico examen, bool saveChanges = true);
        ExamenLabClinico GetExamenLab(int id, bool includeRelatedEntities = true);
        List<ExamenLabClinicoPrecio> GetPreciosExamen(int examenLabClinicoId);
        IList<ExamenLabClinico> GetListExamenesLaboratorio();
        void Add(Examen examen, bool saveChanges = true);
        IList<Examen> GetListExamenesRealizado();
        Examen GetExamenRealizado(int id, bool includeRelatedEntities = true);
        Examen GetExamenResultados(int id, bool includeRelatedEntities = true);

		//List<CajaLab> ListarCajas();
  //      //List<CajaLab> ListarCajas();
        List<ExamenLabClinico> ExamenesLabList(int idCategoria, bool includeRelatedEntities = true);
        List<Examen> PaginacionExamenesRealizados(string sortOrder, string searchString, int? pageNumber, int pageSize, int? estado, bool solicitado = false);
        void saveChanges();
        //CajaLab GetCajaAbierta();
        //void Add(DetalleCajaLab detalle, bool saveChanges = true);
        //void Add(CajaLab detalle, bool saveChanges = true);
        //CajaLab GetCaja(int id, bool includeRelatedEntities = true);
        //void Update(CajaLab caja, bool saveChanges = true);
        //PaginacionList<VentasLab> PaginacionVentasLab(string sortOrder, string searchString, int? pageNumber, int pageSize);
        //VentasLab GetVentaLab(int id, bool includeRelatedEntities = true);
        //void Update(VentasLab examen, bool saveChanges = true);
        DetalleExamen GetDetalleExamenLab(int id, bool includeRelatedEntities = true);
        void Update(DetalleExamen examen, bool saveChanges = true);
        void Add(Resultados resultado, bool saveChanges = true);

        List<Resultados> ResultadosListByDetalleId(int DetalleId, bool includeRelatedEntities = true);
        Resultados GetResultadoById(int id, bool includeRelatedEntities = true);

        public List<Resultados> ResultadosListByExamenId(int Id, bool includeRelatedEntities = true);//Resultados por examen 
        void Update(Resultados resultado, bool saveChanges = true);

        public void ActualizarInventarioInsumoVentaExamenesLaboratorio(int examenId);

        public List<Examen> GetAllExamenesRealizados();


		public IList<ExamenLabClinicoPregunta> GetPreguntasExamenLabClinicoCita(int citaId);
		public IList<ExamenLabClinicoPregunta> GetPreguntasExamenLabClinico(int citaId);

		public ExamenLabClinicoPregunta GetPregunta(int id, bool includeRelatedEntities = true);

		public void UpdatePregunta(ExamenLabClinicoPregunta examen, bool saveChanges = true);
		public List<ExamenLabClinicosSP> GetListExamenesLaboratorioSP();
		List<DatoTipoExamenLabClinico> GetListTipo();

        public Examen GetExamenPaciente(int id, bool includeRelatedEntities = true);

        public Examen GetInfoRequeridaEditarDetalleExamen(int id);

        IQueryable<Examen> ObtenerExamenesQueryable(string searchString, int? estado);
    }
}