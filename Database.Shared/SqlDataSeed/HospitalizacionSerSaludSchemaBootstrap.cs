using System;
using Microsoft.EntityFrameworkCore;

namespace Database.Shared.SqlDataSeed
{
    /// <summary>
    /// Ensures SerSalud hospitalization schema objects exist (tables/columns not covered by EF EnsureCreated).
    /// </summary>
    public static class HospitalizacionSerSaludSchemaBootstrap
    {
        private static readonly string[] Scripts =
        {
            """
            CREATE TABLE IF NOT EXISTS public."RegistrosAnestesia"
            (
                "Id"                 SERIAL PRIMARY KEY,
                "HospitalizacionId"  INTEGER NOT NULL,
                "FechaRegistro"      TIMESTAMP NOT NULL DEFAULT NOW(),
                "FechaActualizacion" TIMESTAMP NULL,
                "UserId"             VARCHAR(450) NULL,
                "DatosJson"          TEXT NULL,
                CONSTRAINT "FK_RegistrosAnestesia_Hospitalizaciones"
                    FOREIGN KEY ("HospitalizacionId")
                    REFERENCES public."Hospitalizaciones" ("Id")
                    ON DELETE CASCADE
            );
            """,
            """
            CREATE INDEX IF NOT EXISTS "IX_RegistrosAnestesia_HospitalizacionId"
                ON public."RegistrosAnestesia" ("HospitalizacionId");
            """,
            """
            ALTER TABLE public."NotaMedica2"
                ADD COLUMN IF NOT EXISTS "TipoNota" VARCHAR(30) NULL;
            """,
            """
            ALTER TABLE public."NotaEnfermeria2"
                ADD COLUMN IF NOT EXISTS "TipoNota" VARCHAR(30) NULL;
            """,
            """
            ALTER TABLE public."ListasChequeo"
                ADD COLUMN IF NOT EXISTS "MedicoTratante" VARCHAR(200) NULL;
            """,
            """
            ALTER TABLE public."ListasChequeo"
                ADD COLUMN IF NOT EXISTS "CP_ApellidoPacienteCirujano" VARCHAR(150) NULL;
            """,
            """
            ALTER TABLE public."ListasChequeo"
                ADD COLUMN IF NOT EXISTS "CP_FechaNacCirujano" DATE NULL;
            """
        };

        public static void EnsureSchema(DbContext context, Action<Exception, string> onError = null)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            foreach (var sql in Scripts)
            {
                try
                {
                    context.Database.ExecuteSqlRaw(sql);
                }
                catch (Exception ex)
                {
                    onError?.Invoke(ex, sql);
                }
            }
        }
    }
}
