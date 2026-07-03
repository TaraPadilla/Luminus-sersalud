using System.Collections.Generic;
using System.Linq;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Database.Shared.SqlDataSeed
{
    /// <summary>
    /// EF HasData seeds are not applied when the database is created outside migrations.
    /// This bootstrap backfills required lookup rows used by FK constraints across the app.
    /// </summary>
    public static class CatalogLookupSeedBootstrap
    {
        public static void EnsureRequiredLookups(Context context, ILogger logger = null)
        {
            var tablesToReset = new HashSet<string>();
            var changed = false;

            changed |= EnsureEstadosPaciente(context, tablesToReset);
            changed |= EnsureTiposPaciente(context, tablesToReset);
            changed |= EnsureAccionesPaciente(context, tablesToReset);
            changed |= EnsureFormasPago(context, tablesToReset);
            changed |= EnsureEstadosPagoConsulta(context, tablesToReset);
            changed |= EnsureEstadosExamen(context, tablesToReset);
            changed |= EnsureTiposMovimientoProducto(context, tablesToReset);
            changed |= EnsureAmbientes(context, tablesToReset);
            changed |= EnsureTiposProducto(context, tablesToReset);
            changed |= EnsureTiposBodega(context, tablesToReset);
            changed |= EnsurePrecios(context, tablesToReset);
            changed |= EnsureTiposCompra(context, tablesToReset);
            changed |= EnsureEstadosRecepcion(context, tablesToReset);
            changed |= EnsureMonedas(context, tablesToReset);
            changed |= EnsureSexos(context, tablesToReset);
            changed |= EnsureEspecialidades(context, tablesToReset);
            changed |= EnsureCompraTiposDocumento(context, tablesToReset);
            changed |= EnsureTipificacionesComunicacion(context, tablesToReset);
            changed |= EnsureTiposPatologia(context, tablesToReset);
            changed |= EnsureFasesTratamiento(context, tablesToReset);
            changed |= EnsureBancos(context, tablesToReset);
            changed |= EnsureEstadosHabitacion(context, tablesToReset);
            changed |= EnsureEstadosTraslados(context, tablesToReset);
            changed |= EnsureTiposRedSocial(context, tablesToReset);
            changed |= EnsureTiposCuenta(context, tablesToReset);
            changed |= EnsureNomenclaturas(context, tablesToReset);
            changed |= EnsureEstadosEnvio(context, tablesToReset);

            if (!changed)
                return;

            context.SaveChanges();

            foreach (var table in tablesToReset)
                ResetSequence(context, table);

            logger?.LogInformation("Seeded {Count} catalog lookup table(s).", tablesToReset.Count);
        }

        private static bool EnsureTiposProducto(Context context, HashSet<string> tablesToReset)
        {
            var defaults = new[]
            {
                (1, "Medicamentos"),
                (2, "Insumos Médicos"),
                (3, "Equipos Médicos"),
                (4, "Equipos Quirúrgicos"),
                (5, "Suministros")
            };

            var added = false;
            foreach (var (id, nombre) in defaults)
            {
                var existing = context.TipoProductos.FirstOrDefault(x => x.Id == id);
                if (existing != null)
                {
                    if (existing.Eliminado || existing.NombreTipoProducto != nombre)
                    {
                        existing.Eliminado = false;
                        existing.NombreTipoProducto = nombre;
                        added = true;
                    }
                    continue;
                }

                context.TipoProductos.Add(new TipoProducto
                {
                    Id = id,
                    NombreTipoProducto = nombre,
                    Eliminado = false
                });
                added = true;
            }

            if (added) tablesToReset.Add("TipoProductos");
            return added;
        }

        private static bool EnsureTiposBodega(Context context, HashSet<string> tablesToReset)
        {
            var defaults = new[]
            {
                (1, "FARMACIA"),
                (2, "CLINICA"),
                (3, "BODEGA"),
                (4, "LABORATORIO"),
                (5, "HOSPITAL"),
                (6, "GLOBAL")
            };

            var added = EnsureRows(context.TipoBodegas, defaults, (id, nombre) => new TipoBodega
            {
                Id = id,
                DescripcionBodega = nombre
            });

            if (added) tablesToReset.Add("TipoBodegas");
            return added;
        }

        private static bool EnsurePrecios(Context context, HashSet<string> tablesToReset)
        {
            var added = false;
            foreach (var (id, nombre) in new[] { (1, "Normal"), (2, "VIP") })
            {
                if (context.Precios.Any(x => x.Id == id))
                    continue;

                context.Precios.Add(new Precio { Id = id, NombrePrecio = nombre, Eliminado = false });
                added = true;
            }

            if (added) tablesToReset.Add("Precios");
            return added;
        }

        private static bool EnsureTiposCompra(Context context, HashSet<string> tablesToReset)
        {
            var defaults = new[] { (1, "CONTADO"), (2, "CREDITO") };
            var added = EnsureRows(context.TipoCompra, defaults, (id, tipo) => new TipoCompra { Id = id, Tipo = tipo });
            if (added) tablesToReset.Add("TipoCompra");
            return added;
        }

        private static bool EnsureEstadosRecepcion(Context context, HashSet<string> tablesToReset)
        {
            var defaults = new[] { (1, "No ingresado"), (2, "Ingresado") };
            var added = EnsureRows(context.EstadoRecepciones, defaults, (id, estado) => new EstadoRecepcion
            {
                Id = id,
                Estado = estado
            });
            if (added) tablesToReset.Add("EstadoRecepciones");
            return added;
        }

        private static bool EnsureMonedas(Context context, HashSet<string> tablesToReset)
        {
            var defaults = new[] { (1, "Dólar"), (2, "Quetzal") };
            var added = EnsureRows(context.Monedas, defaults, (id, nombre) => new Moneda { Id = id, NombreMoneda = nombre });
            if (added) tablesToReset.Add("Monedas");
            return added;
        }

        private static bool EnsureSexos(Context context, HashSet<string> tablesToReset)
        {
            var defaults = new[] { (1, "Masculino"), (2, "Femenino"), (3, "Otro") };
            var added = EnsureRows(context.Sexo, defaults, (id, desc) => new Sexo { Id = id, DescripcionSexo = desc });
            if (added) tablesToReset.Add("Sexo");
            return added;
        }

        private static bool EnsureEspecialidades(Context context, HashSet<string> tablesToReset)
        {
            var defaults = new[]
            {
                (1, "Medicina general"),
                (2, "Medicina estética"),
                (3, "Ginecología y Obstetricia"),
                (4, "Pediatría"),
                (5, "Cirugía"),
                (6, "Traumatología"),
                (7, "Medicina interna"),
                (8, "Laboratorio clínico"),
                (9, "Cardiología"),
                (10, "Nutrición")
            };

            var added = EnsureRows(context.Especialidad, defaults, (id, nombre) => new Especialidad
            {
                Id = id,
                NombreEspecialidad = nombre
            });
            if (added) tablesToReset.Add("Especialidad");
            return added;
        }

        private static bool EnsureCompraTiposDocumento(Context context, HashSet<string> tablesToReset)
        {
            var defaults = new[] { (1, "Orden de compra"), (2, "Compra") };
            var added = EnsureRows(context.CompraTiposDocumento, defaults, (id, nombre) => new CompraTipoDocumento
            {
                Id = id,
                NombreTipoDocumento = nombre
            });
            if (added) tablesToReset.Add("CompraTiposDocumento");
            return added;
        }

        private static bool EnsureTipificacionesComunicacion(Context context, HashSet<string> tablesToReset)
        {
            var defaults = new[]
            {
                (1, "Contactado"),
                (2, "Recontactado"),
                (3, "Nuevo ingreso")
            };
            var added = EnsureRows(context.TipificacionesComunicacion, defaults, (id, nombre) => new TipificacionComunicacion
            {
                Id = id,
                NombreTipificacion = nombre
            });
            if (added) tablesToReset.Add("TipificacionesComunicacion");
            return added;
        }

        private static bool EnsureTiposPatologia(Context context, HashSet<string> tablesToReset)
        {
            var defaults = new (int Id, string Tipo, bool VerInput)[]
            {
                (1, "Diabetes", false),
                (2, "Cardiopatías", false),
                (3, "Neoplasias", false),
                (4, "Epilepsia", false),
                (5, "Malformaciones", false),
                (6, "SIDA", false),
                (7, "Enfermedades renales", false),
                (8, "Hepatitis", false),
                (9, "Artritis", false),
                (10, "Otra", true),
                (11, "Aparentemente sano", false)
            };

            var added = false;
            foreach (var item in defaults)
            {
                if (context.TipoPatologias.Any(x => x.Id == item.Id))
                    continue;

                context.TipoPatologias.Add(new TipoPatologia
                {
                    Id = item.Id,
                    Tipo = item.Tipo,
                    VerInputDescripcion = item.VerInput
                });
                added = true;
            }

            if (added) tablesToReset.Add("TipoPatologias");
            return added;
        }

        private static bool EnsureFasesTratamiento(Context context, HashSet<string> tablesToReset)
        {
            var defaults = new (int Id, string Nombre, decimal Valor)[]
            {
                (1, "Adelgazamiento", 2650),
                (2, "Mantenimiento 1", 1600),
                (3, "Mantenimiento 2", 1600),
                (4, "Mantenimiento 3", 1600)
            };

            var added = false;
            foreach (var item in defaults)
            {
                if (context.FasesTratamiento.Any(x => x.Id == item.Id))
                    continue;

                context.FasesTratamiento.Add(new FaseTratamiento
                {
                    Id = item.Id,
                    NombreFase = item.Nombre,
                    Valor = item.Valor
                });
                added = true;
            }

            if (added) tablesToReset.Add("FasesTratamiento");
            return added;
        }

        private static bool EnsureBancos(Context context, HashSet<string> tablesToReset)
        {
            var defaults = new[]
            {
                (1, "BANCO AGROMERCANTIL DE GUATEMALA S.A."),
                (2, "VIVIBANCO S.A."),
                (3, "BANCO G&T CONTINENTAL S.A."),
                (4, "BANCO DE AMERICA CENTRAL S.A."),
                (5, "BANCO FICOHSA GUATEMALA S.A."),
                (6, "BANCO INTERNACIONAL S.A."),
                (7, "BANCO DE DESARROLLO RURAL S.A."),
                (8, "BANCO INDUSTRIAL S.A."),
                (9, "BANCO DE CREDITO S.A."),
                (10, "BANCO PROMERICA S.A."),
                (11, "CITIBANK N.A SUCURSAL GUATEMALA"),
                (12, "EL CREDITO HIPOTECARIO NACIONAL DE GUATEMALA"),
                (13, "BANCO DE LOS TRABAJADORES"),
                (14, "BANCO INMOBILIARIO S.A.")
            };

            var added = false;
            foreach (var (id, nombre) in defaults)
            {
                if (context.Bancos.Any(x => x.Id == id))
                    continue;

                context.Bancos.Add(new Banco { Id = id, Nombre = nombre, Eliminado = false });
                added = true;
            }

            if (added) tablesToReset.Add("Bancos");
            return added;
        }

        private static bool EnsureEstadosHabitacion(Context context, HashSet<string> tablesToReset)
        {
            var defaults = new[]
            {
                (1, "Disponible"),
                (2, "Ocupada"),
                (3, "En limpieza")
            };
            var added = EnsureRows(context.EstadosHabitacion, defaults, (id, nombre) => new EstadoHabitacion
            {
                Id = id,
                NombreEstado = nombre
            });
            if (added) tablesToReset.Add("EstadosHabitacion");
            return added;
        }

        private static bool EnsureEstadosTraslados(Context context, HashSet<string> tablesToReset)
        {
            var defaults = new[]
            {
                (1, "Aceptado"),
                (2, "Denegado"),
                (3, "Con problema"),
                (4, "Faltantes"),
                (5, "En tránsito")
            };
            var added = EnsureRows(context.EstadoTraslados, defaults, (id, desc) => new EstadoTraslados
            {
                Id = id,
                DescripcionEstado = desc
            });
            if (added) tablesToReset.Add("EstadoTraslados");
            return added;
        }

        private static bool EnsureTiposRedSocial(Context context, HashSet<string> tablesToReset)
        {
            var defaults = new[]
            {
                (1, "Facebook"),
                (2, "Instagram"),
                (3, "Tiktok"),
                (4, "WhatsApp")
            };
            var added = false;
            foreach (var (id, nombre) in defaults)
            {
                if (context.TiposRedSocial.Any(x => x.Id == id))
                    continue;

                context.TiposRedSocial.Add(new TipoRedSocial
                {
                    Id = id,
                    NombreRedSocial = nombre,
                    Eliminado = false
                });
                added = true;
            }
            if (added) tablesToReset.Add("TiposRedSocial");
            return added;
        }

        private static bool EnsureTiposCuenta(Context context, HashSet<string> tablesToReset)
        {
            var defaults = new[] { (1, "Monetaria"), (2, "Ahorro"), (3, "Inversión") };
            var added = EnsureRows(context.TipoCuenta, defaults, (id, nombre) => new TipoCuenta { Id = id, Nombre = nombre });
            if (added) tablesToReset.Add("TipoCuenta");
            return added;
        }

        private static bool EnsureNomenclaturas(Context context, HashSet<string> tablesToReset)
        {
            var defaults = new[] { (1, "Ingreso"), (2, "Egreso") };
            var added = EnsureRows(context.Nomenclaturas, defaults, (id, nombre) => new Nomenclatura { Id = id, Nombre = nombre });
            if (added) tablesToReset.Add("Nomenclaturas");
            return added;
        }

        private static bool EnsureEstadosEnvio(Context context, HashSet<string> tablesToReset)
        {
            var defaults = new[]
            {
                (1, "Pendiente"),
                (2, "En ruta"),
                (3, "Entregado"),
                (4, "Rechazado"),
                (1002, "Liquidado")
            };
            var added = EnsureRows(context.EstadosEnvio, defaults, (id, estado) => new EstadosEnvio
            {
                Id = id,
                Estado = estado
            });
            if (added) tablesToReset.Add("EstadosEnvio");
            return added;
        }

        private static bool EnsureEstadosPaciente(Context context, HashSet<string> tablesToReset)
        {
            var defaults = new[] { (1, "Activo"), (2, "Inactivo") };
            var added = EnsureRows(context.EstadosPaciente, defaults, (id, nombre) => new EstadoPaciente
            {
                Id = id,
                NombreEstado = nombre
            });
            if (added) tablesToReset.Add("EstadosPaciente");
            return added;
        }

        private static bool EnsureTiposPaciente(Context context, HashSet<string> tablesToReset)
        {
            var defaults = new[] { (1, "Nuevo"), (2, "Retomante") };
            var added = EnsureRows(context.TiposPacientes, defaults, (id, nombre) => new TipoPaciente
            {
                Id = id,
                NombreTipo = nombre
            });
            if (added) tablesToReset.Add("TiposPacientes");
            return added;
        }

        private static bool EnsureAccionesPaciente(Context context, HashSet<string> tablesToReset)
        {
            var defaults = new[] { (1, "Registro"), (2, "Retiro") };
            var added = EnsureRows(context.AccionesPaciente, defaults, (id, nombre) => new AccionPaciente
            {
                Id = id,
                NombreAccion = nombre
            });
            if (added) tablesToReset.Add("AccionesPaciente");
            return added;
        }

        private static bool EnsureFormasPago(Context context, HashSet<string> tablesToReset)
        {
            var defaults = new[]
            {
                (1, "Efectivo"),
                (2, "Tarjeta de Débito"),
                (3, "Tarjeta de Crédito"),
                (4, "Cheques"),
                (5, "Transferencia"),
                (6, "Visa Link"),
                (7, "Visa Net"),
                (8, "Credito")
            };

            var added = false;
            foreach (var (id, nombre) in defaults)
            {
                if (context.FormaPagos.Any(x => x.Id == id))
                    continue;

                context.FormaPagos.Add(new FormaPago
                {
                    Id = id,
                    NombreFormaPago = nombre,
                    Eliminada = false,
                    PorcentajeCobroAdicional = 0
                });
                added = true;
            }

            if (added) tablesToReset.Add("FormaPagos");
            return added;
        }

        private static bool EnsureEstadosPagoConsulta(Context context, HashSet<string> tablesToReset)
        {
            var defaults = new[] { (1, "Pagado"), (2, "Pendiente"), (3, "Cortesía") };
            var added = EnsureRows(context.EstadoPagoConsultas, defaults, (id, estado) => new EstadoPagoConsulta
            {
                Id = id,
                Estado = estado
            });
            if (added) tablesToReset.Add("EstadoPagoConsultas");
            return added;
        }

        private static bool EnsureEstadosExamen(Context context, HashSet<string> tablesToReset)
        {
            var defaults = new[]
            {
                (1, "Solicitados"),
                (2, "En proceso"),
                (3, "Cancelados"),
                (4, "Revisión"),
                (5, "Finalizados")
            };
            var added = EnsureRows(context.EstadoExamenes, defaults, (id, nombre) => new EstadoExamen
            {
                Id = id,
                Nombre = nombre
            });
            if (added) tablesToReset.Add("EstadoExamenes");
            return added;
        }

        private static bool EnsureTiposMovimientoProducto(Context context, HashSet<string> tablesToReset)
        {
            var defaults = new[]
            {
                (1, "Entrada"),
                (2, "Salida"),
                (3, "Salida - Venta"),
                (4, "Entrada - Compra")
            };
            var added = EnsureRows(context.TipoMovimientoProducto, defaults, (id, nombre) => new TipoMovimientoProducto
            {
                Id = id,
                Nombre = nombre
            });
            if (added) tablesToReset.Add("TipoMovimientoProducto");
            return added;
        }

        private static bool EnsureAmbientes(Context context, HashSet<string> tablesToReset)
        {
            var defaults = new[]
            {
                (1, "Farmacia"),
                (2, "Clinica"),
                (3, "Hospital"),
                (4, "Laboratorio"),
                (5, "Bodega"),
                (6, "Global")
            };
            var added = EnsureRows(context.Ambientes, defaults, (id, nombre) => new Ambiente
            {
                Id = id,
                NombreAmbiente = nombre
            });
            if (added) tablesToReset.Add("Ambientes");
            return added;
        }

        private static bool EnsureRows<T>(
            DbSet<T> set,
            (int Id, string Name)[] defaults,
            System.Func<int, string, T> factory) where T : class
        {
            var added = false;
            foreach (var (id, name) in defaults)
            {
                if (set.Any(x => EF.Property<int>(x, "Id") == id))
                    continue;

                set.Add(factory(id, name));
                added = true;
            }

            return added;
        }

        private static void ResetSequence(Context context, string tableName, string columnName = "Id")
        {
            context.Database.ExecuteSqlRaw(
                $@"SELECT setval(
                    pg_get_serial_sequence('""{tableName}""', '{columnName}'),
                    COALESCE((SELECT MAX(""{columnName}"") FROM ""{tableName}""), 1),
                    true)");
        }
    }
}
