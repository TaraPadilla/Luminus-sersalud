using System.Collections.Generic;
using System.Linq;
using Database.Shared;
using Database.Shared.Models;
using farmamest.Models;
using Microsoft.EntityFrameworkCore;

namespace farmamest.Service
{
    public static class MedicamentoHistorialPdfBuilder
    {
        public static List<HistorialMedicamentoPdfRow> Build(Context context, int hospitalizacionId)
        {
            var resultado = new List<HistorialMedicamentoPdfRow>();

            var medicamentos = context.HospitalizacionesProductos
                .Include(p => p.Producto)
                .Include(p => p.HospitalizacionesProductosAplicaciones)
                .Where(p => p.HospitalizacionId == hospitalizacionId && !p.Eliminado)
                .ToList();

            var userIds = medicamentos
                .SelectMany(m => m.HospitalizacionesProductosAplicaciones.Where(a => !a.Eliminado))
                .SelectMany(a => new[] { a.UsuarioAplica, a.UsuarioCreaId })
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct()
                .ToList();

            var aplicacionesInsumos = context.HospitalizacionInsumosDirectosAplicaciones
                .Include(a => a.HospitalizacionInsumoDirecto)
                    .ThenInclude(i => i.Producto)
                .Where(a => a.HospitalizacionInsumoDirecto.HospitalizacionId == hospitalizacionId &&
                            !a.HospitalizacionInsumoDirecto.Eliminado)
                .ToList();

            userIds.AddRange(aplicacionesInsumos
                .SelectMany(a => new[] { a.UsuarioAplica, a.UsuarioCreaId })
                .Where(id => !string.IsNullOrWhiteSpace(id)));

            var nombresUsuario = MedicamentoHistorialDisplayHelper.ResolverNombresUsuarios(
                context, userIds.Distinct().ToList());

            foreach (var med in medicamentos)
            {
                var apps = med.HospitalizacionesProductosAplicaciones
                    .Where(a => !a.Eliminado)
                    .OrderBy(a => a.Id)
                    .ToList();

                var horarios = MedicamentoScheduleHelper.CalcularHorariosAplicacion(
                    (int)med.Cantidad,
                    med.FrecuenciaAdministracion ?? "",
                    med.FechaHoraAplicacionManual);

                for (int idx = 0; idx < apps.Count; idx++)
                {
                    var app = apps[idx];
                    if (!app.Aplicado)
                        continue;

                    var fechaProgramada = MedicamentoHistorialDisplayHelper.FormatearFechaProgramadaMedicamento(
                        med, app, idx, horarios);
                    var (fechaApl, horaApl) = MedicamentoHistorialDisplayHelper.FormatearFechaHoraAplicacion(
                        app.Aplicado, app.FechaHoraAplicacion, fechaProgramada);

                    resultado.Add(new HistorialMedicamentoPdfRow
                    {
                        AplicacionId = app.Id,
                        HospitalizacionProductoId = med.Id,
                        Aplicado = app.Aplicado,
                        Origen = "Control de Medicamentos",
                        Nombre = med.Producto?.NombreProducto ?? MedicamentoHistorialDisplayHelper.TextoNoRegistrado,
                        Indicaciones = MedicamentoHistorialDisplayHelper.NormalizarTexto(med.Indicaciones),
                        Via = MedicamentoHistorialDisplayHelper.NormalizarTexto(med.ViaAdministracion),
                        Frecuencia = MedicamentoHistorialDisplayHelper.NormalizarTexto(med.FrecuenciaAdministracion),
                        CantidadRegistrada = app.Cantidad,
                        CantidadAplicada = app.Aplicado ? app.Cantidad : 0,
                        Estado = app.Aplicado ? "APLICADO" : "PENDIENTE",
                        FechaAplicacion = fechaApl,
                        HoraAplicacion = horaApl,
                        AplicadoPor = MedicamentoHistorialDisplayHelper.ResolverAplicadoPor(
                            app.Aplicado, nombresUsuario, app.UsuarioAplica),
                        FechaRegistro = fechaProgramada
                    });
                }
            }

            var indiceInsumoPorId = new Dictionary<int, int>();
            var contadorPorInsumo = new Dictionary<int, int>();
            foreach (var app in aplicacionesInsumos.OrderBy(a => a.HospitalizacionInsumoDirectoId).ThenBy(a => a.Id))
            {
                int insumoId = app.HospitalizacionInsumoDirectoId;
                if (!contadorPorInsumo.ContainsKey(insumoId))
                    contadorPorInsumo[insumoId] = 0;
                indiceInsumoPorId[app.Id] = contadorPorInsumo[insumoId]++;
            }

            foreach (var app in aplicacionesInsumos)
            {
                if (!app.Aplicado)
                    continue;

                var insumo = app.HospitalizacionInsumoDirecto;
                int idx = indiceInsumoPorId[app.Id];
                var fechaProgramada = MedicamentoHistorialDisplayHelper.FormatearFechaProgramadaInsumo(
                    insumo, app, idx, app.Aplicado);
                var (fechaApl, horaApl) = MedicamentoHistorialDisplayHelper.FormatearFechaHoraAplicacion(
                    app.Aplicado, app.FechaHoraAplicacion, fechaProgramada);

                resultado.Add(new HistorialMedicamentoPdfRow
                {
                    AplicacionId = app.Id,
                    Aplicado = app.Aplicado,
                    Origen = "Control de Insumos",
                    Nombre = insumo.Producto?.NombreProducto ?? MedicamentoHistorialDisplayHelper.TextoNoRegistrado,
                    Indicaciones = MedicamentoHistorialDisplayHelper.NormalizarTexto(insumo.Indicaciones),
                    Via = MedicamentoHistorialDisplayHelper.NormalizarTexto(insumo.ViaAdministracion),
                    Frecuencia = MedicamentoHistorialDisplayHelper.NormalizarTexto(insumo.FrecuenciaAdministracion),
                    CantidadRegistrada = 1,
                    CantidadAplicada = app.Aplicado ? 1 : 0,
                    Estado = app.Aplicado ? "APLICADO" : "PENDIENTE",
                    FechaAplicacion = fechaApl,
                    HoraAplicacion = horaApl,
                    AplicadoPor = MedicamentoHistorialDisplayHelper.ResolverAplicadoPor(
                        app.Aplicado, nombresUsuario, app.UsuarioAplica),
                    FechaRegistro = fechaProgramada,
                    MotivoDevolucion = app.MotivoDevolucion
                });
            }

            return resultado
                .OrderBy(r => r.FechaRegistro)
                .ThenBy(r => r.Nombre)
                .ToList();
        }
    }
}
