using Database.Shared.Enumeraciones;
using Database.Shared.IRepository;
using Database.Shared.Models;
using sistema.Models;

namespace sistema.Helpers
{
    public static class HospitalizacionHabitacionHelper
    {
        public static HospitalizacionHabitacionViewModel CrearViewModel(Habitacion habitacion, IHabitacion habitacionRepository)
        {
            var ocupante = "-";
            int? hospitalizacionId = null;
            int? citaId = null;

            if (habitacion.EstadoHabitacionId == (int)EstadoHabitacionEnum.Ocupada)
            {
                var ocupacion = habitacionRepository?.GetOcupacionActual(habitacion.Id);
                if (ocupacion.HasValue)
                {
                    hospitalizacionId = ocupacion.Value.HospitalizacionId;
                    citaId = ocupacion.Value.CitaId;
                    ocupante = ocupacion.Value.Paciente?.Nombre ?? "-";
                }
            }

            return new HospitalizacionHabitacionViewModel
            {
                HabitacionId = habitacion.Id,
                HospitalizacionId = hospitalizacionId,
                CitaId = citaId,
                HabitacionNombre = habitacion.NombreNumeroHabitacion,
                HabitacionCategoria = habitacion.CategoriaHabitacion?.NombreCategoria,
                HabitacionEstadoId = habitacion.EstadoHabitacionId,
                HabitacionEstado = habitacion.EstadoHabitacion?.NombreEstado,
                HabitacionOcupante = ocupante,
                HabitacionNumeroCamas = habitacion.NumeroCamas,
                HabitacionCapacidadPersonas = habitacion.CapacidadPersonas
            };
        }
    }
}
