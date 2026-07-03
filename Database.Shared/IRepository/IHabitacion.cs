using Database.Shared.Models;
using System.Collections.Generic;
using Database.Shared.Paginacion;

namespace Database.Shared.IRepository
{
    public interface IHabitacion
    {
        public Habitacion Add(Habitacion habitacion);
        public Habitacion Update(Habitacion habitacion);
        public Habitacion Get(int habitacionId);
        public Paciente GetPacienteOcupante(int habitacionId);
        public void Delete(int habitacionId);
        public IList<Habitacion> GetHabitaciones();
        public CategoriaHabitacion AddCategoria(CategoriaHabitacion categoria);
        void UpdateCategoria(CategoriaHabitacion categoria);
        public CategoriaHabitacion GetCategoria(int categoriaId);
        public IList<CategoriaHabitacion> GetCategorias(bool includeTarifas = true);
        public IList<CategoriaHabitacionTarifa> GetTarifasHabitacion(int habitacionId);
        public Hospitalizacion GetHospitalizacionActual(int habitacionId);
        public void DeleteCategoria(int categoriaId);
        public IList<EstadoHabitacion> GetEstados();
        public (Paciente, Empleado, string) GetPacienteOcupanteConMedicoYCita(int habitacionId);

        IList<Habitacion> GetHabitacionesPorCategoriaParaAgenda(int categoriaId);
        CategoriaHabitacionTarifa GetTarifaById(int tarifaId);


    }
}