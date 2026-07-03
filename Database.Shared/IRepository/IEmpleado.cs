using Database.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Database.Shared.Paginacion;

namespace Database.Shared.IRepository
{
    public interface IEmpleado
    {
        public void Add(Empleado empleado, bool saveChanges = true);
        void Add(Clinica medico, bool saveChanges = true);
        public List<Empleado> GetList();

        public List<Empleado> GetEmpleadosConDetalles();

        public List<Empleado> GetListEmpleadoTipoProfesional();
        public List<object> GetListEmpleadoTipoProfesionalColegiado();

        public void Update(Empleado model, bool saveChanges = true);
        Clinica GetClinicaByName(string name, bool includeRelatedEntities = true);

        public Empleado Get(int id, bool includeRelatedEntities = true);
        Medicos GetMedicoById(int id, bool includeRelatedEntities = true);
        Medicos GetMedicoByName(string name, bool includeRelatedEntities = true);
        List<Clinica> GetListClinicas();
        List<Medicos> GetListMedicos();
        List<MedicoSecundarioDtoHospi> GetMedicosSecundariosPorIds(List<int> ids);

        void Add(Medicos medico, bool saveChanges = true);

        public PaginacionList<Empleado> PaginacionEmpleados(string sortOrder, string searchString, int? pageNumber, int pageSize, string tipoEmpleado);
        public PaginacionList<Empleado> PaginacionEmpleadosMedicos(string sortOrder, string searchString, int? pageNumber, int pageSize, string tipoEmpleado);

        public List<Empleado> GetByIds(List<int> ids);

        // =========================================================================================
        // NUEVO: Soporte DataTables server-side (PostgreSQL)
        // =========================================================================================
        // Retorna:
        // - RecordsTotal: total según tipo (Medico/Empleado) sin aplicar search
        // - RecordsFiltered: total según tipo aplicando search
        // - Data: página actual (shape usado por la vista DataTables)
        //
        Task<(int RecordsTotal, int RecordsFiltered, List<object> Data)> DataTableEmpleadosAsync(
            string tipoEmpleado,
            string searchValue,
            string orderBy,
            bool orderDescending,
            int start,
            int length
        );
    }
}

// using Database.Shared.Models;
// using System.Collections.Generic;
// using Database.Shared.Paginacion;

// namespace Database.Shared.IRepository
// {
//     public interface IEmpleado
//     {
//         public void Add(Empleado empleado, bool saveChanges = true);
//         void Add(Clinica medico, bool saveChanges = true);
//         public List<Empleado> GetList();
//         public List<Empleado> GetListEmpleadoTipoProfesional();
//         public List<object> GetListEmpleadoTipoProfesionalColegiado();

//         public void Update(Empleado model, bool saveChanges = true);
//         Clinica GetClinicaByName(string name, bool includeRelatedEntities = true);

//         public Empleado Get(int id, bool includeRelatedEntities = true);
//         Medicos GetMedicoById(int id, bool includeRelatedEntities = true);
//         Medicos GetMedicoByName(string name, bool includeRelatedEntities = true);
//         List<Clinica> GetListClinicas();
//         List<Medicos> GetListMedicos();
//         List<MedicoSecundarioDtoHospi> GetMedicosSecundariosPorIds(List<int> ids);

//         void Add(Medicos medico, bool saveChanges = true);

//         public PaginacionList<Empleado> PaginacionEmpleados(string sortOrder, string searchString, int? pageNumber, int pageSize, string tipoEmpleado);
//         public PaginacionList<Empleado> PaginacionEmpleadosMedicos(string sortOrder, string searchString, int? pageNumber, int pageSize, string tipoEmpleado);

//         public List<Empleado> GetByIds(List<int> ids);
//     }
// }