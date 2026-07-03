using Database.Shared.Models;
using System.Collections.Generic;
using Database.Shared.Paginacion;
using System;
using System.Linq;
using Database.Shared.DataBindings;

namespace Database.Shared.IRepository
{
    public interface ICitas
    {
        IList<Citas> GetList();

        IList<Citas> GetAll();

        void Add(Citas cita, bool saveChanges = true);
        void Add(CitasServicio citaServicio, bool saveChanges = true);
        Citas GetCita(int id);
        ExamenFisico GetExamenFisico(int id);
        Especialidad GetEspecilidad(int id);
        Especialidad GetTurnoId(int id);
        List<Citas> GetTurnoEspecialidadCita(int Id);
        IList<Servicio> GetServiciosList();

        string ObtenerSiguienteDpiFicticio();
        List<CitasServicio> GetServiciosCita(int citaId);
        void Update(Citas model, bool saveChanges = true);
        void Update(CitasServicio citaServicio, bool saveChanges = true);
        PaginacionList<Citas> PaginacionCitas(string sortOrder, string searchString, int? pageNumber, int pageSize, int? empleadoId);
        PaginacionList<Citas> PaginacionCitasFinalizadas(string sortOrder, string searchString, int? pageNumber, int pageSize);
        PaginacionList<Citas> PaginacionCitasNoAsistidas(string sortOrder, string searchString, int? pageNumber, int pageSize);

        PaginacionList<Citas> PaginacionTurnos(string sortOrder, string searchString, int? pageNumber, int pageSize);
        IList<Especialidad> GetEspecialidadesList();
        IList<Habitacion> GetHabitacionesList(int categoriaHabitacionId);

        IList<CategoriaHabitacion> GetHabitacionesCategoriasList();
        IList<Citas> CitasNormales(string searchString);
        IList<Citas> CitasPorFecha(DateTime fecha);
        IList<Citas> CitasPorFechas(DateTime fechaInicial, DateTime fechaFinal, int? sucursalId, int? empleadoId, int? especialidadId);
        IList<Citas> CitasCalendarioLineal(DateTime? fechaInicial, DateTime? fechaFinal, int? sucursalId, int? empleadoId, int? especialidadId, int? servicioId, string pacienteNombre);

        IList<Citas> CitasListaPorFecha(DateTime fecha);
        IList<Citas> CitasListado();
        IList<EstadoPagoConsulta> EstadoPagosConsultasLista();

        //Bloqueo de dias
        void AddFechaBloqueada(CalendarioFechaBloqueada fechaBloqueada);
        CalendarioFechaBloqueada GetFechaBloqueada(DateTime fecha);
        void DeleteFechaBloqueada(DateTime fecha);

        IList<Citas> CitasPorFechaHora(DateTime fecha);
        //Traer las citas Proximas
        IList<Citas> GetListCitasProximas();

        // Validación de conflicto de agenda (EmpleadoId + rango de tiempo)
        bool ExisteConflictoEmpleado(int empleadoId, DateTime inicio, DateTime fin, int? citaIdExcluir = null);


        Citas GetCitaActivaPorHabitacion(int habitacionId);

    }
}