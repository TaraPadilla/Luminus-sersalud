using System;
using System.Collections.Generic;
using System.Linq;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Database.Shared.SqlDataSeed
{
    /// <summary>
    /// Seeds clinical catalogs used by Hospitalización Detalles selects (dietas, exámenes, servicios)
    /// and minimal pending control rows so Control de Medicamentos/Insumos/Servicios modals work.
    /// </summary>
    public static class HospitalizacionClinicalCatalogSeedBootstrap
    {
        private static readonly int[] DefaultHospitalizacionIds = { 655, 698, 704, 705 };

        public static void EnsureClinicalCatalogs(Context context, ILogger logger = null)
        {
            var changed = false;
            changed |= EnsureCategoriasRecetas(context);
            changed |= EnsureRecetas(context);
            changed |= EnsureCategoriasLab(context);
            changed |= EnsureExamenesLab(context);
            changed |= EnsureCategoriasServicio(context);
            changed |= EnsureServicios(context);

            if (changed)
                context.SaveChanges();

            foreach (var hospId in DefaultHospitalizacionIds)
                EnsureControlDemoRows(context, hospId, logger);

            context.SaveChanges();
        }

        private static bool EnsureCategoriasRecetas(Context context)
        {
            if (context.CategoriasRecetas.Any())
                return false;

            context.CategoriasRecetas.Add(new CategoriaReceta
            {
                Id = 1,
                Nombre = "Hospitalización",
                Descripcion = "Dietas hospitalarias",
                Eliminado = false,
                FechaHoraCreada = DateTime.Now
            });
            return true;
        }

        private static bool EnsureRecetas(Context context)
        {
            if (context.Recetas.Any(r => !r.Eliminado))
                return false;

            var categoriaId = context.CategoriasRecetas.Select(c => c.Id).FirstOrDefault();
            if (categoriaId == 0)
                categoriaId = 1;

            var now = DateTime.Now;
            var dietas = new[]
            {
                "Dieta blanda",
                "Dieta líquida clara",
                "Dieta líquida completa",
                "Dieta normal",
                "Dieta hiposódica",
                "Dieta diabética",
                "NPO (ayuno)"
            };

            foreach (var nombre in dietas)
            {
                context.Recetas.Add(new Receta
                {
                    NombreReceta = nombre,
                    Ingredientes = nombre,
                    FechaHoraCreada = now,
                    PrecioCosto = 0,
                    PrecioVenta = 0,
                    Eliminado = false,
                    CategoriaId = categoriaId
                });
            }

            return true;
        }

        private static bool EnsureCategoriasLab(Context context)
        {
            if (context.CategoriaLabClinicos.Any(c => !c.Eliminado))
                return false;

            context.CategoriaLabClinicos.Add(new CategoriaLabClinico
            {
                Id = 1,
                Nombre = "Laboratorio general",
                Estado = "Activo",
                FechaCreacion = DateTime.Now,
                UltimoUsuarioModificado = "seed",
                Eliminado = false,
                Activo = true
            });
            return true;
        }

        private static bool EnsureExamenesLab(Context context)
        {
            if (context.ExamenLabClinicos.Any(e => !e.Eliminado))
                return false;

            var categoriaId = context.CategoriaLabClinicos
                .Where(c => !c.Eliminado)
                .Select(c => c.Id)
                .FirstOrDefault();
            if (categoriaId == 0)
                categoriaId = 1;

            var examenes = new (string Codigo, string Nombre, decimal Precio)[]
            {
                ("LAB-001", "Biometría hemática completa", 120),
                ("LAB-002", "Química sanguínea", 150),
                ("LAB-003", "Uroanálisis", 80),
                ("LAB-004", "Gasometría arterial", 200),
                ("LAB-005", "Tiempo de protrombina", 90)
            };

            foreach (var (codigo, nombre, precio) in examenes)
            {
                context.ExamenLabClinicos.Add(new ExamenLabClinico
                {
                    CategoriaLabClinicoId = categoriaId,
                    NombreExamen = nombre,
                    CodigoInterno = codigo,
                    Precio = precio,
                    PrecioB = precio,
                    PrecioC = precio,
                    PrecioCosto = 0,
                    FechaCreacion = DateTime.Now,
                    UltimaModificacion = "seed",
                    Eliminado = false,
                    Activo = true,
                    TipoDeExamen = "Laboratorio"
                });
            }

            return true;
        }

        private static bool EnsureCategoriasServicio(Context context)
        {
            if (context.CategoriasServicios.Any(c => !c.Eliminada))
                return false;

            context.CategoriasServicios.Add(new CategoriaServicio
            {
                Id = 1,
                NombreCategoria = "Hospitalización",
                Eliminada = false
            });
            return true;
        }

        private static bool EnsureServicios(Context context)
        {
            if (context.Servicios.Any(s => !s.Eliminado))
                return false;

            var categoriaId = context.CategoriasServicios
                .Where(c => !c.Eliminada)
                .Select(c => c.Id)
                .FirstOrDefault();
            if (categoriaId == 0)
                categoriaId = 1;

            var servicios = new[]
            {
                "Curación de herida",
                "Control de signos vitales",
                "Nebulización",
                "Oxigenoterapia",
                "Sondaje vesical"
            };

            var codigo = 1;
            foreach (var nombre in servicios)
            {
                context.Servicios.Add(new Servicio
                {
                    CodigoInterno = $"SRV-{codigo:000}",
                    CategoriaServicioId = categoriaId,
                    NombreServicio = nombre,
                    Precio_2 = 100,
                    Precio_3 = 100,
                    Precio_4 = 100,
                    Descripcion = nombre,
                    Eliminado = false
                });
                codigo++;
            }

            return true;
        }

        private static void EnsureControlDemoRows(Context context, int hospitalizacionId, ILogger logger)
        {
            if (!context.Hospitalizaciones.Any(h => h.Id == hospitalizacionId && !h.Eliminada))
                return;

            var userId = context.Users.Select(u => u.Id).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(userId))
                return;

            EnsureMedicamentoPendiente(context, hospitalizacionId, userId);
            EnsureServicioPendiente(context, hospitalizacionId, userId);
            EnsureInsumoDirectoPendiente(context, hospitalizacionId, userId);
        }

        private static void EnsureMedicamentoPendiente(Context context, int hospitalizacionId, string userId)
        {
            var tienePendiente = context.HospitalizacionesProductosAplicaciones
                .Any(a => !a.Eliminado
                          && !a.Aplicado
                          && a.HospitalizacionProducto.HospitalizacionId == hospitalizacionId
                          && !a.HospitalizacionProducto.Eliminado
                          && a.HospitalizacionProducto.Producto.TipoProductoId == 1);

            if (tienePendiente)
                return;

            var producto = context.Productos
                .Where(p => !p.Eliminado && p.TipoProductoId == 1 && p.NombreProducto != null)
                .OrderBy(p => p.Id)
                .FirstOrDefault();
            if (producto == null)
                return;

            var unidadId = context.UnidadesMedidaVenta.Select(u => u.Id).FirstOrDefault();
            if (unidadId == 0)
                return;

            var hp = new HospitalizacionProducto
            {
                HospitalizacionId = hospitalizacionId,
                ProductoId = producto.Id,
                Cantidad = 1,
                Indicaciones = "Control demo - medicamento pendiente",
                ViaAdministracion = "IV",
                FrecuenciaAdministracion = "C/8h",
                Eliminado = false,
                PrecioValor = 0,
                UnidadMedidaVentaId = unidadId
            };
            context.HospitalizacionesProductos.Add(hp);
            context.SaveChanges();

            context.HospitalizacionesProductosAplicaciones.Add(new HospitalizacionProductoAplicacion
            {
                HospitalizacionProductoId = hp.Id,
                Cantidad = 1,
                Aplicado = false,
                UsuarioCreaId = userId,
                Eliminado = false
            });
        }

        private static void EnsureServicioPendiente(Context context, int hospitalizacionId, string userId)
        {
            var tienePendiente = context.HospitalizacionesServicios
                .Any(s => s.HospitalizacionId == hospitalizacionId
                          && !s.Eliminado
                          && !s.Aplicado);

            if (tienePendiente)
                return;

            var servicio = context.Servicios
                .Where(s => !s.Eliminado && s.NombreServicio != null)
                .OrderBy(s => s.Id)
                .FirstOrDefault();
            if (servicio == null)
                return;

            context.HospitalizacionesServicios.Add(new HospitalizacionServicio
            {
                HospitalizacionId = hospitalizacionId,
                ServicioId = servicio.Id,
                Cantidad = 1,
                Precio = servicio.Precio_2,
                Eliminado = false,
                Aplicado = false,
                UsuarioCreaId = userId
            });
        }

        private static void EnsureInsumoDirectoPendiente(Context context, int hospitalizacionId, string userId)
        {
            var tienePendiente = context.HospitalizacionInsumosDirectosAplicaciones
                .Any(a => !a.Aplicado
                          && a.HospitalizacionInsumoDirecto.HospitalizacionId == hospitalizacionId
                          && !a.HospitalizacionInsumoDirecto.Eliminado);

            if (tienePendiente)
                return;

            var producto = context.Productos
                .Where(p => !p.Eliminado && p.TipoProductoId == 2 && p.NombreProducto != null)
                .OrderBy(p => p.Id)
                .FirstOrDefault()
                ?? context.Productos
                    .Where(p => !p.Eliminado && p.TipoProductoId != 1 && p.NombreProducto != null)
                    .OrderBy(p => p.Id)
                    .FirstOrDefault();

            if (producto == null)
                return;

            var unidadId = context.UnidadesMedidaVenta.Select(u => u.Id).FirstOrDefault();
            if (unidadId == 0)
                return;

            var insumo = new HospitalizacionInsumoDirecto
            {
                HospitalizacionId = hospitalizacionId,
                ProductoId = producto.Id,
                UnidadMedidaVentaId = unidadId,
                Cantidad = 1,
                Indicaciones = "Control demo - insumo pendiente",
                ViaAdministracion = "-",
                FrecuenciaAdministracion = "UNICA",
                UsuarioCreaId = userId,
                FechaCreacion = DateTime.Now,
                Eliminado = false
            };
            context.HospitalizacionInsumosDirectos.Add(insumo);
            context.SaveChanges();

            context.HospitalizacionInsumosDirectosAplicaciones.Add(new HospitalizacionInsumoDirectoAplicacion
            {
                HospitalizacionInsumoDirectoId = insumo.Id,
                Cantidad = 1,
                Aplicado = false,
                UsuarioCreaId = userId
            });
        }
    }
}
