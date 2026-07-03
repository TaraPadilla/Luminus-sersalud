using System;
using System.Collections.Generic;
using System.Linq;
using Database.Shared.Enumeraciones;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Database.Shared.SqlDataSeed
{
    /// <summary>
    /// Sample ventas/citas for validating Reporte General de Ventas (Feb 2026 demo range).
    /// Idempotent: skips when enough rows with NoComprobante prefix DEMO-RPT- already exist.
    /// </summary>
    public static class ReporteVentasDemoSeedBootstrap
    {
        private const string Marker = "DEMO-RPT-";
        private const int TargetVentas = 12;

        public static void EnsureDemoVentas(Context context, ILogger logger = null)
        {
            var existentes = context.Ventas
                .Count(v => v.NoComprobante != null && v.NoComprobante.StartsWith(Marker));
            if (existentes >= TargetVentas)
                return;

            var sucursal = context.Sucursales.FirstOrDefault(s => !s.Eliminado);
            if (sucursal == null)
            {
                sucursal = new Sucursal
                {
                    NombreSucursal = "Sucursal Demo Reportes",
                    Direccion = "Ciudad de Guatemala",
                    Horario = "08:00-18:00",
                    ClinicaHabilitada = true,
                    FarmaciaHabilitada = true,
                    LaboratorioHabilitado = true
                };
                context.Sucursales.Add(sucursal);
                context.SaveChanges();
            }

            var recepcionista = ObtenerOCrearEmpleado(context, sucursal.Id, "99990001",
                "María", "Recepción Demo", null, "Recepcionista");
            var medGine = ObtenerOCrearEmpleado(context, sucursal.Id, "99990002",
                "Ana", "Gómez Demo", 3, "Médico");
            var medGeneral = ObtenerOCrearEmpleado(context, sucursal.Id, "99990003",
                "Carlos", "López Demo", 1, "Médico");
            var medPedia = ObtenerOCrearEmpleado(context, sucursal.Id, "99990004",
                "Laura", "Ruiz Demo", 4, "Médico");

            var pacGine = ObtenerOCrearPaciente(context, "DEMO Paciente Ginecología", "5555-0001");
            var pacGeneral = ObtenerOCrearPaciente(context, "DEMO Paciente Medicina General", "5555-0002");
            var pacPedia = ObtenerOCrearPaciente(context, "DEMO Paciente Pediatría", "5555-0003");
            var pacFarm = ObtenerOCrearPaciente(context, "DEMO Paciente Farmacia", "5555-0004");
            var pacLab = ObtenerOCrearPaciente(context, "DEMO Paciente Laboratorio", "5555-0005");

            var prodAinezyl = ObtenerOCrearProducto(context, "DEMO AINEZYL - DICLOFENACO GOTAS",
                "DEMO-AIN-001", 75m, 45m, (int)AmbienteEnum.Farmacia);
            var prodAmbrox = ObtenerOCrearProducto(context, "DEMO AMBROXOL COMPOSITUM",
                "DEMO-AMB-002", 125m, 80m, (int)AmbienteEnum.Farmacia);
            var prodPenic = ObtenerOCrearProducto(context, "DEMO penicilina benzatinica",
                "DEMO-PEN-003", 350m, 220m, (int)AmbienteEnum.Farmacia);
            var prodInsumo = ObtenerOCrearProducto(context, "DEMO Insumo Gasas Estériles",
                "DEMO-INS-004", 0m, 12m, (int)AmbienteEnum.Clinica);
            AsegurarInventarioCosto(context, prodAinezyl.Id, 45m, (int)AmbienteEnum.Farmacia);
            AsegurarInventarioCosto(context, prodAmbrox.Id, 80m, (int)AmbienteEnum.Farmacia);
            AsegurarInventarioCosto(context, prodPenic.Id, 220m, (int)AmbienteEnum.Farmacia);
            AsegurarInventarioCosto(context, prodInsumo.Id, 12m, (int)AmbienteEnum.Clinica);

            var svcGine = ObtenerOCrearServicio(context, "1-CONSULTA GINECOLOGICA Y OBSTETRICA", 350m);
            var svcGeneral = ObtenerOCrearServicio(context, "2-CONSULTA MEDICA GENERAL", 150m);
            var svcPedia = ObtenerOCrearServicio(context, "3-CONSULTA PEDIATRICA", 200m);
            var svcCuracion = ObtenerOCrearServicio(context, "CURACION CON CREMA", 125m);
            var svcPapani = ObtenerOCrearServicio(context, "PAPANICOLAU", 180m);
            AsegurarInsumoServicio(context, svcCuracion.Id, prodInsumo.Id, 2m);

            var catLab = context.CategoriaLabClinicos.FirstOrDefault(c => !c.Eliminado)
                ?? CrearCategoriaLab(context);
            var examHemato = ObtenerOCrearExamen(context, catLab.Id, "DEMO HEMATOLOGÍA COMPLETA",
                "DEMO-HEM", 100m, 35m);
            var examGlucosa = ObtenerOCrearExamen(context, catLab.Id, "DEMO GLUCOSA PRE-PRANDIAL",
                "DEMO-GLU", 35m, 12m);
            var examOrina = ObtenerOCrearExamen(context, catLab.Id, "DEMO ORINA COMPLETA",
                "DEMO-ORI", 35m, 10m);
            var examAbdomen = ObtenerOCrearExamen(context, catLab.Id, "DEMO ABDOMEN COMPLETO",
                "DEMO-ABD", 280m, 95m);

            var escenarios = new List<DemoVentaEscenario>
            {
                new("001", new DateTime(2026, 2, 2, 9, 0, 0), (int)AmbienteEnum.Clinica, pacGine.Id,
                    medGine, svcGine, 350m, new[] { (svcGine.Id, "S", 1, 350m) }),
                new("002", new DateTime(2026, 2, 2, 10, 30, 0), (int)AmbienteEnum.Clinica, pacGeneral.Id,
                    medGeneral, svcGeneral, 150m, new[] { (svcGeneral.Id, "S", 1, 150m) }),
                new("003", new DateTime(2026, 2, 3, 8, 15, 0), (int)AmbienteEnum.Clinica, pacPedia.Id,
                    medPedia, svcPedia, 200m, new[] { (svcPedia.Id, "S", 1, 200m) }),
                new("004", new DateTime(2026, 2, 3, 11, 0, 0), (int)AmbienteEnum.Clinica, pacGine.Id,
                    medGine, svcCuracion, 125m, new[] { (svcCuracion.Id, "S", 1, 125m) }),
                new("005", new DateTime(2026, 2, 3, 14, 0, 0), (int)AmbienteEnum.Clinica, pacGeneral.Id,
                    medGeneral, svcPapani, 180m, new[] { (svcPapani.Id, "S", 1, 180m) }),
                new("006", new DateTime(2026, 2, 4, 9, 30, 0), (int)AmbienteEnum.Farmacia, pacFarm.Id,
                    null, null, 200m, new[]
                    {
                        (prodAinezyl.Id, "B", 1, 75m),
                        (prodAmbrox.Id, "B", 1, 125m)
                    }),
                new("007", new DateTime(2026, 2, 4, 16, 0, 0), (int)AmbienteEnum.Farmacia, pacFarm.Id,
                    null, null, 350m, new[] { (prodPenic.Id, "B", 1, 350m) }),
                new("008", new DateTime(2026, 2, 5, 7, 45, 0), (int)AmbienteEnum.Laboratorio, pacLab.Id,
                    null, null, 170m, new[]
                    {
                        (examHemato.Id, "S", 1, 100m),
                        (examGlucosa.Id, "S", 1, 35m),
                        (examOrina.Id, "S", 1, 35m)
                    }),
                new("009", new DateTime(2026, 2, 5, 10, 0, 0), (int)AmbienteEnum.Laboratorio, pacLab.Id,
                    null, null, 280m, new[] { (examAbdomen.Id, "S", 1, 280m) }),
                new("010", new DateTime(2026, 2, 6, 9, 0, 0), (int)AmbienteEnum.Clinica, pacGeneral.Id,
                    medGeneral, svcGeneral, 300m, new[]
                    {
                        (svcGeneral.Id, "S", 1, 150m),
                        (svcCuracion.Id, "S", 1, 125m),
                        (prodInsumo.Id, "B", 1, 25m)
                    }),
                new("011", new DateTime(2026, 2, 6, 11, 30, 0), (int)AmbienteEnum.Clinica, pacPedia.Id,
                    medPedia, svcPedia, 400m, new[]
                    {
                        (svcPedia.Id, "S", 2, 400m)
                    }),
                new("012", new DateTime(2026, 2, 6, 15, 0, 0), (int)AmbienteEnum.Farmacia, pacFarm.Id,
                    null, null, 450m, new[]
                    {
                        (prodAinezyl.Id, "B", 2, 150m),
                        (prodPenic.Id, "B", 1, 300m)
                    })
            };

            foreach (var esc in escenarios)
            {
                var comprobante = Marker + esc.Numero;
                if (context.Ventas.Any(v => v.NoComprobante == comprobante))
                    continue;

                if (esc.Medico != null && esc.ServicioCita != null)
                    CrearCita(context, sucursal.Id, esc.PacienteId, esc.Medico, esc.ServicioCita,
                        esc.Fecha, esc.Medico.EspecialidadId);

                CrearVenta(context, esc, recepcionista.Id, comprobante, prodAinezyl.Id, prodAmbrox.Id,
                    prodPenic.Id, prodInsumo.Id, svcGine.Id, svcGeneral.Id, svcPedia.Id, svcCuracion.Id,
                    svcPapani.Id, examHemato.Id, examGlucosa.Id, examOrina.Id, examAbdomen.Id);
            }

            context.SaveChanges();
            logger?.LogInformation(
                "Demo sales report data seeded (NoComprobante prefix {Marker}, dates Feb 2026).", Marker);
        }

        private sealed class DemoVentaEscenario
        {
            public DemoVentaEscenario(string numero, DateTime fecha, int ambienteId, int pacienteId,
                Empleado medico, Servicio servicioCita, decimal monto,
                (int itemId, string bienOServicio, int cantidad, decimal subtotal)[] lineas)
            {
                Numero = numero;
                Fecha = fecha;
                AmbienteId = ambienteId;
                PacienteId = pacienteId;
                Medico = medico;
                ServicioCita = servicioCita;
                Monto = monto;
                Lineas = lineas;
            }

            public string Numero { get; }
            public DateTime Fecha { get; }
            public int AmbienteId { get; }
            public int PacienteId { get; }
            public Empleado Medico { get; }
            public Servicio ServicioCita { get; }
            public decimal Monto { get; }
            public (int itemId, string bienOServicio, int cantidad, decimal subtotal)[] Lineas { get; }
        }

        private static void CrearVenta(
            Context context,
            DemoVentaEscenario esc,
            int empleadoCajaId,
            string comprobante,
            int prodAinezylId, int prodAmbroxId, int prodPenicId, int prodInsumoId,
            int svcGineId, int svcGeneralId, int svcPediaId, int svcCuracionId, int svcPapaniId,
            int examHematoId, int examGlucosaId, int examOrinaId, int examAbdomenId)
        {
            var paciente = context.Pacientes.Find(esc.PacienteId);
            var venta = new Venta
            {
                NoComprobante = comprobante,
                FechaVenta = esc.Fecha,
                AmbienteId = esc.AmbienteId,
                PacienteId = esc.PacienteId,
                EmpleadoId = empleadoCajaId,
                Nombres = paciente?.Nombre ?? "DEMO Paciente",
                Nit = "CF",
                Direccion = "Ciudad",
                MontoPago = esc.Monto,
                Vuelto = 0,
                Eliminado = false,
                TipoVenta = esc.AmbienteId == (int)AmbienteEnum.Hospital ? "Hospitalizacion" : "Contado"
            };

            foreach (var linea in esc.Lineas)
            {
                var det = new DetalleVenta
                {
                    BienOServicio = linea.bienOServicio,
                    Cantidad = linea.cantidad,
                    Precio = linea.subtotal / Math.Max(linea.cantidad, 1),
                    Subtotal = linea.subtotal,
                    Total = linea.subtotal,
                    Descuento = 0
                };

                if (linea.bienOServicio == "B")
                    det.ProductoId = linea.itemId;
                else if (EsExamenId(linea.itemId, examHematoId, examGlucosaId, examOrinaId, examAbdomenId))
                    det.ExamenLabClinicoId = linea.itemId;
                else
                    det.ServicioId = linea.itemId;

                venta.DetalleVenta.Add(det);
            }

            venta.Pagos.Add(new Pagos
            {
                FechaHora = esc.Fecha,
                FormaPagoId = 1,
                Monto = esc.Monto,
                Eliminado = false
            });

            context.Ventas.Add(venta);
        }

        private static bool EsExamenId(int id, params int[] examIds) => examIds.Contains(id);

        private static void CrearCita(Context context, int sucursalId, int pacienteId, Empleado medico,
            Servicio servicio, DateTime fecha, int? especialidadId)
        {
            var existe = context.Citass.Any(c =>
                c.PacienteId == pacienteId
                && c.EmpleadoId == medico.Id
                && c.ServicioId == servicio.Id
                && c.FechaInicio.HasValue
                && c.FechaInicio.Value.Date == fecha.Date);
            if (existe) return;

            var cita = new Citas
            {
                PacienteId = pacienteId,
                SucursalId = sucursalId,
                EmpleadoId = medico.Id,
                ServicioId = servicio.Id,
                EspecialidadId = especialidadId ?? medico.EspecialidadId,
                FechaInicio = fecha,
                FechaFinal = fecha.AddMinutes(30),
                EstadoCita = "normal",
                Eliminado = false,
                Finalizada = true,
                CitaTipoAtencion = "PRIVADO"
            };
            context.Citass.Add(cita);
            context.SaveChanges();

            context.Consultas.Add(new Consulta
            {
                CitasId = cita.Id,
                FechaYHoraInicioConsulta = fecha,
                EstadoPagoConsultaId = 1,
                CostoConsulta = servicio.Precio_2,
                TipoConsulta = "Primera consulta"
            });
        }

        private static Empleado ObtenerOCrearEmpleado(Context context, int sucursalId, string dpi,
            string nombre, string apellido, int? especialidadId, string tipo)
        {
            var emp = context.Empleados.FirstOrDefault(e => e.Dpi == dpi && !e.Eliminado);
            if (emp != null) return emp;

            emp = new Empleado
            {
                Nombre = nombre,
                Apellido = apellido,
                Telefono = "5000-0000",
                Dpi = dpi,
                Nit = "CF",
                Email = dpi + "@demo.local",
                SucursalId = sucursalId,
                EspecialidadId = especialidadId,
                TipoEmpleado = tipo,
                FechaInicioLabores = DateTime.Today.AddYears(-2),
                VacacionesProgramadas = DateTime.Today,
                VacacionesProgramadasFinal = DateTime.Today,
                Eliminado = false
            };
            context.Empleados.Add(emp);
            context.SaveChanges();
            return emp;
        }

        private static Paciente ObtenerOCrearPaciente(Context context, string nombre, string telefono)
        {
            var p = context.Pacientes.FirstOrDefault(x => x.Nombre == nombre && !x.Eliminado);
            if (p != null) return p;

            p = new Paciente
            {
                Nombre = nombre,
                Telefono = telefono,
                Celular = telefono,
                Nit = "CF",
                Direccion = "Guatemala",
                TipoPacienteId = 1,
                EstadoPacienteId = 1,
                SexoId = 1,
                FechaNacimiento = new DateTime(1990, 1, 1),
                Eliminado = false
            };
            context.Pacientes.Add(p);
            context.SaveChanges();
            return p;
        }

        private static Producto ObtenerOCrearProducto(Context context, string nombre, string codigo,
            decimal precio, decimal costo, int ambienteId)
        {
            var p = context.Productos.FirstOrDefault(x => x.CodigoReferencia == codigo && !x.Eliminado);
            if (p != null) return p;

            p = new Producto
            {
                NombreProducto = nombre,
                CodigoReferencia = codigo,
                Precio = precio,
                Precio_2 = precio,
                Precio_3 = precio,
                Precio_4 = precio,
                Precio_5 = precio,
                Precio_6 = precio,
                Precio_7 = precio,
                PrecioCosto = costo,
                TipoProductoId = 1,
                AmbienteId = ambienteId,
                Stock = 100,
                StockInical = 100,
                Eliminado = false
            };
            context.Productos.Add(p);
            context.SaveChanges();
            return p;
        }

        private static void AsegurarInventarioCosto(Context context, int productoId, decimal costo, int ambienteId)
        {
            if (context.ProductosInventario.Any(i => i.ProductoId == productoId && !i.Eliminado))
                return;

            var bodegaId = context.Bodegas
                .Where(b => !b.Eliminada && b.AmbienteId == ambienteId)
                .Select(b => (int?)b.Id)
                .FirstOrDefault();

            context.ProductosInventario.Add(new ProductoInventario
            {
                ProductoId = productoId,
                BodegaId = bodegaId,
                PrecioCosto = costo,
                Stock = 50,
                Eliminado = false
            });
        }

        private static Servicio ObtenerOCrearServicio(Context context, string nombre, decimal precio)
        {
            var s = context.Servicios.FirstOrDefault(x => x.NombreServicio == nombre && !x.Eliminado);
            if (s != null) return s;

            s = new Servicio
            {
                NombreServicio = nombre,
                CodigoInterno = "DEMO-" + nombre.GetHashCode().ToString("X"),
                Precio_2 = precio,
                Precio_3 = precio,
                Precio_4 = precio,
                Eliminado = false
            };
            context.Servicios.Add(s);
            context.SaveChanges();
            return s;
        }

        private static void AsegurarInsumoServicio(Context context, int servicioId, int productoId, decimal cantidad)
        {
            if (context.ServiciosInsumos.Any(si =>
                    si.ServicioId == servicioId && si.ProductoId == productoId && !si.Eliminado))
                return;

            var unidadId = context.UnidadesMedidaVenta.Select(u => u.Id).FirstOrDefault();
            if (unidadId == 0)
            {
                var u = new UnidadMedidaVenta { Nombre = "Unidad", Abreviatura = "U" };
                context.UnidadesMedidaVenta.Add(u);
                context.SaveChanges();
                unidadId = u.Id;
            }

            context.ServiciosInsumos.Add(new ServicioInsumo
            {
                ServicioId = servicioId,
                ProductoId = productoId,
                UnidadMedidaVentaId = unidadId,
                CantidadUtilizada = cantidad,
                Eliminado = false
            });
        }

        private static CategoriaLabClinico CrearCategoriaLab(Context context)
        {
            var cat = new CategoriaLabClinico
            {
                Nombre = "Laboratorio Clínico Demo",
                Estado = "Activo",
                FechaCreacion = DateTime.Today,
                Activo = true,
                Eliminado = false
            };
            context.CategoriaLabClinicos.Add(cat);
            context.SaveChanges();
            return cat;
        }

        private static ExamenLabClinico ObtenerOCrearExamen(Context context, int categoriaId,
            string nombre, string codigo, decimal precio, decimal costo)
        {
            var e = context.ExamenLabClinicos.FirstOrDefault(x => x.CodigoInterno == codigo && !x.Eliminado);
            if (e != null) return e;

            e = new ExamenLabClinico
            {
                NombreExamen = nombre,
                CodigoInterno = codigo,
                CategoriaLabClinicoId = categoriaId,
                Precio = precio,
                PrecioB = precio,
                PrecioC = precio,
                PrecioCosto = costo,
                FechaCreacion = DateTime.Today,
                Activo = true,
                Eliminado = false
            };
            context.ExamenLabClinicos.Add(e);
            context.SaveChanges();
            return e;
        }
    }
}
