using Database.Shared.IRepository;
using Database.Shared.Models;
using sistema.Models;
using System;

namespace farmamest.Service
{
    /// <summary>Crea filas de aplicación con horarios equidistantes y helpers de formato.</summary>
    public static class MedicamentoAplicacionHelper
    {
        public static HospitalizacionProducto RegistrarProductoConAplicaciones(
            IHospitalizacion repository,
            HospitalizacionAgregarMedicamentoViewModel model,
            string usuarioCreaId)
        {
            var producto = repository.AddMedicamento(new HospitalizacionProducto
            {
                HospitalizacionId = model.HospitalizacionId,
                ProductoId = model.ProductoId,
                UnidadMedidaVentaId = model.UnidadMedidaVentaId,
                PrecioId = model.PrecioId,
                PrecioValor = model.Precio,
                Cantidad = model.Cantidad,
                Indicaciones = model.Indicaciones,
                ViaAdministracion = model.ViaAdministracion,
                FrecuenciaAdministracion = model.FrecuenciaAdministracion,
                FechaHoraAplicacionManual = model.FechaHoraAplicacionManual
            });

            CrearAplicacionesConHorario(
                repository,
                producto.Id,
                producto.Cantidad,
                model.FrecuenciaAdministracion,
                model.FechaHoraAplicacionManual,
                usuarioCreaId);

            return producto;
        }

        public static void CrearAplicacionesConHorario(
            IHospitalizacion repository,
            int hospitalizacionProductoId,
            decimal cantidad,
            string frecuenciaAdministracion,
            string fechaHoraAplicacionManual,
            string usuarioCreaId)
        {
            var horarios = MedicamentoScheduleHelper.CalcularHorariosAplicacion(
                (int)cantidad,
                frecuenciaAdministracion ?? "",
                fechaHoraAplicacionManual);

            for (int i = 0; i < (int)cantidad; i++)
            {
                repository.AddProductoAplicacion(new HospitalizacionProductoAplicacion
                {
                    HospitalizacionProductoId = hospitalizacionProductoId,
                    Cantidad = 1,
                    Aplicado = false,
                    UsuarioCreaId = usuarioCreaId,
                    FechaHoraAplicacionManual = i < horarios.Count ? horarios[i] : (DateTime?)null
                });
            }
        }

        public static string FormatearFechaProgramada(
            int indiceDosis,
            int cantidadTotal,
            string frecuenciaAdministracion,
            string fechaHoraInicioManual,
            DateTime? fechaAlmacenadaEnFila = null)
        {
            if (fechaAlmacenadaEnFila.HasValue && fechaAlmacenadaEnFila.Value.Year >= 2000)
                return MedicamentoScheduleHelper.FormatearHorarioLocal(fechaAlmacenadaEnFila.Value);

            var horarios = MedicamentoScheduleHelper.CalcularHorariosAplicacion(
                cantidadTotal,
                frecuenciaAdministracion ?? "",
                fechaHoraInicioManual);

            return indiceDosis < horarios.Count && horarios[indiceDosis].Year >= 2000
                ? MedicamentoScheduleHelper.FormatearHorarioLocal(horarios[indiceDosis])
                : "-";
        }

        /// <summary>Texto resumido para historial agrupado (ej. "09:00, 10:00, 11:00").</summary>
        public static string FormatearRangoHorariosProgramados(
            int cantidadTotal,
            string frecuenciaAdministracion,
            string fechaHoraInicioManual)
        {
            var horarios = MedicamentoScheduleHelper.CalcularHorariosAplicacion(
                cantidadTotal,
                frecuenciaAdministracion ?? "",
                fechaHoraInicioManual);

            if (horarios.Count == 0) return "-";
            if (horarios.Count == 1)
                return MedicamentoScheduleHelper.FormatearHorarioLocal(horarios[0]);

            return MedicamentoScheduleHelper.FormatearHorarioLocal(horarios[0])
                   + " → "
                   + MedicamentoScheduleHelper.FormatearHorarioLocal(horarios[horarios.Count - 1]);
        }
    }
}
