using System;
using System.Collections.Generic;
using System.Linq;
using Database.Shared.Enumeraciones;
using Database.Shared.Models;

namespace farmamest.Utilidades
{
    public static class HospitalizacionCargosHelper
    {
        public static string ObtenerTipoProductoCuenta(int? tipoProductoId)
        {
            return (tipoProductoId ?? 0) switch
            {
                (int)TipoProductoEnum.Medicamentos => "Medicamentos",
                (int)TipoProductoEnum.InsumosMedicos => "Insumos médicos",
                (int)TipoProductoEnum.EquiposMedicos => "Equipos médicos",
                (int)TipoProductoEnum.EquiposQuirurgicos => "Equipos quirúrgicos",
                (int)TipoProductoEnum.Suministros => "Suministros",
                _ => "Sin tipo"
            };
        }

        public static IEnumerable<HospitalizacionProductoAplicacion> AplicacionesVigentes(
            IEnumerable<HospitalizacionProductoAplicacion> aplicaciones)
        {
            if (aplicaciones == null)
                return Enumerable.Empty<HospitalizacionProductoAplicacion>();

            return aplicaciones.Where(a => a.Aplicado && !a.Eliminado);
        }

        public static IEnumerable<HospitalizacionInsumoDirectoAplicacion> AplicacionesVigentesInsumoDirecto(
            IEnumerable<HospitalizacionInsumoDirectoAplicacion> aplicaciones)
        {
            if (aplicaciones == null)
                return Enumerable.Empty<HospitalizacionInsumoDirectoAplicacion>();

            return aplicaciones.Where(a => a.Aplicado);
        }

        public static string FormatearFechaAplicacionProducto(
            IEnumerable<HospitalizacionProductoAplicacion> aplicacionesVigentes,
            string fechaManualFallback = null)
        {
            var fecha = aplicacionesVigentes?
                .Where(a => a.FechaHoraAplicacion.HasValue)
                .OrderBy(a => a.FechaHoraAplicacion)
                .Select(a => a.FechaHoraAplicacion)
                .FirstOrDefault();

            if (fecha.HasValue)
                return fecha.Value.ToString("dd/MM/yyyy");

            if (!string.IsNullOrWhiteSpace(fechaManualFallback))
                return fechaManualFallback;

            return string.Empty;
        }

        public static string FormatearFechaAplicacionInsumoDirecto(
            IEnumerable<HospitalizacionInsumoDirectoAplicacion> aplicacionesVigentes,
            string fechaManualFallback = null)
        {
            var fecha = aplicacionesVigentes?
                .Where(a => a.FechaHoraAplicacion.HasValue)
                .OrderBy(a => a.FechaHoraAplicacion)
                .Select(a => a.FechaHoraAplicacion)
                .FirstOrDefault();

            if (fecha.HasValue)
                return fecha.Value.ToString("dd/MM/yyyy");

            if (!string.IsNullOrWhiteSpace(fechaManualFallback))
                return fechaManualFallback;

            return string.Empty;
        }

        public static int OrdenSeccionProductoCuenta(string tipo)
        {
            if (string.IsNullOrWhiteSpace(tipo))
                return 99;

            return tipo.Trim().ToUpperInvariant() switch
            {
                "HABITACIÓN" or "HABITACION" => 1,
                "MEDICAMENTOS" => 2,
                "INSUMOS MÉDICOS" or "INSUMOS MEDICOS" or "INSUMOS" => 3,
                "EQUIPOS MÉDICOS" or "EQUIPOS MEDICOS" => 4,
                "EQUIPOS QUIRÚRGICOS" or "EQUIPOS QUIRURGICOS" => 5,
                "SUMINISTROS" => 6,
                "SERVICIOS" or "SERVICIO" => 7,
                "DIETAS" => 8,
                "EMERGENCIA" => 9,
                "DEPÓSITOS (RESTA)" or "DEPOSITOS (RESTA)" => 10,
                _ => 50
            };
        }
    }
}
