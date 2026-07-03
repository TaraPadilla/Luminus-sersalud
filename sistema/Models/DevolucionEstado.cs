using System.Collections.Generic;

namespace sistema.Models
{

    public static class DevolucionEstado
    {
        public const int Borrador = 1;
        public const int Autorizado = 2;
        public const int EnProceso = 3;
        public const int Completado = 4;
        public const int Rechazado = 5;

        public static string ObtenerNombre(int estado) => estado switch
        {
            Borrador => "Borrador",
            Autorizado => "Autorizado",
            EnProceso => "En Proceso",
            Completado => "Completado",
            Rechazado => "Rechazado",
        };

        public static IEnumerable<(int Id, string Nombre)> ObtenerTodos()
        {
            yield return (Borrador, "Borrador");
            yield return (Autorizado, "Autorizado");
            yield return (EnProceso, "En Proceso");
            yield return (Completado, "Completado");
            yield return (Rechazado, "Rechazado");
        }


        public static string ObtenerClaseCSS(int estado) => estado switch
        {
            Borrador => "secondary",
            Autorizado => "primary",
            EnProceso => "info",
            Rechazado => "danger",
            _ => "light"
        };
    }
}