using Database.Shared.Data;
using Database.Shared.Enumeraciones;
using Database.Shared.IRepository;
using Database.Shared.Models;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using farmamest.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using sistema.Models;
using sistema.Service.IService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Wkhtmltopdf.NetCore;
using static sistema.Models.ExamenLabClinicoViewModel;
using Microsoft.EntityFrameworkCore; // para Include y ThenInclude
using System.IO;
using System.Globalization;
using DocumentFormat.OpenXml.Packaging;

namespace sistema.Controllers
{
    [Authorize]
    public class ConsultasController : Controller
    {

        private readonly ICitas _citasRepository = null;
        private readonly IServicio _serviciosRepository = null;
        private readonly IConsultas _consultasRepository = null;
        private readonly IPacientes _pacientesRepository = null;
        private readonly IGeneratePdf _generatePdf = null;
        private readonly IEmpleado _empleadoRepository = null;
        private readonly ICuentasPorCobrar _cuentasPorCobrarRepository = null;
        private readonly ILaboratorioClinico _laboratorioRepository = null;
        private readonly UserManager<User> _userManager = null;

        //Servicios (logica)
        private readonly IConsultasService _consultasService = null;
        private readonly IPacientesService _pacientesService = null;
        private readonly IProductosService _productosService = null;
        private readonly IHospitalizacionService _hospitalizacionService = null;

        //Oftalmologia
        private readonly IConsultasOftalmologia _oftRepo;

        private readonly IConsultasPodologia _podRepo;

        private readonly IConsultasHistoriaClinicaEnfermeria _enfRepo;

        private readonly IConsultasValoracionInicialEnfermeria _veRepo;

        private readonly IConsultasSueroterapia _sueroRepo;

        private readonly Database.Shared.Context _dbContext;



        public ConsultasController(ICitas citasRepository,
            ICuentasPorCobrar cuentasPorCobrarRepository,
            IConsultas consultasRepository,
            IServicio serviciosRepository,
            IGeneratePdf generatePdf,
            IPacientes pacientesRepository,
            IEmpleado empleadoRepository,
            ILaboratorioClinico laboratorioClinicoRepository,
            UserManager<User> userManager,
            //Servicios
            IPacientesService pacientesService,
            IConsultasService consultasService,
            IHospitalizacionService hospitalizacionService,
            IProductosService productosService,
            IConsultasOftalmologia oftRepo,
            IConsultasPodologia podRepo,
            IConsultasHistoriaClinicaEnfermeria enfRepo,
            IConsultasValoracionInicialEnfermeria veRepo,
            IConsultasSueroterapia sueroRepo,
            Database.Shared.Context dbContext
            )
        {
            _citasRepository = citasRepository;
            _userManager = userManager;
            _consultasRepository = consultasRepository;
            _serviciosRepository = serviciosRepository;
            _pacientesRepository = pacientesRepository;
            _generatePdf = generatePdf;
            _empleadoRepository = empleadoRepository;
            _cuentasPorCobrarRepository = cuentasPorCobrarRepository;
            _laboratorioRepository = laboratorioClinicoRepository;
            //Servicios
            _consultasService = consultasService;
            _pacientesService = pacientesService;
            _productosService = productosService;
            _hospitalizacionService = hospitalizacionService;
            _oftRepo = oftRepo;
            _podRepo = podRepo;
            _enfRepo = enfRepo;
            _veRepo = veRepo;
            _sueroRepo = sueroRepo;
            _dbContext = dbContext;
        }

        // public async Task<IActionResult> Index(string buscar, int? sucursalId, int? empleadoId, int? especialidadId, string fecha)
        // {
        //     var usuarioLogueado = await _userManager.GetUserAsync(HttpContext.User);
        //     var habilitarListaEmpleados = true;
        //     if (User.IsInRole("Medico Externo")
        //         || User.IsInRole("Medico General")
        //         || User.IsInRole("Medico Interno"))
        //     {
        //         empleadoId = usuarioLogueado.EmpleadoId;
        //         habilitarListaEmpleados = false;
        //     }

        //     var consultasPorFecha = _consultasRepository.ListaConsultas();

        //     if (empleadoId != null)
        //     {
        //         consultasPorFecha = consultasPorFecha
        //             .Where(a => a.Citas.EmpleadoId != null && a.Citas.EmpleadoId == empleadoId)
        //             .ToList();
        //     }

        //     if (especialidadId != null)
        //     {
        //         consultasPorFecha = consultasPorFecha
        //             .Where(a => a.Citas.EspecialidadId != null && a.Citas.EspecialidadId == especialidadId)
        //             .ToList();
        //     }

        //     // NUEVO: Agregar filtro por fecha
        //     if (!string.IsNullOrEmpty(fecha) && DateTime.TryParse(fecha, out DateTime fechaSeleccionada))
        //     {
        //         consultasPorFecha = consultasPorFecha
        //             .Where(a => a.FechaYHoraInicioConsulta.Date == fechaSeleccionada.Date)
        //             .ToList();
        //     }

        //     // NUEVO: Pasar la fecha al ViewData para mantener el valor en el input
        //     ViewData["CurrentFilter"] = fecha;

        //     var model = new ConsultasViewModel()
        //     {
        //         Consultas = consultasPorFecha
        //     };

        //     model.Init(_citasRepository, _serviciosRepository, _pacientesRepository, _empleadoRepository);

        //     model.HabilitarListaEmpleados = habilitarListaEmpleados;
        //     model.EmpleadoId = empleadoId;

        //     return View(model);
        // }

        public async Task<IActionResult> Index(string buscar, int? sucursalId, int? empleadoId, int? especialidadId, string fecha)
        {
            var usuarioLogueado = await _userManager.GetUserAsync(HttpContext.User);

            var habilitarListaEmpleados = true;
            if (User.IsInRole("Medico Externo") || User.IsInRole("Medico General") || User.IsInRole("Medico Interno"))
            {
                empleadoId = usuarioLogueado.EmpleadoId;
                habilitarListaEmpleados = false;
            }

            // Mantener valor en input
            ViewData["CurrentFilter"] = fecha;

            // OJO: ya NO cargamos consultas aquí (por rendimiento)
            var model = new ConsultasViewModel
            {
                Consultas = new System.Collections.Generic.List<Consulta>(),
                HabilitarListaEmpleados = habilitarListaEmpleados,
                EmpleadoId = empleadoId,
                EspecialidadId = especialidadId
            };

            // Esto carga combos. Si luego quieres optimizar más, hacemos un InitIndex() liviano.
            model.Init(_citasRepository, _serviciosRepository, _pacientesRepository, _empleadoRepository);

            return View(model);
        }

        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> ListadoData(int? empleadoId, int? especialidadId, string fecha)
        // {
        //     // DataTables params (POST form)
        //     var draw = Request.Form["draw"].FirstOrDefault();
        //     var start = int.TryParse(Request.Form["start"].FirstOrDefault(), out var s) ? s : 0;
        //     var length = int.TryParse(Request.Form["length"].FirstOrDefault(), out var l) ? l : 10;
        //     var searchValue = Request.Form["search[value]"].FirstOrDefault();

        //     // Seguridad de rol: si es médico, forzar empleadoId del usuario
        //     var usuarioLogueado = await _userManager.GetUserAsync(HttpContext.User);
        //     if (User.IsInRole("Medico Externo") || User.IsInRole("Medico General") || User.IsInRole("Medico Interno"))
        //     {
        //         empleadoId = usuarioLogueado.EmpleadoId;
        //     }

        //     // Query SQL (NO materializa todo)
        //     var query = _consultasRepository.QueryConsultasParaListado();

        //     // Filtros
        //     if (empleadoId.HasValue)
        //         query = query.Where(a => a.Citas.EmpleadoId == empleadoId.Value);

        //     if (especialidadId.HasValue)
        //         query = query.Where(a => a.Citas.EspecialidadId == especialidadId.Value);

        //     if (!string.IsNullOrWhiteSpace(fecha))
        //     {
        //         // Evitar ambigüedad cultural: el input date trae yyyy-MM-dd
        //         if (DateTime.TryParseExact(fecha, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fechaSeleccionada))
        //         {
        //             var f = fechaSeleccionada.Date;
        //             query = query.Where(a => a.FechaYHoraInicioConsulta.Date == f);
        //         }
        //     }

        //     // Search (mínimo y basado en campos reales usados por la tabla)
        //     if (!string.IsNullOrWhiteSpace(searchValue))
        //     {
        //         query = query.Where(a =>
        //             (a.Citas.ClienteText != null && a.Citas.ClienteText.Contains(searchValue)) ||
        //             (a.Citas.EmpleadoText != null && a.Citas.EmpleadoText.Contains(searchValue)) ||
        //             (a.Citas.EspecialidadText != null && a.Citas.EspecialidadText.Contains(searchValue)) ||
        //             (a.EstadoPagoConsulta != null && a.EstadoPagoConsulta.Estado != null && a.EstadoPagoConsulta.Estado.Contains(searchValue))
        //         );
        //     }

        //     // Total después de filtros (DataTables espera total filtrado)
        //     var recordsFiltered = await query.CountAsync();

        //     // Orden: por Id desc (como tu UI actual)
        //     query = query.OrderByDescending(a => a.Id);

        //     // Página
        //     var page = await query
        //         .Skip(start)
        //         .Take(length)
        //         .Select(a => new
        //         {
        //             id = a.Id,
        //             fechaYHoraInicioConsulta = a.FechaYHoraInicioConsulta,
        //             numeroTurno = a.Citas.NumeroTurno,
        //             paciente = a.Citas.ClienteText,
        //             tipoAtencion = a.Citas.CitaTipoAtencion,
        //             medico = a.Citas.EmpleadoText,
        //             categoriaHabitacionId = a.Citas.CategoriaHabitacionId,
        //             especialidad = a.Citas.EspecialidadText,
        //             pago = a.EstadoPagoConsulta != null ? a.EstadoPagoConsulta.Estado : "N/A",
        //             precio = a.CostoConsulta,

        //             igssNumero = a.Citas.Paciente != null ? a.Citas.Paciente.IgssNumeroAfiliacion : null,
        //             codigoDeCita = a.Citas.CodigoDeCita,
        //             codigoAutorizacion = a.Citas.CodigoAutorizacion,

        //             servicios = a.ConsultasServicios.Select(cs => cs.Servicio.NombreServicio).ToList()
        //         })
        //         .ToListAsync();

        //     // OJO: recordsTotal “real” sin filtros normalmente sería count total de Consultas,
        //     // pero DataTables funciona con recordsTotal y recordsFiltered.
        //     // Para no hacer 2 counts costosos en tablas gigantes, devolvemos recordsTotal = recordsFiltered.
        //     // Si necesitas exactitud, lo hacemos con otro CountAsync() al query sin filtros.
        //     return Json(new
        //     {
        //         draw,
        //         recordsTotal = recordsFiltered,
        //         recordsFiltered,
        //         data = page
        //     });
        // }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ListadoData(int? empleadoId, int? especialidadId, string fecha, string pacienteNombre)
        {
            // DataTables params
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = int.TryParse(Request.Form["start"].FirstOrDefault(), out var s) ? s : 0;
            var length = int.TryParse(Request.Form["length"].FirstOrDefault(), out var l) ? l : 10;

            // Seguridad de rol
            var usuarioLogueado = await _userManager.GetUserAsync(HttpContext.User);
            if (User.IsInRole("Medico Externo") || User.IsInRole("Medico General") || User.IsInRole("Medico Interno"))
            {
                empleadoId = usuarioLogueado.EmpleadoId;
            }

            // Query SQL
            var query = _consultasRepository.QueryConsultasParaListado();

            // Filtros
            if (empleadoId.HasValue)
                query = query.Where(a => a.Citas.EmpleadoId == empleadoId.Value);

            if (especialidadId.HasValue)
                query = query.Where(a => a.Citas.EspecialidadId == especialidadId.Value);

            if (!string.IsNullOrWhiteSpace(fecha))
            {
                if (DateTime.TryParseExact(fecha, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fechaSeleccionada))
                {
                    var f = fechaSeleccionada.Date;
                    query = query.Where(a => a.FechaYHoraInicioConsulta.Date == f);
                }
            }

            if (!string.IsNullOrWhiteSpace(pacienteNombre))
            {
                pacienteNombre = pacienteNombre.Trim();
                query = query.Where(a =>
                    a.Citas.Paciente != null &&
                    a.Citas.Paciente.Nombre != null &&
                    a.Citas.Paciente.Nombre.ToLower().Contains(pacienteNombre.ToLower())
                );
            }

            // Total después de filtros
            var recordsFiltered = await query.CountAsync();

            // Orden
            query = query.OrderByDescending(a => a.Id);

            var pageRaw = await query
                .Skip(start)
                .Take(length)
                .Select(a => new
                {
                    id = a.Id,
                    fechaYHoraInicioConsulta = a.FechaYHoraInicioConsulta,
                    numeroTurno = a.Citas.NumeroTurno,
                    paciente = a.Citas.ClienteText,
                    tipoAtencion = a.Citas.CitaTipoAtencion,
                    medico = a.Citas.EmpleadoText,
                    categoriaHabitacionId = a.Citas.CategoriaHabitacionId,
                    especialidad = a.Citas.EspecialidadText,
                    pago = a.EstadoPagoConsulta != null ? a.EstadoPagoConsulta.Estado : "N/A",
                    precio = a.CostoConsulta,
                    igssNumero = a.Citas.Paciente != null ? a.Citas.Paciente.IgssNumeroAfiliacion : null,
                    codigoDeCita = a.Citas.CodigoDeCita,
                    codigoAutorizacion = a.Citas.CodigoAutorizacion,
                    servicios = a.ConsultasServicios.Select(cs => cs.Servicio.NombreServicio).ToList()
                })
                .ToListAsync();

            //formatear en memoria con SpecifyKind Unspecified
            // para evitar que .NET aplique conversión de zona horaria al serializar
            var page = pageRaw.Select(a => new
            {
                a.id,
                fechaYHoraInicioConsulta = DateTime.SpecifyKind(a.fechaYHoraInicioConsulta, DateTimeKind.Unspecified)
                                                   .ToString("dd/MM/yyyy HH:mm"),
                a.numeroTurno,
                a.paciente,
                a.tipoAtencion,
                a.medico,
                a.categoriaHabitacionId,
                a.especialidad,
                a.pago,
                a.precio,
                a.igssNumero,
                a.codigoDeCita,
                a.codigoAutorizacion,
                a.servicios
            }).ToList();

            return Json(new
            {
                draw,
                recordsTotal = recordsFiltered,
                recordsFiltered,
                data = page
            });
        }
        public IActionResult IniciarConsulta(int? citaId)
        {
            if (citaId == null)
            {
                Console.WriteLine("citaId es null");
                return StatusCode(400);
            }

            var cita = _citasRepository.GetCita((int)citaId);
            if (cita == null)
            {
                Console.WriteLine($"No se encontró la cita con citaId: {citaId}");
                return StatusCode(404);
            }

            // ============================================================
            // ✅ PASO CLAVE: MARCAR "INICIO DE CONSULTA" => EN ESPERA
            // ============================================================
            // Regla de estados por contadores:
            // - Agendada:   ContadorCitaIniciada == null && ContadorCitaFinalizada == null
            // - En espera:  ContadorCitaIniciada != null && ContadorCitaFinalizada == null
            // - Finalizada: ContadorCitaFinalizada != null (y/o Finalizada == true)
            //
            // Aquí marcamos ContadorCitaIniciada AL ENTRAR a la vista, solo si aún no estaba marcado.
            if (!cita.ContadorCitaIniciada.HasValue)
            {
                cita.ContadorCitaIniciada = DateTime.UtcNow;
                _citasRepository.Update(cita); // IMPORTANT: este update debe guardar (SaveChanges) o persistir cambios
            }

            var consultasPaciente = _consultasRepository.ListaConsultas().ToList();

            consultasPaciente = consultasPaciente
                .Where(c => c.Citas != null && c.Citas.PacienteId == cita.PacienteId)
                .ToList();

            var ultimaConsulta = _consultasRepository.GetUltimaConsultaPaciente(cita.PacienteId);

            var model = new ConsultasViewModel
            {
                CitaId = (int)citaId,
                PacienteId = cita.PacienteId,
                Nombres = cita.ClienteText ?? "Sin Nombre",
                MedicoAsignado = cita.EmpleadoText ?? "Sin Médico",
                FechaYHoraInicio = DateTime.Now,
                CodigoSeguro = cita.CodigoDeCita,

                PacienteNombre = cita.Paciente?.Nombre ?? "Desconocido",
                PacienteNit = cita.Paciente?.Nit ?? "Sin NIT",
                PacienteFechaNacimiento = cita.Paciente?.FechaNacimiento?.ToString() ?? "Sin Fecha",
                PacienteDireccion = cita.Paciente?.Direccion ?? "Sin Dirección",
                PacienteAlias = cita.Paciente?.Alias ?? "Sin Alias",
                PacienteSexo = cita.Paciente?.Sexo?.DescripcionSexo ?? "-",
                PacienteEdad = cita.Paciente?.FechaNacimiento != null
                    ? CalcularEdad((DateTime)cita.Paciente.FechaNacimiento)
                    : "-",
                PacienteTelefono = cita.Paciente?.Telefono ?? "Sin Teléfono",
                PacienteCelular = cita.Paciente?.Celular ?? "Sin Celular",
                PacienteMedicos = cita.Paciente?.AntecedentesMedicos ?? "Sin Datos",
                PacienteDepartamentoId = cita.Paciente?.DepartamentoId,
                PacienteMunicipioId = cita.Paciente?.MunicipioId,
                PacienteQuirurgicos = cita.Paciente?.AntecedentesQuirurgicos ?? "Sin Datos",
                PacienteTraumaticos = cita.Paciente?.AntecedentesTraumaticos ?? "Sin Datos",
                PacienteAlergias = cita.Paciente?.AntecedentesAlergias ?? "Sin Datos",
                PacienteVicios = cita.Paciente?.AntecedentesVicios ?? "Sin Datos",
                PacienteSeguroEPSS = cita.Paciente?.SeguroEpss?.Nombre ?? "Sin Seguro",
                PacienteMedicamentos = cita.Paciente?.AntecedentesMedicamentos ?? "Sin Medicamentos",
            };

            if (ultimaConsulta == null)
            {
                Console.WriteLine("No se encontró una última consulta.");
                model.FechaUltimaConsulta = null;
                model.MotivoUltimaConsulta = null;
            }
            else
            {
                model.FechaUltimaConsulta = ultimaConsulta.FechaYHoraInicioConsulta;
                model.MotivoUltimaConsulta = ultimaConsulta.Citas?.Motivo ?? "Sin Motivo";
            }

            var primeraConsulta = consultasPaciente
                .OrderBy(c => c.FechaYHoraInicioConsulta)
                .FirstOrDefault();

            if (primeraConsulta == null)
            {
                Console.WriteLine("No se encontró una primera consulta.");
                model.PrimeraConsulta = true;
            }
            else
            {
                model.PrimeraConsulta = false;
                model.TerapeuticoDatosGenerales = primeraConsulta.TerapeuticoDatosGenerales ?? "Sin Datos";
            }

            var rangos = _pacientesRepository
                .GetRangosSaludablesPaciente((int)cita.PacienteId)?
                .OrderBy(r => r.Fecha)
                .ToList();

            if (rangos != null && rangos.Any())
            {
                foreach (var rango in rangos)
                {
                    model.RangosSaludablesHistorico.Add(new RangoSaludableConsultaViewModel
                    {
                        Id = rango.Id,
                        Fecha = rango.Fecha,
                        IMC = rango.IMC,
                        Peso = rango.Peso,
                        PorcentajeGrasaCorporal = rango.PorcentajeGrasaCorporal
                    });
                }
            }

            var historico = _consultasRepository
                .ListaConsultas()
                .Where(c => c.Citas.PacienteId == cita.PacienteId && !string.IsNullOrEmpty(c.Cie10Codigo))
                .Select(c => new HistorialCie10ViewModel
                {
                    Fecha = c.FechaYHoraInicioConsulta,
                    Cie10Codigo = c.Cie10Codigo
                })
                .ToList();

            model.HistorialCie10 = historico;

            model.Init(_citasRepository, _serviciosRepository, _pacientesRepository);

            return View(model);
        }



        public IActionResult IniciarYPagarConsulta(int? citaId)
        {
            if (citaId == null) return StatusCode(400);
            var cita = _citasRepository.GetCita((int)citaId);
            if (cita == null) return StatusCode(404);

            var consultasPaciente = _consultasRepository.ListaConsultas()
                .Where(c => c.Citas.PacienteId == cita.PacienteId);
            var ultimaConsulta = _consultasRepository.GetUltimaConsultaPaciente(cita.PacienteId);

            var model = new ConsultasViewModel()
            {
                CitaId = (int)citaId,
                CitaTipoAtencion = cita.CitaTipoAtencion,
                ResponsableNit = cita.ResponsableNit,
                ResponsableNombre = cita.ResponsableNombre,
                ResponsableDireccion = cita.ResponsableDireccion,
                ResponsableCorreo = cita.ResponsableCorreo,
                PacienteId = cita.PacienteId,
                Nombres = cita.ClienteText,
                MedicoAsignado = cita.EmpleadoText,
                FechaYHoraInicio = DateTime.Now,
                PacienteNombre = cita.Paciente.Nombre,
                PacienteNit = cita.Paciente.Nit,
                PacienteFechaNacimiento = cita.Paciente.FechaNacimiento == null ? "" :
                                            cita.Paciente.FechaNacimiento.ToString(),
                PacienteDireccion = cita.Paciente.Direccion,
                PacienteAlias = cita.Paciente.Alias,
                PacienteSexo = cita.Paciente.Sexo != null
                ? cita.Paciente.Sexo.DescripcionSexo
                : "-",
                PacienteEdad = cita.Paciente.FechaNacimiento != null
                ? CalcularEdad((DateTime)cita.Paciente.FechaNacimiento)
                : "-",
                PacienteTelefono = cita.Paciente.Telefono,
                PacienteCelular = cita.Paciente.Celular,
                PacienteMedicos = cita.Paciente.AntecedentesMedicos,
                PacienteQuirurgicos = cita.Paciente.AntecedentesQuirurgicos,
                PacienteTraumaticos = cita.Paciente.AntecedentesTraumaticos,
                PacienteAlergias = cita.Paciente.AntecedentesAlergias,
                PacienteVicios = cita.Paciente.AntecedentesVicios,
                PacienteSeguroEPSS = cita.Paciente.SeguroEpss != null ? cita.Paciente.SeguroEpss.Nombre : "",
                PacienteMedicamentos = cita.Paciente.AntecedentesMedicamentos,

                // Nuevos campos para los datos del padre
                NombrePadre = cita.NombrePadre,
                FechaNacimientoPadre = cita.FechaNacimientoPadre,
                EdadPadre = cita.EdadPadre,
                DPIPadre = cita.DPIPadre,
                DireccionPadre = cita.DireccionPadre,
                TelefonoPadre = cita.TelefonoPadre,
                CorreoPadre = cita.CorreoPadre,
                OcupacionPadre = cita.OcupacionPadre,
                EmpresaPadre = cita.EmpresaPadre,
                TelefonoEmpresaPadre = cita.TelefonoEmpresaPadre,
                DireccionEmpresaPadre = cita.DireccionEmpresaPadre,

                // Nuevos campos para los datos de la madre
                NombreMadre = cita.NombreMadre,
                FechaNacimientoMadre = cita.FechaNacimientoMadre,
                EdadMadre = cita.EdadMadre,
                DPIMadre = cita.DPIMadre,
                DireccionMadre = cita.DireccionMadre,
                TelefonoMadre = cita.TelefonoMadre,
                CorreoMadre = cita.CorreoMadre,
                OcupacionMadre = cita.OcupacionMadre,
                EmpresaMadre = cita.EmpresaMadre,
                TelefonoEmpresaMadre = cita.TelefonoEmpresaMadre,
                DireccionEmpresaMadre = cita.DireccionEmpresaMadre,

                // Nuevos campos para los datos del acompañante
                AcompananteNombre = cita.AcompananteNombre,
                AcompananteDPI = cita.AcompananteDPI,
                AcompananteTelefono = cita.AcompananteTelefono,

                // Carga de datos ginecológicos
                PacienteCicloMenstGine = cita.Paciente.CicloMenstGine,
                PacienteETSGine = cita.Paciente.ETSGine,
                PacienteVIHGine = cita.Paciente.VIHGine,
                PacienteGrupoFactorGine = cita.Paciente.GrupoFactorGine,
                PacienteTorchGine = cita.Paciente.TorchGine,
                PacienteInicioVidaSexualGine = cita.Paciente.InicioVidaSexualGine,
                PacienteParejasSexGine = cita.Paciente.ParejasSexGine,
                PacienteObesidadGine = cita.Paciente.ObesidadGine,
                PacienteDesnutricionGine = cita.Paciente.DesnutricionGine,
                PacienteQGine = cita.Paciente.QGine,
                PacientePGine = cita.Paciente.PGine,
                PacienteABGine = cita.Paciente.ABGine,
                PacienteCGine = cita.Paciente.CGine,
                PacienteFURGine = cita.Paciente.FURGine,
                PacienteMuerteNeoGine = cita.Paciente.MuerteNeoGine,
                PacienteFPPGine = cita.Paciente.FPPGine,
                PacienteHVGine = cita.Paciente.HVGine,
                PacienteMuerteGine = cita.Paciente.MuerteGine,
                PacienteControlPrenatalGine = cita.Paciente.ControlPrenatalGine,
                PacienteComadronaGine = cita.Paciente.ComadronaGine,
                PacienteNoControlesGine = cita.Paciente.NoControlesGine,

                // Información del turno
                NivelPrioridadCita = cita.NivelPrioridadCita,
                EstadoTurno = cita.EstadoTurno,
                NumeroTurno = cita.NumeroTurno,

                SeguimientosNutricionales = new List<SeguimientoNutricionalConsulta>(),
                VacunasPaciente = new List<VacunaPacienteConsulta>()
            };

            // Manejo de última y primera consulta
            if (ultimaConsulta == null)
            {
                model.FechaUltimaConsulta = null;
                model.MotivoUltimaConsulta = null;
            }
            else
            {
                model.FechaUltimaConsulta = ultimaConsulta.FechaYHoraInicioConsulta;
                model.MotivoUltimaConsulta = ultimaConsulta.Citas.Motivo;
            }

            var primeraConsulta = consultasPaciente
                .OrderBy(c => c.FechaYHoraInicioConsulta)
                .FirstOrDefault();

            model.PrimeraConsulta = primeraConsulta == null;

            if (!model.PrimeraConsulta)
            {
                model.TerapeuticoDatosGenerales = primeraConsulta.TerapeuticoDatosGenerales;
                model.TerapeuticoActividadesDiarias = primeraConsulta.TerapeuticoActividadesDiarias;
                model.TerapeuticoConQuienVive = primeraConsulta.TerapeuticoConQuienVive;
                model.TerapeuticoHabitosAlimenticios = primeraConsulta.TerapeuticoHabitosAlimenticios;
                model.TerapeuticoEjercicio = primeraConsulta.TerapeuticoEjercicio;
                model.TerapeuticoFinesSemana = primeraConsulta.TerapeuticoFinesSemana;
                model.TerapeuticoHistoriaMedica = primeraConsulta.TerapeuticoHistoriaMedica;
                model.TerapeuticoHistoriaPeso = primeraConsulta.TerapeuticoHistoriaPeso;
                model.TerapeuticoHistoricoObservaciones = consultasPaciente.Select(c => new HistoricoObservacionesTerapeuticoConsultaViewModel
                {
                    Fecha = c.FechaYHoraInicioConsulta,
                    Observaciones = c.TerapeuticoObservaciones
                }).ToList();
            }

            // Rangos saludables
            var rangos = _pacientesRepository.GetRangosSaludablesPaciente((int)cita.PacienteId)
                .OrderBy(r => r.Fecha);
            if (rangos != null)
            {
                foreach (var rangoPaciente in rangos)
                {
                    model.RangosSaludablesHistorico.Add(new RangoSaludableConsultaViewModel
                    {
                        Id = rangoPaciente.Id,
                        Fecha = rangoPaciente.Fecha,
                        IMC = rangoPaciente.IMC,
                        Peso = rangoPaciente.Peso,
                        PorcentajeGrasaCorporal = rangoPaciente.PorcentajeGrasaCorporal
                    });
                }
            }

            model.Init(_citasRepository, _serviciosRepository, _pacientesRepository);

            return View(model);
        }

        public IActionResult IniciarYPagarCreditoConsulta(int? citaId)
        {

            if (citaId == null) return StatusCode(400);
            var cita = _citasRepository.GetCita((int)citaId);
            if (cita == null) return StatusCode(404);


            var consultasPaciente = _consultasRepository.ListaConsultas()
                .Where(c => c.Citas.PacienteId == cita.PacienteId);
            var ultimaConsulta = _consultasRepository.GetUltimaConsultaPaciente(cita.PacienteId);


            var model = new ConsultasViewModel()
            {
                CitaId = (int)citaId,
                PacienteId = cita.PacienteId,
                Nombres = cita.ClienteText,
                //Servicio = cita.ServicioText,
                MedicoAsignado = cita.EmpleadoText,
                FechaYHoraInicio = DateTime.Now,
                //CostoConsulta = cita.Servicio == null ? 0 : cita.Servicio.prec,

                PacienteNombre = cita.Paciente.Nombre,
                PacienteNit = cita.Paciente.Nit,
                PacienteFechaNacimiento = cita.Paciente.FechaNacimiento == null ? "" :
                                            cita.Paciente.FechaNacimiento.ToString(),
                PacienteDireccion = cita.Paciente.Direccion,
                PacienteAlias = cita.Paciente.Alias,
                PacienteSexo = cita.Paciente.Sexo != null
                ? cita.Paciente.Sexo.DescripcionSexo
                : "-",
                PacienteEdad = cita.Paciente.FechaNacimiento != null
                ? CalcularEdad((DateTime)cita.Paciente.FechaNacimiento)
                : "-",
                PacienteTelefono = cita.Paciente.Telefono,
                PacienteCelular = cita.Paciente.Celular,
                PacienteMedicos = cita.Paciente.AntecedentesMedicos,
                PacienteQuirurgicos = cita.Paciente.AntecedentesQuirurgicos,
                PacienteTraumaticos = cita.Paciente.AntecedentesTraumaticos,
                PacienteAlergias = cita.Paciente.AntecedentesAlergias,
                PacienteVicios = cita.Paciente.AntecedentesVicios,
                PacienteSeguroEPSS = cita.Paciente.SeguroEpss != null ? cita.Paciente.SeguroEpss.Nombre : "",
                PacienteMedicamentos = cita.Paciente.AntecedentesMedicamentos,

                //Carga de Datos Ginecologicos a BaseViewModel Consultas
                PacienteCicloMenstGine = cita.Paciente.CicloMenstGine,
                PacienteETSGine = cita.Paciente.ETSGine,
                PacienteVIHGine = cita.Paciente.VIHGine,
                PacienteGrupoFactorGine = cita.Paciente.GrupoFactorGine,
                PacienteTorchGine = cita.Paciente.TorchGine,
                PacienteInicioVidaSexualGine = cita.Paciente.InicioVidaSexualGine,
                PacienteParejasSexGine = cita.Paciente.ParejasSexGine,
                PacienteObesidadGine = cita.Paciente.ObesidadGine,
                PacienteDesnutricionGine = cita.Paciente.DesnutricionGine,
                PacienteQGine = cita.Paciente.QGine,
                PacientePGine = cita.Paciente.PGine,
                PacienteABGine = cita.Paciente.ABGine,
                PacienteCGine = cita.Paciente.CGine,
                PacienteFURGine = cita.Paciente.FURGine,
                PacienteMuerteNeoGine = cita.Paciente.MuerteNeoGine,
                PacienteFPPGine = cita.Paciente.FPPGine,
                PacienteHVGine = cita.Paciente.HVGine,
                PacienteMuerteGine = cita.Paciente.MuerteGine,
                PacienteControlPrenatalGine = cita.Paciente.ControlPrenatalGine,
                PacienteComadronaGine = cita.Paciente.ComadronaGine,
                PacienteNoControlesGine = cita.Paciente.NoControlesGine,


                //Informacion del TURNO 
                NivelPrioridadCita = cita.NivelPrioridadCita,
                EstadoTurno = cita.EstadoTurno,
                NumeroTurno = cita.NumeroTurno,

                SeguimientosNutricionales = new List<SeguimientoNutricionalConsulta>(),
                VacunasPaciente = new List<VacunaPacienteConsulta>()
            };

            if (ultimaConsulta == null)
            {
                model.FechaUltimaConsulta = null;
                model.MotivoUltimaConsulta = null;
            }
            else
            {
                model.FechaUltimaConsulta = ultimaConsulta.FechaYHoraInicioConsulta;
                model.MotivoUltimaConsulta = ultimaConsulta.Citas.Motivo;
            }

            //Utilizado en caso de requerirse datos de la primera consulta del paciente
            var primeraConsulta = consultasPaciente
                .OrderBy(c => c.FechaYHoraInicioConsulta)
                .FirstOrDefault();
            if (primeraConsulta == null)
            {
                model.PrimeraConsulta = true;
            }
            else
            {
                model.PrimeraConsulta = false;

                model.TerapeuticoDatosGenerales = primeraConsulta.TerapeuticoDatosGenerales;
                model.TerapeuticoActividadesDiarias = primeraConsulta.TerapeuticoActividadesDiarias;
                model.TerapeuticoConQuienVive = primeraConsulta.TerapeuticoConQuienVive;
                model.TerapeuticoHabitosAlimenticios = primeraConsulta.TerapeuticoHabitosAlimenticios;
                model.TerapeuticoEjercicio = primeraConsulta.TerapeuticoEjercicio;
                model.TerapeuticoFinesSemana = primeraConsulta.TerapeuticoFinesSemana;
                model.TerapeuticoHistoriaMedica = primeraConsulta.TerapeuticoHistoriaMedica;
                model.TerapeuticoHistoriaPeso = primeraConsulta.TerapeuticoHistoriaPeso;

                model.TerapeuticoHistoricoObservaciones = new List<HistoricoObservacionesTerapeuticoConsultaViewModel>();

                foreach (var consultaPaciente in consultasPaciente)
                {
                    model.TerapeuticoHistoricoObservaciones.Add(new HistoricoObservacionesTerapeuticoConsultaViewModel
                    {
                        Fecha = consultaPaciente.FechaYHoraInicioConsulta,
                        Observaciones = consultaPaciente.TerapeuticoObservaciones
                    });
                }
            }

            //Rangos saludables
            var rangos = _pacientesRepository.GetRangosSaludablesPaciente((int)cita.PacienteId)
                .OrderBy(r => r.Fecha);
            if (rangos != null)
            {
                foreach (var rangoPaciente in rangos)
                {
                    model.RangosSaludablesHistorico.Add(new RangoSaludableConsultaViewModel
                    {
                        Id = rangoPaciente.Id,
                        Fecha = rangoPaciente.Fecha,
                        IMC = rangoPaciente.IMC,
                        Peso = rangoPaciente.Peso,
                        PorcentajeGrasaCorporal = rangoPaciente.PorcentajeGrasaCorporal
                    });
                }
            }

            model.Init(_citasRepository, _serviciosRepository, _pacientesRepository);

            return View(model);
        }

        public IActionResult IniciarExamenesFisicosCitasConsulta(int? citaId)
        {

            if (citaId == null) return StatusCode(400);
            var cita = _citasRepository.GetCita((int)citaId);
            if (cita == null) return StatusCode(404);


            var consultasPaciente = _consultasRepository.ListaConsultas()
                .Where(c => c.Citas.PacienteId == cita.PacienteId);
            var ultimaConsulta = _consultasRepository.GetUltimaConsultaPaciente(cita.PacienteId);


            var model = new ConsultasViewModel()
            {
                CitaId = (int)citaId,
                PacienteId = cita.PacienteId,
                Nombres = cita.ClienteText,
                //Servicio = cita.ServicioText,
                MedicoAsignado = cita.EmpleadoText,
                FechaYHoraInicio = DateTime.Now,
                //CostoConsulta = cita.Servicio == null ? 0 : cita.Servicio.prec,

                PacienteNombre = cita.Paciente.Nombre,
                PacienteNit = cita.Paciente.Nit,
                PacienteFechaNacimiento = cita.Paciente.FechaNacimiento == null ? "" :
                                            cita.Paciente.FechaNacimiento.ToString(),
                PacienteDireccion = cita.Paciente.Direccion,
                PacienteAlias = cita.Paciente.Alias,
                PacienteSexo = cita.Paciente.Sexo != null
                ? cita.Paciente.Sexo.DescripcionSexo
                : "-",
                PacienteEdad = cita.Paciente.FechaNacimiento != null
                ? CalcularEdad((DateTime)cita.Paciente.FechaNacimiento)
                : "-",
                PacienteTelefono = cita.Paciente.Telefono,
                PacienteCelular = cita.Paciente.Celular,
                PacienteMedicos = cita.Paciente.AntecedentesMedicos,
                PacienteQuirurgicos = cita.Paciente.AntecedentesQuirurgicos,
                PacienteTraumaticos = cita.Paciente.AntecedentesTraumaticos,
                PacienteAlergias = cita.Paciente.AntecedentesAlergias,
                PacienteVicios = cita.Paciente.AntecedentesVicios,
                PacienteSeguroEPSS = cita.Paciente.SeguroEpss != null ? cita.Paciente.SeguroEpss.Nombre : "",
                PacienteMedicamentos = cita.Paciente.AntecedentesMedicamentos,

                //Carga de Datos Ginecologicos a BaseViewModel Consultas
                PacienteCicloMenstGine = cita.Paciente.CicloMenstGine,
                PacienteETSGine = cita.Paciente.ETSGine,
                PacienteVIHGine = cita.Paciente.VIHGine,
                PacienteGrupoFactorGine = cita.Paciente.GrupoFactorGine,
                PacienteTorchGine = cita.Paciente.TorchGine,
                PacienteInicioVidaSexualGine = cita.Paciente.InicioVidaSexualGine,
                PacienteParejasSexGine = cita.Paciente.ParejasSexGine,
                PacienteObesidadGine = cita.Paciente.ObesidadGine,
                PacienteDesnutricionGine = cita.Paciente.DesnutricionGine,
                PacienteQGine = cita.Paciente.QGine,
                PacientePGine = cita.Paciente.PGine,
                PacienteABGine = cita.Paciente.ABGine,
                PacienteCGine = cita.Paciente.CGine,
                PacienteFURGine = cita.Paciente.FURGine,
                PacienteMuerteNeoGine = cita.Paciente.MuerteNeoGine,
                PacienteFPPGine = cita.Paciente.FPPGine,
                PacienteHVGine = cita.Paciente.HVGine,
                PacienteMuerteGine = cita.Paciente.MuerteGine,
                PacienteControlPrenatalGine = cita.Paciente.ControlPrenatalGine,
                PacienteComadronaGine = cita.Paciente.ComadronaGine,
                PacienteNoControlesGine = cita.Paciente.NoControlesGine,


                //Informacion del TURNO 
                NivelPrioridadCita = cita.NivelPrioridadCita,
                EstadoTurno = cita.EstadoTurno,
                NumeroTurno = cita.NumeroTurno,


                // Datos de la Mama
                //PacienteUteroGravioGine = cita.Paciente.UteroGravioGine,
                //PacienteAbdomenObstetricoGine = cita.Paciente.AbdomenObstetricoGine,
                //PacienteFCBGine = cita.Paciente.FCBGine,
                //PacienteAUGine = cita.Paciente.AUGine,
                //PacientePresentacionLeopoldGine = cita.Paciente.PresentacionLeopoldGine,
                //PacienteOtrasGine = cita.Paciente.OtrasGine,
                //PacienteActividadUterinaGine = cita.Paciente.ActividadUterinaGine,
                //PacienteMovimientoFetalPercetibleGine = cita.Paciente.MovimientoFetalPercetibleGine,
                //PacienteEspecifiqueGine = cita.Paciente.EspecifiqueGine,
                //PacienteTactoVaginalGine = cita.Paciente.TactoVaginalGine,
                //PacienteDGine = cita.Paciente.DGine,
                //PacienteCMSGine = cita.Paciente.CMSGine,
                //PacienteBPorcientoGine = cita.Paciente.BPorcientoGine,
                //PacienteAltiutudGine = cita.Paciente.AltiutudGine,
                //PacienteVariedadPosicionGine = cita.Paciente.VariedadPosicionGine,
                //PacienteMembranasOvularesGine = cita.Paciente.MembranasOvularesGine,
                //PacienteLiquidoAmnioticoGine = cita.Paciente.LiquidoAmnioticoGine,
                //PacienteEspecifique2Gine = cita.Paciente.Especifique2Gine,
                //PacientePelvisGine = cita.Paciente.PelvisGine,





                SeguimientosNutricionales = new List<SeguimientoNutricionalConsulta>(),
                VacunasPaciente = new List<VacunaPacienteConsulta>()
            };

            if (ultimaConsulta == null)
            {
                model.FechaUltimaConsulta = null;
                model.MotivoUltimaConsulta = null;
            }
            else
            {
                model.FechaUltimaConsulta = ultimaConsulta.FechaYHoraInicioConsulta;
                model.MotivoUltimaConsulta = ultimaConsulta.Citas.Motivo;
            }

            //Utilizado en caso de requerirse datos de la primera consulta del paciente
            var primeraConsulta = consultasPaciente
                .OrderBy(c => c.FechaYHoraInicioConsulta)
                .FirstOrDefault();
            if (primeraConsulta == null)
            {
                model.PrimeraConsulta = true;
            }
            else
            {
                model.PrimeraConsulta = false;

                model.TerapeuticoDatosGenerales = primeraConsulta.TerapeuticoDatosGenerales;
                model.TerapeuticoActividadesDiarias = primeraConsulta.TerapeuticoActividadesDiarias;
                model.TerapeuticoConQuienVive = primeraConsulta.TerapeuticoConQuienVive;
                model.TerapeuticoHabitosAlimenticios = primeraConsulta.TerapeuticoHabitosAlimenticios;
                model.TerapeuticoEjercicio = primeraConsulta.TerapeuticoEjercicio;
                model.TerapeuticoFinesSemana = primeraConsulta.TerapeuticoFinesSemana;
                model.TerapeuticoHistoriaMedica = primeraConsulta.TerapeuticoHistoriaMedica;
                model.TerapeuticoHistoriaPeso = primeraConsulta.TerapeuticoHistoriaPeso;

                model.TerapeuticoHistoricoObservaciones = new List<HistoricoObservacionesTerapeuticoConsultaViewModel>();

                foreach (var consultaPaciente in consultasPaciente)
                {
                    model.TerapeuticoHistoricoObservaciones.Add(new HistoricoObservacionesTerapeuticoConsultaViewModel
                    {
                        Fecha = consultaPaciente.FechaYHoraInicioConsulta,
                        Observaciones = consultaPaciente.TerapeuticoObservaciones
                    });
                }
            }

            //Rangos saludables
            var rangos = _pacientesRepository.GetRangosSaludablesPaciente((int)cita.PacienteId)
                .OrderBy(r => r.Fecha);
            if (rangos != null)
            {
                foreach (var rangoPaciente in rangos)
                {
                    model.RangosSaludablesHistorico.Add(new RangoSaludableConsultaViewModel
                    {
                        Id = rangoPaciente.Id,
                        Fecha = rangoPaciente.Fecha,
                        IMC = rangoPaciente.IMC,
                        Peso = rangoPaciente.Peso,
                        PorcentajeGrasaCorporal = rangoPaciente.PorcentajeGrasaCorporal
                    });
                }
            }

            model.Init(_citasRepository, _serviciosRepository, _pacientesRepository);

            return View(model);
        }

        [HttpPost]
        public string ConsultarServicios()
        {
            try
            {
                var serviciosExistentes = new List<ConsultaServicioExistenteViewModel>();
                var serviciosBd = _serviciosRepository.GetListaServicios();
                if (serviciosBd != null)
                {
                    foreach (var servicio in serviciosBd)
                    {
                        serviciosExistentes.Add(new ConsultaServicioExistenteViewModel
                        {
                            Id = servicio.Id,
                            CodigoInterno = servicio.CodigoInterno,
                            NombreServicio = servicio.NombreServicio,
                            ServicioNombreCodigo = servicio.CodigoInterno != null ? servicio.NombreServicio
                            + " - " +
                            servicio.CodigoInterno
                            : servicio.NombreServicio
                        });
                    }
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = serviciosExistentes
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar servicios: " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarHabitacionesDisponibles()
        {
            try
            {
                var habitacionesDisponibles = _hospitalizacionService.GetHabitaciones(true, false);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = habitacionesDisponibles
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar habitaciones disponibles: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Este metodo se utiliza para traer los productos existentes para la 
        /// receta media
        /// 16-08-2024: AUTOR: JUAN PABLO ALFONSO
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string ConsultarProductosExistentes()
        {
            int bodegaFarmaciaId = 1;
            try
            {
                var inventario = _productosService.GetInventario(null, bodegaFarmaciaId);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = inventario
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar productos existentes. " + ex.Message
                });
            }
        }

        [HttpPost]
        public string ConsultarPreciosServicio(int servicioId)
        {
            try
            {
                var preciosServicio = new List<ConsultasPrecioServicioViewModel>();
                var preciosBd = _serviciosRepository.GetPreciosServicio(servicioId)
                    .Where(a => a.Activar)
                    .ToList();
                if (preciosBd != null)
                {
                    foreach (var precio in preciosBd)
                    {
                        preciosServicio.Add(new ConsultasPrecioServicioViewModel
                        {
                            PrecioId = precio.PrecioId,
                            PrecioNombre = precio.Precio.NombrePrecio,
                            PrecioNombreValor = precio.Precio.NombrePrecio + " - " + precio.Valor,
                            PrecioValor = precio.Valor
                        });
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = preciosServicio
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar precios. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string IniciarConsulta(ConsultasViewModel model)
        {
            try
            {
                Console.WriteLine(model.ObservacionesAdicionales);
                Console.WriteLine(model.FechaYHoraInicio);
                Console.WriteLine(model.CostoConsulta);
                Console.WriteLine(model.FaseTratamientoId);
                Console.WriteLine(model.EstadoPagoId);

                var paciente = _pacientesRepository.Get((int)model.PacienteId);

                // var historia = new Historia()
                // {
                //     HistoriaProblema = model.HistoriaProblema,
                //     Sintomas = model.Sintomas,
                //     Diagnostico = model.Diagnostico
                // };

                // var examenFisico = new ExamenFisico()
                // {
                //     Temperatura = model.ExamenFisicoTemperatura,
                //     FrecuenciaRespiratoria = model.ExamenFisicoFrecuenciaRespiratoria,
                //     FrecuenciaCardiaca = model.ExamenFisicoFrecuenciaCardiaca,

                // };
                // var numeroSemanasEmbarazo = 0;
                // try { numeroSemanasEmbarazo = Convert.ToInt32(model.NumeroSemanasEmbarazo); } catch { numeroSemanasEmbarazo = 0; }
                var consulta = new Consulta
                {
                    //General
                    CitasId = model.CitaId,
                    ObservacionesAdicionales = model.ObservacionesAdicionales,
                    FechaYHoraInicioConsulta = (DateTime)model.FechaYHoraInicio,
                    CostoConsulta = (decimal)model.CostoConsulta,
                    FaseTratamientoId = model.FaseTratamientoId,
                    EstadoPagoConsultaId = (int)model.EstadoPagoId,
                    Cie10Codigo = model.Cie10Codigo,
                    // Historia = historia,
                    // ExamenFisico = examenFisico,
                    // ConsultaAntNoPatologicosGinecologia = new ConsultaAntNoPatologicosGinecologia(),
                    // ConsultaAntPatologicosGinecologia = new ConsultaAntPatologicosGinecologia(),
                    // ConsultaExamenFisicoGinecologia = new ConsultaExamenFisicoGinecologia(),


                };


                //Servicios
                var servicios = new List<ConsultaServicio>();
                if (model.ServiciosAgregados != null && model.ServiciosAgregados.Count > 0)
                {
                    foreach (var servicioAg in model.ServiciosAgregados)
                    {
                        if (servicioAg.Aplicar)
                        {
                            servicios.Add(new ConsultaServicio
                            {
                                ServicioId = (int)servicioAg.ServicioId,
                                NumeroDiente = servicioAg.NumeroDiente,
                                PrecioId = servicioAg.PrecioId,
                                PrecioValor = servicioAg.PrecioValor,
                                Cantidad = servicioAg.ServicioCantidad,
                                PrecioCubiertoSeguro = servicioAg.ServicioValorCubiertoSeguro,
                                PrecioCopago = servicioAg.ServicioValorCopago
                            });

                            _serviciosRepository.ActualizarInventarioVentaServicio((int)servicioAg.ServicioId);
                        }
                    }
                }
                consulta.ConsultasServicios = servicios;


                var examenes = new List<ConsultaExamenLabClinico>();
                if (model.ExamenesAgregados != null && model.ExamenesAgregados.Count > 0)
                {
                    foreach (var examen in model.ExamenesAgregados)
                    {
                        Console.WriteLine($"Agregando ExamenId: {examen.ExamenId}, PrecioId: {examen.PrecioId}, PrecioValor: {examen.ValorUnitario}, Cantidad: {examen.Cantidad}");

                        examenes.Add(new ConsultaExamenLabClinico
                        {
                            ExamenLabClinicoId = examen.ExamenId, // Corregido aquí
                            PrecioId = examen.PrecioId,
                            PrecioValor = examen.ValorUnitario,
                            Cantidad = examen.Cantidad,
                            PrecioValorCubiertoSeguro = examen.ValorCubiertoSeguro,
                            PrecioValorCopago = examen.ValorCopago,
                            FechaRegistro = examen.FechaRegistro ?? DateTime.Now, // Usa la fecha si existe, sino pone la actual
                            Pagado = examen.Pagado
                        });
                    }
                }
                consulta.ConsultaExamenesAgregados = examenes;



                //Caracteristicas dentales
                // var caracteristicas = new List<ConsultaCaracteristicaDental>();
                // foreach (var caracteristica in model.CaracteristicasDientes)
                // {
                //     caracteristicas.Add(new ConsultaCaracteristicaDental
                //     {
                //         NumeroDiente = caracteristica.NumeroDiente,
                //         Percusiones_VerticalMas = caracteristica.Percusiones_VerticalMas,
                //         Percusiones_HorizontalMas = caracteristica.Percusiones_HorizontalMas,
                //         Percusiones_VerticalMenos = caracteristica.Percusiones_VerticalMenos,
                //         Percusiones_HorizontalMenos = caracteristica.Percusiones_HorizontalMenos,
                //         Dolor_Localizado = caracteristica.Dolor_Localizado,
                //         Dolor_Fugaz = caracteristica.Dolor_Fugaz,
                //         Dolor_Persistente = caracteristica.Dolor_Persistente,
                //         Dolor_Referido = caracteristica.Dolor_Referido,
                //         Dolor_Espontaneo = caracteristica.Dolor_Espontaneo,
                //         Estimulo_Frio = caracteristica.Estimulo_Frio,
                //         Estimulo_Calor = caracteristica.Estimulo_Calor,
                //         Estimulo_DulceAcido = caracteristica.Estimulo_DulceAcido,
                //         Estimulo_Masticacion = caracteristica.Estimulo_Masticacion,
                //         Estimulo_Otro = caracteristica.Estimulo_Otro,
                //         TermicaFrio_Positiva = caracteristica.TermicaFrio_Positiva,
                //         TermicaFrio_Negativa = caracteristica.TermicaFrio_Negativa,
                //         TermicaFrio_Localizada = caracteristica.TermicaFrio_Localizada,
                //         TermicaFrio_Fugaz = caracteristica.TermicaFrio_Fugaz,
                //         TermicaFrio_Incrementa = caracteristica.TermicaFrio_Incrementa,
                //         TermicaFrio_Referida = caracteristica.TermicaFrio_Referida,
                //         TermicaFrio_Irradiado = caracteristica.TermicaFrio_Irradiado,
                //         TermicaFrio_Persistente = caracteristica.TermicaFrio_Persistente,
                //         TermicaFrio_Decrece = caracteristica.TermicaFrio_Decrece,
                //         TermicaCalor_Positiva = caracteristica.TermicaCalor_Positiva,
                //         TermicaCalor_Negativa = caracteristica.TermicaCalor_Negativa,
                //         TermicaCalor_Localizada = caracteristica.TermicaCalor_Localizada,
                //         TermicaCalor_Fugaz = caracteristica.TermicaCalor_Fugaz,
                //         TermicaCalor_Incrementa = caracteristica.TermicaCalor_Incrementa,
                //         TermicaCalor_Referida = caracteristica.TermicaCalor_Referida,
                //         TermicaCalor_Irradiado = caracteristica.TermicaCalor_Irradiado,
                //         TermicaCalor_Persistente = caracteristica.TermicaCalor_Persistente,
                //         TermicaCalor_Decrece = caracteristica.TermicaCalor_Decrece,
                //         Diagnostico_ManchaBlanca = caracteristica.Diagnostico_ManchaBlanca,
                //         Diagnostico_Caries = caracteristica.Diagnostico_Caries,
                //         Diagnostico_Traumatismo = caracteristica.Diagnostico_Traumatismo,
                //         Diagnostico_Abfraccion = caracteristica.Diagnostico_Abfraccion,
                //         Diagnostico_Atricion = caracteristica.Diagnostico_Atricion,
                //         Diagnostico_Erosion = caracteristica.Diagnostico_Erosion,
                //         Diagnostico_Restauracion = caracteristica.Diagnostico_Restauracion,
                //         Diagnostico_Ajustada = caracteristica.Diagnostico_Ajustada,
                //         Diagnostico_Desajustada = caracteristica.Diagnostico_Desajustada,
                //         Diagnostico_PulpaSana = caracteristica.Diagnostico_PulpaSana,
                //         Diagnostico_PulpitisReversible = caracteristica.Diagnostico_PulpitisReversible,
                //         Diagnostico_PulpitisIrreversible = caracteristica.Diagnostico_PulpitisIrreversible,
                //         Diagnostico_NecrosisPulpar = caracteristica.Diagnostico_NecrosisPulpar
                //     });
                // }
                // consulta.ConsultasCaracteristicasDentales = caracteristicas;

                //Seguimientos nutricionales
                // if (model.SeguimientosNutricionales != null
                //  && model.SeguimientosNutricionales.Count > 0)
                // {
                //     foreach (var seguimiento in model.SeguimientosNutricionales)
                //     {
                //         var seguimientoConsulta = new PacienteSeguimientoNutricional
                //         {
                //             PacienteId = model.PacienteId,
                //             Fecha = seguimiento.Fecha,
                //             Peso = seguimiento.Peso,
                //             IMC = seguimiento.IMC,
                //             PGC = seguimiento.PGC,
                //             Cuello = seguimiento.Cuello,
                //             Busto = seguimiento.Busto,
                //             CinturaAbdomen = seguimiento.CinturaAbdomen,
                //             Cadera = seguimiento.Cadera,
                //             Muslo = seguimiento.Muslo,
                //             Brazo = seguimiento.Brazo,
                //             Muñeca = seguimiento.Munneca,
                //         };
                //         if (seguimiento.Id != 0)
                //         {
                //             seguimientoConsulta.Id = (int)seguimiento.Id;
                //             _pacientesRepository.UpdateSeguimientoNutricional(seguimientoConsulta);
                //         }
                //         else
                //         {
                //             _pacientesRepository.AddSeguimientoNutricional(seguimientoConsulta);
                //         }
                //     }
                // }

                //Resultados examenes laboratorio
                // if (model.ExamenesLaboratorio != null
                //  && model.ExamenesLaboratorio.Count > 0)
                // {
                //     foreach (var examen in model.ExamenesLaboratorio)
                //     {
                //         Console.WriteLine($"Guardando Examen: {examen.Fecha}, GlucosaPre: {examen.GlucosaPre}, Triglicéridos: {examen.Trigliceridos}");

                //         var examenLaboratorioConsulta = new PacienteResultadoExamenLaboratorio
                //         {
                //             PacienteId = model.PacienteId,
                //             Fecha = examen.Fecha,
                //             GlucosaPre = examen.GlucosaPre,
                //             GlucosaPos = examen.GlucosaPos,
                //             HemoglobinaGlicosilada = examen.HemoglobinaGlicosilada,
                //             CurvaGlucosa = examen.CurvaGlucosa,
                //             ColesterolTotal = examen.ColesterolTotal,
                //             Trigliceridos = examen.Trigliceridos,
                //             PerfilLipidico = examen.PerfilLipidico,
                //             Creatinina = examen.Creatinina,
                //             AcidoUrico = examen.AcidoUrico,
                //             Hemoglobina = examen.Hemoglobina,
                //             T3 = examen.T3,
                //             T4 = examen.T4,
                //             ExamenHeces = examen.ExamenHeces,
                //             ExamenOrina = examen.ExamenOrina,
                //             Otros = examen.Otros,
                //             //UrlResultados = examen.UrlResultados
                //         };
                //         if (examen.Id != null)
                //         {
                //             examenLaboratorioConsulta.Id = (int)examen.Id;
                //             _pacientesRepository.UpdateResultadoExamenLaboratorio(examenLaboratorioConsulta);
                //         }
                //         else
                //         {
                //             _pacientesRepository.AddResultadoExamenLaboratorio(examenLaboratorioConsulta);
                //         }
                //     }
                // }


                //Vacunas paciente
                // var vacunasAgregadas = new List<VacunaPaciente>();
                // foreach (var vacuna in model.VacunasPaciente)
                // {
                //     var vacunaPaciente = new VacunaPaciente
                //     {
                //         PacienteId = (int)model.PacienteId,
                //         VacunaId = (int)vacuna.VacunaId,
                //         Primera = (bool)vacuna.Primera,
                //         FechaPrimera = vacuna.FechaPrimera,
                //         Segunda = (bool)vacuna.Segunda,
                //         FechaSegunda = vacuna.FechaSegunda,
                //         Tercera = (bool)vacuna.Tercera,
                //         FechaTercera = vacuna.FechaTercera,
                //         PrimerRefuerzo = (bool)vacuna.PrimerRefuerzo,
                //         FechaPrimerRefuerzo = vacuna.FechaPrimerRefuerzo,
                //         SegundoRefuerzo = (bool)vacuna.SegundoRefuerzo,
                //         FechaSegundoRefuerzo = vacuna.FechaSegundoRefuerzo
                //     };
                //     vacunasAgregadas.Add(vacunaPaciente);
                // }
                // var listaVacunasExistentes = _pacientesRepository.GetVacunasPaciente((int)model.PacienteId);
                // if (listaVacunasExistentes == null || listaVacunasExistentes.Count == 0)
                // {
                //     paciente.VacunasPaciente = vacunasAgregadas;
                //     _pacientesRepository.Update(paciente);
                // }

                // //Antecedentes familiares
                // var antecedentesAgregados = new List<PatologiaPaciente>();
                // if (model.AntecedentesFamiliaresPaciente != null)
                // {
                //     foreach (var antecedente in model.AntecedentesFamiliaresPaciente)
                //     {
                //         var antecedentePaciente = new PatologiaPaciente
                //         {
                //             PacienteId = paciente.Id,
                //             AbuelaMaterna = antecedente.AbuelaMaterna,
                //             AbuelaPaterna = antecedente.AbuelaPaterna,
                //             AbueloMaterno = antecedente.AbueloMaterno,
                //             AbueloPaterno = antecedente.AbueloPaterno,
                //             DescripcionOtraPatologia = antecedente.DescripcionOtraPatologia,
                //             Hermanos = antecedente.Hermanos,
                //             Madre = antecedente.Madre,
                //             OtrosMaterno = antecedente.OtrosMaterno,
                //             OtrosPaterno = antecedente.OtrosPaterno,
                //             Padre = antecedente.Padre,
                //             TipoPatologiaId = antecedente.TipoPatologiaId
                //         };
                //         antecedentesAgregados.Add(antecedentePaciente);
                //     }
                // }


                // paciente.PatologiasPaciente = antecedentesAgregados;
                // _pacientesRepository.Update(paciente);



                //Información médica
                // paciente.MedicaUsaLentesContacto = model.MedicaUsaLentesContacto;
                // paciente.MedicaUsaLentesContactoDescripcion = model.MedicaUsaLentesContactoDescripcion;
                // paciente.MedicaArticulacionesArtificiales = model.MedicaArticulacionesArtificiales;
                // paciente.MedicaArticulacionesArtificialesFecha = model.MedicaArticulacionesArtificialesFecha;
                // paciente.MedicaArticulacionesArtificialesComplicaciones = model.MedicaArticulacionesArtificialesComplicaciones;
                // paciente.MedicaTomaAlendronato = model.MedicaTomaAlendronato;
                // paciente.MedicaTomaAlendronatoFecha = model.MedicaTomaAlendronatoFecha;
                // paciente.MedicaTratamientoDolorHuesos = model.MedicaTratamientoDolorHuesos;
                // paciente.MedicaTratamientoDolorHuesosFechaInicio = model.MedicaTratamientoDolorHuesosFechaInicio;
                // paciente.MedicaTratamientoDolorHuesosDescripcionCaso = model.MedicaTratamientoDolorHuesosDescripcionCaso;
                // paciente.MedicaSustanciasReguladorasDrogas = model.MedicaSustanciasReguladorasDrogas;
                // paciente.MedicaSustanciasReguladorasDrogasFecha = model.MedicaSustanciasReguladorasDrogasFecha;
                // paciente.MedicaUsaTabaco = model.MedicaUsaTabaco;
                // paciente.MedicaBebidasAlcoholicas = model.MedicaBebidasAlcoholicas;
                // paciente.MedicaBebidasAlcoholicasDescripcion = model.MedicaBebidasAlcoholicasDescripcion;



                // //Información dental
                // paciente.DentalSangradoCepillar = model.DentalSangradoCepillar;
                // paciente.DentalDolorFrio = model.DentalDolorFrio;
                // paciente.DentalDolorPresionar = model.DentalDolorPresionar;
                // paciente.DentalObjetosAtorados = model.DentalObjetosAtorados;
                // paciente.DentalBocaSeca = model.DentalBocaSeca;
                // paciente.DentalTratamientoPeriondal = model.DentalTratamientoPeriondal;
                // paciente.DentalTratamientoOrtodoncia = model.DentalTratamientoOrtodoncia;
                // paciente.DentalProblemasTratamientoDental = model.DentalProblemasTratamientoDental;
                // paciente.DentalProblemasTratamientoDentalDescripcion = model.DentalProblemasTratamientoDentalDescripcion;
                // paciente.DentalFluoradaAguaDomicilio = model.DentalFluoradaAguaDomicilio;
                // paciente.DentalBebeAguaFiltrada = model.DentalBebeAguaFiltrada;
                // paciente.DentalDolorOidos = model.DentalDolorOidos;
                // paciente.DentalMolestiaRuidoAlto = model.DentalMolestiaRuidoAlto;
                // paciente.DentalMolestiaRuidoAltoDescripcion = model.DentalMolestiaRuidoAltoDescripcion;
                // paciente.DentalBrumismo = model.DentalBrumismo;
                // paciente.DentalLesiones = model.DentalLesiones;
                // paciente.DentalLesionesDescripcion = model.DentalLesionesDescripcion;
                // paciente.DentalDentaduraPlacas = model.DentalDentaduraPlacas;
                // paciente.DentalDentaduraPlacasDescripcion = model.DentalDentaduraPlacasDescripcion;
                // paciente.DentalActividadesRecreacion = model.DentalActividadesRecreacion;
                // paciente.DentalActividadesRecreacionDescripcion = model.DentalActividadesRecreacionDescripcion;
                // paciente.DentalLesionesCabeza = model.DentalLesionesCabeza;
                // paciente.DentalLesionesCabezaDescripcion = model.DentalLesionesCabezaDescripcion;

                var ncita = _citasRepository.GetCita((int)model.CitaId);

                // PASO 2: setear contador de finalización (solo si aún no existe)
                if (ncita.ContadorCitaFinalizada == null)
                {
                    ncita.ContadorCitaFinalizada = DateTime.UtcNow;
                    Console.WriteLine($"[FINALIZAR-CITA] CitaId={ncita.Id} ContadorCitaFinalizada seteado a {ncita.ContadorCitaFinalizada:O}");
                }

                ncita.Finalizada = true;
                ncita.EstadoTurno = model.EstadoTurno;

                _citasRepository.Update(ncita);
                Console.WriteLine($"[FINALIZAR-CITA] CitaId={ncita.Id} Finalizada={ncita.Finalizada} EstadoTurno={ncita.EstadoTurno}");




                // id real de la consulta recién creada
                var idConsultaRegistrada = _consultasRepository.AddConsulta(consulta);

                #region Oftalmologia

                // 1. Verificamos si hay algún dato cargado en la sección de Oftalmología
                bool hayOft = !(
                    // --- Motivo / Antecedentes ---
                    string.IsNullOrWhiteSpace(model.Oft_HistoriaEnfermedadActual)
                    && string.IsNullOrWhiteSpace(model.Oft_PacienteMedicos)
                    && string.IsNullOrWhiteSpace(model.Oft_PacienteQuirurgicos)
                    && string.IsNullOrWhiteSpace(model.Oft_PacienteTraumaticos)
                    && string.IsNullOrWhiteSpace(model.Oft_PacienteAlergias)
                    && string.IsNullOrWhiteSpace(model.Oft_PacienteFamiliares)

                    // --- Agudeza Visual SC ---
                    && string.IsNullOrWhiteSpace(model.Oft_AgudezaSC_Test)
                    && string.IsNullOrWhiteSpace(model.Oft_AgudezaSC_OD)
                    && string.IsNullOrWhiteSpace(model.Oft_AgudezaSC_OS)

                    // --- Contraste ---
                    && string.IsNullOrWhiteSpace(model.Oft_Contraste_OD)
                    && string.IsNullOrWhiteSpace(model.Oft_Contraste_OS)

                    // --- AV Cerca ---
                    && string.IsNullOrWhiteSpace(model.Oft_AVCerca_OD)
                    && string.IsNullOrWhiteSpace(model.Oft_AVCerca_OS)

                    // --- Tests Especiales ---
                    && string.IsNullOrWhiteSpace(model.Oft_TestIshihara_OD)
                    && string.IsNullOrWhiteSpace(model.Oft_TestIshihara_OS)
                    && string.IsNullOrWhiteSpace(model.Oft_TestEstereopsis_OD)
                    && string.IsNullOrWhiteSpace(model.Oft_TestEstereopsis_OS)

                    // --- Lensometría ---
                    && model.Oft_Lensometria_OD_Esfera == null
                    && model.Oft_Lensometria_OD_Cilindro == null
                    && model.Oft_Lensometria_OD_Eje == null
                    && string.IsNullOrWhiteSpace(model.Oft_Lensometria_OD_Agudeza)
                    && model.Oft_Lensometria_OS_Esfera == null
                    && model.Oft_Lensometria_OS_Cilindro == null
                    && model.Oft_Lensometria_OS_Eje == null
                    && string.IsNullOrWhiteSpace(model.Oft_Lensometria_OS_Agudeza)

                    // --- Óptica Final ---
                    && model.Oft_Final_OD_Esfera == null
                    && model.Oft_Final_OD_Cilindro == null
                    && model.Oft_Final_OD_Eje == null
                    && string.IsNullOrWhiteSpace(model.Oft_Final_OD_Agudeza)
                    && model.Oft_Final_OS_Esfera == null
                    && model.Oft_Final_OS_Cilindro == null
                    && model.Oft_Final_OS_Eje == null
                    && string.IsNullOrWhiteSpace(model.Oft_Final_OS_Agudeza)
                    && model.Oft_Final_Adicion == null
                    && model.Oft_Final_DIP_mm == null

                    // --- Retinoscopía ---
                    && model.Oft_Retino_OD_Esfera == null
                    && model.Oft_Retino_OD_Cilindro == null
                    && model.Oft_Retino_OD_Eje == null
                    && model.Oft_Retino_OS_Esfera == null
                    && model.Oft_Retino_OS_Cilindro == null
                    && model.Oft_Retino_OS_Eje == null

                    // --- Lentes / Material ---
                    && string.IsNullOrWhiteSpace(model.Oft_TipoLente)
                    && string.IsNullOrWhiteSpace(model.Oft_LenteMaterialTratamiento)

                    // --- Inspección (Segmento Anterior) ---
                    && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_MovExtraoculares_OD)
                    && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_MovExtraoculares_OS)
                    && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_Cejas_OD)
                    && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_Cejas_OS)
                    && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_ParpadosPestanas_OD)
                    && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_ParpadosPestanas_OS)
                    && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_ViaLagrimal_OD)
                    && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_ViaLagrimal_OS)
                    && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_Conjuntiva_OD)
                    && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_Conjuntiva_OS)
                    && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_CorneaEsclera_OD)
                    && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_CorneaEsclera_OS)
                    && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_CamaraAnteriorAngulo_OD)
                    && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_CamaraAnteriorAngulo_OS)
                    && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_IrisPupila_OD)
                    && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_IrisPupila_OS)
                    && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_Cristalino_OD)
                    && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_Cristalino_OS)
                    && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_BUT_OD)
                    && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_BUT_OS)
                    && model.Oft_Inspeccion_PresionIntraocular_OD == null
                    && model.Oft_Inspeccion_PresionIntraocular_OS == null

                    // --- Segmento Posterior ---
                    && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_Vitreo_OD)
                    && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_Vitreo_OS)
                    && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_NervioOptico_OD)
                    && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_NervioOptico_OS)
                    && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_Macula_OD)
                    && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_Macula_OS)
                    && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_Retina_OD)
                    && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_Retina_OS)

                    // --- Impresión Clínica ---
                    && string.IsNullOrWhiteSpace(model.Oft_HistoriaClinicaImpresionClinica)
                    && string.IsNullOrWhiteSpace(model.Oft_HistoriaClinicaComentario)
                );

                // 2. Si hay datos y tenemos paciente, guardamos
                if (hayOft && model.PacienteId.HasValue)
                {
                    var oft = new ConsultasOftalmologia
                    {
                        ConsultaId = idConsultaRegistrada,
                        PacienteId = model.PacienteId.Value,

                        // Motivo / Antecedentes
                        HistoriaEnfermedadActual = model.Oft_HistoriaEnfermedadActual,
                        PacienteMedicos = model.Oft_PacienteMedicos,
                        PacienteQuirurgicos = model.Oft_PacienteQuirurgicos,
                        PacienteTraumaticos = model.Oft_PacienteTraumaticos,
                        PacienteAlergias = model.Oft_PacienteAlergias,
                        PacienteFamiliares = model.Oft_PacienteFamiliares,

                        // Datos objetivos
                        AgudezaSC_Test = model.Oft_AgudezaSC_Test,
                        AgudezaSC_OD = model.Oft_AgudezaSC_OD,
                        AgudezaSC_OS = model.Oft_AgudezaSC_OS,

                        Contraste_OD = model.Oft_Contraste_OD,
                        Contraste_OS = model.Oft_Contraste_OS,

                        AVCerca_OD = model.Oft_AVCerca_OD,
                        AVCerca_OS = model.Oft_AVCerca_OS,

                        // Tests especiales
                        TestIshihara_OD = model.Oft_TestIshihara_OD,
                        TestIshihara_OS = model.Oft_TestIshihara_OS,
                        TestEstereopsis_OD = model.Oft_TestEstereopsis_OD,
                        TestEstereopsis_OS = model.Oft_TestEstereopsis_OS,

                        // Óptica - Lensometría
                        Lensometria_OD_Esfera = model.Oft_Lensometria_OD_Esfera,
                        Lensometria_OD_Cilindro = model.Oft_Lensometria_OD_Cilindro,
                        Lensometria_OD_Eje = model.Oft_Lensometria_OD_Eje,
                        Lensometria_OD_Agudeza = model.Oft_Lensometria_OD_Agudeza,

                        Lensometria_OS_Esfera = model.Oft_Lensometria_OS_Esfera,
                        Lensometria_OS_Cilindro = model.Oft_Lensometria_OS_Cilindro,
                        Lensometria_OS_Eje = model.Oft_Lensometria_OS_Eje,
                        Lensometria_OS_Agudeza = model.Oft_Lensometria_OS_Agudeza,

                        // Óptica - Final
                        Final_OD_Esfera = model.Oft_Final_OD_Esfera,
                        Final_OD_Cilindro = model.Oft_Final_OD_Cilindro,
                        Final_OD_Eje = model.Oft_Final_OD_Eje,
                        Final_OD_Agudeza = model.Oft_Final_OD_Agudeza,

                        Final_OS_Esfera = model.Oft_Final_OS_Esfera,
                        Final_OS_Cilindro = model.Oft_Final_OS_Cilindro,
                        Final_OS_Eje = model.Oft_Final_OS_Eje,
                        Final_OS_Agudeza = model.Oft_Final_OS_Agudeza,

                        Final_Adicion = model.Oft_Final_Adicion,
                        Final_DIP_mm = model.Oft_Final_DIP_mm,

                        // Óptica - Retinoscopía
                        Retino_OD_Esfera = model.Oft_Retino_OD_Esfera,
                        Retino_OD_Cilindro = model.Oft_Retino_OD_Cilindro,
                        Retino_OD_Eje = model.Oft_Retino_OD_Eje,

                        Retino_OS_Esfera = model.Oft_Retino_OS_Esfera,
                        Retino_OS_Cilindro = model.Oft_Retino_OS_Cilindro,
                        Retino_OS_Eje = model.Oft_Retino_OS_Eje,

                        // Tipo de lente / Material
                        TipoLente = model.Oft_TipoLente,
                        LenteMaterialTratamiento = model.Oft_LenteMaterialTratamiento,

                        // Inspección - LH / Oftalmoscopía
                        Inspeccion_MovExtraoculares_OD = model.Oft_Inspeccion_MovExtraoculares_OD,
                        Inspeccion_MovExtraoculares_OS = model.Oft_Inspeccion_MovExtraoculares_OS,
                        Inspeccion_Cejas_OD = model.Oft_Inspeccion_Cejas_OD,
                        Inspeccion_Cejas_OS = model.Oft_Inspeccion_Cejas_OS,
                        Inspeccion_ParpadosPestanas_OD = model.Oft_Inspeccion_ParpadosPestanas_OD,
                        Inspeccion_ParpadosPestanas_OS = model.Oft_Inspeccion_ParpadosPestanas_OS,
                        Inspeccion_ViaLagrimal_OD = model.Oft_Inspeccion_ViaLagrimal_OD,
                        Inspeccion_ViaLagrimal_OS = model.Oft_Inspeccion_ViaLagrimal_OS,

                        // Segmento anterior
                        Inspeccion_Conjuntiva_OD = model.Oft_Inspeccion_Conjuntiva_OD,
                        Inspeccion_Conjuntiva_OS = model.Oft_Inspeccion_Conjuntiva_OS,
                        Inspeccion_CorneaEsclera_OD = model.Oft_Inspeccion_CorneaEsclera_OD,
                        Inspeccion_CorneaEsclera_OS = model.Oft_Inspeccion_CorneaEsclera_OS,
                        Inspeccion_CamaraAnteriorAngulo_OD = model.Oft_Inspeccion_CamaraAnteriorAngulo_OD,
                        Inspeccion_CamaraAnteriorAngulo_OS = model.Oft_Inspeccion_CamaraAnteriorAngulo_OS,
                        Inspeccion_IrisPupila_OD = model.Oft_Inspeccion_IrisPupila_OD,
                        Inspeccion_IrisPupila_OS = model.Oft_Inspeccion_IrisPupila_OS,
                        Inspeccion_Cristalino_OD = model.Oft_Inspeccion_Cristalino_OD,
                        Inspeccion_Cristalino_OS = model.Oft_Inspeccion_Cristalino_OS,
                        Inspeccion_BUT_OD = model.Oft_Inspeccion_BUT_OD,
                        Inspeccion_BUT_OS = model.Oft_Inspeccion_BUT_OS,
                        Inspeccion_PresionIntraocular_OD = model.Oft_Inspeccion_PresionIntraocular_OD,
                        Inspeccion_PresionIntraocular_OS = model.Oft_Inspeccion_PresionIntraocular_OS,

                        // Segmento posterior
                        Inspeccion_Vitreo_OD = model.Oft_Inspeccion_Vitreo_OD,
                        Inspeccion_Vitreo_OS = model.Oft_Inspeccion_Vitreo_OS,
                        Inspeccion_NervioOptico_OD = model.Oft_Inspeccion_NervioOptico_OD,
                        Inspeccion_NervioOptico_OS = model.Oft_Inspeccion_NervioOptico_OS,
                        Inspeccion_Macula_OD = model.Oft_Inspeccion_Macula_OD,
                        Inspeccion_Macula_OS = model.Oft_Inspeccion_Macula_OS,
                        Inspeccion_Retina_OD = model.Oft_Inspeccion_Retina_OD,
                        Inspeccion_Retina_OS = model.Oft_Inspeccion_Retina_OS,

                        // Impresión clínica / Tratamiento
                        HistoriaClinicaImpresionClinica = model.Oft_HistoriaClinicaImpresionClinica,
                        HistoriaClinicaComentario = model.Oft_HistoriaClinicaComentario,

                        // Fecha (Campo que faltaba en el modelo pero existe en BD)
                        Fecha = DateTime.Now
                    };

                    _oftRepo.Add(oft);
                }
                // ==== FIN OFTALMOLOGÍA ====
                #endregion

                #region Podologia

                bool hayPod =
                    // 1) Antecedentes
                    ((model.Pod_Enfermedades != null && model.Pod_Enfermedades.Length > 0)
                     || !string.IsNullOrWhiteSpace(model.Pod_Enfermedades_Otros)
                     || !string.IsNullOrWhiteSpace(model.Pod_Medicamentos)
                     || !string.IsNullOrWhiteSpace(model.Pod_PresionArterial)
                    )
                    // 2) Examen del Pie
                    || !string.IsNullOrWhiteSpace(model.Pod_Pulso_Pedio)
                    || !string.IsNullOrWhiteSpace(model.Pod_Pulso_TibialPosterior)
                    || !string.IsNullOrWhiteSpace(model.Pod_Pulso_Popliteo)
                    || !string.IsNullOrWhiteSpace(model.Pod_TemperaturaPie)
                    || model.Pod_ProblemasCirculatorios.HasValue
                    || !string.IsNullOrWhiteSpace(model.Pod_EstadoPiel)
                    || !string.IsNullOrWhiteSpace(model.Pod_ObservacionesExamen)
                    // 3) Tratamiento
                    || (model.Pod_Procedimientos != null && model.Pod_Procedimientos.Length > 0)
                    || !string.IsNullOrWhiteSpace(model.Pod_OtrosProcedimientos)
                    || !string.IsNullOrWhiteSpace(model.Pod_ObservacionesTratamiento)
                    // 4) Indicaciones y Datos Finales
                    || !string.IsNullOrWhiteSpace(model.Pod_Indicaciones)
                    || model.Pod_PesoKg.HasValue
                    || model.Pod_EstaturaM.HasValue
                    || model.Pod_FechaAtencion.HasValue
                    || !string.IsNullOrWhiteSpace(model.Pod_Profesional);

                if (hayPod && model.PacienteId.HasValue)
                {
                    var pod = new ConsultasPodologia
                    {
                        // OJO: usar SIEMPRE el id devuelto por AddConsulta en el flujo de crear
                        ConsultaId = idConsultaRegistrada,       // int
                        PacienteId = model.PacienteId.Value,     // int

                        // 1) Antecedentes
                        Enfermedades = model.Pod_Enfermedades ?? Array.Empty<string>(),
                        Enfermedades_Otros = model.Pod_Enfermedades_Otros,
                        Medicamentos = model.Pod_Medicamentos,
                        PresionArterial = model.Pod_PresionArterial,

                        // 2) Examen del Pie
                        Pulso_Pedio = model.Pod_Pulso_Pedio,
                        Pulso_TibialPosterior = model.Pod_Pulso_TibialPosterior,
                        Pulso_Popliteo = model.Pod_Pulso_Popliteo,
                        TemperaturaPie = model.Pod_TemperaturaPie,
                        ProblemasCirculatorios = model.Pod_ProblemasCirculatorios,
                        EstadoPiel = model.Pod_EstadoPiel,
                        ObservacionesExamen = model.Pod_ObservacionesExamen,

                        // 3) Tratamiento
                        Procedimientos = model.Pod_Procedimientos ?? Array.Empty<string>(),
                        OtrosProcedimientos = model.Pod_OtrosProcedimientos,
                        ObservacionesTratamiento = model.Pod_ObservacionesTratamiento,

                        // 4) Indicaciones y Datos Finales
                        Indicaciones = model.Pod_Indicaciones,
                        PesoKg = model.Pod_PesoKg,
                        EstaturaM = model.Pod_EstaturaM,
                        FechaAtencion = model.Pod_FechaAtencion,
                        Profesional = model.Pod_Profesional
                    };

                    _podRepo.Add(pod);
                }
                // ==== FIN PODOLOGÍA ====
                #endregion

                #region Historia Clínica de Enfermería

                bool hayHce =
                    !string.IsNullOrWhiteSpace(model.Hce_TipoConsulta)
                    || !string.IsNullOrWhiteSpace(model.Hce_MotivoConsulta)

                    // 3) Antecedentes
                    || !string.IsNullOrWhiteSpace(model.Hce_AntecedentesPatologicos)
                    || !string.IsNullOrWhiteSpace(model.Hce_AntecedentesQuirurgicos)
                    || !string.IsNullOrWhiteSpace(model.Hce_AntecedentesTraumaticos)
                    || !string.IsNullOrWhiteSpace(model.Hce_Hospitalizaciones)
                    || !string.IsNullOrWhiteSpace(model.Hce_Alergias)
                    || !string.IsNullOrWhiteSpace(model.Hce_AntecedentesFamiliares)

                    // 4) Hábitos
                    || !string.IsNullOrWhiteSpace(model.Hce_HabitoAlimentacion)
                    || !string.IsNullOrWhiteSpace(model.Hce_ActividadFisica)
                    || !string.IsNullOrWhiteSpace(model.Hce_HabitoAlcoholTexto)
                    || !string.IsNullOrWhiteSpace(model.Hce_HabitoTabacoTexto)
                    || !string.IsNullOrWhiteSpace(model.Hce_OtrosHabitos)

                    // 5) Signos vitales y antropometría
                    || !string.IsNullOrWhiteSpace(model.Hce_PresionArterialTxt)
                    || model.Hce_FC.HasValue || model.Hce_FR.HasValue
                    || model.Hce_TemperaturaC.HasValue || model.Hce_SPO2.HasValue
                    || model.Hce_PesoKg.HasValue || model.Hce_TallaM.HasValue || model.Hce_IMC.HasValue

                    // 6) Exploración por sistemas
                    || !string.IsNullOrWhiteSpace(model.Hce_CabezaCuello)
                    || !string.IsNullOrWhiteSpace(model.Hce_ToraxPulmones)
                    || !string.IsNullOrWhiteSpace(model.Hce_Corazon)
                    || !string.IsNullOrWhiteSpace(model.Hce_Abdomen)
                    || !string.IsNullOrWhiteSpace(model.Hce_Extremidades)
                    || !string.IsNullOrWhiteSpace(model.Hce_SistemaNeurologico)
                    || !string.IsNullOrWhiteSpace(model.Hce_PielAnexos)

                    // 8) Valoración de enfermería
                    || !string.IsNullOrWhiteSpace(model.Hce_ValConcienciaOrientacion)
                    || !string.IsNullOrWhiteSpace(model.Hce_ValEstadoNutricional)
                    || !string.IsNullOrWhiteSpace(model.Hce_ValEliminacion)
                    || !string.IsNullOrWhiteSpace(model.Hce_ValSuenoDescanso)
                    || !string.IsNullOrWhiteSpace(model.Hce_ValActividadMovilidad)
                    || !string.IsNullOrWhiteSpace(model.Hce_ValAutonomia)

                    // 9–11) Labs, Dx, Plan, Seguimiento
                    || !string.IsNullOrWhiteSpace(model.Hce_Laboratorios)
                    || !string.IsNullOrWhiteSpace(model.Hce_DiagnosticoEnfermeria)
                    || !string.IsNullOrWhiteSpace(model.Hce_AccionesRealizadas)
                    || !string.IsNullOrWhiteSpace(model.Hce_MedicamentosAdministrados)
                    || !string.IsNullOrWhiteSpace(model.Hce_Tratamiento)
                    || !string.IsNullOrWhiteSpace(model.Hce_Seguimiento);

                if (hayHce && model.PacienteId.HasValue)
                {
                    var hce = new ConsultasHistoriaClinicaEnfermeria
                    {
                        ConsultaId = idConsultaRegistrada,
                        PacienteId = model.PacienteId.Value,

                        // 1–2
                        TipoConsulta = model.Hce_TipoConsulta,
                        MotivoConsulta = model.Hce_MotivoConsulta,

                        // 3) Antecedentes
                        AntecedentesPatologicos = model.Hce_AntecedentesPatologicos,
                        AntecedentesQuirurgicos = model.Hce_AntecedentesQuirurgicos,
                        AntecedentesTraumaticos = model.Hce_AntecedentesTraumaticos,
                        Hospitalizaciones = model.Hce_Hospitalizaciones,
                        Alergias = model.Hce_Alergias,
                        AntecedentesFamiliares = model.Hce_AntecedentesFamiliares,

                        // 4) Hábitos
                        HabitoAlimentacion = model.Hce_HabitoAlimentacion,
                        ActividadFisica = model.Hce_ActividadFisica,
                        HabitoAlcoholTexto = model.Hce_HabitoAlcoholTexto,
                        HabitoTabacoTexto = model.Hce_HabitoTabacoTexto,
                        OtrosHabitos = model.Hce_OtrosHabitos,

                        // 5) Signos vitales y antropometría
                        PresionArterialTxt = model.Hce_PresionArterialTxt,
                        FC = model.Hce_FC,
                        FR = model.Hce_FR,
                        TemperaturaC = model.Hce_TemperaturaC,
                        SPO2 = model.Hce_SPO2,
                        PesoKg = model.Hce_PesoKg,
                        TallaM = model.Hce_TallaM,
                        IMC = model.Hce_IMC,

                        // 6) Exploración por sistemas
                        CabezaCuello = model.Hce_CabezaCuello,
                        ToraxPulmones = model.Hce_ToraxPulmones,
                        Corazon = model.Hce_Corazon,
                        Abdomen = model.Hce_Abdomen,
                        Extremidades = model.Hce_Extremidades,
                        SistemaNeurologico = model.Hce_SistemaNeurologico,
                        PielAnexos = model.Hce_PielAnexos,

                        // 8) Valoración de enfermería
                        ValConcienciaOrientacion = model.Hce_ValConcienciaOrientacion,
                        ValEstadoNutricional = model.Hce_ValEstadoNutricional,
                        ValEliminacion = model.Hce_ValEliminacion,
                        ValSuenoDescanso = model.Hce_ValSuenoDescanso,
                        ValActividadMovilidad = model.Hce_ValActividadMovilidad,
                        ValAutonomia = model.Hce_ValAutonomia,

                        // 9) Laboratorios
                        Laboratorios = model.Hce_Laboratorios,

                        // 10) Diagnóstico
                        DiagnosticoEnfermeria = model.Hce_DiagnosticoEnfermeria,

                        // 11) Plan de cuidados / Intervenciones
                        AccionesRealizadas = model.Hce_AccionesRealizadas,
                        MedicamentosAdministrados = model.Hce_MedicamentosAdministrados,
                        Tratamiento = model.Hce_Tratamiento,

                        // 12) Seguimiento
                        Seguimiento = model.Hce_Seguimiento
                    };

                    _enfRepo.Add(hce);
                }
                #endregion

                #region ValoracionInicialEnfermeria
                bool hayVe =
                    !(
                        string.IsNullOrWhiteSpace(model.Ve_Motivo)
                        && string.IsNullOrWhiteSpace(model.Ve_DiagnosticoMedico)
                        && string.IsNullOrWhiteSpace(model.Ve_Labs)
                        && (model.Ve_Medio == null || model.Ve_Medio.Length == 0)
                        && (model.Ve_Resp == null || model.Ve_Resp.Length == 0)
                        && (model.Ve_Circ == null || model.Ve_Circ.Length == 0)
                        && (model.Ve_Nutricion == null || model.Ve_Nutricion.Length == 0)
                        && string.IsNullOrWhiteSpace(model.Ve_NutricionObs)
                        && (model.Ve_Urinario == null || model.Ve_Urinario.Length == 0)
                        && (model.Ve_Intestinal == null || model.Ve_Intestinal.Length == 0)
                        && (model.Ve_Mov == null || model.Ve_Mov.Length == 0)
                        && (model.Ve_Conciencia == null || model.Ve_Conciencia.Length == 0)
                        && (model.Ve_Sueno == null || model.Ve_Sueno.Length == 0)
                        && string.IsNullOrWhiteSpace(model.Ve_Vestirse)
                        && string.IsNullOrWhiteSpace(model.Ve_Higiene)
                        && (model.Ve_Piel == null || model.Ve_Piel.Length == 0)
                        && string.IsNullOrWhiteSpace(model.Ve_PielUbicacion)
                        && (model.Ve_Lenguaje == null || model.Ve_Lenguaje.Length == 0)
                        && (model.Ve_Vision == null || model.Ve_Vision.Length == 0)
                        && (model.Ve_Oido == null || model.Ve_Oido.Length == 0)
                        && (model.Ve_Seg == null || model.Ve_Seg.Length == 0)
                        && string.IsNullOrWhiteSpace(model.Ve_Religiosos)
                        && string.IsNullOrWhiteSpace(model.Ve_CreenciasObservaciones)
                        && string.IsNullOrWhiteSpace(model.Ve_ConoceMotivo)
                        && string.IsNullOrWhiteSpace(model.Ve_NecesitaInfo)
                        && string.IsNullOrWhiteSpace(model.Ve_MedicacionActual)
                        && string.IsNullOrWhiteSpace(model.Ve_PlanTerapeutico)
                    );

                if (hayVe && model.PacienteId.HasValue)
                {
                    var ve = new ConsultasValoracionInicialEnfermeria
                    {
                        ConsultaId = idConsultaRegistrada,
                        PacienteId = model.PacienteId.Value,

                        Motivo = model.Ve_Motivo,
                        DiagnosticoMedico = model.Ve_DiagnosticoMedico,
                        Labs = model.Ve_Labs,

                        Medio = model.Ve_Medio ?? Array.Empty<string>(),

                        Resp = model.Ve_Resp ?? Array.Empty<string>(),
                        Circ = model.Ve_Circ ?? Array.Empty<string>(),

                        Nutricion = model.Ve_Nutricion ?? Array.Empty<string>(),
                        NutricionObs = model.Ve_NutricionObs,

                        Urinario = model.Ve_Urinario ?? Array.Empty<string>(),
                        Intestinal = model.Ve_Intestinal ?? Array.Empty<string>(),

                        Mov = model.Ve_Mov ?? Array.Empty<string>(),
                        Conciencia = model.Ve_Conciencia ?? Array.Empty<string>(),

                        Sueno = model.Ve_Sueno ?? Array.Empty<string>(),
                        Vestirse = model.Ve_Vestirse,
                        Higiene = model.Ve_Higiene,
                        Piel = model.Ve_Piel ?? Array.Empty<string>(),
                        PielUbicacion = model.Ve_PielUbicacion,

                        Lenguaje = model.Ve_Lenguaje ?? Array.Empty<string>(),
                        Vision = model.Ve_Vision ?? Array.Empty<string>(),
                        Oido = model.Ve_Oido ?? Array.Empty<string>(),

                        Seg = model.Ve_Seg ?? Array.Empty<string>(),
                        Religiosos = model.Ve_Religiosos,
                        CreenciasObservaciones = model.Ve_CreenciasObservaciones,
                        ConoceMotivo = model.Ve_ConoceMotivo,
                        NecesitaInfo = model.Ve_NecesitaInfo,

                        MedicacionActual = model.Ve_MedicacionActual,
                        PlanTerapeutico = model.Ve_PlanTerapeutico
                    };

                    _veRepo.Add(ve);
                }
                // ==== FIN VALORACIÓN INICIAL DE ENFERMERÍA ====
                #endregion

                #region Sueroterapia
                bool haySuero =
                    !(
                        string.IsNullOrWhiteSpace(model.Suero_Motivo)
                        && string.IsNullOrWhiteSpace(model.Suero_DiagnosticoMedico)
                        && string.IsNullOrWhiteSpace(model.Suero_Labs)
                        && (model.Suero_Medio == null || model.Suero_Medio.Length == 0)
                        && (model.Suero_Resp == null || model.Suero_Resp.Length == 0)
                        && (model.Suero_Circ == null || model.Suero_Circ.Length == 0)
                        && (model.Suero_Nutricion == null || model.Suero_Nutricion.Length == 0)
                        && string.IsNullOrWhiteSpace(model.Suero_NutricionObs)
                        && string.IsNullOrWhiteSpace(model.Suero_PlanTerapeutico)
                    );

                if (haySuero && model.PacienteId.HasValue)
                {
                    var suero = new ConsultasSueroterapia
                    {
                        ConsultaId = idConsultaRegistrada,
                        PacienteId = model.PacienteId.Value,

                        // 1) Valoración inicial
                        Motivo = model.Suero_Motivo,
                        DiagnosticoMedico = model.Suero_DiagnosticoMedico,
                        Labs = model.Suero_Labs,

                        // 2) Medio (checkbox múltiple)
                        Medio = model.Suero_Medio ?? Array.Empty<string>(),

                        // 3) Oxigenación / Circulación
                        Resp = model.Suero_Resp ?? Array.Empty<string>(),
                        Circ = model.Suero_Circ ?? Array.Empty<string>(),

                        // 4) Nutrición
                        Nutricion = model.Suero_Nutricion ?? Array.Empty<string>(),
                        NutricionObs = model.Suero_NutricionObs,

                        // 5) Plan
                        PlanTerapeutico = model.Suero_PlanTerapeutico
                    };

                    _sueroRepo.Add(suero);
                }
                #endregion

                #region HISTORICO OFTALMOLOGIA
                try
                {
                    var pacienteId = model.PacienteId.Value;
                    if (pacienteId > 0)
                    {
                        // Última consulta (resumen)
                        var oftUltima = _oftRepo.GetConsultaByPaciente(pacienteId);
                        if (oftUltima != null)
                        {
                            model.FechaUltimaConsulta = oftUltima.Fecha;
                            model.MotivoUltimaConsulta = string.IsNullOrWhiteSpace(oftUltima.HistoriaClinicaImpresionClinica)
                                ? "-"
                                : oftUltima.HistoriaClinicaImpresionClinica;
                        }

                        // Histórico completo
                        var oftHist = _oftRepo.GetConsultasByPaciente(pacienteId) ?? Enumerable.Empty<ConsultasOftalmologia>();

                        // (si te sirve la tabla-resumen, conserva este bloque)
                        var top = 10;
                        var filas = oftHist
                            .Take(top)
                            .Select(x => new
                            {
                                x.ConsultaId,
                                x.Fecha,
                                x.AgudezaSC_OD,
                                x.AgudezaSC_OS,
                                x.AVCerca_OD,
                                x.AVCerca_OS,
                                x.Final_OD_Esfera,
                                x.Final_OD_Cilindro,
                                x.Final_OD_Eje,
                                x.Final_OS_Esfera,
                                x.Final_OS_Cilindro,
                                x.Final_OS_Eje,
                                x.Inspeccion_PresionIntraocular_OD,
                                x.Inspeccion_PresionIntraocular_OS,
                                x.HistoriaClinicaImpresionClinica
                            })
                            .ToList();

                        ViewBag.OftHistorial = filas;
                        ViewBag.OftHistorialTotal = oftHist.Count();

                        // 👉 **CLAVE PARA MOSTRAR TODOS LOS CAMPOS**:
                        ViewBag.OftHistorialFull = oftHist.ToList(); // lista completa de ConsultasOftalmologia
                    }
                    else
                    {
                        ViewBag.OftHistorial = new List<object>();
                        ViewBag.OftHistorialTotal = 0;
                        ViewBag.OftHistorialFull = new List<ConsultasOftalmologia>();
                    }
                }
                catch
                {
                    ViewBag.OftHistorial = new List<object>();
                    ViewBag.OftHistorialTotal = 0;
                    ViewBag.OftHistorialFull = new List<ConsultasOftalmologia>();
                }
                #endregion


                TempData["Message"] = "¡La consulta se ha guardado con éxito.!";
                return JsonSerializer.Serialize(new { Exitoso = true, ConsultaId = idConsultaRegistrada });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Error al registrar consulta. " + ex.Message });
            }
        }

        [HttpPost]
        public string IniciarConsultaYPagar(ConsultasViewModel model)
        {
            try
            {
                var paciente = _pacientesRepository.Get((int)model.PacienteId);

                var historia = new Historia()
                {
                    HistoriaProblema = model.HistoriaProblema,
                    Sintomas = model.Sintomas,
                    Diagnostico = model.Diagnostico
                };
                var ExamenFisicoId = _citasRepository.GetCita((int)model.CitaId).ExamenFisicoId;
                var examenFisico = new ExamenFisico();
                if (ExamenFisicoId == null)
                {
                    examenFisico = new ExamenFisico()
                    {
                        Temperatura = model.ExamenFisicoTemperatura,
                        FrecuenciaRespiratoria = model.ExamenFisicoFrecuenciaRespiratoria,
                        FrecuenciaCardiaca = model.ExamenFisicoFrecuenciaCardiaca,
                        SaturacionDeOxigeno = model.ExamenFisicoSaturacionOxigeno,
                        PresionArterialBrazoDerecho = model.ExamenFisicoDerecho,
                        PresionArterialBrazoIzquierdo = model.ExamenFisicoIzquierdo, /* zzz */
                        PresionArterialMedia = model.ExamenFisicoPresionArterialMedia,
                        Observaciones = model.ExamenFisicoObservaciones,
                        Peso = model.ExamenFisicoPeso,
                        Talla = model.ExamenFisicoTalla,
                        Glucosa = model.ExamenFisicoGlucosa,
                        IMC = model.ExamenFisicoIMC
                    };
                    //ExamenFisicoId = examenFisico.Id;
                }
                else
                {

                    examenFisico = _citasRepository.GetExamenFisico((int)ExamenFisicoId);
                }

                var numeroSemanasEmbarazo = 0;
                try { numeroSemanasEmbarazo = Convert.ToInt32(model.NumeroSemanasEmbarazo); } catch { numeroSemanasEmbarazo = 0; }
                var consulta = new Consulta
                {
                    //General
                    CitasId = model.CitaId,
                    ObservacionesAdicionales = model.ObservacionesAdicionales,
                    FechaYHoraInicioConsulta = (DateTime)model.FechaYHoraInicio,
                    FechaProximaConsulta = model.FechaProximaConsulta,
                    EstadoPagoConsultaId = (int)model.EstadoPagoId,
                    TipoConsulta = model.TipoConsulta,
                    TipoReferencia = model.TipoReferencia,
                    MedicoReferido = model.MedicoReferido,
                    CostoConsulta = (decimal)model.CostoConsulta,
                    FaseTratamientoId = model.FaseTratamientoId,

                    Historia = historia,



                    // Datos del padre
                    NombrePadre = model.NombrePadre,
                    FechaNacimientoPadre = model.FechaNacimientoPadre,
                    EdadPadre = model.EdadPadre,
                    DPIPadre = model.DPIPadre,
                    DireccionPadre = model.DireccionPadre,
                    TelefonoPadre = model.TelefonoPadre,
                    CorreoPadre = model.CorreoPadre,
                    OcupacionPadre = model.OcupacionPadre,
                    EmpresaPadre = model.EmpresaPadre,
                    TelefonoEmpresaPadre = model.TelefonoEmpresaPadre,
                    DireccionEmpresaPadre = model.DireccionEmpresaPadre,

                    // Datos de la madre
                    NombreMadre = model.NombreMadre,
                    FechaNacimientoMadre = model.FechaNacimientoMadre,
                    EdadMadre = model.EdadMadre,
                    DPIMadre = model.DPIMadre,
                    DireccionMadre = model.DireccionMadre,
                    TelefonoMadre = model.TelefonoMadre,
                    CorreoMadre = model.CorreoMadre,
                    OcupacionMadre = model.OcupacionMadre,
                    EmpresaMadre = model.EmpresaMadre,
                    TelefonoEmpresaMadre = model.TelefonoEmpresaMadre,
                    DireccionEmpresaMadre = model.DireccionEmpresaMadre,

                    // Datos del acompañante
                    AcompananteNombre = model.AcompananteNombre,
                    AcompananteDPI = model.AcompananteDPI,
                    AcompananteTelefono = model.AcompananteTelefono,


                    ExamenFisico = examenFisico,

                    //Estetico
                    Estetico_Metabolismo = model.Metabolismo,
                    Estetico_Grasa = model.Grasa,
                    Estetico_Agua = model.Agua,
                    Estetico_IMC = model.IMC,
                    Estetico_Obesidad = model.Obesidad,
                    Estetico_ContornoBrazos = model.ContornoBrazos,
                    Estetico_ContornoBusto = model.ContornoBusto,
                    Estetico_ContornoAbdomen = model.ContornoAbdomen,
                    Estetico_ContornoCadera = model.ContornoCadera,
                    Estetico_ContornoPiernas = model.ContornoPiernas,
                    Estetico_Estatura = model.Estatura,

                    //Sección solo para mujeres
                    EstaEmbarazada = model.EstaEmbarazada,
                    NumeroSemanasEmbarazo = numeroSemanasEmbarazo,
                    TomaPildorasAnticonceptivas = model.TomaPildorasAnticonceptivas,
                    EstaAmamantando = model.EstaAmamantando,

                    //Bebidas Alcoholicas
                    BebeBebidasAlcoholicas = model.BebeBebidasAlcoholicas,
                    AlcoholUltimas24Horas = model.AlcoholUltimas24Horas,
                    AlcoholSemanal = model.AlcoholSemanal,

                    //Dental
                    FechaUltimaRadiografiaDental = model.FechaUltimaRadiografiaDental,

                    //Neurologico
                    ExamenNeurologico = model.ExamenNeurologico,

                    //Ginecologico
                    ExamenGinecologico = model.ExamenGinecologico,

                    //Sistemas
                    SistemaCardiopulmonar = model.SistemaCardiopulmonar,
                    SistemaOsteoarticular = model.SistemaOsteoarticular,
                    SistemaHematologico = model.SistemaHematologico,

                    //Area terapeutica
                    TerapeuticoDatosGenerales = model.TerapeuticoDatosGenerales,
                    TerapeuticoActividadesDiarias = model.TerapeuticoActividadesDiarias,
                    TerapeuticoConQuienVive = model.TerapeuticoConQuienVive,
                    TerapeuticoHabitosAlimenticios = model.TerapeuticoHabitosAlimenticios,
                    TerapeuticoEjercicio = model.TerapeuticoEjercicio,
                    TerapeuticoFinesSemana = model.TerapeuticoFinesSemana,
                    TerapeuticoHistoriaMedica = model.TerapeuticoHistoriaMedica,
                    TerapeuticoHistoriaPeso = model.TerapeuticoHistoriaPeso,

                    //Datos de la Mamá Ginecologia 
                    EvaluacionObstetricaUteroGravio = model.EvaluacionObstetricaUteroGravio,
                    EvaluacionObstetricaAbdomenObstetrico = model.EvaluacionObstetricaAbdomenObstetrico,
                    EvaluacionObstetricaFcf = model.EvaluacionObstetricaFcf,
                    EvaluacionObstetricaAu = model.EvaluacionObstetricaAu,
                    EvaluacionObstetricaBishop = model.EvaluacionObstetricaBishop,
                    EvaluacionObstetricaPresentacionLeopold = model.EvaluacionObstetricaPresentacionLeopold,
                    EvaluacionObstetricaOtras = model.EvaluacionObstetricaOtras,
                    EvaluacionObstetricaActividadUterina = model.EvaluacionObstetricaActividadUterina,
                    EvaluacionObstetricaMovimientoFetalPercetible = model.EvaluacionObstetricaMovimientoFetalPercetible,
                    EvaluacionObstetricaMovimientoFetalEspecifique = model.EvaluacionObstetricaMovimientoFetalEspecifique,
                    EvaluacionObstetricaTactoVaginal = model.EvaluacionObstetricaTactoVaginal,
                    EvaluacionObstetricaD = model.EvaluacionObstetricaD,
                    EvaluacionObstetricaCms = model.EvaluacionObstetricaCms,
                    EvaluacionObstetricaBPorciento = model.EvaluacionObstetricaBPorciento,
                    EvaluacionObstetricaAltitud = model.EvaluacionObstetricaAltitud,
                    EvaluacionObstetricaPosicionCervix = model.EvaluacionObstetricaPosicionCervix,
                    EvaluacionObstetricaMembranasOvulares = model.EvaluacionObstetricaMembranasOvulares,
                    EvaluacionObstetricaLiquidoAmniotico = model.EvaluacionObstetricaLiquidoAmniotico,
                    EvaluacionObstetricaLiquidoAmnioticoEspecifique = model.EvaluacionObstetricaLiquidoAmnioticoEspecifique,
                    EvaluacionObstetricaPelvis = model.EvaluacionObstetricaPelvis,

                    //Datos ginecologicos2 de la Mama 

                    EcografiaObstetricaFeto = model.EcografiaObstetricaFeto,
                    EcografiaObstetricaSituacion = model.EcografiaObstetricaSituacion,
                    EcografiaObstetricaPresentacion = model.EcografiaObstetricaPresentacion,
                    EcografiaObstetricaPosicion = model.EcografiaObstetricaPosicion,
                    EcografiaObstetricaDorso = model.EcografiaObstetricaDorso,
                    ObsteBiometriaRlc = model.ObsteBiometriaRlc,
                    ObsteBiometriaSg = model.ObsteBiometriaSg,
                    ObsteBiometriaW = model.ObsteBiometriaW,
                    ObsteBiometriaDbp = model.ObsteBiometriaDbp,
                    ObsteBiometriaHc = model.ObsteBiometriaHc,
                    ObsteBiometriaAc = model.ObsteBiometriaAc,
                    ObsteBiometriaLf = model.ObsteBiometriaLf,
                    ObsteBiometriaEg = model.ObsteBiometriaEg,
                    ObsteBiometriaFcf = model.ObsteBiometriaFcf,
                    ObsteBiometriaPlacenta = model.ObsteBiometriaPlacenta,
                    ObsteBiometriaGrado = model.ObsteBiometriaGrado,
                    ObsteBiometriaIla = model.ObsteBiometriaIla,
                    ObsteBiometriaMalformaciones = model.ObsteBiometriaMalformaciones,
                    ObsteBiometriaPeso = model.ObsteBiometriaPeso,
                    ObsteBiometriaSexo = model.ObsteBiometriaSexo,
                    ObsteBiometriaFechaParto = model.ObsteBiometriaFechaParto,
                    ObsteBiometriaComentario = model.ObsteBiometriaComentario,
                    EcografiaEndocavitariaUtero = model.EcografiaEndocavitariaUtero,
                    EcografiaEndocavitariaLongitudinal = model.EcografiaEndocavitariaLongitudinal,
                    EcografiaEndocavitariaTransverso = model.EcografiaEndocavitariaTransverso,
                    EcografiaEndocavitariaEndometrio = model.EcografiaEndocavitariaEndometrio,
                    EcografiaEndocavitariaOvarioDerecho = model.EcografiaEndocavitariaOvarioDerecho,
                    EcografiaEndocavitariaOvarioIzquierdo = model.EcografiaEndocavitariaOvarioIzquierdo,
                    EcografiaEndocavitariaFondoSaco = model.EcografiaEndocavitariaFondoSaco,
                    EcografiaEndocavitariaImpresionClinica = model.EcografiaEndocavitariaImpresionClinica,
                    EcografiaEndocavitariaComentario = model.EcografiaEndocavitariaComentario,

                    //nota operatoria
                    notaOperatoria = model.notaOperatoria,



                };



                //Servicios
                var servicios = new List<ConsultaServicio>();
                if (model.ServiciosAgregados != null && model.ServiciosAgregados.Count > 0)
                {
                    foreach (var servicioAg in model.ServiciosAgregados)
                    {
                        if (servicioAg.Aplicar)
                        {
                            servicios.Add(new ConsultaServicio
                            {
                                ServicioId = (int)servicioAg.ServicioId,
                                NumeroDiente = servicioAg.NumeroDiente,
                                PrecioId = servicioAg.PrecioId,
                                PrecioValor = servicioAg.PrecioValor,
                                Cantidad = servicioAg.ServicioCantidad,
                                PrecioCubiertoSeguro = servicioAg.ServicioValorCubiertoSeguro,
                                PrecioCopago = servicioAg.ServicioValorCopago
                            });

                            _serviciosRepository.ActualizarInventarioVentaServicio((int)servicioAg.ServicioId);
                        }
                    }
                }
                consulta.ConsultasServicios = servicios;

                //Caracteristicas dentales
                var caracteristicas = new List<ConsultaCaracteristicaDental>();
                foreach (var caracteristica in model.CaracteristicasDientes)
                {
                    caracteristicas.Add(new ConsultaCaracteristicaDental
                    {
                        NumeroDiente = caracteristica.NumeroDiente,
                        Percusiones_VerticalMas = caracteristica.Percusiones_VerticalMas,
                        Percusiones_HorizontalMas = caracteristica.Percusiones_HorizontalMas,
                        Percusiones_VerticalMenos = caracteristica.Percusiones_VerticalMenos,
                        Percusiones_HorizontalMenos = caracteristica.Percusiones_HorizontalMenos,
                        Dolor_Localizado = caracteristica.Dolor_Localizado,
                        Dolor_Fugaz = caracteristica.Dolor_Fugaz,
                        Dolor_Persistente = caracteristica.Dolor_Persistente,
                        Dolor_Referido = caracteristica.Dolor_Referido,
                        Dolor_Espontaneo = caracteristica.Dolor_Espontaneo,
                        Estimulo_Frio = caracteristica.Estimulo_Frio,
                        Estimulo_Calor = caracteristica.Estimulo_Calor,
                        Estimulo_DulceAcido = caracteristica.Estimulo_DulceAcido,
                        Estimulo_Masticacion = caracteristica.Estimulo_Masticacion,
                        Estimulo_Otro = caracteristica.Estimulo_Otro,
                        TermicaFrio_Positiva = caracteristica.TermicaFrio_Positiva,
                        TermicaFrio_Negativa = caracteristica.TermicaFrio_Negativa,
                        TermicaFrio_Localizada = caracteristica.TermicaFrio_Localizada,
                        TermicaFrio_Fugaz = caracteristica.TermicaFrio_Fugaz,
                        TermicaFrio_Incrementa = caracteristica.TermicaFrio_Incrementa,
                        TermicaFrio_Referida = caracteristica.TermicaFrio_Referida,
                        TermicaFrio_Irradiado = caracteristica.TermicaFrio_Irradiado,
                        TermicaFrio_Persistente = caracteristica.TermicaFrio_Persistente,
                        TermicaFrio_Decrece = caracteristica.TermicaFrio_Decrece,
                        TermicaCalor_Positiva = caracteristica.TermicaCalor_Positiva,
                        TermicaCalor_Negativa = caracteristica.TermicaCalor_Negativa,
                        TermicaCalor_Localizada = caracteristica.TermicaCalor_Localizada,
                        TermicaCalor_Fugaz = caracteristica.TermicaCalor_Fugaz,
                        TermicaCalor_Incrementa = caracteristica.TermicaCalor_Incrementa,
                        TermicaCalor_Referida = caracteristica.TermicaCalor_Referida,
                        TermicaCalor_Irradiado = caracteristica.TermicaCalor_Irradiado,
                        TermicaCalor_Persistente = caracteristica.TermicaCalor_Persistente,
                        TermicaCalor_Decrece = caracteristica.TermicaCalor_Decrece,
                        Diagnostico_ManchaBlanca = caracteristica.Diagnostico_ManchaBlanca,
                        Diagnostico_Caries = caracteristica.Diagnostico_Caries,
                        Diagnostico_Traumatismo = caracteristica.Diagnostico_Traumatismo,
                        Diagnostico_Abfraccion = caracteristica.Diagnostico_Abfraccion,
                        Diagnostico_Atricion = caracteristica.Diagnostico_Atricion,
                        Diagnostico_Erosion = caracteristica.Diagnostico_Erosion,
                        Diagnostico_Restauracion = caracteristica.Diagnostico_Restauracion,
                        Diagnostico_Ajustada = caracteristica.Diagnostico_Ajustada,
                        Diagnostico_Desajustada = caracteristica.Diagnostico_Desajustada,
                        Diagnostico_PulpaSana = caracteristica.Diagnostico_PulpaSana,
                        Diagnostico_PulpitisReversible = caracteristica.Diagnostico_PulpitisReversible,
                        Diagnostico_PulpitisIrreversible = caracteristica.Diagnostico_PulpitisIrreversible,
                        Diagnostico_NecrosisPulpar = caracteristica.Diagnostico_NecrosisPulpar
                    });
                }
                consulta.ConsultasCaracteristicasDentales = caracteristicas;

                //Seguimientos nutricionales
                if (model.SeguimientosNutricionales != null
                 && model.SeguimientosNutricionales.Count > 0)
                {
                    foreach (var seguimiento in model.SeguimientosNutricionales)
                    {
                        var seguimientoConsulta = new PacienteSeguimientoNutricional
                        {
                            PacienteId = model.PacienteId,
                            Fecha = seguimiento.Fecha,
                            Peso = seguimiento.Peso,
                            IMC = seguimiento.IMC,
                            PGC = seguimiento.PGC,
                            Cuello = seguimiento.Cuello,
                            Busto = seguimiento.Busto,
                            CinturaAbdomen = seguimiento.CinturaAbdomen,
                            Cadera = seguimiento.Cadera,
                            Muslo = seguimiento.Muslo,
                            Brazo = seguimiento.Brazo,
                            Muñeca = seguimiento.Munneca,
                        };
                        if (seguimiento.Id != 0)
                        {
                            seguimientoConsulta.Id = (int)seguimiento.Id;
                            _pacientesRepository.UpdateSeguimientoNutricional(seguimientoConsulta);
                        }
                        else
                        {
                            _pacientesRepository.AddSeguimientoNutricional(seguimientoConsulta);
                        }
                    }
                }

                #region EXAMENES AGREGADOS

                if (model.ExamenesAgregados != null && model.ExamenesAgregados.Count() > 0)
                {
                    consulta.ConsultaExamenesAgregados = new List<ConsultaExamenLabClinico>();
                    foreach (var examen in model.ExamenesAgregados)
                    {
                        var fechaRegistro = DateTime.Now;
                        if (examen.FechaRegistro != null)
                        {
                            fechaRegistro = ((DateTime)examen.FechaRegistro);
                        }
                        consulta.ConsultaExamenesAgregados.Add(new ConsultaExamenLabClinico
                        {
                            ExamenLabClinicoId = examen.ExamenId,
                            FechaRegistro = fechaRegistro,
                            PrecioId = examen.PrecioId,
                            Cantidad = examen.Cantidad == 0 ? 1 : examen.Cantidad,
                            PrecioValor = examen.ValorUnitario,
                            PrecioValorCopago = examen.ValorCopago,
                            PrecioValorCubiertoSeguro = examen.ValorCubiertoSeguro
                        });
                    }
                }

                #endregion


                #region Resultados examenes laboratorio

                if (model.ExamenesLaboratorio != null
                 && model.ExamenesLaboratorio.Count > 0)
                {
                    foreach (var examen in model.ExamenesLaboratorio)
                    {
                        var examenLaboratorioConsulta = new PacienteResultadoExamenLaboratorio
                        {
                            PacienteId = model.PacienteId,
                            Fecha = examen.Fecha,
                            GlucosaPre = examen.GlucosaPre,
                            GlucosaPos = examen.GlucosaPos,
                            HemoglobinaGlicosilada = examen.HemoglobinaGlicosilada,
                            CurvaGlucosa = examen.CurvaGlucosa,
                            ColesterolTotal = examen.ColesterolTotal,
                            Trigliceridos = examen.Trigliceridos,
                            PerfilLipidico = examen.PerfilLipidico,
                            Creatinina = examen.Creatinina,
                            AcidoUrico = examen.AcidoUrico,
                            Hemoglobina = examen.Hemoglobina,
                            T3 = examen.T3,
                            T4 = examen.T4,
                            ExamenHeces = examen.ExamenHeces,
                            ExamenOrina = examen.ExamenOrina,
                            Otros = examen.Otros,
                            //UrlResultados = examen.UrlResultados
                        };
                        if (examen.Id != null)
                        {
                            examenLaboratorioConsulta.Id = (int)examen.Id;
                            _pacientesRepository.UpdateResultadoExamenLaboratorio(examenLaboratorioConsulta);
                        }
                        else
                        {
                            _pacientesRepository.AddResultadoExamenLaboratorio(examenLaboratorioConsulta);
                        }
                    }
                }

                #endregion

                #region Vacunas paciente
                var vacunasAgregadas = new List<VacunaPaciente>();
                foreach (var vacuna in model.VacunasPaciente)
                {
                    var vacunaPaciente = new VacunaPaciente
                    {
                        PacienteId = (int)model.PacienteId,
                        VacunaId = (int)vacuna.VacunaId,
                        Primera = (bool)vacuna.Primera,
                        FechaPrimera = vacuna.FechaPrimera,
                        Segunda = (bool)vacuna.Segunda,
                        FechaSegunda = vacuna.FechaSegunda,
                        Tercera = (bool)vacuna.Tercera,
                        FechaTercera = vacuna.FechaTercera,
                        PrimerRefuerzo = (bool)vacuna.PrimerRefuerzo,
                        FechaPrimerRefuerzo = vacuna.FechaPrimerRefuerzo,
                        SegundoRefuerzo = (bool)vacuna.SegundoRefuerzo,
                        FechaSegundoRefuerzo = vacuna.FechaSegundoRefuerzo
                    };
                    vacunasAgregadas.Add(vacunaPaciente);
                }
                var listaVacunasExistentes = _pacientesRepository.GetVacunasPaciente((int)model.PacienteId);
                if (listaVacunasExistentes == null || listaVacunasExistentes.Count == 0)
                {
                    paciente.VacunasPaciente = vacunasAgregadas;
                    _pacientesRepository.Update(paciente);
                }
                #endregion

                //Antecedentes familiares
                var antecedentesAgregados = new List<PatologiaPaciente>();
                if (model.AntecedentesFamiliaresPaciente != null)
                {
                    foreach (var antecedente in model.AntecedentesFamiliaresPaciente)
                    {
                        var antecedentePaciente = new PatologiaPaciente
                        {
                            PacienteId = paciente.Id,
                            AbuelaMaterna = antecedente.AbuelaMaterna,
                            AbuelaPaterna = antecedente.AbuelaPaterna,
                            AbueloMaterno = antecedente.AbueloMaterno,
                            AbueloPaterno = antecedente.AbueloPaterno,
                            DescripcionOtraPatologia = antecedente.DescripcionOtraPatologia,
                            Hermanos = antecedente.Hermanos,
                            Madre = antecedente.Madre,
                            OtrosMaterno = antecedente.OtrosMaterno,
                            OtrosPaterno = antecedente.OtrosPaterno,
                            Padre = antecedente.Padre,
                            TipoPatologiaId = antecedente.TipoPatologiaId
                        };
                        antecedentesAgregados.Add(antecedentePaciente);
                    }
                }


                paciente.PatologiasPaciente = antecedentesAgregados;
                _pacientesRepository.Update(paciente);



                //Información médica
                paciente.MedicaUsaLentesContacto = model.MedicaUsaLentesContacto;
                paciente.MedicaUsaLentesContactoDescripcion = model.MedicaUsaLentesContactoDescripcion;
                paciente.MedicaArticulacionesArtificiales = model.MedicaArticulacionesArtificiales;
                paciente.MedicaArticulacionesArtificialesFecha = model.MedicaArticulacionesArtificialesFecha;
                paciente.MedicaArticulacionesArtificialesComplicaciones = model.MedicaArticulacionesArtificialesComplicaciones;
                paciente.MedicaTomaAlendronato = model.MedicaTomaAlendronato;
                paciente.MedicaTomaAlendronatoFecha = model.MedicaTomaAlendronatoFecha;
                paciente.MedicaTratamientoDolorHuesos = model.MedicaTratamientoDolorHuesos;
                paciente.MedicaTratamientoDolorHuesosFechaInicio = model.MedicaTratamientoDolorHuesosFechaInicio;
                paciente.MedicaTratamientoDolorHuesosDescripcionCaso = model.MedicaTratamientoDolorHuesosDescripcionCaso;
                paciente.MedicaSustanciasReguladorasDrogas = model.MedicaSustanciasReguladorasDrogas;
                paciente.MedicaSustanciasReguladorasDrogasFecha = model.MedicaSustanciasReguladorasDrogasFecha;
                paciente.MedicaUsaTabaco = model.MedicaUsaTabaco;
                paciente.MedicaBebidasAlcoholicas = model.MedicaBebidasAlcoholicas;
                paciente.MedicaBebidasAlcoholicasDescripcion = model.MedicaBebidasAlcoholicasDescripcion;



                //Información dental
                paciente.DentalSangradoCepillar = model.DentalSangradoCepillar;
                paciente.DentalDolorFrio = model.DentalDolorFrio;
                paciente.DentalDolorPresionar = model.DentalDolorPresionar;
                paciente.DentalObjetosAtorados = model.DentalObjetosAtorados;
                paciente.DentalBocaSeca = model.DentalBocaSeca;
                paciente.DentalTratamientoPeriondal = model.DentalTratamientoPeriondal;
                paciente.DentalTratamientoOrtodoncia = model.DentalTratamientoOrtodoncia;
                paciente.DentalProblemasTratamientoDental = model.DentalProblemasTratamientoDental;
                paciente.DentalProblemasTratamientoDentalDescripcion = model.DentalProblemasTratamientoDentalDescripcion;
                paciente.DentalFluoradaAguaDomicilio = model.DentalFluoradaAguaDomicilio;
                paciente.DentalBebeAguaFiltrada = model.DentalBebeAguaFiltrada;
                paciente.DentalDolorOidos = model.DentalDolorOidos;
                paciente.DentalMolestiaRuidoAlto = model.DentalMolestiaRuidoAlto;
                paciente.DentalMolestiaRuidoAltoDescripcion = model.DentalMolestiaRuidoAltoDescripcion;
                paciente.DentalBrumismo = model.DentalBrumismo;
                paciente.DentalLesiones = model.DentalLesiones;
                paciente.DentalLesionesDescripcion = model.DentalLesionesDescripcion;
                paciente.DentalDentaduraPlacas = model.DentalDentaduraPlacas;
                paciente.DentalDentaduraPlacasDescripcion = model.DentalDentaduraPlacasDescripcion;
                paciente.DentalActividadesRecreacion = model.DentalActividadesRecreacion;
                paciente.DentalActividadesRecreacionDescripcion = model.DentalActividadesRecreacionDescripcion;
                paciente.DentalLesionesCabeza = model.DentalLesionesCabeza;
                paciente.DentalLesionesCabezaDescripcion = model.DentalLesionesCabezaDescripcion;



                // ====== FINALIZAR CITA (EDITAR CONSULTA) ======
                if (model.CitaId.HasValue && model.CitaId.Value > 0)
                {
                    var ncita = _citasRepository.GetCita(model.CitaId.Value);

                    if (ncita != null)
                    {
                        if (!ncita.Finalizada)
                        {
                            if (ncita.ContadorCitaFinalizada == null)
                            {
                                ncita.ContadorCitaFinalizada = DateTime.UtcNow;
                                Console.WriteLine($"[FINALIZAR-CITA-EDIT] CitaId={ncita.Id} ContadorCitaFinalizada={ncita.ContadorCitaFinalizada:O}");
                            }

                            ncita.Finalizada = true;
                        }

                        ncita.EstadoTurno = model.EstadoTurno;
                        _citasRepository.Update(ncita);
                    }
                    else
                    {
                        Console.WriteLine($"[FINALIZAR-CITA-EDIT] Cita no encontrada. CitaId={model.CitaId.Value}");
                    }
                }
                else
                {
                    Console.WriteLine("[FINALIZAR-CITA-EDIT] Se omite finalización: model.CitaId es null");
                }
                // ====== /FINALIZAR CITA (EDITAR CONSULTA) ======

                //Agenda automatica de proxima cita
                if (model.FechaProximaConsulta != null)
                {
                    var proximaCita = new Citas
                    {
                        PacienteId = model.PacienteId,
                        FechaInicio = Convert.ToDateTime(model.FechaProximaConsulta),
                        FechaFinal = Convert.ToDateTime(model.FechaProximaConsulta),
                        ContadorCitaAgendada = Convert.ToDateTime(model.FechaProximaConsulta),
                        ConsultaId = model.ConsultaId,
                        Eliminado = false,
                        Finalizada = false,
                        EstadoCita = "normal",
                        Bloqueada = false
                    };

                    _citasRepository.Add(proximaCita);
                }

                //Registro de consulta
                // Registro de la consulta
                var idConsultaRegistrada = _consultasRepository.AddConsulta(consulta);


                // Actualizar la Cita con el ConsultaId recién creado
                var cita = _citasRepository.GetCita((int)model.CitaId);
                if (cita != null)
                {
                    cita.ConsultaId = idConsultaRegistrada;  // Asignar la ConsultaId a la Cita
                    _citasRepository.Update(cita);  // Guardar la cita con la nueva ConsultaId
                }

                // Notificar que la consulta se guardó correctamente
                TempData["Message"] = "¡La consulta se ha guardado con éxito.!";

                // Devolver respuesta JSON con el resultado
                return JsonSerializer.Serialize(new { Exitoso = true, ConsultaId = idConsultaRegistrada });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Error al registrar consulta. " + ex.Message });
            }
        }



        [HttpPost]
        public string GenerarCuentaPendiente(ConsultasViewModel model)
        {
            var paciente = _pacientesRepository.GetPacientePorId((int)model.PacienteId);
            var usuarioAgendocita = _userManager.GetUserId(HttpContext.User);
            //Medico
            var medico = _empleadoRepository.GetMedicoByName(model.MedicoAsignado);

            if (medico == null)
            {
                medico = new Medicos()
                {
                    Nombres = model.MedicoAsignado
                };
            }

            var clinica = _empleadoRepository.GetClinicaByName(" ");
            if (clinica == null)
            {
                clinica = new Clinica()
                {
                    NombreClinica = " "
                };
            }

            //Trae todos los examenes
            var examenesBd = _consultasRepository.GetExamenesAgregadosConsulta((int)model.ConsultaId);
            decimal? ValorTotal = 0;
            if (examenesBd != null && examenesBd.Count() > 0)
            {
                var examen = new Examen()
                {
                    Paciente = paciente,
                    EstadoExamenId = 1,
                    FechaRealizacion = DateTime.Now,
                    Medicos = medico,
                    Clinicas = clinica,
                    ClinicaReferida = clinica.NombreClinica,
                    PagoCredito = true,
                    EmpleadoId = 9,
                    UsuarioSolicita = usuarioAgendocita

                };
                foreach (var examenVenta in examenesBd)
                {
                    var nuevodetalle = new DetalleExamen()
                    {
                        Examen = examen,
                        Cantidad = 1,
                        PrecioValor = examenVenta.PrecioValor,
                        PrecioId = examenVenta.PrecioId,
                        Descuento = 0,
                        Subtotal = examenVenta.PrecioValor * 1,
                        Total = examenVenta.PrecioValor * 1,
                        ExamenLabClinicoId = (int)examenVenta.ExamenLabClinicoId,
                    };

                    _laboratorioRepository.Add(nuevodetalle, false);

                    var datos = _laboratorioRepository.DatosLabList((int)examenVenta.ExamenLabClinicoId);

                    foreach (var dato in datos)
                    {
                        var newDato = new Resultados()
                        {
                            DatosExamenesLabClinico = dato,
                            DetalleExamen = nuevodetalle
                        };

                        _laboratorioRepository.Add(newDato, false);
                    }

                    //if (examenVenta.e) 
                    //{ 
                    //}
                    //Descontar stock el insumo utilizado  por el examen
                    _laboratorioRepository.ActualizarInventarioInsumoVentaExamenesLaboratorio((int)examenVenta.ExamenLabClinicoId);
                }
            }


            foreach (var examen1 in examenesBd)
            {

                ValorTotal += examen1.PrecioValor;
            }
            if (model.ServiciosAgregados != null)
            {
                foreach (var servicio in model.ServiciosAgregados)
                {
                    ValorTotal += servicio.ServicioValorTotal;
                }
            }

            try
            {

                var nuevaCuenta = new CuentaPorCobrar
                {
                    FechaLimitePago = DateTime.Today.AddMonths(1),
                    PacienteId = model.PacienteId,
                    Valor = ValorTotal,
                    Pagada = false,
                    Eliminada = false
                };
                _cuentasPorCobrarRepository.Add(nuevaCuenta);



                TempData["Message"] = "¡La consulta se ha guardado con éxito.!";
                return JsonSerializer.Serialize(new { Exitoso = true, });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Error al registrar cuenta pendiente. " + ex.Message });
            }
        }
        [HttpPost]
        public string IniciarExamenesFisicosCitasConsulta(ConsultasViewModel model)
        {
            try
            {
                var paciente = _pacientesRepository.Get((int)model.PacienteId);

                var historia = new Historia()
                {
                    HistoriaProblema = model.HistoriaProblema == null ? "--- Historia del problema ---" : model.HistoriaProblema,
                    Sintomas = model.Sintomas == null ? "--- Sintomas ---" : model.Sintomas,
                    Diagnostico = model.Diagnostico == null ? "--- Diagnostico ---" : model.Diagnostico
                };
                var examenFisico = new ExamenFisico()
                {
                    Temperatura = model.ExamenFisicoTemperatura == null ?
                         "--- Temperatura ---" : model.ExamenFisicoTemperatura,
                    FrecuenciaRespiratoria = model.ExamenFisicoFrecuenciaRespiratoria == null ?
                        "--- FrecuenciaRespiratoria ---" : model.ExamenFisicoFrecuenciaRespiratoria,
                    FrecuenciaCardiaca = model.ExamenFisicoFrecuenciaCardiaca == null ?
                        "--- Frecuencia Cardiaca ---" : model.ExamenFisicoFrecuenciaCardiaca,
                    SaturacionDeOxigeno = model.ExamenFisicoSaturacionOxigeno == null ?
                        "--- Saturacion De Oxigeno ---" : model.ExamenFisicoSaturacionOxigeno,
                    PresionArterialBrazoDerecho = model.ExamenFisicoDerecho == null ?
                        "--- Presion Arterial Brazo Derecho ---" : model.ExamenFisicoDerecho,
                    PresionArterialBrazoIzquierdo = model.ExamenFisicoIzquierdo == null ? /* zzz */
                        "--- Presion Arterial Brazo Izquierdo ---" : model.ExamenFisicoIzquierdo,
                    PresionArterialMedia = model.ExamenFisicoPresionArterialMedia == null ?
                        "--- Presion Arterial Media ---" : model.ExamenFisicoPresionArterialMedia,
                    Observaciones = model.ExamenFisicoObservaciones == null ?
                        "--- Observaciones o texto adicional ---" : model.ExamenFisicoObservaciones,
                    Peso = model.ExamenFisicoPeso == null ?
                        "--- Peso ---" : model.ExamenFisicoPeso,
                    Talla = model.ExamenFisicoTalla == null ?
                        "--- Talla ---" : model.ExamenFisicoTalla,
                    Glucosa = model.ExamenFisicoGlucosa == null ?
                        "--- Glucosa ---" : model.ExamenFisicoGlucosa,
                    IMC = model.ExamenFisicoIMC == null ?
                        "--- IMC ---" : model.ExamenFisicoIMC
                };


                _consultasRepository.AddExamenFisico(examenFisico);




                //Actualizar el id Examen Fisico de cita
                var ncita = _citasRepository.GetCita((int)model.CitaId);
                ncita.ExamenFisicoId = examenFisico.Id;
                ncita.EstadoTurno = model.EstadoTurno;
                _citasRepository.Update(ncita);


                //Registro de consulta
                //var idConsultaRegistrada = _consultasRepository.AddConsulta(consulta);





                TempData["Message"] = "¡La consulta se ha guardado con éxito.!";
                return JsonSerializer.Serialize(new { Exitoso = true, ConsultaId = model.CitaId });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Error al registrar consulta. " + ex.Message });
            }
        }

        [HttpPost]
        public string GenerarPrescripcion(int citaId, List<ConsultaPrescripcionViewModel> elementosPrescripcion)
        {
            try
            {
                var prescripcionesCita = _consultasRepository.GetPrescripcionesCita(citaId);
                if (prescripcionesCita != null)
                {
                    foreach (var item in prescripcionesCita)
                    {
                        item.Eliminada = true;
                        _consultasRepository.Update(item);
                    }
                }

                var prescripcion = new Prescripcion()
                {
                    CitasId = citaId,
                };

                foreach (var elemento in elementosPrescripcion)
                {
                    var detallePrescripcion = new DetallePrescripcion
                    {
                        Item = elemento.Item,
                        Medicine = elemento.Medicamento,
                        Cantidad = elemento.Cantidad,
                        Indications = elemento.Observaciones,
                        Color = elemento.Color,
                        FechaPrescripcion = DateTime.Now,
                    };
                    prescripcion.DetallePrescripcion.Add(detallePrescripcion);
                }

                _consultasRepository.AddPrescipcion(prescripcion);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = prescripcion.Id
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al generar prescripcion. " + ex.Message
                });
            }
        }

        [HttpPost]
        public string GuardarPresupuestoDental(ConsultasViewModel model)
        {
            try
            {
                var presupuestoDental = new PresupuestoDental
                {
                    PacienteId = (int)model.PacienteId
                };

                if (model.ServiciosAgregados != null && model.ServiciosAgregados.Count() > 0)
                {
                    foreach (var servicio in model.ServiciosAgregados)
                    {
                        presupuestoDental.PresupuestosDentalesDetalles.Add(new PresupuestoDentalDetalle
                        {
                            Codigo = servicio.ServicioCodigo,
                            NombreServicio = servicio.NombreServicio,
                            Diente = servicio.NumeroDiente == null ? "" : servicio.NumeroDiente.ToString(),
                            Precio = servicio.PrecioNombre,
                            Valor = (servicio.PrecioValor ?? 0).ToString()
                        });
                    }
                }
                else
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "No hay servicios agregados"
                    });
                }

                presupuestoDental = _pacientesRepository.AddPresupuestoDental(presupuestoDental);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = presupuestoDental.Id
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al guardar presupuesto dental. " + ex.Message
                });
            }
        }

        public string CalcularEdad(DateTime fechaNacimiento)
        {
            var today = DateTime.Today;
            var edad = today.Year - fechaNacimiento.Year;

            if (fechaNacimiento.Date > today.AddYears(-edad))
                edad--;

            var meses = today.Month - fechaNacimiento.Month;
            if (meses < 0)
            {
                meses += 12;
            }

            var dias = today.Day - fechaNacimiento.Day;
            if (dias < 0)
            {
                var lastMonth = today.AddMonths(-1);
                dias += DateTime.DaysInMonth(lastMonth.Year, lastMonth.Month);
            }

            // Formatear los resultados con singular o plural
            string Formatear(int cantidad, string unidadSingular, string unidadPlural)
            {
                return cantidad == 1 ? $"{cantidad} {unidadSingular}" : $"{cantidad} {unidadPlural}";
            }

            var edadTexto = Formatear(edad, "Año", "Años");
            var mesesTexto = Formatear(meses, "Mes", "Meses");
            var diasTexto = Formatear(dias, "Día", "Días");

            return $"{edadTexto}, {mesesTexto} y {diasTexto}";
        }


        public IActionResult Informacion(int? id)
        {
            if (id == null) return StatusCode(404);
            var consulta = _consultasRepository.GetConsulta((int)id);

            var archisvosExamen = _consultasRepository.GetExamenArchivo((int)id);

            if (consulta == null) return StatusCode(400);

            var paciente = consulta.Citas.Paciente ?? new Paciente();
            var seguroEpss = consulta.Citas.Paciente.SeguroEpss ?? new SeguroEpss();
            var examenFisico = consulta.ExamenFisico ?? new ExamenFisico();
            var examenFisicoPediatria = consulta.ExamenFisicoPediatria ?? new ExamenFisicoPediatria();
            var revisionSistemas = consulta.ConsultaRevisionSistemas ?? new ConsultaRevisionSistemas();
            var revisionSistemasPediatria = consulta.ConsultaRevisionSistemasPediatria ?? new ConsultaRevisionSistemasPediatria();
            var historia = consulta.Historia ?? new Historia();
            var historiaPediatria = consulta.HistoriaPediatria ?? new HistoriaPediatria();
            var pacienteListaApnp = paciente.PacienteApnp ?? new List<PacienteApnp>();
            var pacienteApnp = pacienteListaApnp.FirstOrDefault() ?? new PacienteApnp();
            var pacienteApnpPediatrico = paciente.PacientePediatricoApnp ?? new PacientePediatricoApnp();
            var gineAntPatologicos = consulta.ConsultaAntPatologicosGinecologia ?? new ConsultaAntPatologicosGinecologia();
            var gineApnp = consulta.ConsultaAntNoPatologicosGinecologia ?? new ConsultaAntNoPatologicosGinecologia();
            var obsteApnp = consulta.ConsultaAntNoPatologicosObstetricia ?? new ConsultaAntNoPatologicosObstetricia();
            var gineExamenFisico = consulta.ConsultaExamenFisicoGinecologia ?? new ConsultaExamenFisicoGinecologia();





            var model = new InfoConsultaViewModel
            {
                HabilitarEdicion = false,
                ConsultaId = consulta.Id,
                CitaId = consulta.CitasId,
                ConsultaMotivo = consulta.ConsultaMotivo,
                HistoriaPediatriaConsultaMotivo = consulta.ConsultaMotivoPediatria,
                CitaTipoAtencion = consulta.Citas.CitaTipoAtencion,
                FechaProximaConsulta = consulta.FechaProximaConsulta,
                ProximaCitaAgendada = consulta.ProximaCitaAgendada,
                NivelPrioridadCita = consulta.Citas.NivelPrioridadCita,
                EstadoTurno = consulta.Citas.EstadoTurno,
                NumeroTurno = consulta.Citas.NumeroTurno,
                Cie10Codigo = consulta.Cie10Codigo,
                EmpleadoText = consulta.Citas?.Empleado?.NombreYApellidos ?? "Sin Asignar",  // Asegurarse que EmpleadoText es usado


                #region CONSULTA - DATOS GENERALES

                FechaYHoraInicio = consulta.FechaYHoraInicioConsulta,
                TipoConsulta = consulta.TipoConsulta,
                ObservacionesAdicionales = consulta.ObservacionesAdicionales,
                FechaUltimaConsulta = null, //por setear
                MotivoUltimaConsulta = "-", //por setear


                #endregion



                #region SOLO PARA MUJERES

                EstaEmbarazada = consulta.EstaEmbarazada,
                NumeroSemanasEmbarazo = consulta.NumeroSemanasEmbarazo == null ? "" :
                ((int)consulta.NumeroSemanasEmbarazo).ToString(),
                TomaPildorasAnticonceptivas = consulta.TomaPildorasAnticonceptivas,
                EstaAmamantando = consulta.EstaAmamantando,

                #endregion

                #region PRESCRIPCIÓN - MEDICAMENTOS OTROS
                MedicamentosOtros = new Func<List<MedicamentoOtro>>(() =>
                {
                    Console.WriteLine("== [TRACE] Carga MedicamentosOtros (GetConsulta.Include) ==");

                    var src = consulta.MedicamentosOtros;

                    if (src == null)
                    {
                        Console.WriteLine("consulta.MedicamentosOtros: NULL");
                        return new List<MedicamentoOtro>();
                    }

                    if (!src.Any())
                    {
                        Console.WriteLine("consulta.MedicamentosOtros: vacío");
                        return new List<MedicamentoOtro>();
                    }

                    Console.WriteLine($"consulta.MedicamentosOtros.Count = {src.Count}");

                    var lista = src
                        .OrderBy(x => x.Id)
                        .Select((m, i) =>
                        {
                            Console.WriteLine($"  [{i}] Id:{m.Id} ConsultaId:{m.ConsultaId} Nombre:'{m.Nombre}' Cantidad:{m.Cantidad} Indicaciones:'{m.Indicaciones}' FechaPrescripcion:'{m.FechaPrescripcion}'");
                            return new MedicamentoOtro
                            {
                                Id = m.Id,
                                Nombre = m.Nombre,
                                Indicaciones = m.Indicaciones,
                                Cantidad = m.Cantidad,
                                FechaPrescripcion = m.FechaPrescripcion
                            };
                        })
                        .ToList();

                    Console.WriteLine($"VM.MedicamentosOtros.Count = {lista.Count}");
                    Console.WriteLine("== [TRACE] Fin mapeo MedicamentosOtros ==\n");

                    return lista;
                })(),
                #endregion




                #region PACIENTE - DATOS PERSONALES

                PacienteId = paciente.Id,
                PacienteNombre = paciente.Nombre ?? "-",
                NombreEncargado = paciente.NombreEncargado ?? "-",
                DPIEncargado = paciente.DPIEncargado ?? "-",
                PacienteEdad = paciente.FechaNacimiento != null
                ? CalcularEdad((DateTime)paciente.FechaNacimiento)
                : "-",
                PacienteCelular = paciente.Celular ?? "-",
                PacienteNit = paciente.Nit ?? "-",
                PacienteDireccion = paciente.Direccion ?? "-",
                PacienteAlias = paciente.Alias ?? "-",
                PacienteSexo = paciente.sexoText ?? "-",
                PacienteFechaNacimiento = paciente.FechaNacimiento != null
                ? ((DateTime)paciente.FechaNacimiento).ToString("dd-MM-yyyy")
                : "-",
                PacienteTelefono = paciente.Telefono,
                PacienteMunicipioId = paciente.MunicipioId,
                PacienteDepartamentoId = paciente.DepartamentoId,
                PacienteSeguroEPSS = seguroEpss.Nombre,

                #endregion

                #region PACIENTE - APNP - ANT NO PATOLOGICOS

                PacienteApnpAbortos = pacienteApnp.Abortos,
                PacienteApnpCesareas = pacienteApnp.Cesareas,
                PacienteApnpGestas = pacienteApnp.Gestas,
                PacienteApnpOtros = pacienteApnp.Otros,
                PacienteApnpHijosMuertos = pacienteApnp.HijosMuertos,
                PacienteApnpHijosVivos = pacienteApnp.HijosVivos,
                PacienteApnpMenarquia = pacienteApnp.Menarquia,
                PacienteApnpPartos = pacienteApnp.Partos,
                PacienteApnpFechaUltimaRegla = pacienteApnp.FechaUltimaRegla != null
                ? ((DateTime)pacienteApnp.FechaUltimaRegla).ToString("yyyy-MM-dd")
                : "-",
                PacienteApnpFechaProbableParto = pacienteApnp.FechaProbableParto != null
                ? ((DateTime)pacienteApnp.FechaProbableParto).ToString("yyyy-MM-dd")
                : "-",

                #endregion

                #region PACIENTE - ANT PATOLOGICOS

                PacienteMedicos = paciente.AntecedentesMedicos ?? "-",
                PacienteTraumaticos = paciente.AntecedentesTraumaticos ?? "-",
                PacienteAlergias = paciente.AntecedentesAlergias ?? "-",
                PacienteVicios = paciente.AntecedentesVicios ?? "-",
                PacienteMedicamentos = paciente.AntecedentesMedicamentos ?? "-",
                PacienteQuirurgicos = paciente.AntecedentesQuirurgicos ?? "-",

                #endregion

                #region PEDIATRICOS - ANT PATOLOGICOS

                PediatricoAntMedicos = paciente.PediatricosAntMedicos ?? "-",
                PediatricoAntTraumaticos = paciente.PediatricosAntTraumaticos ?? "-",
                PediatricoAntAlergias = paciente.PediatricosAntAlergias ?? "-",
                PediatricoAntVicios = paciente.PediatricosAntVicios ?? "-",
                PediatricoAntMedicamentos = paciente.PediatricosAntMedicamentos ?? "-",
                PediatricoAntQuirurgicos = paciente.PediatricosAntQuirurgicos ?? "-",

                #endregion

                #region PEDIATRICOS - ANT NO PATOLOGICOS - APNP

                PediatricoApnpParto = pacienteApnpPediatrico.Parto ?? "-",
                PediatricoApnpAtendidoPor = pacienteApnpPediatrico.AtendidoPor ?? "-",
                PediatricoApnpGesta = pacienteApnpPediatrico.Gesta ?? "-",
                PediatricoApnpInmunizaciones = pacienteApnpPediatrico.Inmunizaciones ?? "-",
                PediatricoApnpPesoAlNacer = pacienteApnpPediatrico.PesoAlNacer ?? "-",

                #endregion

                #region HISTORIA CLINICA

                Sintomas = historia.Sintomas ?? "-",
                Diagnostico = historia.Diagnostico ?? "-",
                HistoriaEnfermedadActual = historia.HistoriaEnfermedadActual ?? "-",
                HistoriaClinicaComentario = historia.Comentario ?? "-",
                HistoriaClinicaImpresionClinica = historia.ImpresionClinica ?? "-",

                #endregion

                #region HISTORIA CLINICA - PEDIATRIA

                HistoriaPediatriaSintomas = historiaPediatria.Sintomas ?? "-",
                HistoriaPediatriaDiagnostico = historiaPediatria.Diagnostico ?? "-",
                HistoriaPediatriaHistoriaEnfermedadActual = historiaPediatria.HistoriaEnfermedadActual ?? "-",
                HistoriaPediatriaHistoriaClinicaComentario = historiaPediatria.Comentario ?? "-",
                HistoriaPediatriaHistoriaClinicaImpresionClinica = historiaPediatria.ImpresionClinica ?? "-",

                #endregion

                #region EXAMEN FISICO

                ExamenFisicoEstadoGeneral = examenFisico.EstadoGeneral ?? "-",
                ExamenFisicoPeso = examenFisico.Peso ?? "-",
                ExamenFisicoTalla = examenFisico.Talla ?? "-",
                ExamenFisicoFrecuenciaCardiaca = examenFisico.FrecuenciaCardiaca ?? "-",
                ExamenFisicoFrecuenciaRespiratoria = examenFisico.FrecuenciaRespiratoria ?? "-",
                ExamenFisicoPresionArterial = examenFisico.PresionArterial ?? "-",
                ExamenFisicoTemperatura = examenFisico.Temperatura ?? "-",
                ExamenFisicoSaturacionOxigeno = examenFisico.SaturacionDeOxigeno ?? "-",
                ExamenFisicoGlasgow = examenFisico.Glasgow ?? "-",

                #endregion

                #region EXAMEN FISICO - PEDIATRIA

                ExamenFisicoPediatriaEstadoGeneral = examenFisicoPediatria.EstadoGeneral ?? "-",
                ExamenFisicoPediatriaPeso = examenFisicoPediatria.Peso ?? "-",
                ExamenFisicoPediatriaTalla = examenFisicoPediatria.Talla ?? "-",
                ExamenFisicoPediatriaFrecuenciaCardiaca = examenFisicoPediatria.FrecuenciaCardiaca ?? "-",
                ExamenFisicoPediatriaFrecuenciaRespiratoria = examenFisicoPediatria.FrecuenciaRespiratoria ?? "-",
                ExamenFisicoPediatriaPresionArterial = examenFisicoPediatria.PresionArterial ?? "-",
                ExamenFisicoPediatriaTemperatura = examenFisicoPediatria.Temperatura ?? "-",
                ExamenFisicoPediatriaSaturacionOxigeno = examenFisicoPediatria.SaturacionDeOxigeno ?? "-",
                ExamenFisicoPediatriaGlasgow = examenFisicoPediatria.Glasgow ?? "-",
                ExamenFisicoPediatricoPesoEdad = examenFisicoPediatria.PediatricoPesoEdad ?? "-",
                ExamenFisicoPediatricoPesoTalla = examenFisicoPediatria.PediatricoPesoTalla ?? "-",
                ExamenFisicoPediatricoTallaEdad = examenFisicoPediatria.PediatricoTallaEdad ?? "-",

                #endregion

                // #region REVISION SISTEMAS

                // RevisionSistemasAbdomen = revisionSistemas.Abdomen ?? "-",
                // RevisionSistemasCabeza = revisionSistemas.Cabeza ?? "-",
                // RevisionSistemasCuello = revisionSistemas.Cuello ?? "-",
                // RevisionSistemasOidosBoca = revisionSistemas.OidosBoca ?? "-",
                // RevisionSistemasAparienciaGeneral = revisionSistemas.AparienciaGeneral ?? "-",
                // RevisionSistemasTorax = revisionSistemas.Torax ?? "-",
                // RevisionSistemasDorsoYExtremidades = revisionSistemas.DorsoYExtremidades ?? "-",
                // RevisionSistemasGenitales = revisionSistemas.Genitales ?? "-",


                // #endregion


                #region REVISION SISTEMAS

                NewRevisionSistemasNeurologico = revisionSistemas.Neurologico ?? "-",
                NewRevisionSistemasCardiovascular = revisionSistemas.Cardiovascular ?? "-",
                NewRevisionSistemasRespiratorio = revisionSistemas.Respiratorio ?? "-",
                NewRevisionSistemasGastrointestinal = revisionSistemas.Gastrointestinal ?? "-",
                NewRevisionSistemasMusculoesqueletico = revisionSistemas.Musculoesqueletico ?? "-",
                NewRevisionSistemasPielFanera = revisionSistemas.PielFanera ?? "-",
                NewRevisionSistemasGenitourinario = revisionSistemas.Genitourinario ?? "-",

                RevisionSistemasAbdomen = revisionSistemas.Abdomen ?? "-",
                RevisionSistemasCabeza = revisionSistemas.Cabeza ?? "-",
                RevisionSistemasCuello = revisionSistemas.Cuello ?? "-",
                RevisionSistemasOidosBoca = revisionSistemas.OidosBoca ?? "-",
                RevisionSistemasAparienciaGeneral = revisionSistemas.AparienciaGeneral ?? "-",
                RevisionSistemasTorax = revisionSistemas.Torax ?? "-",
                RevisionSistemasDorsoYExtremidades = revisionSistemas.DorsoYExtremidades ?? "-",
                RevisionSistemasGenitales = revisionSistemas.Genitales ?? "-",

                #endregion


                #region REVISION SISTEMAS - PEDIATRIA

                RevisionSistemasPediatriaAbdomen = revisionSistemasPediatria.Abdomen ?? "-",
                RevisionSistemasPediatriaCabeza = revisionSistemasPediatria.Cabeza ?? "-",
                RevisionSistemasPediatriaCuello = revisionSistemasPediatria.Cuello ?? "-",
                RevisionSistemasPediatriaOidosBoca = revisionSistemasPediatria.OidosBoca ?? "-",
                RevisionSistemasPediatriaAparienciaGeneral = revisionSistemasPediatria.AparienciaGeneral ?? "-",
                RevisionSistemasPediatriaTorax = revisionSistemasPediatria.Torax ?? "-",
                RevisionSistemasPediatriaDorsoYExtremidades = revisionSistemasPediatria.DorsoYExtremidades ?? "-",
                RevisionSistemasPediatriaGenitales = revisionSistemasPediatria.Genitales ?? "-",

                #endregion

                #region GINECOLOGIA - ANT NO PATOLOGICOS
                GinecologiaConsultaMotivo = consulta.GinecologiaConsultaMotivo ?? "-",
                GinecologiaAntNoPatologicosAbortos = gineApnp.Abortos ?? "-",
                GinecologiaAntNoPatologicosCesareas = gineApnp.Cesareas ?? "-",
                GinecologiaAntNoPatologicosCicloMenstrual = gineApnp.CicloMenstrual ?? "-",
                GinecologiaAntNoPatologicosGestas = gineApnp.Gestas ?? "-",
                GinecologiaAntNoPatologicosMenarquia = gineApnp.Menarquia ?? "-",
                GinecologiaAntNoPatologicosFechaUltimaRegla = gineApnp.FechaUltimaRegla ?? "-",
                GinecologiaAntNoPatologicosHijosMuertos = gineApnp.HijosMuertos ?? "-",
                GinecologiaAntNoPatologicosHijosVivos = gineApnp.HijosVivos ?? "-",
                GinecologiaAntNoPatologicosLactanciaMaterna = gineApnp.LactanciaMaterna ?? "-",
                GinecologiaAntNoPatologicosMetodoAnticonceptivo = gineApnp.MetodoAnticonceptivo ?? "-",
                GinecologiaAntNoPatologicosOtros = gineApnp.Otros ?? "-",
                GinecologiaAntNoPatologicosPartos = gineApnp.Partos ?? "-",

                #endregion

                #region GINECOLOGIA - ANT PATOLOGICOS

                GinecologiaAntPatologicosEts = gineAntPatologicos.Ets ?? "-",
                GinecologiaAntPatologicosInfecciones = gineAntPatologicos.Infecciones ?? "-",
                GinecologiaAntPatologicosOtros = gineAntPatologicos.Otros ?? "-",
                GinecologiaAntPatologicosPapanicolau = gineAntPatologicos.Papanicolau ?? "-",

                #endregion

                #region GINECOLOGIA - EXAMEN FISICO

                GinecologiaExamenFisicoEspeculoscopia = gineExamenFisico.Especuloscopia ?? "-",
                GinecologiaExamenFisicoMamas = gineExamenFisico.Mamas ?? "-",
                GinecologiaExamenFisicoTactoRectal = gineExamenFisico.TactoRectal ?? "-",
                GinecologiaExamenFisicoTactoVaginal = gineExamenFisico.TactoVaginal ?? "-",
                GinecologiaExamenFisicoVulvaVagina = gineExamenFisico.VulvaVagina ?? "-",

                #endregion

                #region OBSTETRICIA - ANT NO PATOLOGICOS - ANTECEDENTES - APNP

                ObstetriciaAntNoPatologicosAbortos = obsteApnp.Abortos ?? "-",
                ObstetriciaAntNoPatologicosCesareas = obsteApnp.Cesareas ?? "-",
                ObstetriciaAntNoPatologicosFechaProbableParto = obsteApnp.FechaProbableParto != null
                ? ((DateTime)obsteApnp.FechaProbableParto).ToString("yyyy-MM-dd")
                : "-",
                ObstetriciaAntNoPatologicosFechaUltimaRegla = obsteApnp.FechaUltimaRegla != null
                ? ((DateTime)obsteApnp.FechaUltimaRegla).ToString("yyyy-MM-dd")
                : "-",
                ObstetriciaAntNoPatologicosGestas = obsteApnp.Gestas ?? "-",
                ObstetriciaAntNoPatologicosHijosMuertos = obsteApnp.HijosMuertos ?? "-",
                ObstetriciaAntNoPatologicosHijosVivos = obsteApnp.HijosVivos ?? "-",
                ObstetriciaAntNoPatologicosPartos = obsteApnp.Partos ?? "-",
                ObstetriciaAntNoPatologicosCotarquia = obsteApnp.Cotarquia ?? "-",
                ObstetriciaAntNoPatologicosUltrasonido = obsteApnp.Ultrasonido ?? "-",
                ObstetriciaAntNoPatologicosNumeroParejas = obsteApnp.NumeroParejas,

                #endregion

                #region OBSTETRICIA - ECOGRAFIA OBSTETRICA

                EcografiaObstetricaDorso = consulta.EcografiaObstetricaDorso ?? "-",
                EcografiaObstetricaEstado = consulta.EcografiaObstetricaEstado ?? "-",
                EcografiaObstetricaFeto = consulta.EcografiaObstetricaFeto ?? "-",
                EcografiaObstetricaPosicion = consulta.EcografiaObstetricaPosicion ?? "-",
                EcografiaObstetricaPresentacion = consulta.EcografiaObstetricaPresentacion ?? "-",
                EcografiaObstetricaSituacion = consulta.EcografiaObstetricaSituacion ?? "-",

                #endregion

                #region OBSTETRICIA - BIOMETRIA

                NumeroBebes = consulta.NumeroBebes,

                // Bebé 1
                ObsteBiometriaAc = consulta.ObsteBiometriaAc ?? "-",
                ObsteBiometriaComentario = consulta.ObsteBiometriaComentario ?? "-",
                ObsteBiometriaDbp = consulta.ObsteBiometriaDbp ?? "-",
                ObsteBiometriaEg = consulta.ObsteBiometriaEg ?? "-",
                ObsteBiometriaFcf = consulta.ObsteBiometriaFcf ?? "-",
                ObsteBiometriaFechaParto = consulta.ObsteBiometriaFechaParto ?? "-",
                ObsteBiometriaGrado = consulta.ObsteBiometriaGrado ?? "-",
                ObsteBiometriaHc = consulta.ObsteBiometriaHc ?? "-",
                ObsteBiometriaIla = consulta.ObsteBiometriaIla ?? "-",
                ObsteBiometriaLf = consulta.ObsteBiometriaLf ?? "-",
                ObsteBiometriaMalformaciones = consulta.ObsteBiometriaMalformaciones ?? "-",
                ObsteBiometriaPeso = consulta.ObsteBiometriaPeso ?? "-",
                ObsteBiometriaPlacenta = consulta.ObsteBiometriaPlacenta ?? "-",
                ObsteBiometriaRlc = consulta.ObsteBiometriaRlc ?? "-",
                ObsteBiometriaSexo = consulta.ObsteBiometriaSexo ?? "-",
                ObsteBiometriaSg = consulta.ObsteBiometriaSg ?? "-",
                ObsteBiometriaW = consulta.ObsteBiometriaW ?? "-",
                ObsteBiometriaPresentacion = consulta.ObsteBiometriaPresentacion ?? "-",

                // Bebé 2
                ObsteBiometriaAc2 = consulta.ObsteBiometriaAc2 ?? "-",
                ObsteBiometriaComentario2 = consulta.ObsteBiometriaComentario2 ?? "-",
                ObsteBiometriaDbp2 = consulta.ObsteBiometriaDbp2 ?? "-",
                ObsteBiometriaEg2 = consulta.ObsteBiometriaEg2 ?? "-",
                ObsteBiometriaFcf2 = consulta.ObsteBiometriaFcf2 ?? "-",
                ObsteBiometriaFechaParto2 = consulta.ObsteBiometriaFechaParto2 ?? "-",
                ObsteBiometriaGrado2 = consulta.ObsteBiometriaGrado2 ?? "-",
                ObsteBiometriaHc2 = consulta.ObsteBiometriaHc2 ?? "-",
                ObsteBiometriaIla2 = consulta.ObsteBiometriaIla2 ?? "-",
                ObsteBiometriaLf2 = consulta.ObsteBiometriaLf2 ?? "-",
                ObsteBiometriaMalformaciones2 = consulta.ObsteBiometriaMalformaciones2 ?? "-",
                ObsteBiometriaPeso2 = consulta.ObsteBiometriaPeso2 ?? "-",
                ObsteBiometriaPlacenta2 = consulta.ObsteBiometriaPlacenta2 ?? "-",
                ObsteBiometriaRlc2 = consulta.ObsteBiometriaRlc2 ?? "-",
                ObsteBiometriaSexo2 = consulta.ObsteBiometriaSexo2 ?? "-",
                ObsteBiometriaSg2 = consulta.ObsteBiometriaSg2 ?? "-",
                ObsteBiometriaW2 = consulta.ObsteBiometriaW2 ?? "-",
                ObsteBiometriaPresentacion2 = consulta.ObsteBiometriaPresentacion2 ?? "-",

                // Bebé 3
                ObsteBiometriaAc3 = consulta.ObsteBiometriaAc3 ?? "-",
                ObsteBiometriaComentario3 = consulta.ObsteBiometriaComentario3 ?? "-",
                ObsteBiometriaDbp3 = consulta.ObsteBiometriaDbp3 ?? "-",
                ObsteBiometriaEg3 = consulta.ObsteBiometriaEg3 ?? "-",
                ObsteBiometriaFcf3 = consulta.ObsteBiometriaFcf3 ?? "-",
                ObsteBiometriaFechaParto3 = consulta.ObsteBiometriaFechaParto3 ?? "-",
                ObsteBiometriaGrado3 = consulta.ObsteBiometriaGrado3 ?? "-",
                ObsteBiometriaHc3 = consulta.ObsteBiometriaHc3 ?? "-",
                ObsteBiometriaIla3 = consulta.ObsteBiometriaIla3 ?? "-",
                ObsteBiometriaLf3 = consulta.ObsteBiometriaLf3 ?? "-",
                ObsteBiometriaMalformaciones3 = consulta.ObsteBiometriaMalformaciones3 ?? "-",
                ObsteBiometriaPeso3 = consulta.ObsteBiometriaPeso3 ?? "-",
                ObsteBiometriaPlacenta3 = consulta.ObsteBiometriaPlacenta3 ?? "-",
                ObsteBiometriaRlc3 = consulta.ObsteBiometriaRlc3 ?? "-",
                ObsteBiometriaSexo3 = consulta.ObsteBiometriaSexo3 ?? "-",
                ObsteBiometriaSg3 = consulta.ObsteBiometriaSg3 ?? "-",
                ObsteBiometriaW3 = consulta.ObsteBiometriaW3 ?? "-",
                ObsteBiometriaPresentacion3 = consulta.ObsteBiometriaPresentacion3 ?? "-",

                // Bebé 4
                ObsteBiometriaAc4 = consulta.ObsteBiometriaAc4 ?? "-",
                ObsteBiometriaComentario4 = consulta.ObsteBiometriaComentario4 ?? "-",
                ObsteBiometriaDbp4 = consulta.ObsteBiometriaDbp4 ?? "-",
                ObsteBiometriaEg4 = consulta.ObsteBiometriaEg4 ?? "-",
                ObsteBiometriaFcf4 = consulta.ObsteBiometriaFcf4 ?? "-",
                ObsteBiometriaFechaParto4 = consulta.ObsteBiometriaFechaParto4 ?? "-",
                ObsteBiometriaGrado4 = consulta.ObsteBiometriaGrado4 ?? "-",
                ObsteBiometriaHc4 = consulta.ObsteBiometriaHc4 ?? "-",
                ObsteBiometriaIla4 = consulta.ObsteBiometriaIla4 ?? "-",
                ObsteBiometriaLf4 = consulta.ObsteBiometriaLf4 ?? "-",
                ObsteBiometriaMalformaciones4 = consulta.ObsteBiometriaMalformaciones4 ?? "-",
                ObsteBiometriaPeso4 = consulta.ObsteBiometriaPeso4 ?? "-",
                ObsteBiometriaPlacenta4 = consulta.ObsteBiometriaPlacenta4 ?? "-",
                ObsteBiometriaRlc4 = consulta.ObsteBiometriaRlc4 ?? "-",
                ObsteBiometriaSexo4 = consulta.ObsteBiometriaSexo4 ?? "-",
                ObsteBiometriaSg4 = consulta.ObsteBiometriaSg4 ?? "-",
                ObsteBiometriaW4 = consulta.ObsteBiometriaW4 ?? "-",
                ObsteBiometriaPresentacion4 = consulta.ObsteBiometriaPresentacion4 ?? "-",


                #endregion

                #region ECOGRAFIA ENDOCAVITARIA

                EcografiaEndocavitariaComentario = consulta.EcografiaEndocavitariaComentario ?? "-",
                EcografiaEndocavitariaEndometrio = consulta.EcografiaEndocavitariaEndometrio ?? "-",
                EcografiaEndocavitariaFondoSaco = consulta.EcografiaEndocavitariaFondoSaco ?? "-",
                EcografiaEndocavitariaImpresionClinica = consulta.EcografiaEndocavitariaImpresionClinica ?? "-",
                EcografiaEndocavitariaLongitudinal = consulta.EcografiaEndocavitariaLongitudinal ?? "-",
                EcografiaEndocavitariaOvarioDerecho = consulta.EcografiaEndocavitariaOvarioDerecho ?? "-",
                EcografiaEndocavitariaOvarioIzquierdo = consulta.EcografiaEndocavitariaOvarioIzquierdo ?? "-",
                EcografiaEndocavitariaTransverso = consulta.EcografiaEndocavitariaTransverso ?? "-",
                EcografiaEndocavitariaUtero = consulta.EcografiaEndocavitariaUtero ?? "-",

                #endregion

                #region OBSTETRICIA - EVALUACION OBSTETRICA

                EvaluacionObstetricaAbdomenObstetrico = consulta.EvaluacionObstetricaAbdomenObstetrico ?? "-",
                EvaluacionObstetricaActividadUterina = consulta.EvaluacionObstetricaActividadUterina ?? "-",
                EvaluacionObstetricaAltitud = consulta.EvaluacionObstetricaAltitud ?? "-",
                EvaluacionObstetricaAu = consulta.EvaluacionObstetricaAu ?? "-",
                EvaluacionObstetricaBishop = consulta.EvaluacionObstetricaBishop ?? "-",
                EvaluacionObstetricaBPorciento = consulta.EvaluacionObstetricaBPorciento ?? "-",
                EvaluacionObstetricaCms = consulta.EvaluacionObstetricaCms ?? "-",
                EvaluacionObstetricaConsistencia = consulta.EvaluacionObstetricaConsistencia ?? "-",
                EvaluacionObstetricaD = consulta.EvaluacionObstetricaD ?? "-",
                EvaluacionObstetricaFcf = consulta.EvaluacionObstetricaFcf ?? "-",
                EvaluacionObstetricaLiquidoAmniotico = consulta.EvaluacionObstetricaLiquidoAmniotico ?? "-",
                EvaluacionObstetricaLiquidoAmnioticoEspecifique = consulta.EvaluacionObstetricaLiquidoAmnioticoEspecifique ?? "-",
                EvaluacionObstetricaMembranasOvulares = consulta.EvaluacionObstetricaMembranasOvulares ?? "-",
                EvaluacionObstetricaMovimientoFetalEspecifique = consulta.EvaluacionObstetricaMovimientoFetalEspecifique ?? "-",
                EvaluacionObstetricaMovimientoFetalPercetible = consulta.EvaluacionObstetricaMovimientoFetalPercetible ?? "-",
                EvaluacionObstetricaOtras = consulta.EvaluacionObstetricaOtras ?? "-",
                EvaluacionObstetricaPelvis = consulta.EvaluacionObstetricaPelvis ?? "-",
                EvaluacionObstetricaPosicionCervix = consulta.EvaluacionObstetricaPosicionCervix ?? "-",
                EvaluacionObstetricaPresentacionLeopold = consulta.EvaluacionObstetricaPresentacionLeopold ?? "-",
                EvaluacionObstetricaTactoVaginal = consulta.EvaluacionObstetricaTactoVaginal ?? "-",
                EvaluacionObstetricaUteroGravio = consulta.EvaluacionObstetricaUteroGravio ?? "-",

                #endregion
            };
            model.Consulta = consulta;
            model.Consulta.ConsultaExamenArchivos = archisvosExamen;
            var cita = _citasRepository.GetCita((int)consulta.CitasId);

            //model.Paciente = cita.Paciente;

            var prescripcion = _consultasRepository.GetPrescripcionConsulta((int)id);

            if (prescripcion != null)
            {
                model.PrescripcionId = prescripcion.Id;
                model.Color = prescripcion.Color;
                model.HayPrescripcion = true;
                //model.Prescripcion = prescripcion;
            }

            #region OFTALMOLOGIA REPOSITORIOS
            // ==== OFTALMOLOGÍA (llenado en modo lectura/Información) ====
            // No debe bloquear la vista si la tabla no existe o hay timeout.
            try
            {
                // Obtener el registro de la tabla ConsultasOftalmologia para esta consulta
                var oft = _oftRepo.GetConsulta((int)id);
                if (oft != null)
                {
                    // IDs para upsert (si abres luego en edición)
                    model.Oft_Id = (int)oft.Id;
                    model.Oft_ConsultaId = oft.ConsultaId;

                    // Motivo / Antecedentes
                    model.Oft_HistoriaEnfermedadActual = oft.HistoriaEnfermedadActual;
                    model.Oft_PacienteMedicos = oft.PacienteMedicos;
                    model.Oft_PacienteQuirurgicos = oft.PacienteQuirurgicos;
                    model.Oft_PacienteTraumaticos = oft.PacienteTraumaticos;
                    model.Oft_PacienteAlergias = oft.PacienteAlergias;
                    model.Oft_PacienteFamiliares = oft.PacienteFamiliares;

                    // Datos objetivos - AV sin corrección
                    model.Oft_AgudezaSC_Test = oft.AgudezaSC_Test;
                    model.Oft_AgudezaSC_OD = oft.AgudezaSC_OD;
                    model.Oft_AgudezaSC_OS = oft.AgudezaSC_OS;

                    // Sensibilidad de contraste
                    model.Oft_Contraste_OD = oft.Contraste_OD;
                    model.Oft_Contraste_OS = oft.Contraste_OS;

                    // AV cerca sin corrección
                    model.Oft_AVCerca_OD = oft.AVCerca_OD;
                    model.Oft_AVCerca_OS = oft.AVCerca_OS;

                    // Tests especiales
                    model.Oft_TestIshihara_OD = oft.TestIshihara_OD;
                    model.Oft_TestIshihara_OS = oft.TestIshihara_OS;
                    model.Oft_TestEstereopsis_OD = oft.TestEstereopsis_OD;
                    model.Oft_TestEstereopsis_OS = oft.TestEstereopsis_OS;

                    // Óptica - Lensometría (Histórico)
                    model.Oft_Lensometria_OD_Esfera = oft.Lensometria_OD_Esfera;
                    model.Oft_Lensometria_OD_Cilindro = oft.Lensometria_OD_Cilindro;
                    model.Oft_Lensometria_OD_Eje = oft.Lensometria_OD_Eje;
                    model.Oft_Lensometria_OD_Agudeza = oft.Lensometria_OD_Agudeza;
                    model.Oft_Lensometria_OS_Esfera = oft.Lensometria_OS_Esfera;
                    model.Oft_Lensometria_OS_Cilindro = oft.Lensometria_OS_Cilindro;
                    model.Oft_Lensometria_OS_Eje = oft.Lensometria_OS_Eje;
                    model.Oft_Lensometria_OS_Agudeza = oft.Lensometria_OS_Agudeza;

                    // Óptica - Final
                    model.Oft_Final_OD_Esfera = oft.Final_OD_Esfera;
                    model.Oft_Final_OD_Cilindro = oft.Final_OD_Cilindro;
                    model.Oft_Final_OD_Eje = oft.Final_OD_Eje;
                    model.Oft_Final_OD_Agudeza = oft.Final_OD_Agudeza;
                    model.Oft_Final_OS_Esfera = oft.Final_OS_Esfera;
                    model.Oft_Final_OS_Cilindro = oft.Final_OS_Cilindro;
                    model.Oft_Final_OS_Eje = oft.Final_OS_Eje;
                    model.Oft_Final_OS_Agudeza = oft.Final_OS_Agudeza;
                    model.Oft_Final_Adicion = oft.Final_Adicion;
                    model.Oft_Final_DIP_mm = oft.Final_DIP_mm;

                    // Óptica - Retinoscopía
                    model.Oft_Retino_OD_Esfera = oft.Retino_OD_Esfera;
                    model.Oft_Retino_OD_Cilindro = oft.Retino_OD_Cilindro;
                    model.Oft_Retino_OD_Eje = oft.Retino_OD_Eje;
                    model.Oft_Retino_OS_Esfera = oft.Retino_OS_Esfera;
                    model.Oft_Retino_OS_Cilindro = oft.Retino_OS_Cilindro;
                    model.Oft_Retino_OS_Eje = oft.Retino_OS_Eje;

                    // Tipo de lente / Material
                    model.Oft_TipoLente = oft.TipoLente;
                    model.Oft_LenteMaterialTratamiento = oft.LenteMaterialTratamiento;

                    // Inspección / LH / Oftalmoscopía
                    model.Oft_Inspeccion_MovExtraoculares_OD = oft.Inspeccion_MovExtraoculares_OD;
                    model.Oft_Inspeccion_MovExtraoculares_OS = oft.Inspeccion_MovExtraoculares_OS;
                    model.Oft_Inspeccion_Cejas_OD = oft.Inspeccion_Cejas_OD;
                    model.Oft_Inspeccion_Cejas_OS = oft.Inspeccion_Cejas_OS;
                    model.Oft_Inspeccion_ParpadosPestanas_OD = oft.Inspeccion_ParpadosPestanas_OD;
                    model.Oft_Inspeccion_ParpadosPestanas_OS = oft.Inspeccion_ParpadosPestanas_OS;
                    model.Oft_Inspeccion_ViaLagrimal_OD = oft.Inspeccion_ViaLagrimal_OD;
                    model.Oft_Inspeccion_ViaLagrimal_OS = oft.Inspeccion_ViaLagrimal_OS;

                    // Segmento anterior
                    model.Oft_Inspeccion_Conjuntiva_OD = oft.Inspeccion_Conjuntiva_OD;
                    model.Oft_Inspeccion_Conjuntiva_OS = oft.Inspeccion_Conjuntiva_OS;
                    model.Oft_Inspeccion_CorneaEsclera_OD = oft.Inspeccion_CorneaEsclera_OD;
                    model.Oft_Inspeccion_CorneaEsclera_OS = oft.Inspeccion_CorneaEsclera_OS;
                    model.Oft_Inspeccion_CamaraAnteriorAngulo_OD = oft.Inspeccion_CamaraAnteriorAngulo_OD;
                    model.Oft_Inspeccion_CamaraAnteriorAngulo_OS = oft.Inspeccion_CamaraAnteriorAngulo_OS;
                    model.Oft_Inspeccion_IrisPupila_OD = oft.Inspeccion_IrisPupila_OD;
                    model.Oft_Inspeccion_IrisPupila_OS = oft.Inspeccion_IrisPupila_OS;
                    model.Oft_Inspeccion_Cristalino_OD = oft.Inspeccion_Cristalino_OD;
                    model.Oft_Inspeccion_Cristalino_OS = oft.Inspeccion_Cristalino_OS;
                    model.Oft_Inspeccion_BUT_OD = oft.Inspeccion_BUT_OD;
                    model.Oft_Inspeccion_BUT_OS = oft.Inspeccion_BUT_OS;
                    model.Oft_Inspeccion_PresionIntraocular_OD = oft.Inspeccion_PresionIntraocular_OD;
                    model.Oft_Inspeccion_PresionIntraocular_OS = oft.Inspeccion_PresionIntraocular_OS;

                    // Segmento posterior
                    model.Oft_Inspeccion_Vitreo_OD = oft.Inspeccion_Vitreo_OD;
                    model.Oft_Inspeccion_Vitreo_OS = oft.Inspeccion_Vitreo_OS;
                    model.Oft_Inspeccion_NervioOptico_OD = oft.Inspeccion_NervioOptico_OD;
                    model.Oft_Inspeccion_NervioOptico_OS = oft.Inspeccion_NervioOptico_OS;
                    model.Oft_Inspeccion_Macula_OD = oft.Inspeccion_Macula_OD;
                    model.Oft_Inspeccion_Macula_OS = oft.Inspeccion_Macula_OS;
                    model.Oft_Inspeccion_Retina_OD = oft.Inspeccion_Retina_OD;
                    model.Oft_Inspeccion_Retina_OS = oft.Inspeccion_Retina_OS;

                    // Impresión clínica / Comentario
                    model.Oft_HistoriaClinicaImpresionClinica = oft.HistoriaClinicaImpresionClinica;
                    model.Oft_HistoriaClinicaComentario = oft.HistoriaClinicaComentario;
                }
            }
            catch
            {
                // No romper la vista de Información si falla solo oftalmología
            }
            #endregion

            #region PODOLOGIA REPOSITORIOS
            // ==== PODOLOGÍA (llenado en modo lectura/Información) ====
            // No debe bloquear la vista si la tabla no existe o hay timeout.
            try
            {
                // Obtener el registro de la tabla ConsultasPodologia para esta consulta
                var pod = _podRepo.GetConsulta((int)id); // id == ConsultaId
                if (pod != null)
                {
                    // IDs para upsert (si abres luego en edición)
                    model.Pod_Id = (int)pod.Id;
                    model.Pod_ConsultaId = pod.ConsultaId;
                    model.Pod_PacienteId = pod.PacienteId;

                    // 1) Antecedentes Médicos
                    model.Pod_Enfermedades = pod.Enfermedades ?? Array.Empty<string>();
                    model.Pod_Enfermedades_Otros = pod.Enfermedades_Otros;
                    model.Pod_Medicamentos = pod.Medicamentos;
                    model.Pod_PresionArterial = pod.PresionArterial;

                    // 2) Examen del Pie
                    model.Pod_Pulso_Pedio = pod.Pulso_Pedio;
                    model.Pod_Pulso_TibialPosterior = pod.Pulso_TibialPosterior;
                    model.Pod_Pulso_Popliteo = pod.Pulso_Popliteo;
                    model.Pod_TemperaturaPie = pod.TemperaturaPie;
                    model.Pod_ProblemasCirculatorios = pod.ProblemasCirculatorios;
                    model.Pod_EstadoPiel = pod.EstadoPiel;
                    model.Pod_ObservacionesExamen = pod.ObservacionesExamen;

                    // 3) Tratamiento Realizado
                    model.Pod_Procedimientos = pod.Procedimientos ?? Array.Empty<string>();
                    model.Pod_OtrosProcedimientos = pod.OtrosProcedimientos;
                    model.Pod_ObservacionesTratamiento = pod.ObservacionesTratamiento;

                    // 4) Indicaciones y Datos Finales
                    model.Pod_Indicaciones = pod.Indicaciones;
                    model.Pod_PesoKg = pod.PesoKg;
                    model.Pod_EstaturaM = pod.EstaturaM;
                    model.Pod_FechaAtencion = pod.FechaAtencion;
                    model.Pod_Profesional = pod.Profesional;
                }
            }
            catch
            {
                // No romper la vista de Información si falla solo podología
            }
            #endregion

            #region ENFERMERÍA REPOSITORIOS
            try
            {
                // Aquí sí buscamos por ConsultaId (id)
                var hce = _enfRepo.GetConsulta((int)id);
                if (hce != null)
                {
                    // PK/FKs
                    model.Hce_Id = hce.Id;
                    model.Hce_ConsultaId = hce.ConsultaId;
                    model.Hce_PacienteId = hce.PacienteId;

                    // 1) Tipo de consulta
                    model.Hce_TipoConsulta = hce.TipoConsulta;

                    // 3) Antecedentes (nueva estructura)
                    model.Hce_AntecedentesPatologicos = hce.AntecedentesPatologicos;
                    model.Hce_AntecedentesQuirurgicos = hce.AntecedentesQuirurgicos;
                    model.Hce_AntecedentesTraumaticos = hce.AntecedentesTraumaticos;
                    model.Hce_Hospitalizaciones = hce.Hospitalizaciones;
                    model.Hce_Alergias = hce.Alergias;
                    model.Hce_AntecedentesFamiliares = hce.AntecedentesFamiliares;

                    // 2) Motivo de consulta
                    model.Hce_MotivoConsulta = hce.MotivoConsulta;

                    // 4) Hábitos
                    model.Hce_HabitoAlimentacion = hce.HabitoAlimentacion;
                    model.Hce_ActividadFisica = hce.ActividadFisica;
                    model.Hce_HabitoAlcoholTexto = hce.HabitoAlcoholTexto;
                    model.Hce_HabitoTabacoTexto = hce.HabitoTabacoTexto;
                    model.Hce_OtrosHabitos = hce.OtrosHabitos;

                    // 5) Signos vitales y antropometría
                    model.Hce_PresionArterialTxt = hce.PresionArterialTxt;
                    model.Hce_FC = hce.FC;
                    model.Hce_FR = hce.FR;
                    model.Hce_TemperaturaC = hce.TemperaturaC;
                    model.Hce_SPO2 = hce.SPO2;
                    model.Hce_PesoKg = hce.PesoKg;
                    model.Hce_TallaM = hce.TallaM;
                    model.Hce_IMC = hce.IMC;

                    // 6) Exploración por aparatos y sistemas
                    model.Hce_CabezaCuello = hce.CabezaCuello;
                    model.Hce_ToraxPulmones = hce.ToraxPulmones;
                    model.Hce_Corazon = hce.Corazon;
                    model.Hce_Abdomen = hce.Abdomen;
                    model.Hce_Extremidades = hce.Extremidades;
                    model.Hce_SistemaNeurologico = hce.SistemaNeurologico;
                    model.Hce_PielAnexos = hce.PielAnexos;

                    // 8) Valoración de enfermería
                    model.Hce_ValConcienciaOrientacion = hce.ValConcienciaOrientacion;
                    model.Hce_ValEstadoNutricional = hce.ValEstadoNutricional;
                    model.Hce_ValEliminacion = hce.ValEliminacion;
                    model.Hce_ValSuenoDescanso = hce.ValSuenoDescanso;
                    model.Hce_ValActividadMovilidad = hce.ValActividadMovilidad;
                    model.Hce_ValAutonomia = hce.ValAutonomia;

                    // 9) Laboratorios
                    model.Hce_Laboratorios = hce.Laboratorios;

                    // 10) Diagnóstico de enfermería
                    model.Hce_DiagnosticoEnfermeria = hce.DiagnosticoEnfermeria;

                    // 11) Plan de cuidados / Intervenciones
                    model.Hce_AccionesRealizadas = hce.AccionesRealizadas;
                    model.Hce_MedicamentosAdministrados = hce.MedicamentosAdministrados;
                    model.Hce_Tratamiento = hce.Tratamiento;

                    // 12) Seguimiento / Evolución / Cita
                    model.Hce_Seguimiento = hce.Seguimiento;
                }
            }
            catch
            {
                // No romper la vista de Información si falla solo enfermería
            }
            #endregion

            #region VALORACION INICIAL ENFERMERIA REPOSITORIOS
            // ==== VE (llenado en modo lectura/Información) ====
            // No debe bloquear la vista si la tabla no existe o hay timeout.
            try
            {
                var ve = _veRepo.GetConsulta((int)id); // id == ConsultaId
                if (ve != null)
                {
                    // IDs para upsert (si abres luego en edición)
                    model.Ve_Id = (int)ve.Id;
                    model.Ve_ConsultaId = ve.ConsultaId;
                    model.Ve_PacienteId = ve.PacienteId;

                    // 1) Datos de Valoración Inicial
                    model.Ve_Motivo = ve.Motivo;
                    model.Ve_DiagnosticoMedico = ve.DiagnosticoMedico;
                    model.Ve_Labs = ve.Labs;

                    // 2) ¿Cómo se enteró del servicio?
                    model.Ve_Medio = ve.Medio ?? Array.Empty<string>();

                    // 3) Oxigenación y Circulación
                    model.Ve_Resp = ve.Resp ?? Array.Empty<string>();
                    model.Ve_Circ = ve.Circ ?? Array.Empty<string>();

                    // 4) Necesidad de Nutrición
                    model.Ve_Nutricion = ve.Nutricion ?? Array.Empty<string>();
                    model.Ve_NutricionObs = ve.NutricionObs;

                    // 5) Necesidad de Eliminación
                    model.Ve_Urinario = ve.Urinario ?? Array.Empty<string>();
                    model.Ve_Intestinal = ve.Intestinal ?? Array.Empty<string>();

                    // 6) Movilización y Estado de Conciencia
                    model.Ve_Mov = ve.Mov ?? Array.Empty<string>();
                    model.Ve_Conciencia = ve.Conciencia ?? Array.Empty<string>();

                    // 7) Autocuidado y Reposo
                    model.Ve_Sueno = ve.Sueno ?? Array.Empty<string>();
                    model.Ve_Vestirse = ve.Vestirse;
                    model.Ve_Higiene = ve.Higiene;
                    model.Ve_Piel = ve.Piel ?? Array.Empty<string>();
                    model.Ve_PielUbicacion = ve.PielUbicacion;

                    // 8) Necesidad de Comunicación
                    model.Ve_Lenguaje = ve.Lenguaje ?? Array.Empty<string>();
                    model.Ve_Vision = ve.Vision ?? Array.Empty<string>();
                    model.Ve_Oido = ve.Oido ?? Array.Empty<string>();

                    // 9) Seguridad y Factores Psicosociales
                    model.Ve_Seg = ve.Seg ?? Array.Empty<string>();
                    model.Ve_Religiosos = ve.Religiosos;
                    model.Ve_CreenciasObservaciones = ve.CreenciasObservaciones;
                    model.Ve_ConoceMotivo = ve.ConoceMotivo;
                    model.Ve_NecesitaInfo = ve.NecesitaInfo;

                    // 10) Medicación y Plan
                    model.Ve_MedicacionActual = ve.MedicacionActual;
                    model.Ve_PlanTerapeutico = ve.PlanTerapeutico;
                }
            }
            catch
            {
                // No romper la vista de Información si falla solo VE
            }
            #endregion

            #region SUEROTERAPIA REPOSITORIOS
            // ==== SUEROTERAPIA (llenado en modo lectura/Información) ====
            try
            {
                var suero = _sueroRepo.GetConsulta((int)id); // id == ConsultaId
                if (suero != null)
                {
                    // IDs para upsert (si luego editas)
                    model.Suero_Id = suero.Id;
                    model.Suero_ConsultaId = suero.ConsultaId;
                    model.Suero_PacienteId = suero.PacienteId;

                    // 1) Datos de Valoración Inicial
                    model.Suero_Motivo = suero.Motivo;
                    model.Suero_DiagnosticoMedico = suero.DiagnosticoMedico;
                    model.Suero_Labs = suero.Labs;

                    // 2) ¿Cómo se enteró?
                    model.Suero_Medio = suero.Medio ?? Array.Empty<string>();

                    // 3) Oxigenación y Circulación
                    model.Suero_Resp = suero.Resp ?? Array.Empty<string>();
                    model.Suero_Circ = suero.Circ ?? Array.Empty<string>();

                    // 4) Nutrición
                    model.Suero_Nutricion = suero.Nutricion ?? Array.Empty<string>();
                    model.Suero_NutricionObs = suero.NutricionObs;

                    // 5) Plan
                    model.Suero_PlanTerapeutico = suero.PlanTerapeutico;
                }
            }
            catch
            {
                // No romper la vista si falla solo Sueroterapia
            }
            #endregion

            #region HISTORICO OFTALMOLOGIA
            try
            {
                var pacienteId = paciente?.Id ?? 0;
                if (pacienteId > 0)
                {
                    // Última consulta (resumen)
                    var oftUltima = _oftRepo.GetConsultaByPaciente(pacienteId);
                    if (oftUltima != null)
                    {
                        model.FechaUltimaConsulta = oftUltima.Fecha;
                        model.MotivoUltimaConsulta = string.IsNullOrWhiteSpace(oftUltima.HistoriaClinicaImpresionClinica)
                            ? "-"
                            : oftUltima.HistoriaClinicaImpresionClinica;
                    }

                    // Histórico completo
                    var oftHist = _oftRepo.GetConsultasByPaciente(pacienteId) ?? Enumerable.Empty<ConsultasOftalmologia>();

                    // (si te sirve la tabla-resumen, conserva este bloque)
                    var top = 10;
                    var filas = oftHist
                        .Take(top)
                        .Select(x => new
                        {
                            x.ConsultaId,
                            x.Fecha,
                            x.AgudezaSC_OD,
                            x.AgudezaSC_OS,
                            x.AVCerca_OD,
                            x.AVCerca_OS,
                            x.Final_OD_Esfera,
                            x.Final_OD_Cilindro,
                            x.Final_OD_Eje,
                            x.Final_OS_Esfera,
                            x.Final_OS_Cilindro,
                            x.Final_OS_Eje,
                            x.Inspeccion_PresionIntraocular_OD,
                            x.Inspeccion_PresionIntraocular_OS,
                            x.HistoriaClinicaImpresionClinica
                        })
                        .ToList();

                    ViewBag.OftHistorial = filas;
                    ViewBag.OftHistorialTotal = oftHist.Count();

                    // 👉 **CLAVE PARA MOSTRAR TODOS LOS CAMPOS**:
                    ViewBag.OftHistorialFull = oftHist.ToList(); // lista completa de ConsultasOftalmologia
                }
                else
                {
                    ViewBag.OftHistorial = new List<object>();
                    ViewBag.OftHistorialTotal = 0;
                    ViewBag.OftHistorialFull = new List<ConsultasOftalmologia>();
                }
            }
            catch
            {
                ViewBag.OftHistorial = new List<object>();
                ViewBag.OftHistorialTotal = 0;
                ViewBag.OftHistorialFull = new List<ConsultasOftalmologia>();
            }
            #endregion

            #region HISTORICO PODOLOGIA
            try
            {
                var pacienteId = paciente?.Id ?? 0;
                if (pacienteId > 0)
                {
                    // Última consulta (resumen rápido)
                    var podUltima = _podRepo.GetConsultaByPaciente(pacienteId);
                    if (podUltima != null)
                    {
                        // Reusa las mismas propiedades de VM que ya llenabas con oftalmología,
                        // así no rompes nada del layout superior.
                        model.FechaUltimaConsulta = podUltima.Fecha;
                        // Toma un texto significativo: primero Observaciones del examen o, si están vacías, las Indicaciones
                        var resumen = string.IsNullOrWhiteSpace(podUltima.ObservacionesExamen)
                            ? podUltima.Indicaciones
                            : podUltima.ObservacionesExamen;
                        model.MotivoUltimaConsulta = string.IsNullOrWhiteSpace(resumen) ? "-" : resumen;
                    }

                    // Histórico completo
                    var podHist = _podRepo.GetConsultasByPaciente(pacienteId)
                                 ?? Enumerable.Empty<ConsultasPodologia>();

                    // Tabla-resumen (opcional): top N filas con datos clave
                    var top = 10;
                    var filas = podHist
                        .Take(top)
                        .Select(x => new
                        {
                            x.ConsultaId,
                            x.Fecha,
                            x.PresionArterial,
                            x.Pulso_Pedio,
                            x.Pulso_TibialPosterior,
                            x.Pulso_Popliteo,
                            x.TemperaturaPie,
                            x.EstadoPiel,
                            x.ProblemasCirculatorios,
                            x.PesoKg,
                            x.EstaturaM,
                            x.Profesional,
                            Resumen = string.IsNullOrWhiteSpace(x.ObservacionesExamen) ? x.Indicaciones : x.ObservacionesExamen
                        })
                        .ToList();

                    ViewBag.PodHistorial = filas;
                    ViewBag.PodHistorialTotal = podHist.Count();

                    // 👉 CLAVE: lista completa para que el formulario/partial pueda pintar TODOS los campos
                    ViewBag.PodHistorialFull = podHist.ToList();
                }
                else
                {
                    ViewBag.PodHistorial = new List<object>();
                    ViewBag.PodHistorialTotal = 0;
                    ViewBag.PodHistorialFull = new List<ConsultasPodologia>();
                }
            }
            catch
            {
                ViewBag.PodHistorial = new List<object>();
                ViewBag.PodHistorialTotal = 0;
                ViewBag.PodHistorialFull = new List<ConsultasPodologia>();
            }
            #endregion

            #region HISTORICO HISTORIA CLÍNICA DE ENFERMERÍA
            try
            {
                var pacienteId = paciente?.Id ?? 0;
                if (pacienteId > 0)
                {
                    // Última consulta (resumen)
                    var hceUltima = _enfRepo.GetConsultaByPaciente(pacienteId);
                    if (hceUltima != null)
                    {
                        model.FechaUltimaConsulta = hceUltima.Fecha;
                        // Si hay diagnóstico de enfermería úsalo; si no, cae al motivo de consulta; si tampoco, usa "-"
                        model.MotivoUltimaConsulta =
                            !string.IsNullOrWhiteSpace(hceUltima.DiagnosticoEnfermeria) ? hceUltima.DiagnosticoEnfermeria :
                            !string.IsNullOrWhiteSpace(hceUltima.MotivoConsulta) ? hceUltima.MotivoConsulta :
                            "-";
                    }

                    // Histórico completo
                    var hceHist = _enfRepo.GetConsultasByPaciente(pacienteId)
                                ?? Enumerable.Empty<ConsultasHistoriaClinicaEnfermeria>();

                    // (Si te sirve la tabla-resumen, conserva este bloque "filas")
                    var top = 10;
                    var filas = hceHist
                        .Take(top)
                        .Select(x => new
                        {
                            x.ConsultaId,
                            x.Fecha,

                            // Vitales y antropometría
                            x.PresionArterialTxt,
                            x.FC,
                            x.FR,
                            x.TemperaturaC,
                            x.SPO2,
                            x.PesoKg,
                            x.TallaM,
                            x.IMC,

                            // Un resumen clínico útil
                            x.DiagnosticoEnfermeria
                        })
                        .ToList();

                    ViewBag.HceHistorial = filas;
                    ViewBag.HceHistorialTotal = hceHist.Count();

                    // 👉 Lista completa para el acordeón con todos los campos del modelo de HCE
                    ViewBag.HceHistorialFull = hceHist.ToList();
                }
                else
                {
                    ViewBag.HceHistorial = new List<object>();
                    ViewBag.HceHistorialTotal = 0;
                    ViewBag.HceHistorialFull = new List<ConsultasHistoriaClinicaEnfermeria>();
                }
            }
            catch
            {
                ViewBag.HceHistorial = new List<object>();
                ViewBag.HceHistorialTotal = 0;
                ViewBag.HceHistorialFull = new List<ConsultasHistoriaClinicaEnfermeria>();
            }
            #endregion

            #region HISTORICO VALORACION INICIAL ENFERMERIA
            try
            {
                var pacienteId = paciente?.Id ?? 0;
                if (pacienteId > 0)
                {
                    // Última valoración (resumen)
                    var vieUltima = _veRepo.GetConsultaByPaciente(pacienteId);
                    if (vieUltima != null)
                    {
                        model.FechaUltimaConsulta = vieUltima.Fecha;
                        // Para el resumen, mostramos el Motivo (puedes cambiar a DiagnosticoMedico si prefieres)
                        model.MotivoUltimaConsulta = string.IsNullOrWhiteSpace(vieUltima.Motivo)
                            ? "-"
                            : vieUltima.Motivo;
                    }

                    // Histórico completo
                    var vieHist = _veRepo.GetConsultasByPaciente(pacienteId)
                                 ?? Enumerable.Empty<ConsultasValoracionInicialEnfermeria>();

                    // Tabla-resumen (opcional)
                    var top = 10;
                    var filas = vieHist
                        .Take(top)
                        .Select(x => new
                        {
                            x.ConsultaId,
                            x.Fecha,
                            x.Motivo,
                            x.DiagnosticoMedico,
                            x.Labs
                            // Si quisieras, podrías incluir contadores de checks: 
                            // Resp = (x.Resp ?? Array.Empty<string>()).Length, etc.
                        })
                        .ToList();

                    ViewBag.VeHistorial = filas;
                    ViewBag.VeHistorialTotal = vieHist.Count();

                    // 👉 Lista completa para poder renderizar TODOS los campos en la vista
                    ViewBag.VeHistorialFull = vieHist.ToList();
                }
                else
                {
                    ViewBag.VeHistorial = new List<object>();
                    ViewBag.VeHistorialTotal = 0;
                    ViewBag.VeHistorialFull = new List<ConsultasValoracionInicialEnfermeria>();
                }
            }
            catch
            {
                ViewBag.VeHistorial = new List<object>();
                ViewBag.VeHistorialTotal = 0;
                ViewBag.VeHistorialFull = new List<ConsultasValoracionInicialEnfermeria>();
            }
            #endregion

            #region HISTORICO SUEROTERAPIA
            try
            {
                var pacienteId = paciente?.Id ?? 0;
                if (pacienteId > 0)
                {
                    // Última consulta (resumen)
                    var sueroUltima = _sueroRepo.GetConsultaByPaciente(pacienteId);
                    if (sueroUltima != null)
                    {
                        model.FechaUltimaConsulta = sueroUltima.Fecha;
                        // En sueroterapia no hay "impresión clínica". Dejar un resumen simple con Motivo.
                        model.MotivoUltimaConsulta = string.IsNullOrWhiteSpace(sueroUltima.Motivo)
                            ? "-"
                            : sueroUltima.Motivo;
                    }

                    // Histórico completo
                    var sueroHist = _sueroRepo.GetConsultasByPaciente(pacienteId)
                                    ?? Enumerable.Empty<Database.Shared.Models.ConsultasSueroterapia>();

                    // Tabla-resumen (opcional, top 10)
                    var top = 10;
                    var filas = sueroHist
                        .Take(top)
                        .Select(x => new
                        {
                            x.ConsultaId,
                            x.Fecha,
                            x.Motivo,
                            x.DiagnosticoMedico,
                            x.Labs,
                            Medio = x.Medio,       // string[] (no transformar aquí)
                            Resp = x.Resp,        // string[]
                            Circ = x.Circ,        // string[]
                            Nutricion = x.Nutricion, // string[]
                            x.NutricionObs,
                            x.PlanTerapeutico
                        })
                        .ToList();

                    ViewBag.SueroHistorial = filas;
                    ViewBag.SueroHistorialTotal = sueroHist.Count();

                    // 👉 Lista completa, con TODOS los campos del modelo:
                    ViewBag.SueroHistorialFull = sueroHist.ToList();
                }
                else
                {
                    ViewBag.SueroHistorial = new List<object>();
                    ViewBag.SueroHistorialTotal = 0;
                    ViewBag.SueroHistorialFull = new List<Database.Shared.Models.ConsultasSueroterapia>();
                }
            }
            catch
            {
                ViewBag.SueroHistorial = new List<object>();
                ViewBag.SueroHistorialTotal = 0;
                ViewBag.SueroHistorialFull = new List<Database.Shared.Models.ConsultasSueroterapia>();
            }
            #endregion

            #region HISTORICO EXAMENES COMPLEMENTARIOS

            // Historial de exámenes complementarios del paciente
            try
            {
                // Valores por defecto en el ViewBag para no romper la vista en caso de error
                ViewBag.ExamenesHistorial = new List<object>();
                ViewBag.ExamenesHistorialTieneMas = false;
                ViewBag.ExamenesHistorialTotal = 0;

                var pacienteId = paciente?.Id ?? 0;

                if (pacienteId > 0)
                {
                    // 1) EXÁMENES VINCULADOS A CONSULTA (mundo consulta)
                    var examenesConsulta = _dbContext.ConsultasExamenLabClinicos
                        .Include(e => e.ExamenLabClinico)
                        .Include(e => e.Consulta)
                            .ThenInclude(c => c.Citas)
                        .Where(e =>
                            e.Consulta != null &&
                            e.Consulta.Citas != null &&
                            e.Consulta.Citas.PacienteId == pacienteId &&
                            e.Pagado == true) // SOLO exámenes pagados
                        .OrderByDescending(e => e.Consulta.FechaYHoraInicioConsulta)
                        .ToList();

                    // Exámenes de laboratorio para este paciente que SÍ tienen consulta
                    var examenesLaboratorioConConsulta = _dbContext.Examenes
                        .Where(x =>
                            x.PacienteId == pacienteId &&
                            x.ConsultaId != null &&
                            !x.Eliminado)
                        .ToList();

                    // Proyección de exámenes de consulta → historial
                    var examenesHistorialConsulta = examenesConsulta
                        .Select(e =>
                        {
                            var consultaId = e.ConsultaId ?? 0;
                            var pacienteIdConsulta = e.Consulta?.Citas?.PacienteId;

                            // Match por ConsultaId + PacienteId
                            var examenLab = examenesLaboratorioConConsulta.FirstOrDefault(x =>
                                x.ConsultaId == consultaId &&
                                x.PacienteId == pacienteIdConsulta);

                            return new
                            {
                                // Id de ConsultaExamenLabClinico (histórico original)
                                Id = (int?)e.Id,

                                // Usamos la fecha de la consulta
                                Fecha = (DateTime?)e.Consulta.FechaYHoraInicioConsulta,

                                // No. consulta (siempre > 0 en este caso)
                                ConsultaId = consultaId,

                                ExamenCodigo = e.ExamenLabClinico != null ? e.ExamenLabClinico.CodigoInterno : string.Empty,
                                ExamenNombre = e.ExamenLabClinico != null ? e.ExamenLabClinico.NombreExamen : string.Empty,

                                Cantidad = (int?)e.Cantidad,
                                ValorTotal = (decimal?)(e.PrecioValor * e.Cantidad),
                                Pagado = e.Pagado,

                                // Id de examen de laboratorio (para GenerarResultados)
                                ExamenLaboratorioId = examenLab != null ? (int?)examenLab.Id : null,
                                FechaRealizacion = examenLab != null ? (DateTime?)examenLab.FechaRealizacion : null
                            };
                        });

                    // 2) EXÁMENES DE LABORATORIO SIN CONSULTA (mundo laboratorio)
                    //    Se muestran también en el historial, marcados como "Sin consulta"
                    var examenesLaboratorioSinConsulta = _dbContext.Examenes
                        .Include(x => x.DetalleExamenes)
                            .ThenInclude(d => d.ExamenLabClinico)
                        .Where(x =>
                            x.PacienteId == pacienteId &&
                            x.ConsultaId == null &&
                            !x.Eliminado)
                        .ToList();

                    var examenesHistorialSinConsulta = examenesLaboratorioSinConsulta
                        .SelectMany(ex => ex.DetalleExamenes.Select(det => new
                        {
                            // No hay registro en ConsultaExamenLabClinico para estos casos
                            Id = (int?)null,

                            // Usamos la fecha de realización del examen de laboratorio
                            Fecha = (DateTime?)ex.FechaRealizacion,

                            // Sin consulta asociada → usamos 0 como sentinel
                            ConsultaId = 0, // "Sin consulta" en la vista

                            ExamenCodigo = det.ExamenLabClinico != null ? det.ExamenLabClinico.CodigoInterno : string.Empty,
                            ExamenNombre = det.ExamenLabClinico != null ? det.ExamenLabClinico.NombreExamen : string.Empty,

                            // No tenemos información económica en este flujo → dejamos vacío
                            Cantidad = (int?)null,
                            ValorTotal = (decimal?)null,
                            Pagado = false,

                            // Siempre existe examen de laboratorio aquí
                            ExamenLaboratorioId = (int?)ex.Id,
                            FechaRealizacion = (DateTime?)ex.FechaRealizacion
                        }));

                    // 3) UNIMOS AMBOS CONJUNTOS PARA EL HISTORIAL
                    var examenesHistorial = examenesHistorialConsulta
                        .Concat(examenesHistorialSinConsulta)
                        .Where(x => x.Fecha.HasValue) // evitar filas sin fecha
                        .OrderByDescending(x => x.Fecha.Value)
                        .ToList();

                    ViewBag.ExamenesHistorial = examenesHistorial;
                    ViewBag.ExamenesHistorialTieneMas = false;
                    ViewBag.ExamenesHistorialTotal = examenesHistorial.Count;
                }
            }
            catch (Exception ex)
            {
                // Ante cualquier error dejamos el historial vacío y registramos el problema
                Console.WriteLine("Error al cargar el historial de exámenes del paciente: " + ex.Message);
                ViewBag.ExamenesHistorial = new List<object>();
                ViewBag.ExamenesHistorialTieneMas = false;
                ViewBag.ExamenesHistorialTotal = 0;
            }

            #endregion


            #region HISTORICO SERVICIOS CONSULTA

            // Historial de servicios del paciente (todas las consultas del paciente)
            try
            {
                // Valores por defecto
                ViewBag.ServiciosHistorial = new List<object>();
                ViewBag.ServiciosHistorialTieneMas = false;
                ViewBag.ServiciosHistorialTotal = 0;

                var pacienteId = paciente?.Id ?? 0;

                if (pacienteId > 0)
                {
                    var serviciosPaciente = _dbContext.ConsultasServicios
                        .Include(s => s.Servicio)
                        .Include(s => s.Consulta)
                            .ThenInclude(c => c.Citas)
                        .Where(s =>
                            s.Consulta != null &&
                            s.Consulta.Citas != null &&
                            s.Consulta.Citas.PacienteId == pacienteId &&
                            s.Pagado == true) // ← SOLO servicios pagados
                        .OrderByDescending(s => s.Consulta.FechaYHoraInicioConsulta)
                        .ToList();

                    var totalRegistros = serviciosPaciente.Count;

                    var serviciosHistorial = serviciosPaciente
                        .Select(s => new
                        {
                            // La vista espera: Fecha, ConsultaId, ServicioCodigo, ServicioNombre, Cantidad, ValorTotal
                            Fecha = (DateTime?)s.Consulta.FechaYHoraInicioConsulta,
                            ConsultaId = s.ConsultaId,
                            ServicioCodigo = s.Servicio != null ? s.Servicio.CodigoInterno : string.Empty,
                            ServicioNombre = s.Servicio != null ? s.Servicio.NombreServicio : string.Empty,
                            Cantidad = s.Cantidad,
                            ValorTotal = s.PrecioValor * s.Cantidad,
                            Pagado = s.Pagado // ← lo exponemos para el color en la vista
                        })
                        .ToList();

                    ViewBag.ServiciosHistorial = serviciosHistorial;
                    ViewBag.ServiciosHistorialTieneMas = false; // ya no hay tope de 10
                    ViewBag.ServiciosHistorialTotal = totalRegistros;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cargar el historial de servicios del paciente: " + ex.Message);
                ViewBag.ServiciosHistorial = new List<object>();
                ViewBag.ServiciosHistorialTieneMas = false;
                ViewBag.ServiciosHistorialTotal = 0;
            }

            #endregion


            return View(model);
        }


        [HttpPost]
        public JsonResult ConsultarInformacion(int consultaId)
        {
            var consulta = _consultasRepository.GetConsulta(consultaId);
            return Json(new { Exitoso = true, Resultado = consulta });
        }
        [HttpPost]
        public string ConsultarVacunasPaciente(int pacienteId)
        {
            var listaVacunas = new List<VacunaPacienteConsulta>();
            var vacunasPaciente = _pacientesRepository.GetVacunasPaciente(pacienteId);


            var vacunas = _pacientesRepository.GetVacunas();

            if (vacunasPaciente == null || vacunasPaciente.Count == 0)
            {
                foreach (var vacuna in vacunas)
                {
                    var vacunaConsulta = new VacunaPacienteConsulta
                    {
                        VacunaId = vacuna.Id,
                        NombreVacuna = vacuna.Nombre,
                        Primera = false,
                        FechaPrimera = new DateTime(),
                        Segunda = false,
                        FechaSegunda = new DateTime(),
                        Tercera = false,
                        FechaTercera = new DateTime(),
                        PrimerRefuerzo = false,
                        FechaPrimerRefuerzo = new DateTime(),
                        SegundoRefuerzo = false,
                        FechaSegundoRefuerzo = new DateTime()
                    };
                    listaVacunas.Add(vacunaConsulta);
                }
            }
            else
            {
                foreach (var vacuna in vacunas)
                {
                    var existe = vacunasPaciente
                        .Where(v => v.VacunaId == vacuna.Id)
                        .FirstOrDefault();
                    if (existe == null)
                    {
                        var vacunaConsulta = new VacunaPacienteConsulta
                        {
                            VacunaId = vacuna.Id,
                            NombreVacuna = vacuna.Nombre,
                            //Inicializar las vacunas que no estan asociadas a el paciente pero son nuevas
                            Primera = false,
                            FechaPrimera = new DateTime(),
                            Segunda = false,
                            FechaSegunda = new DateTime(),
                            Tercera = false,
                            FechaTercera = new DateTime(),
                            PrimerRefuerzo = false,
                            FechaPrimerRefuerzo = new DateTime(),
                            SegundoRefuerzo = false,
                            FechaSegundoRefuerzo = new DateTime()
                        };
                        listaVacunas.Add(vacunaConsulta);
                    }
                    else
                    {
                        var vacunaConsulta = new VacunaPacienteConsulta
                        {
                            VacunaPacienteId = existe.Id,
                            VacunaId = existe.VacunaId,
                            NombreVacuna = existe.Vacuna.Nombre,
                            Primera = existe.Primera,
                            FechaPrimera = existe.FechaPrimera,
                            Segunda = existe.Segunda,
                            FechaSegunda = existe.FechaSegunda,
                            Tercera = existe.Tercera,
                            FechaTercera = existe.FechaTercera,
                            PrimerRefuerzo = existe.PrimerRefuerzo,
                            FechaPrimerRefuerzo = existe.FechaPrimerRefuerzo,
                            SegundoRefuerzo = existe.SegundoRefuerzo,
                            FechaSegundoRefuerzo = existe.FechaSegundoRefuerzo
                        };
                        listaVacunas.Add(vacunaConsulta);
                    }
                }
            }
            return JsonSerializer.Serialize(new { Exitoso = true, Resultado = listaVacunas });
        }
        [HttpPost]
        public string EliminarArchivoConsulta(int archivoId)
        {
            try
            {
                _consultasService.DeleteArchivoConsulta(archivoId);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Mensaje = "Archivo eliminado"
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al eliminar archivo: " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarArchivosConsulta(int pacienteId)
        {
            try
            {
                var consultas = _consultasService.GetArchivosConsultaNew(pacienteId);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = consultas
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar archivos: " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarExamenesArchivo(int consultaId)
        {
            try
            {
                var listaexamenes = new List<ConsultaExamenArchivo>();
                var archivosExamenes = _consultasRepository.GetExamenArchivo(consultaId);

                if (archivosExamenes != null)
                {
                    foreach (var archivoExamen in archivosExamenes)
                    {
                        listaexamenes.Add(new ConsultaExamenArchivo
                        {
                            ConsultaId = consultaId,
                            Id = archivoExamen.Id,
                            NombreArchivo = archivoExamen.NombreArchivo,
                            UrlArchivo = archivoExamen.UrlArchivo

                        });


                    }
                }
                return JsonSerializer.Serialize(new { Exitoso = true, Resultado = listaexamenes });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar servicios. " + ex.Message
                });
            }

        }
        [HttpPost]
        public string ConsultarAntecedentesFamiliaresPaciente(int pacienteId)
        {
            try
            {
                var listaPatologias = new List<AntecedenteFamiliarPacienteConsultaViewModel>();
                var patologiasPaciente = _pacientesRepository.GetAntecedentesFamiliaresPaciente(pacienteId);


                var patologias = _pacientesRepository.GetTipoPatologias();

                if (patologiasPaciente == null || patologiasPaciente.Count() == 0)
                {
                    foreach (var patologia in patologias)
                    {
                        var patologiaConsulta = new AntecedenteFamiliarPacienteConsultaViewModel
                        {
                            TipoPatologiaId = patologia.Id,
                            TipoPatologia = patologia.Tipo,
                            AbuelaMaterna = false,
                            AbueloMaterno = false,
                            Madre = false,
                            OtrosMaterno = false,
                            AbuelaPaterna = false,
                            AbueloPaterno = false,
                            OtrosPaterno = false,
                            Hermanos = false,
                            Padre = false,
                            TipoPatologiaVerInputDescripcion = patologia.VerInputDescripcion
                        };
                        listaPatologias.Add(patologiaConsulta);
                    }
                }
                else
                {
                    foreach (var patologia in patologias)
                    {
                        var existe = patologiasPaciente
                            .Where(v => v.TipoPatologiaId == patologia.Id)
                            .FirstOrDefault();
                        if (existe == null)
                        {
                            var patologiaConsulta = new AntecedenteFamiliarPacienteConsultaViewModel
                            {
                                TipoPatologiaId = patologia.Id,
                                TipoPatologia = patologia.Tipo
                            };
                            listaPatologias.Add(patologiaConsulta);
                        }
                        else
                        {
                            var patologiaConsulta = new AntecedenteFamiliarPacienteConsultaViewModel
                            {
                                Id = existe.Id,
                                TipoPatologiaId = existe.TipoPatologiaId,
                                TipoPatologia = existe.TipoPatologia.Tipo,
                                Madre = existe.Madre,
                                AbuelaMaterna = existe.AbuelaMaterna,
                                AbueloMaterno = existe.AbueloMaterno,
                                AbuelaPaterna = existe.AbuelaPaterna,
                                AbueloPaterno = existe.AbueloPaterno,
                                OtrosMaterno = existe.OtrosMaterno,
                                OtrosPaterno = existe.OtrosPaterno,
                                Hermanos = existe.OtrosPaterno,
                                Padre = existe.Padre,
                                DescripcionOtraPatologia = existe.DescripcionOtraPatologia,
                                TipoPatologiaVerInputDescripcion = existe.TipoPatologia.VerInputDescripcion
                            };
                            listaPatologias.Add(patologiaConsulta);
                        }
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaPatologias
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar antecedentes familiares. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarCaracteristicasDentales(int? consultaId)
        {
            var listaCaracteristicas = new List<CaracteristicasDiente>();
            if (consultaId == null)
            {
                for (int i = 1; i <= 32; i++)
                {
                    var caracteristica = new CaracteristicasDiente
                    {
                        NumeroDiente = i
                    };
                    listaCaracteristicas.Add(caracteristica);
                }
            }
            else
            {
                var caracteristicasDentalesConsulta = _consultasRepository.GetCaracteristicasDentales(consultaId);
                foreach (var caracteristica in caracteristicasDentalesConsulta)
                {
                    var caracteristicaConsulta = new CaracteristicasDiente
                    {
                        NumeroDiente = (int)caracteristica.NumeroDiente,
                        Percusiones_VerticalMas = caracteristica.Percusiones_VerticalMas,
                        Percusiones_HorizontalMas = caracteristica.Percusiones_HorizontalMas,
                        Percusiones_VerticalMenos = caracteristica.Percusiones_VerticalMenos,
                        Percusiones_HorizontalMenos = caracteristica.Percusiones_HorizontalMenos,
                        Dolor_Localizado = caracteristica.Dolor_Localizado,
                        Dolor_Fugaz = caracteristica.Dolor_Fugaz,
                        Dolor_Persistente = caracteristica.Dolor_Persistente,
                        Dolor_Referido = caracteristica.Dolor_Referido,
                        Dolor_Espontaneo = caracteristica.Dolor_Espontaneo,
                        Estimulo_Frio = caracteristica.Estimulo_Frio,
                        Estimulo_Calor = caracteristica.Estimulo_Calor,
                        Estimulo_DulceAcido = caracteristica.Estimulo_DulceAcido,
                        Estimulo_Masticacion = caracteristica.Estimulo_Masticacion,
                        Estimulo_Otro = caracteristica.Estimulo_Otro,
                        TermicaFrio_Positiva = caracteristica.TermicaFrio_Positiva,
                        TermicaFrio_Negativa = caracteristica.TermicaFrio_Negativa,
                        TermicaFrio_Localizada = caracteristica.TermicaFrio_Localizada,
                        TermicaFrio_Fugaz = caracteristica.TermicaFrio_Fugaz,
                        TermicaFrio_Incrementa = caracteristica.TermicaFrio_Incrementa,
                        TermicaFrio_Referida = caracteristica.TermicaFrio_Referida,
                        TermicaFrio_Irradiado = caracteristica.TermicaFrio_Irradiado,
                        TermicaFrio_Persistente = caracteristica.TermicaFrio_Persistente,
                        TermicaFrio_Decrece = caracteristica.TermicaFrio_Decrece,
                        TermicaCalor_Positiva = caracteristica.TermicaCalor_Positiva,
                        TermicaCalor_Negativa = caracteristica.TermicaCalor_Negativa,
                        TermicaCalor_Localizada = caracteristica.TermicaCalor_Localizada,
                        TermicaCalor_Fugaz = caracteristica.TermicaCalor_Fugaz,
                        TermicaCalor_Incrementa = caracteristica.TermicaCalor_Incrementa,
                        TermicaCalor_Referida = caracteristica.TermicaCalor_Referida,
                        TermicaCalor_Irradiado = caracteristica.TermicaCalor_Irradiado,
                        TermicaCalor_Persistente = caracteristica.TermicaCalor_Persistente,
                        TermicaCalor_Decrece = caracteristica.TermicaCalor_Decrece,
                        Diagnostico_ManchaBlanca = caracteristica.Diagnostico_ManchaBlanca,
                        Diagnostico_Caries = caracteristica.Diagnostico_Caries,
                        Diagnostico_Traumatismo = caracteristica.Diagnostico_Traumatismo,
                        Diagnostico_Abfraccion = caracteristica.Diagnostico_Abfraccion,
                        Diagnostico_Atricion = caracteristica.Diagnostico_Atricion,
                        Diagnostico_Erosion = caracteristica.Diagnostico_Erosion,
                        Diagnostico_Restauracion = caracteristica.Diagnostico_Restauracion,
                        Diagnostico_Ajustada = caracteristica.Diagnostico_Ajustada,
                        Diagnostico_Desajustada = caracteristica.Diagnostico_Desajustada,
                        Diagnostico_PulpaSana = caracteristica.Diagnostico_PulpaSana,
                        Diagnostico_PulpitisReversible = caracteristica.Diagnostico_PulpitisReversible,
                        Diagnostico_PulpitisIrreversible = caracteristica.Diagnostico_PulpitisIrreversible,
                        Diagnostico_NecrosisPulpar = caracteristica.Diagnostico_NecrosisPulpar
                    };
                    listaCaracteristicas.Add(caracteristicaConsulta);
                }
            }
            return JsonSerializer.Serialize(new { Exitoso = true, Resultado = listaCaracteristicas });
        }
        [HttpPost]
        public string ConsultarSeguimientosNutricionalesPaciente(int pacienteId)
        {
            var listaSeguimientos = new List<SeguimientoNutricionalConsulta>();
            var seguimientosPaciente = _pacientesRepository.GetSeguimientosNutricionalesPaciente(pacienteId);
            if (seguimientosPaciente != null && seguimientosPaciente.Count > 0)
            {
                foreach (var seguimiento in seguimientosPaciente)
                {
                    var seguimientoConsulta = new SeguimientoNutricionalConsulta
                    {
                        Id = seguimiento.Id,
                        Fecha = seguimiento.Fecha,
                        Nuevo = false,
                        Peso = seguimiento.Peso,
                        IMC = seguimiento.IMC,
                        PGC = seguimiento.PGC,
                        Cuello = seguimiento.Cuello,
                        Busto = seguimiento.Busto,
                        CinturaAbdomen = seguimiento.CinturaAbdomen,
                        Cadera = seguimiento.Cadera,
                        Muslo = seguimiento.Muslo,
                        Brazo = seguimiento.Brazo,
                        Munneca = seguimiento.Muñeca
                    };

                    listaSeguimientos.Add(seguimientoConsulta);
                }
            }
            return JsonSerializer.Serialize(new { Exitoso = true, Resultado = listaSeguimientos });
        }
        [HttpPost]
        public string ConsultarServiciosAgregadosConsulta(int consultaId)
        {
            var listaServicios = new List<ConsultasServicioAgregadoViewModel>();

            var serviciosConsulta = _consultasRepository.GetServiciosAgregados(consultaId);
            foreach (var servicio in serviciosConsulta)
            {
                var servicioConsulta = new ConsultasServicioAgregadoViewModel
                {
                    Id = servicio.Id,
                    ConsultaId = servicio.ConsultaId,
                    ServicioId = servicio.ServicioId,
                    ServicioCodigo = servicio.Servicio.CodigoInterno,
                    NombreServicio = servicio.Servicio.NombreServicio,
                    NumeroDiente = servicio.NumeroDiente,
                    PrecioId = servicio.PrecioId,
                    PrecioNombre = servicio.Precio.NombrePrecio,
                    PrecioValor = servicio.PrecioValor,
                    ServicioCantidad = servicio.Cantidad,
                    Aplicar = true,
                    ServicioValorTotal = servicio.PrecioValor * servicio.Cantidad,
                    ServicioValorCubiertoSeguro = servicio.PrecioCubiertoSeguro,
                    ServicioValorCopago = servicio.PrecioCopago,
                    Pagado = servicio.Pagado
                };
                listaServicios.Add(servicioConsulta);
            }
            return JsonSerializer.Serialize(new { Exitoso = true, Resultado = listaServicios });
        }
        [HttpPost]
        public string ConsultarServiciosCita(int citaId)
        {
            try
            {
                var listaServicios = new List<ConsultasServicioAgregadoViewModel>();
                var serviciosCitaBd = _citasRepository.GetCita(citaId)
                    .CitasServicios
                    .ToList();
                if (serviciosCitaBd != null)
                {
                    foreach (var servicio in serviciosCitaBd)
                    {
                        listaServicios.Add(new ConsultasServicioAgregadoViewModel
                        {
                            Id = servicio.Id,
                            ServicioCodigo = servicio.Servicio.CodigoInterno,
                            Aplicar = true,
                            NombreServicio = servicio.Servicio.NombreServicio,
                            ServicioId = servicio.ServicioId,
                            PrecioId = servicio.PrecioId,
                            ServicioCantidad = 1,
                            PrecioNombre = servicio.Precio.NombrePrecio,
                            PrecioValor = servicio.PrecioValor,
                            ServicioValorTotal = servicio.PrecioValor,
                            ServicioValorCubiertoSeguro = servicio.PrecioValorCubiertoSeguro,
                            ServicioValorCopago = servicio.PrecioValorCopago
                        });
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaServicios
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar servicios. " + ex.Message
                });
            }
        }

        /// <summary>
        /// REVISADO JUANPA FECHA: 17-AGOSTO-2024. consulta de examenes agregados cuando es una CITA
        /// </summary>
        /// <param name="citaId"></param>
        /// <returns></returns>
        public string ConsultarExamenesAgregadosCita(int citaId)
        {
            try
            {
                var listaExamenesLab = new List<CitaExamenesViewModel>();
                var ExamenesLabCitaBd = _citasRepository.GetCita(citaId)
                    .CitasExamenes
                    .ToList();
                if (ExamenesLabCitaBd != null)
                {
                    foreach (var examen in ExamenesLabCitaBd)
                    {
                        var precio = examen.Precio ?? new Precio();
                        listaExamenesLab.Add(new CitaExamenesViewModel
                        {
                            Id = examen.ExamenLabClinico.Id,
                            ExamenCodigo = examen.ExamenLabClinico.CodigoInterno,
                            NombreExamen = examen.ExamenLabClinico.NombreExamen,
                            PrecioId = examen.PrecioId,
                            PrecioNombre = precio.NombrePrecio,
                            Cantidad = 1,
                            PrecioValor = examen.PrecioValor,
                            ValorCubiertoSeguro = examen.PrecioValorCubiertoSeguro,
                            ValorCopago = examen.PrecioValorCopago,
                        });
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaExamenesLab
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar servicios. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarPrescripcionCita(int citaId)
        {
            try
            {
                var listaPrescripcion = new List<ConsultaPrescripcionViewModel>();
                var prescripcionConsulta = _consultasRepository.GetPrescripcionesCita(citaId).FirstOrDefault();
                if (prescripcionConsulta != null
                    && prescripcionConsulta.DetallePrescripcion != null)
                {
                    foreach (var elemento in prescripcionConsulta.DetallePrescripcion)
                    {
                        var unidadVenta = elemento.UnidadMedidaVenta ?? new UnidadMedidaVenta();
                        var producto = elemento.Producto ?? new Producto();
                        var precio = elemento.Precio ?? new Precio();
                        listaPrescripcion.Add(new ConsultaPrescripcionViewModel
                        {
                            Item = elemento.Item,
                            Medicamento = elemento.Medicine,
                            PrecioValor = elemento.PrecioValor,
                            ProductoIndicaciones = elemento.Indications,
                            ValorTotal = elemento.PrecioValor * elemento.Cantidad,
                            ProductoPrecioId = elemento.PrecioId,
                            UnidadMedidaVentaId = elemento.UnidadMedidaVentaId,
                            UnidadMedidaVentaNombre = unidadVenta.Nombre,
                            Precio = precio.NombrePrecio,
                            ProductoId = elemento.ProductoId,
                            ProductoCodigo = producto.CodigoReferencia ?? "N/A",
                            ProductoNombre = producto.NombreProducto ?? elemento.Medicine,
                            Cantidad = elemento.Cantidad,
                            Observaciones = elemento.Indications,
                            Pagado = elemento.Pagado,
                            Color = elemento.Color,
                            FechaPrescripcion = elemento.FechaPrescripcion,
                        });
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaPrescripcion
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar prescripcion. " + ex.Message
                });
            }
        }

        [HttpPost]
        public string ConsultarPrescripcionesPaciente(int pacienteId, int? citaId)
        {
            try
            {
                var listaPrescripcion = new List<ConsultaPrescripcionViewModel>();

                // Obtener TODAS las prescripciones del paciente (no solo la primera)
                var prescripcionesPaciente = _consultasRepository.GetPrescripcionesPaciente(pacienteId);

                if (prescripcionesPaciente != null && prescripcionesPaciente.Any())
                {
                    // Iterar por cada prescripción del paciente
                    foreach (var prescripcion in prescripcionesPaciente)
                    {
                        if (prescripcion.DetallePrescripcion != null && prescripcion.DetallePrescripcion.Any())
                        {

                            bool esAnother = prescripcion.CitasId != citaId;

                            foreach (var elemento in prescripcion.DetallePrescripcion)
                            {
                                var unidadVenta = elemento.UnidadMedidaVenta ?? new UnidadMedidaVenta();
                                var producto = elemento.Producto ?? new Producto();
                                var precio = elemento.Precio ?? new Precio();

                                listaPrescripcion.Add(new ConsultaPrescripcionViewModel
                                {
                                    // Agregar información de la prescripción padre
                                    PrescripcionId = prescripcion.Id,
                                    ColorPrescripcion = prescripcion.Color,
                                    ConsultaId = (int)prescripcion.ConsultaId,
                                    // Información del detalle
                                    Item = elemento.Item,
                                    Medicamento = elemento.Medicine,
                                    PrecioValor = elemento.PrecioValor,
                                    ProductoIndicaciones = elemento.Indications,
                                    ValorTotal = elemento.PrecioValor * elemento.Cantidad,
                                    ProductoPrecioId = elemento.PrecioId,
                                    UnidadMedidaVentaId = elemento.UnidadMedidaVentaId,
                                    UnidadMedidaVentaNombre = unidadVenta.Nombre,
                                    Precio = precio.NombrePrecio,
                                    ProductoId = elemento.ProductoId,
                                    ProductoCodigo = producto.CodigoReferencia ?? "N/A",
                                    ProductoNombre = producto.NombreProducto ?? elemento.Medicine,
                                    Cantidad = elemento.Cantidad,
                                    Observaciones = elemento.Indications,
                                    Pagado = elemento.Pagado,
                                    Color = elemento.Color,
                                    Another = esAnother,
                                    FechaPrescripcion = elemento.FechaPrescripcion, // O usar otra fecha si existe
                                });
                            }
                        }
                    }
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaPrescripcion,
                    TotalPrescripciones = prescripcionesPaciente?.Count ?? 0,
                    TotalDetalles = listaPrescripcion.Count
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar prescripciones. " + ex.Message
                });
            }
        }


        public IActionResult EditarConsulta(int? id)
        {
            if (id == null) return StatusCode(404);

            var consulta = _consultasRepository.GetConsulta((int)id);
            if (consulta == null) return StatusCode(400);


            var paciente = consulta.Citas.Paciente ?? new Paciente();
            var seguroEpss = consulta.Citas.Paciente.SeguroEpss ?? new SeguroEpss();
            var examenFisico = consulta.ExamenFisico ?? new ExamenFisico();
            var examenFisicoPediatria = consulta.ExamenFisicoPediatria ?? new ExamenFisicoPediatria();
            var revisionSistemasPediatria = consulta.ConsultaRevisionSistemasPediatria ?? new ConsultaRevisionSistemasPediatria();
            var historia = consulta.Historia ?? new Historia();
            var historiaPediatria = consulta.HistoriaPediatria ?? new HistoriaPediatria();
            var pacienteListaApnp = paciente.PacienteApnp ?? new List<PacienteApnp>();
            var pacienteApnpPediatrico = paciente.PacientePediatricoApnp ?? new PacientePediatricoApnp();
            var gineApnp = consulta.ConsultaAntNoPatologicosGinecologia ?? new ConsultaAntNoPatologicosGinecologia();
            var obsteApnp = consulta.ConsultaAntNoPatologicosObstetricia ?? new ConsultaAntNoPatologicosObstetricia();

            var model = new ConsultasViewModel();
            model.CitaId = consulta.CitasId;
            model.ConsultaId = consulta.Id;
            model.PacienteId = consulta.Citas.Paciente.Id;
            model.PacienteNombre = consulta.Citas.Paciente.Nombre;
            model.PacienteNit = consulta.Citas.Paciente.Nit;
            model.PacienteDireccion = consulta.Citas.Paciente.Direccion;
            model.PacienteAlias = consulta.Citas.Paciente.Alias;
            model.PacienteSexo = consulta.Citas.Paciente.sexoText;
            model.PacienteFechaNacimiento = consulta.Citas.Paciente.FechaNacimiento == null ? "" :
                ((DateTime)consulta.Citas.Paciente.FechaNacimiento).ToShortDateString();
            model.PacienteEdad = consulta.Citas.Paciente.FechaNacimiento != null
                ? CalcularEdad((DateTime)consulta.Citas.Paciente.FechaNacimiento)
                : "-";
            model.PacienteTelefono = consulta.Citas.Paciente.Telefono;
            model.PacienteCelular = consulta.Citas.Paciente.Celular;

            model.Cie10Codigo = consulta.Cie10Codigo ?? "";


            #region PACIENTE - ANTECEDENTES PATOLOGICOS

            model.PacienteMedicos = consulta.Citas.Paciente.AntecedentesMedicos;
            model.PacienteQuirurgicos = consulta.Citas.Paciente.AntecedentesQuirurgicos;
            model.PacienteTraumaticos = consulta.Citas.Paciente.AntecedentesTraumaticos;
            model.PacienteAlergias = consulta.Citas.Paciente.AntecedentesAlergias;
            model.PacienteVicios = consulta.Citas.Paciente.AntecedentesVicios;
            model.PacienteMedicamentos = consulta.Citas.Paciente.AntecedentesMedicamentos;

            #endregion

            #region PACIENTE - ANTECEDENTES PATOLOGICOS

            model.PediatricoAntMedicos = consulta.Citas.Paciente.PediatricosAntMedicos;
            model.PediatricoAntQuirurgicos = consulta.Citas.Paciente.PediatricosAntQuirurgicos;
            model.PediatricoAntTraumaticos = consulta.Citas.Paciente.PediatricosAntTraumaticos;
            model.PediatricoAntAlergias = consulta.Citas.Paciente.PediatricosAntAlergias;
            model.PediatricoAntVicios = consulta.Citas.Paciente.PediatricosAntVicios;
            model.PediatricoAntMedicamentos = consulta.Citas.Paciente.PediatricosAntMedicamentos;

            #endregion

            model.FechaYHoraInicio = consulta.FechaYHoraInicioConsulta;
            model.FechaProximaConsulta = consulta.FechaProximaConsulta?.ToLocalTime();

            model.ObservacionesAdicionales = consulta.ObservacionesAdicionales;
            model.CostoConsulta = consulta.CostoConsulta;
            model.UrlFiles = consulta.UrlFiles;
            model.TipoConsulta = consulta.TipoConsulta;
            model.CitaTipoAtencion = consulta.Citas.CitaTipoAtencion;
            model.TipoReferencia = consulta.TipoReferencia;
            model.MedicoReferido = consulta.MedicoReferido;
            model.ConsultaMotivo = consulta.ConsultaMotivo;
            model.HistoriaPediatriaConsultaMotivo = consulta.ConsultaMotivoPediatria;
            model.GinecologiaConsultaMotivo = consulta.GinecologiaConsultaMotivo;

            //MANANTIALES NO utiliza estos campos
            //model.FaseTratamientoId = consulta.FaseTratamientoId;
            //model.EstadoPagoId = consulta.EstadoPagoConsultaId;

            #region HOSPITALIZACION

            model.Hospitalizada = consulta.Hospitalizado;
            model.HospitalizacionId = consulta.HospitalizacionId;
            if (consulta.Hospitalizado && consulta.HospitalizacionId != null)
            {
                var hospitalizacion = consulta.Hospitalizacion ?? new Hospitalizacion();
                var habitacion = hospitalizacion.Habitacion ?? new Habitacion();
                model.HospitalizacionNumeroHabitacion = habitacion.NombreNumeroHabitacion;
            }

            #endregion

            #region APNP PACIENTE - ANTECEDENTES NO PATOLOGICOS

            var pacienteApnp = new PacienteApnp();
            if (paciente.PacienteApnp != null && pacienteListaApnp.Count() > 0)
            {
                pacienteApnp = pacienteListaApnp.FirstOrDefault();
            }
            //Fecha de ultima regla
            string fechaUltimaRegla = null;
            if (pacienteApnp.FechaUltimaRegla != null)
                fechaUltimaRegla = ((DateTime)pacienteApnp.FechaUltimaRegla).ToString("yyyy-MM-dd");
            //Fecha probable de parto
            string fechaProbableParto = null;
            if (pacienteApnp.FechaProbableParto != null)
                fechaProbableParto = ((DateTime)pacienteApnp.FechaProbableParto).ToString("yyyy-MM-dd");

            model.PacienteApnpGestas = pacienteApnp.Gestas;
            model.PacienteApnpPartos = pacienteApnp.Partos;
            model.PacienteApnpAbortos = pacienteApnp.Abortos;
            model.PacienteApnpCesareas = pacienteApnp.Cesareas;
            model.PacienteApnpMenarquia = pacienteApnp.Menarquia;
            model.PacienteApnpHijosVivos = pacienteApnp.HijosVivos;
            model.PacienteApnpHijosMuertos = pacienteApnp.HijosMuertos;
            model.PacienteApnpFechaUltimaRegla = fechaUltimaRegla;
            model.PacienteApnpFechaProbableParto = fechaProbableParto;
            model.PacienteApnpOtros = pacienteApnp.Otros;

            #endregion

            #region CONSULTA - HISTORIA CLINICA

            model.HistoriaId = consulta.HistoriaId;
            model.HistoriaProblema = historia.HistoriaProblema;
            model.Sintomas = historia.Sintomas;
            model.Diagnostico = historia.Diagnostico;
            model.HistoriaEnfermedadActual = historia.HistoriaEnfermedadActual;
            model.HistoriaClinicaComentario = historia.Comentario;
            model.HistoriaClinicaImpresionClinica = historia.ImpresionClinica;

            #endregion

            #region PACIENTE - PEDIATRICO - APNP - ANTECEDENTES NO PATOLOGICOS

            var pediatricoApnp = new PacientePediatricoApnp();
            if (paciente.PacientePediatricoApnpId != null && paciente.PacientePediatricoApnp != null)
            {
                pediatricoApnp = paciente.PacientePediatricoApnp;
            }
            model.PediatricoApnpParto = pediatricoApnp.Parto;
            model.PediatricoApnpAtendidoPor = pediatricoApnp.AtendidoPor;
            model.PediatricoApnpPesoAlNacer = pediatricoApnp.PesoAlNacer;
            model.PediatricoApnpInmunizaciones = pediatricoApnp.Inmunizaciones;
            model.PediatricoApnpGesta = pediatricoApnp.Gesta;

            #endregion

            //Ginecologico
            model.ExamenGinecologico = consulta.ExamenGinecologico;

            #region GINECOLOGIA - ANT NO PATOLOGICOS

            if (consulta.ConsultaAntNoPatologicosGinecologiaId != null && consulta.ConsultaAntNoPatologicosGinecologia != null)
            {
                var gineAntNoPatologicos = consulta.ConsultaAntNoPatologicosGinecologia;
                model.GinecologiaAntNoPatologicosMenarquia = gineAntNoPatologicos.Menarquia;
                model.GinecologiaAntNoPatologicosFechaUltimaRegla = gineAntNoPatologicos.FechaUltimaRegla;
                model.GinecologiaAntNoPatologicosCicloMenstrual = gineAntNoPatologicos.CicloMenstrual;
                model.GinecologiaAntNoPatologicosMetodoAnticonceptivo = gineAntNoPatologicos.MetodoAnticonceptivo;
                model.GinecologiaAntNoPatologicosLactanciaMaterna = gineAntNoPatologicos.LactanciaMaterna;
                model.GinecologiaAntNoPatologicosGestas = gineAntNoPatologicos.Gestas;
                model.GinecologiaAntNoPatologicosPartos = gineAntNoPatologicos.Partos;
                model.GinecologiaAntNoPatologicosAbortos = gineAntNoPatologicos.Abortos;
                model.GinecologiaAntNoPatologicosCesareas = gineAntNoPatologicos.Cesareas;
                model.GinecologiaAntNoPatologicosHijosVivos = gineAntNoPatologicos.HijosVivos;
                model.GinecologiaAntNoPatologicosHijosMuertos = gineAntNoPatologicos.HijosMuertos;
                model.GinecologiaAntNoPatologicosOtros = gineAntNoPatologicos.Otros;
            }

            #endregion

            #region HISTORIA CLINICA - PEDIATRIA

            if (consulta.HistoriaPediatriaId != null && consulta.HistoriaPediatria != null)
            {
                var historiaClinicaPediatria = consulta.HistoriaPediatria;
                model.HistoriaPediatriaHistoriaEnfermedadActual = historiaClinicaPediatria.HistoriaEnfermedadActual;
                model.HistoriaPediatriaHistoriaProblema = historiaClinicaPediatria.HistoriaProblema;
                model.HistoriaPediatriaHistoriaClinicaComentario = historiaClinicaPediatria.Comentario;
                model.HistoriaPediatriaHistoriaClinicaImpresionClinica = historiaClinicaPediatria.ImpresionClinica;
                model.HistoriaPediatriaDiagnostico = historiaClinicaPediatria.Diagnostico;
                model.HistoriaPediatriaSintomas = historiaClinicaPediatria.Sintomas;
            }

            #endregion

            #region OBSTETRICIA - ANT NO PATOLOGICOS

            if (consulta.ConsultaAntNoPatologicosObstetriciaId != null && consulta.ConsultaAntNoPatologicosObstetricia != null)
            {
                var obsteAntNoPatologicos = consulta.ConsultaAntNoPatologicosObstetricia;
                model.ObstetriciaAntNoPatologicosGestas = obsteAntNoPatologicos.Gestas;
                model.ObstetriciaAntNoPatologicosPartos = obsteAntNoPatologicos.Partos;
                model.ObstetriciaAntNoPatologicosCotarquia = obsteApnp.Cotarquia;
                model.ObstetriciaAntNoPatologicosUltrasonido = obsteApnp.Ultrasonido;
                model.ObstetriciaAntNoPatologicosNumeroParejas = obsteApnp.NumeroParejas;
                model.ObstetriciaAntNoPatologicosAbortos = obsteAntNoPatologicos.Abortos;
                model.ObstetriciaAntNoPatologicosCesareas = obsteAntNoPatologicos.Cesareas;
                model.ObstetriciaAntNoPatologicosHijosVivos = obsteAntNoPatologicos.HijosVivos;
                model.ObstetriciaAntNoPatologicosHijosMuertos = obsteAntNoPatologicos.HijosMuertos;
                model.ObstetriciaAntNoPatologicosFechaUltimaRegla = obsteAntNoPatologicos.FechaUltimaRegla != null
                    ? ((DateTime)obsteAntNoPatologicos.FechaUltimaRegla).ToString("yyyy-MM-dd")
                    : null;
                model.ObstetriciaAntNoPatologicosFechaProbableParto = obsteAntNoPatologicos.FechaProbableParto != null
                    ? ((DateTime)obsteAntNoPatologicos.FechaProbableParto).ToString("yyyy-MM-dd")
                    : null;
            }

            #endregion

            #region GINECOLOGIA - ANT PATOLOGICOS

            if (consulta.ConsultaAntPatologicosGinecologiaId != null && consulta.ConsultaAntPatologicosGinecologia != null)
            {
                var gineAntPatologicos = consulta.ConsultaAntPatologicosGinecologia;
                model.GinecologiaAntPatologicosInfecciones = gineAntPatologicos.Infecciones;
                model.GinecologiaAntPatologicosEts = gineAntPatologicos.Ets;
                model.GinecologiaAntPatologicosPapanicolau = gineAntPatologicos.Papanicolau;
                model.GinecologiaAntPatologicosOtros = gineAntPatologicos.Otros;
            }

            #endregion

            #region GINECOLOGIA - EXAMEN FISICO

            if (consulta.ConsultaExamenFisicoGinecologiaId != null && consulta.ConsultaExamenFisicoGinecologia != null)
            {
                var gineExamenFisico = consulta.ConsultaExamenFisicoGinecologia;
                model.GinecologiaExamenFisicoMamas = gineExamenFisico.Mamas;
                model.GinecologiaExamenFisicoTactoRectal = gineExamenFisico.TactoRectal;
                model.GinecologiaExamenFisicoTactoVaginal = gineExamenFisico.TactoVaginal;
                model.GinecologiaExamenFisicoVulvaVagina = gineExamenFisico.VulvaVagina;
                model.GinecologiaExamenFisicoEspeculoscopia = gineExamenFisico.Especuloscopia;
            }

            #endregion


            //Neurologico
            model.ExamenNeurologico = consulta.ExamenNeurologico;

            #region EXAMEN FISICO

            model.ExamenFisicoId = consulta.ExamenFisicoId;
            model.ExamenFisicoEstadoGeneral = examenFisico.EstadoGeneral;
            model.ExamenFisicoTemperatura = examenFisico.Temperatura;
            model.ExamenFisicoFrecuenciaRespiratoria = examenFisico.FrecuenciaRespiratoria;
            model.ExamenFisicoFrecuenciaCardiaca = examenFisico.FrecuenciaCardiaca;
            model.ExamenFisicoSaturacionOxigeno = examenFisico.SaturacionDeOxigeno;
            model.ExamenFisicoDerecho = examenFisico.PresionArterialBrazoDerecho;
            model.ExamenFisicoIzquierdo = examenFisico.PresionArterialBrazoIzquierdo; /* zzz */
            model.ExamenFisicoPresionArterialMedia = examenFisico.PresionArterialMedia;
            model.ExamenFisicoPeso = examenFisico.Peso;
            model.ExamenFisicoTalla = examenFisico.Talla;
            model.ExamenFisicoIMC = examenFisico.IMC;
            model.ExamenFisicoGlucosa = examenFisico.Glucosa;
            model.ExamenFisicoObservaciones = examenFisico.Observaciones;
            model.ExamenFisicoTensionArterial = examenFisico.TensionArterialMmhg;
            model.ExamenFisicoGlasgow = examenFisico.Glasgow;
            model.ExamenFisicoPresionArterial = examenFisico.PresionArterial;

            #endregion

            #region EXAMEN FISICO - PEDIATRIA

            if (consulta.ExamenFisicoPediatriaId == null)
            {
                consulta.ExamenFisicoPediatria = new ExamenFisicoPediatria();
            }
            model.ExamenFisicoPediatriaEstadoGeneral = consulta.ExamenFisicoPediatria.EstadoGeneral;
            model.ExamenFisicoPediatriaTemperatura = consulta.ExamenFisicoPediatria.Temperatura;
            model.ExamenFisicoPediatriaFrecuenciaRespiratoria = consulta.ExamenFisicoPediatria.FrecuenciaRespiratoria;
            model.ExamenFisicoPediatriaFrecuenciaCardiaca = consulta.ExamenFisicoPediatria.FrecuenciaCardiaca;
            model.ExamenFisicoPediatriaSaturacionOxigeno = consulta.ExamenFisicoPediatria.SaturacionDeOxigeno;
            model.ExamenFisicoPediatriaDerecho = consulta.ExamenFisicoPediatria.PresionArterialBrazoDerecho;
            model.ExamenFisicoPediatriaIzquierdo = consulta.ExamenFisicoPediatria.PresionArterialBrazoIzquierdo;
            model.ExamenFisicoPediatriaPeso = consulta.ExamenFisicoPediatria.Peso;
            model.ExamenFisicoPediatriaTalla = consulta.ExamenFisicoPediatria.Talla;
            model.ExamenFisicoPediatriaIMC = consulta.ExamenFisicoPediatria.IMC;
            model.ExamenFisicoPediatriaGlucosa = consulta.ExamenFisicoPediatria.Glucosa;
            model.ExamenFisicoPediatriaObservaciones = consulta.ExamenFisicoPediatria.Observaciones;
            model.ExamenFisicoPediatriaTensionArterial = consulta.ExamenFisicoPediatria.TensionArterialMmhg;
            model.ExamenFisicoPediatriaGlasgow = consulta.ExamenFisicoPediatria.Glasgow;
            model.ExamenFisicoPediatricoPresionArterial = consulta.ExamenFisicoPediatria.PresionArterial;
            model.ExamenFisicoPediatricoPesoEdad = consulta.ExamenFisicoPediatria.PediatricoPesoEdad;
            model.ExamenFisicoPediatricoPesoTalla = consulta.ExamenFisicoPediatria.PediatricoPesoTalla;
            model.ExamenFisicoPediatricoTallaEdad = consulta.ExamenFisicoPediatria.PediatricoTallaEdad;

            #endregion


            #region SOLO PARA MUJERES

            model.EstaEmbarazada = consulta.EstaEmbarazada;
            model.NumeroSemanasEmbarazo = consulta.NumeroSemanasEmbarazo == null ? "" :
             ((int)consulta.NumeroSemanasEmbarazo).ToString();
            model.TomaPildorasAnticonceptivas = consulta.TomaPildorasAnticonceptivas;
            model.EstaAmamantando = consulta.EstaAmamantando;

            #endregion

            //Bebidas Alcoholicas
            model.BebeBebidasAlcoholicas = consulta.BebeBebidasAlcoholicas;
            model.AlcoholUltimas24Horas = consulta.AlcoholUltimas24Horas;
            model.AlcoholSemanal = consulta.AlcoholSemanal;

            //Dental
            model.FechaUltimaRadiografiaDental = consulta.FechaUltimaRadiografiaDental;

            #region Estetico

            model.Metabolismo = consulta.Estetico_Metabolismo;
            model.Grasa = consulta.Estetico_Grasa;
            model.IMC = consulta.Estetico_IMC;
            model.Agua = consulta.Estetico_Agua;
            model.Obesidad = consulta.Estetico_Obesidad;
            model.ContornoBrazos = consulta.Estetico_ContornoBrazos;
            model.ContornoBusto = consulta.Estetico_ContornoBusto;
            model.ContornoAbdomen = consulta.Estetico_ContornoAbdomen;
            model.ContornoCadera = consulta.Estetico_ContornoCadera;
            model.ContornoPiernas = consulta.Estetico_ContornoPiernas;
            model.Estatura = consulta.Estetico_Estatura;

            #endregion

            #region REVISION POR SISTEMAS

            var revisionSistemas = consulta.ConsultaRevisionSistemas ?? new ConsultaRevisionSistemas();
            model.SistemaCardiopulmonar = consulta.SistemaCardiopulmonar;
            model.SistemaOsteoarticular = consulta.SistemaOsteoarticular;
            model.SistemaHematologico = consulta.SistemaHematologico;

            // Campos existentes
            model.RevisionSistemasAparienciaGeneral = revisionSistemas.AparienciaGeneral;
            model.RevisionSistemasCabeza = revisionSistemas.Cabeza;
            model.RevisionSistemasCuello = revisionSistemas.Cuello;
            model.RevisionSistemasOidosBoca = revisionSistemas.OidosBoca;
            model.RevisionSistemasAbdomen = revisionSistemas.Abdomen;
            model.RevisionSistemasTorax = revisionSistemas.Torax;
            model.RevisionSistemasDorsoYExtremidades = revisionSistemas.DorsoYExtremidades;
            model.RevisionSistemasGenitales = revisionSistemas.Genitales;

            // Nuevos campos
            model.NewRevisionSistemasNeurologico = revisionSistemas.Neurologico;
            model.NewRevisionSistemasCardiovascular = revisionSistemas.Cardiovascular;
            model.NewRevisionSistemasRespiratorio = revisionSistemas.Respiratorio;
            model.NewRevisionSistemasGastrointestinal = revisionSistemas.Gastrointestinal;
            model.NewRevisionSistemasMusculoesqueletico = revisionSistemas.Musculoesqueletico;
            model.NewRevisionSistemasPielFanera = revisionSistemas.PielFanera;
            model.NewRevisionSistemasGenitourinario = revisionSistemas.Genitourinario;

            #endregion

            #region REVISION POR SISTEMAS - PEDIATRICO

            if (consulta.ConsultaRevisionSistemasPediatriaId == null)
            {
                consulta.ConsultaRevisionSistemasPediatria = new ConsultaRevisionSistemasPediatria();
            }

            model.RevisionSistemasPediatriaAparienciaGeneral = consulta.ConsultaRevisionSistemasPediatria.AparienciaGeneral;
            model.RevisionSistemasPediatriaCabeza = consulta.ConsultaRevisionSistemasPediatria.Cabeza;
            model.RevisionSistemasPediatriaCuello = consulta.ConsultaRevisionSistemasPediatria.Cuello;
            model.RevisionSistemasPediatriaOidosBoca = consulta.ConsultaRevisionSistemasPediatria.OidosBoca;
            model.RevisionSistemasPediatriaAbdomen = consulta.ConsultaRevisionSistemasPediatria.Abdomen;
            model.RevisionSistemasPediatriaTorax = consulta.ConsultaRevisionSistemasPediatria.Torax;
            model.RevisionSistemasDorsoYExtremidades = consulta.ConsultaRevisionSistemasPediatria.DorsoYExtremidades;
            model.RevisionSistemasGenitales = consulta.ConsultaRevisionSistemasPediatria.Genitales;


            #endregion

            #region Area terapeutica

            model.TerapeuticoDatosGenerales = consulta.TerapeuticoDatosGenerales;
            model.TerapeuticoActividadesDiarias = consulta.TerapeuticoActividadesDiarias;
            model.TerapeuticoConQuienVive = consulta.TerapeuticoConQuienVive;
            model.TerapeuticoHabitosAlimenticios = consulta.TerapeuticoHabitosAlimenticios;
            model.TerapeuticoEjercicio = consulta.TerapeuticoEjercicio;
            model.TerapeuticoFinesSemana = consulta.TerapeuticoFinesSemana;
            model.TerapeuticoHistoriaMedica = consulta.TerapeuticoHistoriaMedica;
            model.TerapeuticoHistoriaPeso = consulta.TerapeuticoHistoriaPeso;

            #endregion

            #region Información médica

            model.MedicaUsaLentesContacto = paciente.MedicaUsaLentesContacto;
            model.MedicaUsaLentesContactoDescripcion = paciente.MedicaUsaLentesContactoDescripcion;
            model.MedicaArticulacionesArtificiales = paciente.MedicaArticulacionesArtificiales;
            model.MedicaArticulacionesArtificialesFecha = paciente.MedicaArticulacionesArtificialesFecha;
            model.MedicaArticulacionesArtificialesComplicaciones = paciente.MedicaArticulacionesArtificialesComplicaciones;
            model.MedicaTomaAlendronato = paciente.MedicaTomaAlendronato;
            model.MedicaTomaAlendronatoFecha = paciente.MedicaTomaAlendronatoFecha;
            model.MedicaTratamientoDolorHuesos = paciente.MedicaTratamientoDolorHuesos;
            model.MedicaTratamientoDolorHuesosFechaInicio = paciente.MedicaTratamientoDolorHuesosFechaInicio;
            model.MedicaTratamientoDolorHuesosDescripcionCaso = paciente.MedicaTratamientoDolorHuesosDescripcionCaso;
            model.MedicaSustanciasReguladorasDrogas = paciente.MedicaSustanciasReguladorasDrogas;
            model.MedicaSustanciasReguladorasDrogasFecha = paciente.MedicaSustanciasReguladorasDrogasFecha;
            model.MedicaUsaTabaco = paciente.MedicaUsaTabaco;
            model.MedicaBebidasAlcoholicas = paciente.MedicaBebidasAlcoholicas;
            model.MedicaBebidasAlcoholicasDescripcion = paciente.MedicaBebidasAlcoholicasDescripcion;

            #endregion

            #region Información dental

            model.DentalSangradoCepillar = paciente.DentalSangradoCepillar;
            model.DentalDolorFrio = paciente.DentalDolorFrio;
            model.DentalDolorPresionar = paciente.DentalDolorPresionar;
            model.DentalObjetosAtorados = paciente.DentalObjetosAtorados;
            model.DentalBocaSeca = paciente.DentalBocaSeca;
            model.DentalTratamientoPeriondal = paciente.DentalTratamientoPeriondal;
            model.DentalTratamientoOrtodoncia = paciente.DentalTratamientoOrtodoncia;
            model.DentalProblemasTratamientoDental = paciente.DentalProblemasTratamientoDental;
            model.DentalProblemasTratamientoDentalDescripcion = paciente.DentalProblemasTratamientoDentalDescripcion;
            model.DentalFluoradaAguaDomicilio = paciente.DentalFluoradaAguaDomicilio;
            model.DentalBebeAguaFiltrada = paciente.DentalBebeAguaFiltrada;
            model.DentalDolorOidos = paciente.DentalDolorOidos;
            model.DentalMolestiaRuidoAlto = paciente.DentalMolestiaRuidoAlto;
            model.DentalMolestiaRuidoAltoDescripcion = paciente.DentalMolestiaRuidoAltoDescripcion;
            model.DentalBrumismo = paciente.DentalBrumismo;
            model.DentalLesiones = paciente.DentalLesiones;
            model.DentalLesionesDescripcion = paciente.DentalLesionesDescripcion;
            model.DentalDentaduraPlacas = paciente.DentalDentaduraPlacas;
            model.DentalDentaduraPlacasDescripcion = paciente.DentalDentaduraPlacasDescripcion;
            model.DentalActividadesRecreacion = paciente.DentalActividadesRecreacion;
            model.DentalActividadesRecreacionDescripcion = paciente.DentalActividadesRecreacionDescripcion;
            model.DentalLesionesCabeza = paciente.DentalLesionesCabeza;
            model.DentalLesionesCabezaDescripcion = paciente.DentalLesionesCabezaDescripcion;

            #endregion

            #region     EVALUACION OBSTETRICA - OBSTETRICIA

            model.EvaluacionObstetricaUteroGravio = consulta.EvaluacionObstetricaUteroGravio;
            model.EvaluacionObstetricaAbdomenObstetrico = consulta.EvaluacionObstetricaAbdomenObstetrico;
            model.EvaluacionObstetricaFcf = consulta.EvaluacionObstetricaFcf;
            model.EvaluacionObstetricaAu = consulta.EvaluacionObstetricaAu;
            model.EvaluacionObstetricaBishop = consulta.EvaluacionObstetricaBishop;
            model.EvaluacionObstetricaPresentacionLeopold = consulta.EvaluacionObstetricaPresentacionLeopold;
            model.EvaluacionObstetricaOtras = consulta.EvaluacionObstetricaOtras;
            model.EvaluacionObstetricaActividadUterina = consulta.EvaluacionObstetricaActividadUterina;
            model.EvaluacionObstetricaMovimientoFetalPercetible = consulta.EvaluacionObstetricaMovimientoFetalPercetible;
            model.EvaluacionObstetricaMovimientoFetalEspecifique = consulta.EvaluacionObstetricaMovimientoFetalEspecifique;
            model.EvaluacionObstetricaTactoVaginal = consulta.EvaluacionObstetricaTactoVaginal;
            model.EvaluacionObstetricaD = consulta.EvaluacionObstetricaD;
            model.EvaluacionObstetricaCms = consulta.EvaluacionObstetricaCms;
            model.EvaluacionObstetricaBPorciento = consulta.EvaluacionObstetricaBPorciento;
            model.EvaluacionObstetricaAltitud = consulta.EvaluacionObstetricaAltitud;
            model.EvaluacionObstetricaPosicionCervix = consulta.EvaluacionObstetricaPosicionCervix;
            model.EvaluacionObstetricaMembranasOvulares = consulta.EvaluacionObstetricaMembranasOvulares;
            model.EvaluacionObstetricaLiquidoAmniotico = consulta.EvaluacionObstetricaLiquidoAmniotico;
            model.EvaluacionObstetricaLiquidoAmnioticoEspecifique = consulta.EvaluacionObstetricaLiquidoAmnioticoEspecifique;
            model.EvaluacionObstetricaPelvis = consulta.EvaluacionObstetricaPelvis;
            model.EvaluacionObstetricaConsistencia = consulta.EvaluacionObstetricaConsistencia;

            #endregion

            #region ECOGRAFIA OBSTETRICA

            model.EcografiaObstetricaFeto = consulta.EcografiaObstetricaFeto;
            model.EcografiaObstetricaEstado = consulta.EcografiaObstetricaEstado;
            model.EcografiaObstetricaSituacion = consulta.EcografiaObstetricaSituacion;
            model.EcografiaObstetricaPresentacion = consulta.EcografiaObstetricaPresentacion;
            model.EcografiaObstetricaPosicion = consulta.EcografiaObstetricaPosicion;
            model.EcografiaObstetricaDorso = consulta.EcografiaObstetricaDorso;

            #endregion

            #region OBSTETRICIA - BIOMETRIA
            model.NumeroBebes = consulta.NumeroBebes;
            model.ObsteBiometriaRlc = consulta.ObsteBiometriaRlc;
            model.ObsteBiometriaSg = consulta.ObsteBiometriaSg;
            model.ObsteBiometriaW = consulta.ObsteBiometriaW;
            model.ObsteBiometriaDbp = consulta.ObsteBiometriaDbp;
            model.ObsteBiometriaHc = consulta.ObsteBiometriaHc;
            model.ObsteBiometriaAc = consulta.ObsteBiometriaAc;
            model.ObsteBiometriaLf = consulta.ObsteBiometriaLf;
            model.ObsteBiometriaEg = consulta.ObsteBiometriaEg;
            model.ObsteBiometriaFcf = consulta.ObsteBiometriaFcf;
            model.ObsteBiometriaPlacenta = consulta.ObsteBiometriaPlacenta;
            model.ObsteBiometriaGrado = consulta.ObsteBiometriaGrado;
            model.ObsteBiometriaIla = consulta.ObsteBiometriaIla;
            model.ObsteBiometriaMalformaciones = consulta.ObsteBiometriaMalformaciones;
            model.ObsteBiometriaPeso = consulta.ObsteBiometriaPeso;
            model.ObsteBiometriaSexo = consulta.ObsteBiometriaSexo;
            model.ObsteBiometriaFechaParto = consulta.ObsteBiometriaFechaParto;
            model.ObsteBiometriaComentario = consulta.ObsteBiometriaComentario;
            model.ObsteBiometriaPresentacion = consulta.ObsteBiometriaPresentacion;

            // Cargar los datos de las biometrías adicionales
            model.ObsteBiometriaRlc2 = consulta.ObsteBiometriaRlc2;
            model.ObsteBiometriaSg2 = consulta.ObsteBiometriaSg2;
            model.ObsteBiometriaDbp2 = consulta.ObsteBiometriaDbp2;
            model.ObsteBiometriaHc2 = consulta.ObsteBiometriaHc2;
            model.ObsteBiometriaAc2 = consulta.ObsteBiometriaAc2;
            model.ObsteBiometriaLf2 = consulta.ObsteBiometriaLf2;
            model.ObsteBiometriaEg2 = consulta.ObsteBiometriaEg2;
            model.ObsteBiometriaFcf2 = consulta.ObsteBiometriaFcf2;
            model.ObsteBiometriaPlacenta2 = consulta.ObsteBiometriaPlacenta2;
            model.ObsteBiometriaGrado2 = consulta.ObsteBiometriaGrado2;
            model.ObsteBiometriaIla2 = consulta.ObsteBiometriaIla2;
            model.ObsteBiometriaMalformaciones2 = consulta.ObsteBiometriaMalformaciones2;
            model.ObsteBiometriaPeso2 = consulta.ObsteBiometriaPeso2;
            model.ObsteBiometriaSexo2 = consulta.ObsteBiometriaSexo2;
            model.ObsteBiometriaFechaParto2 = consulta.ObsteBiometriaFechaParto2;
            model.ObsteBiometriaComentario2 = consulta.ObsteBiometriaComentario2;
            model.ObsteBiometriaPresentacion2 = consulta.ObsteBiometriaPresentacion2;

            model.ObsteBiometriaRlc3 = consulta.ObsteBiometriaRlc3;
            model.ObsteBiometriaSg3 = consulta.ObsteBiometriaSg3;
            model.ObsteBiometriaDbp3 = consulta.ObsteBiometriaDbp3;
            model.ObsteBiometriaHc3 = consulta.ObsteBiometriaHc3;
            model.ObsteBiometriaAc3 = consulta.ObsteBiometriaAc3;
            model.ObsteBiometriaLf3 = consulta.ObsteBiometriaLf3;
            model.ObsteBiometriaEg3 = consulta.ObsteBiometriaEg3;
            model.ObsteBiometriaFcf3 = consulta.ObsteBiometriaFcf3;
            model.ObsteBiometriaPlacenta3 = consulta.ObsteBiometriaPlacenta3;
            model.ObsteBiometriaGrado3 = consulta.ObsteBiometriaGrado3;
            model.ObsteBiometriaIla3 = consulta.ObsteBiometriaIla3;
            model.ObsteBiometriaMalformaciones3 = consulta.ObsteBiometriaMalformaciones3;
            model.ObsteBiometriaPeso3 = consulta.ObsteBiometriaPeso3;
            model.ObsteBiometriaSexo3 = consulta.ObsteBiometriaSexo3;
            model.ObsteBiometriaFechaParto3 = consulta.ObsteBiometriaFechaParto3;
            model.ObsteBiometriaComentario3 = consulta.ObsteBiometriaComentario3;
            model.ObsteBiometriaPresentacion3 = consulta.ObsteBiometriaPresentacion3;

            model.ObsteBiometriaRlc4 = consulta.ObsteBiometriaRlc4;
            model.ObsteBiometriaSg4 = consulta.ObsteBiometriaSg4;
            model.ObsteBiometriaDbp4 = consulta.ObsteBiometriaDbp4;
            model.ObsteBiometriaHc4 = consulta.ObsteBiometriaHc4;
            model.ObsteBiometriaAc4 = consulta.ObsteBiometriaAc4;
            model.ObsteBiometriaLf4 = consulta.ObsteBiometriaLf4;
            model.ObsteBiometriaEg4 = consulta.ObsteBiometriaEg4;
            model.ObsteBiometriaFcf4 = consulta.ObsteBiometriaFcf4;
            model.ObsteBiometriaPlacenta4 = consulta.ObsteBiometriaPlacenta4;
            model.ObsteBiometriaGrado4 = consulta.ObsteBiometriaGrado4;
            model.ObsteBiometriaIla4 = consulta.ObsteBiometriaIla4;
            model.ObsteBiometriaMalformaciones4 = consulta.ObsteBiometriaMalformaciones4;
            model.ObsteBiometriaPeso4 = consulta.ObsteBiometriaPeso4;
            model.ObsteBiometriaSexo4 = consulta.ObsteBiometriaSexo4;
            model.ObsteBiometriaFechaParto4 = consulta.ObsteBiometriaFechaParto4;
            model.ObsteBiometriaComentario4 = consulta.ObsteBiometriaComentario4;
            model.ObsteBiometriaPresentacion4 = consulta.ObsteBiometriaPresentacion4;

            #endregion

            #region PRESCRIPCIÓN - MEDICAMENTOS OTROS

            if (consulta.MedicamentosOtros != null && consulta.MedicamentosOtros.Any())
            {
                model.MedicamentosOtros = consulta.MedicamentosOtros.Select(m => new MedicamentoOtro
                {
                    Id = m.Id,
                    Nombre = m.Nombre,
                    Indicaciones = m.Indicaciones,
                    Cantidad = m.Cantidad,
                    FechaPrescripcion = m.FechaPrescripcion
                }).ToList();
            }
            else
            {
                model.MedicamentosOtros = new List<MedicamentoOtro>();
            }


            #endregion

            #region ECOGRAFIA ENDOCAVITARIA

            model.EcografiaEndocavitariaUtero = consulta.EcografiaEndocavitariaUtero;
            model.EcografiaEndocavitariaLongitudinal = consulta.EcografiaEndocavitariaLongitudinal;
            model.EcografiaEndocavitariaTransverso = consulta.EcografiaEndocavitariaTransverso;
            model.EcografiaEndocavitariaEndometrio = consulta.EcografiaEndocavitariaEndometrio;
            model.EcografiaEndocavitariaOvarioDerecho = consulta.EcografiaEndocavitariaOvarioDerecho;
            model.EcografiaEndocavitariaOvarioIzquierdo = consulta.EcografiaEndocavitariaOvarioIzquierdo;
            model.EcografiaEndocavitariaFondoSaco = consulta.EcografiaEndocavitariaFondoSaco;
            model.EcografiaEndocavitariaImpresionClinica = consulta.EcografiaEndocavitariaImpresionClinica;
            model.EcografiaEndocavitariaComentario = consulta.EcografiaEndocavitariaComentario;

            #endregion

            model.notaOperatoria = consulta.notaOperatoria;
            model.NivelPrioridadCita = consulta.Citas.NivelPrioridadCita;
            model.NumeroTurno = consulta.Citas.NumeroTurno;
            model.EstadoTurno = consulta.Citas.EstadoTurno;


            ViewBag.ConsultaId = id;
            ViewBag.PacienteId = model.PacienteId;

            #region OFTALMOLOGÍA
            // ==== OFTALMOLOGÍA (llenado en modo lectura/edición) ====
            // No debe bloquear la vista si la tabla no existe o hay timeout
            try
            {
                var oft = _oftRepo.GetConsulta((int)id);

                if (oft != null)
                {
                    model.Oft_Id = (int)oft.Id;
                    model.Oft_ConsultaId = oft.ConsultaId;

                    // Motivo / Antecedentes
                    model.Oft_HistoriaEnfermedadActual = oft.HistoriaEnfermedadActual;
                    model.Oft_PacienteMedicos = oft.PacienteMedicos;
                    model.Oft_PacienteQuirurgicos = oft.PacienteQuirurgicos;
                    model.Oft_PacienteTraumaticos = oft.PacienteTraumaticos;
                    model.Oft_PacienteAlergias = oft.PacienteAlergias;
                    model.Oft_PacienteFamiliares = oft.PacienteFamiliares;

                    // Datos objetivos - AV sin corrección
                    model.Oft_AgudezaSC_Test = oft.AgudezaSC_Test;
                    model.Oft_AgudezaSC_OD = oft.AgudezaSC_OD;
                    model.Oft_AgudezaSC_OS = oft.AgudezaSC_OS;

                    // Sensibilidad de contraste
                    model.Oft_Contraste_OD = oft.Contraste_OD;
                    model.Oft_Contraste_OS = oft.Contraste_OS;

                    // AV cerca sin corrección
                    model.Oft_AVCerca_OD = oft.AVCerca_OD;
                    model.Oft_AVCerca_OS = oft.AVCerca_OS;

                    // Tests especiales
                    model.Oft_TestIshihara_OD = oft.TestIshihara_OD;
                    model.Oft_TestIshihara_OS = oft.TestIshihara_OS;
                    model.Oft_TestEstereopsis_OD = oft.TestEstereopsis_OD;
                    model.Oft_TestEstereopsis_OS = oft.TestEstereopsis_OS;

                    // Óptica - Lensometría (Histórico)
                    model.Oft_Lensometria_OD_Esfera = oft.Lensometria_OD_Esfera;
                    model.Oft_Lensometria_OD_Cilindro = oft.Lensometria_OD_Cilindro;
                    model.Oft_Lensometria_OD_Eje = oft.Lensometria_OD_Eje;
                    model.Oft_Lensometria_OD_Agudeza = oft.Lensometria_OD_Agudeza;
                    model.Oft_Lensometria_OS_Esfera = oft.Lensometria_OS_Esfera;
                    model.Oft_Lensometria_OS_Cilindro = oft.Lensometria_OS_Cilindro;
                    model.Oft_Lensometria_OS_Eje = oft.Lensometria_OS_Eje;
                    model.Oft_Lensometria_OS_Agudeza = oft.Lensometria_OS_Agudeza;

                    // Óptica - Final
                    model.Oft_Final_OD_Esfera = oft.Final_OD_Esfera;
                    model.Oft_Final_OD_Cilindro = oft.Final_OD_Cilindro;
                    model.Oft_Final_OD_Eje = oft.Final_OD_Eje;
                    model.Oft_Final_OD_Agudeza = oft.Final_OD_Agudeza;
                    model.Oft_Final_OS_Esfera = oft.Final_OS_Esfera;
                    model.Oft_Final_OS_Cilindro = oft.Final_OS_Cilindro;
                    model.Oft_Final_OS_Eje = oft.Final_OS_Eje;
                    model.Oft_Final_OS_Agudeza = oft.Final_OS_Agudeza;
                    model.Oft_Final_Adicion = oft.Final_Adicion;
                    model.Oft_Final_DIP_mm = oft.Final_DIP_mm;

                    // Óptica - Retinoscopía
                    model.Oft_Retino_OD_Esfera = oft.Retino_OD_Esfera;
                    model.Oft_Retino_OD_Cilindro = oft.Retino_OD_Cilindro;
                    model.Oft_Retino_OD_Eje = oft.Retino_OD_Eje;
                    model.Oft_Retino_OS_Esfera = oft.Retino_OS_Esfera;
                    model.Oft_Retino_OS_Cilindro = oft.Retino_OS_Cilindro;
                    model.Oft_Retino_OS_Eje = oft.Retino_OS_Eje;

                    // Tipo de lente / Material
                    model.Oft_TipoLente = oft.TipoLente;
                    model.Oft_LenteMaterialTratamiento = oft.LenteMaterialTratamiento;

                    // Inspección / LH / Oftalmoscopía
                    model.Oft_Inspeccion_MovExtraoculares_OD = oft.Inspeccion_MovExtraoculares_OD;
                    model.Oft_Inspeccion_MovExtraoculares_OS = oft.Inspeccion_MovExtraoculares_OS;
                    model.Oft_Inspeccion_Cejas_OD = oft.Inspeccion_Cejas_OD;
                    model.Oft_Inspeccion_Cejas_OS = oft.Inspeccion_Cejas_OS;
                    model.Oft_Inspeccion_ParpadosPestanas_OD = oft.Inspeccion_ParpadosPestanas_OD;
                    model.Oft_Inspeccion_ParpadosPestanas_OS = oft.Inspeccion_ParpadosPestanas_OS;
                    model.Oft_Inspeccion_ViaLagrimal_OD = oft.Inspeccion_ViaLagrimal_OD;
                    model.Oft_Inspeccion_ViaLagrimal_OS = oft.Inspeccion_ViaLagrimal_OS;

                    // Segmento anterior
                    model.Oft_Inspeccion_Conjuntiva_OD = oft.Inspeccion_Conjuntiva_OD;
                    model.Oft_Inspeccion_Conjuntiva_OS = oft.Inspeccion_Conjuntiva_OS;
                    model.Oft_Inspeccion_CorneaEsclera_OD = oft.Inspeccion_CorneaEsclera_OD;
                    model.Oft_Inspeccion_CorneaEsclera_OS = oft.Inspeccion_CorneaEsclera_OS;
                    model.Oft_Inspeccion_CamaraAnteriorAngulo_OD = oft.Inspeccion_CamaraAnteriorAngulo_OD;
                    model.Oft_Inspeccion_CamaraAnteriorAngulo_OS = oft.Inspeccion_CamaraAnteriorAngulo_OS;
                    model.Oft_Inspeccion_IrisPupila_OD = oft.Inspeccion_IrisPupila_OD;
                    model.Oft_Inspeccion_IrisPupila_OS = oft.Inspeccion_IrisPupila_OS;
                    model.Oft_Inspeccion_Cristalino_OD = oft.Inspeccion_Cristalino_OD;
                    model.Oft_Inspeccion_Cristalino_OS = oft.Inspeccion_Cristalino_OS;
                    model.Oft_Inspeccion_BUT_OD = oft.Inspeccion_BUT_OD;
                    model.Oft_Inspeccion_BUT_OS = oft.Inspeccion_BUT_OS;
                    model.Oft_Inspeccion_PresionIntraocular_OD = oft.Inspeccion_PresionIntraocular_OD;
                    model.Oft_Inspeccion_PresionIntraocular_OS = oft.Inspeccion_PresionIntraocular_OS;

                    // Segmento posterior
                    model.Oft_Inspeccion_Vitreo_OD = oft.Inspeccion_Vitreo_OD;
                    model.Oft_Inspeccion_Vitreo_OS = oft.Inspeccion_Vitreo_OS;
                    model.Oft_Inspeccion_NervioOptico_OD = oft.Inspeccion_NervioOptico_OD;
                    model.Oft_Inspeccion_NervioOptico_OS = oft.Inspeccion_NervioOptico_OS;
                    model.Oft_Inspeccion_Macula_OD = oft.Inspeccion_Macula_OD;
                    model.Oft_Inspeccion_Macula_OS = oft.Inspeccion_Macula_OS;
                    model.Oft_Inspeccion_Retina_OD = oft.Inspeccion_Retina_OD;
                    model.Oft_Inspeccion_Retina_OS = oft.Inspeccion_Retina_OS;

                    model.Oft_HistoriaClinicaImpresionClinica = oft.HistoriaClinicaImpresionClinica;
                    model.Oft_HistoriaClinicaComentario = oft.HistoriaClinicaComentario;
                }

            }
            catch (Exception ex)
            {

            }
            #endregion

            #region PODOLOGÍA
            // ==== PODOLOGÍA (llenado en modo lectura/edición) ====
            // No debe bloquear la vista si la tabla no existe o hay timeout
            try
            {
                var pod = _podRepo.GetConsulta((int)id);

                if (pod != null)
                {
                    model.Pod_Id = (int)pod.Id;
                    model.Pod_ConsultaId = pod.ConsultaId;
                    model.Pod_PacienteId = pod.PacienteId;

                    // 1) Antecedentes Médicos
                    model.Pod_Enfermedades = pod.Enfermedades ?? Array.Empty<string>();
                    model.Pod_Enfermedades_Otros = pod.Enfermedades_Otros;
                    model.Pod_Medicamentos = pod.Medicamentos;
                    model.Pod_PresionArterial = pod.PresionArterial;

                    // 2) Examen del Pie
                    model.Pod_Pulso_Pedio = pod.Pulso_Pedio;
                    model.Pod_Pulso_TibialPosterior = pod.Pulso_TibialPosterior;
                    model.Pod_Pulso_Popliteo = pod.Pulso_Popliteo;
                    model.Pod_TemperaturaPie = pod.TemperaturaPie;
                    model.Pod_ProblemasCirculatorios = pod.ProblemasCirculatorios;
                    model.Pod_EstadoPiel = pod.EstadoPiel;
                    model.Pod_ObservacionesExamen = pod.ObservacionesExamen;

                    // 3) Tratamiento Realizado
                    model.Pod_Procedimientos = pod.Procedimientos ?? Array.Empty<string>();
                    model.Pod_OtrosProcedimientos = pod.OtrosProcedimientos;
                    model.Pod_ObservacionesTratamiento = pod.ObservacionesTratamiento;

                    // 4) Indicaciones y Datos Finales
                    model.Pod_Indicaciones = pod.Indicaciones;
                    model.Pod_PesoKg = pod.PesoKg;
                    model.Pod_EstaturaM = pod.EstaturaM;
                    model.Pod_FechaAtencion = pod.FechaAtencion;
                    model.Pod_Profesional = pod.Profesional;
                }

            }
            catch (Exception ex)
            {
            }
            #endregion

            #region ENFERMERÍA REPOSITORIOS
            try
            {
                var consultaId = (int)id;

                var hce = _enfRepo.GetConsulta(consultaId);

                if (hce != null)
                {
                    // PK/FKs
                    model.Hce_Id = hce.Id;
                    model.Hce_ConsultaId = hce.ConsultaId;
                    model.Hce_PacienteId = hce.PacienteId;

                    // 1) Tipo de consulta
                    model.Hce_TipoConsulta = hce.TipoConsulta;

                    // 2) Motivo de consulta
                    model.Hce_MotivoConsulta = hce.MotivoConsulta;

                    // 3) Antecedentes (estructura nueva)
                    model.Hce_AntecedentesPatologicos = hce.AntecedentesPatologicos;
                    model.Hce_AntecedentesQuirurgicos = hce.AntecedentesQuirurgicos;
                    model.Hce_AntecedentesTraumaticos = hce.AntecedentesTraumaticos;
                    model.Hce_Hospitalizaciones = hce.Hospitalizaciones;
                    model.Hce_Alergias = hce.Alergias;
                    model.Hce_AntecedentesFamiliares = hce.AntecedentesFamiliares;

                    // 4) Hábitos
                    model.Hce_HabitoAlimentacion = hce.HabitoAlimentacion;
                    model.Hce_ActividadFisica = hce.ActividadFisica;
                    model.Hce_HabitoAlcoholTexto = hce.HabitoAlcoholTexto;
                    model.Hce_HabitoTabacoTexto = hce.HabitoTabacoTexto;
                    model.Hce_OtrosHabitos = hce.OtrosHabitos;

                    // 5) Signos vitales y antropometría
                    model.Hce_PresionArterialTxt = hce.PresionArterialTxt;
                    model.Hce_FC = hce.FC;
                    model.Hce_FR = hce.FR;
                    model.Hce_TemperaturaC = hce.TemperaturaC;
                    model.Hce_SPO2 = hce.SPO2;
                    model.Hce_PesoKg = hce.PesoKg;
                    model.Hce_TallaM = hce.TallaM;
                    model.Hce_IMC = hce.IMC;

                    // 6) Exploración por aparatos y sistemas
                    model.Hce_CabezaCuello = hce.CabezaCuello;
                    model.Hce_ToraxPulmones = hce.ToraxPulmones;
                    model.Hce_Corazon = hce.Corazon;
                    model.Hce_Abdomen = hce.Abdomen;
                    model.Hce_Extremidades = hce.Extremidades;
                    model.Hce_SistemaNeurologico = hce.SistemaNeurologico;
                    model.Hce_PielAnexos = hce.PielAnexos;

                    // 8) Valoración de enfermería
                    model.Hce_ValConcienciaOrientacion = hce.ValConcienciaOrientacion;
                    model.Hce_ValEstadoNutricional = hce.ValEstadoNutricional;
                    model.Hce_ValEliminacion = hce.ValEliminacion;
                    model.Hce_ValSuenoDescanso = hce.ValSuenoDescanso;
                    model.Hce_ValActividadMovilidad = hce.ValActividadMovilidad;
                    model.Hce_ValAutonomia = hce.ValAutonomia;

                    // 9) Laboratorios
                    model.Hce_Laboratorios = hce.Laboratorios;

                    // 10) Diagnóstico de enfermería
                    model.Hce_DiagnosticoEnfermeria = hce.DiagnosticoEnfermeria;

                    // 11) Plan de cuidados / Intervenciones
                    model.Hce_AccionesRealizadas = hce.AccionesRealizadas;
                    model.Hce_MedicamentosAdministrados = hce.MedicamentosAdministrados;
                    model.Hce_Tratamiento = hce.Tratamiento;

                    // 12) Seguimiento / Evolución / Cita
                    model.Hce_Seguimiento = hce.Seguimiento;
                }

            }
            catch (Exception ex)
            {
            }
            #endregion

            #region VALORACION INICIAL ENFERMERIA
            // ==== VE (llenado en modo lectura/edición) ====
            // No debe bloquear la vista si la tabla no existe o hay timeout
            try
            {
                var ve = _veRepo.GetConsulta((int)id);

                if (ve != null)
                {
                    // IDs para upsert (si abres luego en edición)
                    model.Ve_Id = (int)ve.Id;
                    model.Ve_ConsultaId = ve.ConsultaId;
                    model.Ve_PacienteId = ve.PacienteId;

                    // 1) Datos de Valoración Inicial
                    model.Ve_Motivo = ve.Motivo;
                    model.Ve_DiagnosticoMedico = ve.DiagnosticoMedico;
                    model.Ve_Labs = ve.Labs;

                    // 2) ¿Cómo se enteró del servicio?
                    model.Ve_Medio = ve.Medio ?? Array.Empty<string>();

                    // 3) Oxigenación y Circulación
                    model.Ve_Resp = ve.Resp ?? Array.Empty<string>();
                    model.Ve_Circ = ve.Circ ?? Array.Empty<string>();

                    // 4) Necesidad de Nutrición
                    model.Ve_Nutricion = ve.Nutricion ?? Array.Empty<string>();
                    model.Ve_NutricionObs = ve.NutricionObs;

                    // 5) Necesidad de Eliminación
                    model.Ve_Urinario = ve.Urinario ?? Array.Empty<string>();
                    model.Ve_Intestinal = ve.Intestinal ?? Array.Empty<string>();

                    // 6) Movilización y Estado de Conciencia
                    model.Ve_Mov = ve.Mov ?? Array.Empty<string>();
                    model.Ve_Conciencia = ve.Conciencia ?? Array.Empty<string>();

                    // 7) Autocuidado y Reposo
                    model.Ve_Sueno = ve.Sueno ?? Array.Empty<string>();
                    model.Ve_Vestirse = ve.Vestirse;
                    model.Ve_Higiene = ve.Higiene;
                    model.Ve_Piel = ve.Piel ?? Array.Empty<string>();
                    model.Ve_PielUbicacion = ve.PielUbicacion;

                    // 8) Necesidad de Comunicación
                    model.Ve_Lenguaje = ve.Lenguaje ?? Array.Empty<string>();
                    model.Ve_Vision = ve.Vision ?? Array.Empty<string>();
                    model.Ve_Oido = ve.Oido ?? Array.Empty<string>();

                    // 9) Seguridad y Factores Psicosociales
                    model.Ve_Seg = ve.Seg ?? Array.Empty<string>();
                    model.Ve_Religiosos = ve.Religiosos;
                    model.Ve_CreenciasObservaciones = ve.CreenciasObservaciones;
                    model.Ve_ConoceMotivo = ve.ConoceMotivo;
                    model.Ve_NecesitaInfo = ve.NecesitaInfo;

                    // 10) Medicación y Plan Terapéutico
                    model.Ve_MedicacionActual = ve.MedicacionActual;
                    model.Ve_PlanTerapeutico = ve.PlanTerapeutico;
                }
            }
            catch (Exception ex)
            {
            }
            #endregion

            #region SUEROTERAPIA
            // ==== SUEROTERAPIA (llenado en modo lectura/edición) ====
            // No debe bloquear la vista si la tabla no existe o hay timeout
            try
            {
                var suero = _sueroRepo.GetConsulta((int)id);

                if (suero != null)
                {
                    // IDs para upsert (si abres luego en edición)
                    model.Suero_Id = suero.Id;
                    model.Suero_ConsultaId = suero.ConsultaId;
                    model.Suero_PacienteId = suero.PacienteId;

                    // 1) Datos de Valoración Inicial
                    model.Suero_Motivo = suero.Motivo;
                    model.Suero_DiagnosticoMedico = suero.DiagnosticoMedico;
                    model.Suero_Labs = suero.Labs;

                    // 2) ¿Cómo se enteró del servicio?
                    model.Suero_Medio = suero.Medio ?? Array.Empty<string>();

                    // 3) Oxigenación y Circulación
                    model.Suero_Resp = suero.Resp ?? Array.Empty<string>();
                    model.Suero_Circ = suero.Circ ?? Array.Empty<string>();

                    // 4) Necesidad de Nutrición
                    model.Suero_Nutricion = suero.Nutricion ?? Array.Empty<string>();
                    model.Suero_NutricionObs = suero.NutricionObs;

                    // 5) Plan Terapéutico
                    model.Suero_PlanTerapeutico = suero.PlanTerapeutico;
                }

            }
            catch (Exception ex)
            {

            }
            #endregion

            #region HISTORICO OFTALMOLOGIA
            try
            {
                var pacienteId = paciente?.Id ?? 0;
                if (pacienteId > 0)
                {
                    // Última consulta (resumen)
                    var oftUltima = _oftRepo.GetConsultaByPaciente(pacienteId);
                    if (oftUltima != null)
                    {
                        model.FechaUltimaConsulta = oftUltima.Fecha;
                        model.MotivoUltimaConsulta = string.IsNullOrWhiteSpace(oftUltima.HistoriaClinicaImpresionClinica)
                            ? "-"
                            : oftUltima.HistoriaClinicaImpresionClinica;
                    }

                    // Histórico completo
                    var oftHist = _oftRepo.GetConsultasByPaciente(pacienteId) ?? Enumerable.Empty<ConsultasOftalmologia>();

                    // (si te sirve la tabla-resumen, conserva este bloque)
                    var top = 10;
                    var filas = oftHist
                        .Take(top)
                        .Select(x => new
                        {
                            x.ConsultaId,
                            x.Fecha,
                            x.AgudezaSC_OD,
                            x.AgudezaSC_OS,
                            x.AVCerca_OD,
                            x.AVCerca_OS,
                            x.Final_OD_Esfera,
                            x.Final_OD_Cilindro,
                            x.Final_OD_Eje,
                            x.Final_OS_Esfera,
                            x.Final_OS_Cilindro,
                            x.Final_OS_Eje,
                            x.Inspeccion_PresionIntraocular_OD,
                            x.Inspeccion_PresionIntraocular_OS,
                            x.HistoriaClinicaImpresionClinica
                        })
                        .ToList();

                    ViewBag.OftHistorial = filas;
                    ViewBag.OftHistorialTotal = oftHist.Count();

                    ViewBag.OftHistorialFull = oftHist.ToList();
                }
                else
                {
                    ViewBag.OftHistorial = new List<object>();
                    ViewBag.OftHistorialTotal = 0;
                    ViewBag.OftHistorialFull = new List<ConsultasOftalmologia>();
                }
            }
            catch
            {
                ViewBag.OftHistorial = new List<object>();
                ViewBag.OftHistorialTotal = 0;
                ViewBag.OftHistorialFull = new List<ConsultasOftalmologia>();
            }
            #endregion

            #region HISTORICO PODOLOGIA
            try
            {
                var pacienteId = paciente?.Id ?? 0;
                if (pacienteId > 0)
                {
                    // Última consulta (resumen rápido)
                    var podUltima = _podRepo.GetConsultaByPaciente(pacienteId);
                    if (podUltima != null)
                    {
                        // Reusa las mismas propiedades de VM que ya llenabas con oftalmología,
                        // así no rompes nada del layout superior.
                        model.FechaUltimaConsulta = podUltima.Fecha;
                        // Toma un texto significativo: primero Observaciones del examen o, si están vacías, las Indicaciones
                        var resumen = string.IsNullOrWhiteSpace(podUltima.ObservacionesExamen)
                            ? podUltima.Indicaciones
                            : podUltima.ObservacionesExamen;
                        model.MotivoUltimaConsulta = string.IsNullOrWhiteSpace(resumen) ? "-" : resumen;
                    }

                    // Histórico completo
                    var podHist = _podRepo.GetConsultasByPaciente(pacienteId)
                                 ?? Enumerable.Empty<ConsultasPodologia>();

                    // Tabla-resumen (opcional): top N filas con datos clave
                    var top = 10;
                    var filas = podHist
                        .Take(top)
                        .Select(x => new
                        {
                            x.ConsultaId,
                            x.Fecha,
                            x.PresionArterial,
                            x.Pulso_Pedio,
                            x.Pulso_TibialPosterior,
                            x.Pulso_Popliteo,
                            x.TemperaturaPie,
                            x.EstadoPiel,
                            x.ProblemasCirculatorios,
                            x.PesoKg,
                            x.EstaturaM,
                            x.Profesional,
                            Resumen = string.IsNullOrWhiteSpace(x.ObservacionesExamen) ? x.Indicaciones : x.ObservacionesExamen
                        })
                        .ToList();

                    ViewBag.PodHistorial = filas;
                    ViewBag.PodHistorialTotal = podHist.Count();

                    // 👉 CLAVE: lista completa para que el formulario/partial pueda pintar TODOS los campos
                    ViewBag.PodHistorialFull = podHist.ToList();
                }
                else
                {
                    ViewBag.PodHistorial = new List<object>();
                    ViewBag.PodHistorialTotal = 0;
                    ViewBag.PodHistorialFull = new List<ConsultasPodologia>();
                }
            }
            catch
            {
                ViewBag.PodHistorial = new List<object>();
                ViewBag.PodHistorialTotal = 0;
                ViewBag.PodHistorialFull = new List<ConsultasPodologia>();
            }
            #endregion

            #region HISTORICO HISTORIA CLÍNICA DE ENFERMERÍA
            try
            {
                var pacienteId = paciente?.Id ?? 0;
                if (pacienteId > 0)
                {
                    // Última consulta (resumen)
                    var hceUltima = _enfRepo.GetConsultaByPaciente(pacienteId);
                    if (hceUltima != null)
                    {
                        model.FechaUltimaConsulta = hceUltima.Fecha;
                        // Si hay diagnóstico de enfermería úsalo; si no, cae al motivo de consulta; si tampoco, usa "-"
                        model.MotivoUltimaConsulta =
                            !string.IsNullOrWhiteSpace(hceUltima.DiagnosticoEnfermeria) ? hceUltima.DiagnosticoEnfermeria :
                            !string.IsNullOrWhiteSpace(hceUltima.MotivoConsulta) ? hceUltima.MotivoConsulta :
                            "-";
                    }

                    // Histórico completo
                    var hceHist = _enfRepo.GetConsultasByPaciente(pacienteId)
                                ?? Enumerable.Empty<ConsultasHistoriaClinicaEnfermeria>();

                    // (Si te sirve la tabla-resumen, conserva este bloque "filas")
                    var top = 10;
                    var filas = hceHist
                        .Take(top)
                        .Select(x => new
                        {
                            x.ConsultaId,
                            x.Fecha,

                            // Vitales y antropometría
                            x.PresionArterialTxt,
                            x.FC,
                            x.FR,
                            x.TemperaturaC,
                            x.SPO2,
                            x.PesoKg,
                            x.TallaM,
                            x.IMC,

                            // Un resumen clínico útil
                            x.DiagnosticoEnfermeria
                        })
                        .ToList();

                    ViewBag.HceHistorial = filas;
                    ViewBag.HceHistorialTotal = hceHist.Count();

                    // 👉 Lista completa para el acordeón con todos los campos del modelo de HCE
                    ViewBag.HceHistorialFull = hceHist.ToList();
                }
                else
                {
                    ViewBag.HceHistorial = new List<object>();
                    ViewBag.HceHistorialTotal = 0;
                    ViewBag.HceHistorialFull = new List<ConsultasHistoriaClinicaEnfermeria>();
                }
            }
            catch
            {
                ViewBag.HceHistorial = new List<object>();
                ViewBag.HceHistorialTotal = 0;
                ViewBag.HceHistorialFull = new List<ConsultasHistoriaClinicaEnfermeria>();
            }
            #endregion

            #region HISTORICO VALORACION INICIAL ENFERMERIA
            try
            {
                var pacienteId = paciente?.Id ?? 0;
                if (pacienteId > 0)
                {
                    // Última valoración (resumen)
                    var vieUltima = _veRepo.GetConsultaByPaciente(pacienteId);
                    if (vieUltima != null)
                    {
                        model.FechaUltimaConsulta = vieUltima.Fecha;
                        // Para el resumen, mostramos el Motivo (puedes cambiar a DiagnosticoMedico si prefieres)
                        model.MotivoUltimaConsulta = string.IsNullOrWhiteSpace(vieUltima.Motivo)
                            ? "-"
                            : vieUltima.Motivo;
                    }

                    // Histórico completo
                    var vieHist = _veRepo.GetConsultasByPaciente(pacienteId)
                                 ?? Enumerable.Empty<ConsultasValoracionInicialEnfermeria>();

                    // Tabla-resumen (opcional)
                    var top = 10;
                    var filas = vieHist
                        .Take(top)
                        .Select(x => new
                        {
                            x.ConsultaId,
                            x.Fecha,
                            x.Motivo,
                            x.DiagnosticoMedico,
                            x.Labs
                            // Si quisieras, podrías incluir contadores de checks: 
                            // Resp = (x.Resp ?? Array.Empty<string>()).Length, etc.
                        })
                        .ToList();

                    ViewBag.VeHistorial = filas;
                    ViewBag.VeHistorialTotal = vieHist.Count();

                    // 👉 Lista completa para poder renderizar TODOS los campos en la vista
                    ViewBag.VeHistorialFull = vieHist.ToList();
                }
                else
                {
                    ViewBag.VeHistorial = new List<object>();
                    ViewBag.VeHistorialTotal = 0;
                    ViewBag.VeHistorialFull = new List<ConsultasValoracionInicialEnfermeria>();
                }
            }
            catch
            {
                ViewBag.VeHistorial = new List<object>();
                ViewBag.VeHistorialTotal = 0;
                ViewBag.VeHistorialFull = new List<ConsultasValoracionInicialEnfermeria>();
            }
            #endregion


            #region HISTORICO SUEROTERAPIA
            try
            {
                var pacienteId = paciente?.Id ?? 0;
                if (pacienteId > 0)
                {
                    // Última consulta (resumen)
                    var sueroUltima = _sueroRepo.GetConsultaByPaciente(pacienteId);
                    if (sueroUltima != null)
                    {
                        model.FechaUltimaConsulta = sueroUltima.Fecha;
                        // En sueroterapia no hay "impresión clínica". Dejar un resumen simple con Motivo.
                        model.MotivoUltimaConsulta = string.IsNullOrWhiteSpace(sueroUltima.Motivo)
                            ? "-"
                            : sueroUltima.Motivo;
                    }

                    // Histórico completo
                    var sueroHist = _sueroRepo.GetConsultasByPaciente(pacienteId)
                                    ?? Enumerable.Empty<Database.Shared.Models.ConsultasSueroterapia>();

                    // Tabla-resumen (opcional, top 10)
                    var top = 10;
                    var filas = sueroHist
                        .Take(top)
                        .Select(x => new
                        {
                            x.ConsultaId,
                            x.Fecha,
                            x.Motivo,
                            x.DiagnosticoMedico,
                            x.Labs,
                            Medio = x.Medio,       // string[] (no transformar aquí)
                            Resp = x.Resp,        // string[]
                            Circ = x.Circ,        // string[]
                            Nutricion = x.Nutricion, // string[]
                            x.NutricionObs,
                            x.PlanTerapeutico
                        })
                        .ToList();

                    ViewBag.SueroHistorial = filas;
                    ViewBag.SueroHistorialTotal = sueroHist.Count();

                    // 👉 Lista completa, con TODOS los campos del modelo:
                    ViewBag.SueroHistorialFull = sueroHist.ToList();
                }
                else
                {
                    ViewBag.SueroHistorial = new List<object>();
                    ViewBag.SueroHistorialTotal = 0;
                    ViewBag.SueroHistorialFull = new List<Database.Shared.Models.ConsultasSueroterapia>();
                }
            }
            catch
            {
                ViewBag.SueroHistorial = new List<object>();
                ViewBag.SueroHistorialTotal = 0;
                ViewBag.SueroHistorialFull = new List<Database.Shared.Models.ConsultasSueroterapia>();
            }
            #endregion

            #region HISTORICO EXAMENES COMPLEMENTARIOS

            // Historial de exámenes complementarios del paciente
            try
            {
                // Valores por defecto en el ViewBag para no romper la vista en caso de error
                ViewBag.ExamenesHistorial = new List<object>();
                ViewBag.ExamenesHistorialTieneMas = false;
                ViewBag.ExamenesHistorialTotal = 0;

                var pacienteId = paciente?.Id ?? 0;

                if (pacienteId > 0)
                {
                    // 1) EXÁMENES VINCULADOS A CONSULTA (mundo consulta)
                    var examenesConsulta = _dbContext.ConsultasExamenLabClinicos
                        .Include(e => e.ExamenLabClinico)
                        .Include(e => e.Consulta)
                            .ThenInclude(c => c.Citas)
                        .Where(e =>
                            e.Consulta != null &&
                            e.Consulta.Citas != null &&
                            e.Consulta.Citas.PacienteId == pacienteId &&
                            e.Pagado == true) // SOLO exámenes pagados
                        .OrderByDescending(e => e.Consulta.FechaYHoraInicioConsulta)
                        .ToList();

                    // Exámenes de laboratorio para este paciente que SÍ tienen consulta
                    var examenesLaboratorioConConsulta = _dbContext.Examenes
                        .Where(x =>
                            x.PacienteId == pacienteId &&
                            x.ConsultaId != null &&
                            !x.Eliminado)
                        .ToList();

                    // Proyección de exámenes de consulta → historial
                    var examenesHistorialConsulta = examenesConsulta
                        .Select(e =>
                        {
                            var consultaId = e.ConsultaId ?? 0;
                            var pacienteIdConsulta = e.Consulta?.Citas?.PacienteId;

                            // Match por ConsultaId + PacienteId
                            var examenLab = examenesLaboratorioConConsulta.FirstOrDefault(x =>
                                x.ConsultaId == consultaId &&
                                x.PacienteId == pacienteIdConsulta);

                            return new
                            {
                                // Id de ConsultaExamenLabClinico (histórico original)
                                Id = (int?)e.Id,

                                // Usamos la fecha de la consulta
                                Fecha = (DateTime?)e.Consulta.FechaYHoraInicioConsulta,

                                // No. consulta (siempre > 0 en este caso)
                                ConsultaId = consultaId,

                                ExamenCodigo = e.ExamenLabClinico != null ? e.ExamenLabClinico.CodigoInterno : string.Empty,
                                ExamenNombre = e.ExamenLabClinico != null ? e.ExamenLabClinico.NombreExamen : string.Empty,

                                Cantidad = (int?)e.Cantidad,
                                ValorTotal = (decimal?)(e.PrecioValor * e.Cantidad),
                                Pagado = e.Pagado,

                                // Id de examen de laboratorio (para GenerarResultados)
                                ExamenLaboratorioId = examenLab != null ? (int?)examenLab.Id : null,
                                FechaRealizacion = examenLab != null ? (DateTime?)examenLab.FechaRealizacion : null
                            };
                        });

                    // 2) EXÁMENES DE LABORATORIO SIN CONSULTA (mundo laboratorio)
                    //    Se muestran también en el historial, marcados como "Sin consulta"
                    var examenesLaboratorioSinConsulta = _dbContext.Examenes
                        .Include(x => x.DetalleExamenes)
                            .ThenInclude(d => d.ExamenLabClinico)
                        .Where(x =>
                            x.PacienteId == pacienteId &&
                            x.ConsultaId == null &&
                            !x.Eliminado)
                        .ToList();

                    var examenesHistorialSinConsulta = examenesLaboratorioSinConsulta
                        .SelectMany(ex => ex.DetalleExamenes.Select(det => new
                        {
                            // No hay registro en ConsultaExamenLabClinico para estos casos
                            Id = (int?)null,

                            // Usamos la fecha de realización del examen de laboratorio
                            Fecha = (DateTime?)ex.FechaRealizacion,

                            // Sin consulta asociada → usamos 0 como sentinel
                            ConsultaId = 0, // "Sin consulta" en la vista

                            ExamenCodigo = det.ExamenLabClinico != null ? det.ExamenLabClinico.CodigoInterno : string.Empty,
                            ExamenNombre = det.ExamenLabClinico != null ? det.ExamenLabClinico.NombreExamen : string.Empty,

                            // No tenemos información económica en este flujo → dejamos vacío
                            Cantidad = (int?)null,
                            ValorTotal = (decimal?)null,
                            Pagado = false,

                            // Siempre existe examen de laboratorio aquí
                            ExamenLaboratorioId = (int?)ex.Id,
                            FechaRealizacion = (DateTime?)ex.FechaRealizacion
                        }));

                    // 3) UNIMOS AMBOS CONJUNTOS PARA EL HISTORIAL
                    var examenesHistorial = examenesHistorialConsulta
                        .Concat(examenesHistorialSinConsulta)
                        .Where(x => x.Fecha.HasValue) // evitar filas sin fecha
                        .OrderByDescending(x => x.Fecha.Value)
                        .ToList();

                    ViewBag.ExamenesHistorial = examenesHistorial;
                    ViewBag.ExamenesHistorialTieneMas = false;
                    ViewBag.ExamenesHistorialTotal = examenesHistorial.Count;
                }
            }
            catch (Exception ex)
            {
                // Ante cualquier error dejamos el historial vacío y registramos el problema
                Console.WriteLine("Error al cargar el historial de exámenes del paciente: " + ex.Message);
                ViewBag.ExamenesHistorial = new List<object>();
                ViewBag.ExamenesHistorialTieneMas = false;
                ViewBag.ExamenesHistorialTotal = 0;
            }

            #endregion

            #region HISTORICO SERVICIOS CONSULTA

            // Historial de servicios del paciente (todas las consultas del paciente)
            try
            {
                // Valores por defecto
                ViewBag.ServiciosHistorial = new List<object>();
                ViewBag.ServiciosHistorialTieneMas = false;
                ViewBag.ServiciosHistorialTotal = 0;

                var pacienteId = paciente?.Id ?? 0;

                if (pacienteId > 0)
                {
                    var serviciosPaciente = _dbContext.ConsultasServicios
                        .Include(s => s.Servicio)
                        .Include(s => s.Consulta)
                            .ThenInclude(c => c.Citas)
                        .Where(s =>
                            s.Consulta != null &&
                            s.Consulta.Citas != null &&
                            s.Consulta.Citas.PacienteId == pacienteId &&
                            s.Pagado == true) // ← SOLO servicios pagados
                        .OrderByDescending(s => s.Consulta.FechaYHoraInicioConsulta)
                        .ToList();

                    var totalRegistros = serviciosPaciente.Count;

                    var serviciosHistorial = serviciosPaciente
                        .Select(s => new
                        {
                            // La vista espera: Fecha, ConsultaId, ServicioCodigo, ServicioNombre, Cantidad, ValorTotal
                            Fecha = (DateTime?)s.Consulta.FechaYHoraInicioConsulta,
                            ConsultaId = s.ConsultaId,
                            ServicioCodigo = s.Servicio != null ? s.Servicio.CodigoInterno : string.Empty,
                            ServicioNombre = s.Servicio != null ? s.Servicio.NombreServicio : string.Empty,
                            Cantidad = s.Cantidad,
                            ValorTotal = s.PrecioValor * s.Cantidad,
                            Pagado = s.Pagado // ← lo exponemos para el color en la vista
                        })
                        .ToList();

                    ViewBag.ServiciosHistorial = serviciosHistorial;
                    ViewBag.ServiciosHistorialTieneMas = false; // ya no hay tope de 10
                    ViewBag.ServiciosHistorialTotal = totalRegistros;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cargar el historial de servicios del paciente: " + ex.Message);
                ViewBag.ServiciosHistorial = new List<object>();
                ViewBag.ServiciosHistorialTieneMas = false;
                ViewBag.ServiciosHistorialTotal = 0;
            }

            #endregion

            model.Init(_citasRepository, _serviciosRepository, _pacientesRepository);
            return View(model);
        }


        [HttpPost]
        public string EditarConsulta(ConsultasViewModel model)
        {
            try
            {
                var paciente = _pacientesRepository.Get((int)model.PacienteId, false);

                #region ANTECEDENTES PATOLOGICOS PACIENTE


                paciente.AntecedentesAlergias = model.PacienteAlergias ?? "";
                paciente.AntecedentesMedicamentos = model.PacienteMedicamentos ?? "";
                paciente.AntecedentesMedicos = model.PacienteMedicos ?? "";
                paciente.AntecedentesTraumaticos = model.PacienteTraumaticos ?? "";
                paciente.AntecedentesVicios = model.PacienteVicios ?? "";
                paciente.AntecedentesQuirurgicos = model.PacienteQuirurgicos ?? "";

                #endregion

                #region ANTECEDENTES PATOLOGICOS PACIENTE - PEDIATRICO - NIÑOS

                //Se modifican los antecedentes del paciente PEDIATRICO de acuerdo a la informacion recibida
                //en la edicion de la consulta
                paciente.PediatricosAntAlergias = model.PediatricoAntAlergias ?? "";
                paciente.PediatricosAntMedicamentos = model.PediatricoAntMedicamentos ?? "";
                paciente.PediatricosAntMedicos = model.PediatricoAntMedicos ?? "";
                paciente.PediatricosAntTraumaticos = model.PediatricoAntTraumaticos ?? "";
                paciente.PediatricosAntVicios = model.PediatricoAntVicios ?? "";
                paciente.PediatricosAntQuirurgicos = model.PediatricoAntQuirurgicos ?? "";

                #endregion

                #region PACIENTE - APNP - ANTECEDENTES NO PATOLOGICOS

                //Se modifican los APNP
                //del paciente de acuerdo a la informacion recibida
                paciente.PacienteApnp = new List<PacienteApnp>();
                DateTime? fechaUltimaRegla = null;
                if (model.PacienteApnpFechaUltimaRegla != null)
                {
                    fechaUltimaRegla = Convert.ToDateTime(model.PacienteApnpFechaUltimaRegla);
                }

                DateTime? fechaProbableParto = null;
                if (model.PacienteApnpFechaProbableParto != null)
                {
                    fechaProbableParto = Convert.ToDateTime(model.PacienteApnpFechaProbableParto);
                }
                paciente.PacienteApnp.Add(new PacienteApnp
                {
                    Gestas = model.PacienteApnpGestas,
                    Abortos = model.PacienteApnpAbortos,
                    Cesareas = model.PacienteApnpCesareas,
                    HijosMuertos = model.PacienteApnpHijosMuertos,
                    HijosVivos = model.PacienteApnpHijosVivos,
                    Menarquia = model.PacienteApnpMenarquia,
                    FechaUltimaRegla = fechaUltimaRegla,
                    FechaProbableParto = fechaProbableParto,
                    Otros = model.PacienteApnpOtros,
                    Partos = model.PacienteApnpPartos
                });

                #endregion

                #region PACIENTE - PEDIATRICO - APNP

                if (paciente.PacientePediatricoApnpId == null)
                {
                    paciente.PacientePediatricoApnp = new PacientePediatricoApnp();
                }
                paciente.PacientePediatricoApnp.Parto = model.PediatricoApnpParto;
                paciente.PacientePediatricoApnp.AtendidoPor = model.PediatricoApnpAtendidoPor;
                paciente.PacientePediatricoApnp.PesoAlNacer = model.PediatricoApnpPesoAlNacer;
                paciente.PacientePediatricoApnp.Inmunizaciones = model.PediatricoApnpInmunizaciones;
                paciente.PacientePediatricoApnp.Gesta = model.PediatricoApnpGesta;

                #endregion

                //Se guardan la entidad PACIENTE modificada
                _pacientesRepository.Update(paciente);

                Console.WriteLine("Cie10Codigo recibido: " + model.Cie10Codigo);
                //Se trae la entidad CONSULTA
                //de la base de datos
                var consultaEditada = _consultasRepository.GetConsulta((int)model.ConsultaId);

                consultaEditada.ObservacionesAdicionales = model.ObservacionesAdicionales;
                consultaEditada.FechaYHoraInicioConsulta = (DateTime)model.FechaYHoraInicio;
                consultaEditada.FechaProximaConsulta = model.FechaProximaConsulta.HasValue ? DateTime.SpecifyKind(model.FechaProximaConsulta.Value, DateTimeKind.Local) : null;
                consultaEditada.UrlFiles = model.UrlFiles;
                //consultaEditada.EstadoPagoConsultaId = (int)model.EstadoPagoId;
                consultaEditada.TipoConsulta = model.TipoConsulta;
                consultaEditada.TipoReferencia = model.TipoReferencia;
                consultaEditada.MedicoReferido = model.MedicoReferido;
                // consultaEditada.CostoConsulta = (decimal)model.CostoConsulta;
                consultaEditada.Cie10Codigo = model.Cie10Codigo;
                consultaEditada.CostoConsulta =
                    (model.ExamenesAgregados ?? new List<ConsultaExamenAgregadoViewModel>())
                        .Where(e => !e.Pagado)
                        .Sum(e => e.ValorUnitario * e.Cantidad)
                    +
                    (model.ServiciosAgregados ?? new List<ConsultasServicioAgregadoViewModel>())
                        .Where(s => !s.Pagado)
                        .Sum(s => (s.PrecioValor ?? 0) * s.ServicioCantidad)
                    +
                    (model.ElementosPrescripcion ?? new List<ConsultaPrescripcionViewModel>())
                        .Where(p => !p.Pagado)
                        .Sum(p => (p.PrecioValor ?? 0) * p.Cantidad);






                consultaEditada.ConsultaMotivo = model.ConsultaMotivo;
                consultaEditada.ConsultaMotivoPediatria = model.HistoriaPediatriaConsultaMotivo;

                //MANANTIALES NO utiliza la FaseTratamientoId
                //consultaEditada.FaseTratamientoId = model.FaseTratamientoId;


                #region HISTORIA CLINICA

                if (consultaEditada.HistoriaId == null)
                {
                    consultaEditada.Historia = new Historia();
                }
                consultaEditada.Historia.HistoriaProblema = model.HistoriaProblema;
                consultaEditada.Historia.Sintomas = model.Sintomas;
                consultaEditada.Historia.Diagnostico = model.Diagnostico;
                consultaEditada.Historia.HistoriaEnfermedadActual = model.HistoriaEnfermedadActual;
                consultaEditada.Historia.Comentario = model.HistoriaClinicaComentario;
                consultaEditada.Historia.ImpresionClinica = model.HistoriaClinicaImpresionClinica;

                #endregion

                #region HISTORIA CLINICA - PEDIATRIA

                if (consultaEditada.HistoriaPediatriaId == null)
                {
                    consultaEditada.HistoriaPediatria = new HistoriaPediatria();
                }
                consultaEditada.HistoriaPediatria.HistoriaProblema = model.HistoriaPediatriaHistoriaProblema;
                consultaEditada.HistoriaPediatria.Sintomas = model.HistoriaPediatriaSintomas;
                consultaEditada.HistoriaPediatria.Diagnostico = model.HistoriaPediatriaDiagnostico;
                consultaEditada.HistoriaPediatria.HistoriaEnfermedadActual = model.HistoriaPediatriaHistoriaEnfermedadActual;
                consultaEditada.HistoriaPediatria.Comentario = model.HistoriaPediatriaHistoriaClinicaComentario;
                consultaEditada.HistoriaPediatria.ImpresionClinica = model.HistoriaPediatriaHistoriaClinicaImpresionClinica;

                #endregion

                #region EXAMEN FISICO

                if (consultaEditada.ExamenFisicoId == null)
                {
                    consultaEditada.ExamenFisico = new ExamenFisico();
                }
                consultaEditada.ExamenFisico.EstadoGeneral = model.ExamenFisicoEstadoGeneral;
                consultaEditada.ExamenFisico.TensionArterialMmhg = model.ExamenFisicoTensionArterial;
                consultaEditada.ExamenFisico.Temperatura = model.ExamenFisicoTemperatura;
                consultaEditada.ExamenFisico.FrecuenciaRespiratoria = model.ExamenFisicoFrecuenciaRespiratoria;
                consultaEditada.ExamenFisico.FrecuenciaCardiaca = model.ExamenFisicoFrecuenciaCardiaca;

                consultaEditada.ExamenFisico.SaturacionDeOxigeno = model.ExamenFisicoSaturacionOxigeno;
                consultaEditada.ExamenFisico.PresionArterialBrazoDerecho = model.ExamenFisicoDerecho;
                consultaEditada.ExamenFisico.PresionArterialBrazoIzquierdo = model.ExamenFisicoIzquierdo;
                consultaEditada.ExamenFisico.Observaciones = model.ExamenFisicoObservaciones;
                consultaEditada.ExamenFisico.Peso = model.ExamenFisicoPeso;
                consultaEditada.ExamenFisico.Talla = model.ExamenFisicoTalla;
                consultaEditada.ExamenFisico.Glucosa = model.ExamenFisicoGlucosa;
                consultaEditada.ExamenFisico.IMC = model.ExamenFisicoIMC;
                consultaEditada.ExamenFisico.Glasgow = model.ExamenFisicoGlasgow;
                consultaEditada.ExamenFisico.PresionArterial = model.ExamenFisicoPresionArterial;

                #endregion

                #region EXAMEN FISICO - PEDIATRIA

                if (consultaEditada.ExamenFisicoPediatriaId == null)
                {
                    consultaEditada.ExamenFisicoPediatria = new ExamenFisicoPediatria();
                }
                consultaEditada.ExamenFisicoPediatria.EstadoGeneral = model.ExamenFisicoPediatriaEstadoGeneral;
                consultaEditada.ExamenFisicoPediatria.TensionArterialMmhg = model.ExamenFisicoPediatriaTensionArterial;
                consultaEditada.ExamenFisicoPediatria.Temperatura = model.ExamenFisicoPediatriaTemperatura;
                consultaEditada.ExamenFisicoPediatria.FrecuenciaRespiratoria = model.ExamenFisicoPediatriaFrecuenciaRespiratoria;
                consultaEditada.ExamenFisicoPediatria.FrecuenciaCardiaca = model.ExamenFisicoPediatriaFrecuenciaCardiaca;
                consultaEditada.ExamenFisicoPediatria.SaturacionDeOxigeno = model.ExamenFisicoPediatriaSaturacionOxigeno;
                consultaEditada.ExamenFisicoPediatria.PresionArterialBrazoDerecho = model.ExamenFisicoPediatriaDerecho;
                consultaEditada.ExamenFisicoPediatria.PresionArterialBrazoIzquierdo = model.ExamenFisicoPediatriaIzquierdo;
                consultaEditada.ExamenFisicoPediatria.Observaciones = model.ExamenFisicoPediatriaObservaciones;
                consultaEditada.ExamenFisicoPediatria.Peso = model.ExamenFisicoPediatriaPeso;
                consultaEditada.ExamenFisicoPediatria.Talla = model.ExamenFisicoPediatriaTalla;
                consultaEditada.ExamenFisicoPediatria.Glucosa = model.ExamenFisicoPediatriaGlucosa;
                consultaEditada.ExamenFisicoPediatria.IMC = model.ExamenFisicoPediatriaIMC;
                consultaEditada.ExamenFisicoPediatria.Glasgow = model.ExamenFisicoPediatriaGlasgow;
                consultaEditada.ExamenFisicoPediatria.PresionArterial = model.ExamenFisicoPediatriaPresionArterial;
                consultaEditada.ExamenFisicoPediatria.PediatricoPesoEdad = model.ExamenFisicoPediatricoPesoEdad;
                consultaEditada.ExamenFisicoPediatria.PediatricoPesoTalla = model.ExamenFisicoPediatricoPesoTalla;
                consultaEditada.ExamenFisicoPediatria.PediatricoTallaEdad = model.ExamenFisicoPediatricoTallaEdad;

                #endregion

                #region GINECOLOGIA - ANT NO PATOLOGICOS

                if (consultaEditada.ConsultaAntNoPatologicosGinecologiaId == null)
                {
                    consultaEditada.ConsultaAntNoPatologicosGinecologia = new ConsultaAntNoPatologicosGinecologia();
                }
                consultaEditada.GinecologiaConsultaMotivo = model.GinecologiaConsultaMotivo;
                consultaEditada.ConsultaAntNoPatologicosGinecologia.Menarquia = model.GinecologiaAntNoPatologicosMenarquia;
                consultaEditada.ConsultaAntNoPatologicosGinecologia.FechaUltimaRegla = model.GinecologiaAntNoPatologicosFechaUltimaRegla;
                consultaEditada.ConsultaAntNoPatologicosGinecologia.CicloMenstrual = model.GinecologiaAntNoPatologicosCicloMenstrual;
                consultaEditada.ConsultaAntNoPatologicosGinecologia.MetodoAnticonceptivo = model.GinecologiaAntNoPatologicosMetodoAnticonceptivo;
                consultaEditada.ConsultaAntNoPatologicosGinecologia.LactanciaMaterna = model.GinecologiaAntNoPatologicosLactanciaMaterna;
                consultaEditada.ConsultaAntNoPatologicosGinecologia.Gestas = model.GinecologiaAntNoPatologicosGestas;
                consultaEditada.ConsultaAntNoPatologicosGinecologia.Partos = model.GinecologiaAntNoPatologicosPartos;
                consultaEditada.ConsultaAntNoPatologicosGinecologia.Abortos = model.GinecologiaAntNoPatologicosAbortos;
                consultaEditada.ConsultaAntNoPatologicosGinecologia.Cesareas = model.GinecologiaAntNoPatologicosCesareas;
                consultaEditada.ConsultaAntNoPatologicosGinecologia.HijosVivos = model.GinecologiaAntNoPatologicosHijosVivos;
                consultaEditada.ConsultaAntNoPatologicosGinecologia.HijosMuertos = model.GinecologiaAntNoPatologicosHijosMuertos;
                consultaEditada.ConsultaAntNoPatologicosGinecologia.Otros = model.GinecologiaAntNoPatologicosOtros;

                #endregion

                #region OBSTETRICIA - ANT NO PATOLOGICOS

                if (consultaEditada.ConsultaAntNoPatologicosObstetriciaId == null)
                {
                    consultaEditada.ConsultaAntNoPatologicosObstetricia = new ConsultaAntNoPatologicosObstetricia();
                }
                DateTime? obstetriciaAntNoPatologicosfechaUltimaRegla = null;
                if (model.ObstetriciaAntNoPatologicosFechaUltimaRegla != null
                   && model.ObstetriciaAntNoPatologicosFechaUltimaRegla.Trim() != "")
                {
                    obstetriciaAntNoPatologicosfechaUltimaRegla = Convert.ToDateTime(model.ObstetriciaAntNoPatologicosFechaUltimaRegla);
                }
                DateTime? obstetriciaAntNoPatologicosfechaProbableParto = null;
                if (model.ObstetriciaAntNoPatologicosFechaProbableParto != null
                    && model.ObstetriciaAntNoPatologicosFechaProbableParto.Trim() != "")
                {
                    obstetriciaAntNoPatologicosfechaProbableParto = Convert.ToDateTime(model.ObstetriciaAntNoPatologicosFechaProbableParto);
                }
                consultaEditada.ConsultaAntNoPatologicosObstetricia.Gestas = model.ObstetriciaAntNoPatologicosGestas;
                consultaEditada.ConsultaAntNoPatologicosObstetricia.Partos = model.ObstetriciaAntNoPatologicosPartos;
                consultaEditada.ConsultaAntNoPatologicosObstetricia.Cotarquia = model.ObstetriciaAntNoPatologicosCotarquia;
                consultaEditada.ConsultaAntNoPatologicosObstetricia.Ultrasonido = model.ObstetriciaAntNoPatologicosUltrasonido;
                consultaEditada.ConsultaAntNoPatologicosObstetricia.NumeroParejas = model.ObstetriciaAntNoPatologicosNumeroParejas;

                consultaEditada.ConsultaAntNoPatologicosObstetricia.Abortos = model.ObstetriciaAntNoPatologicosAbortos;
                consultaEditada.ConsultaAntNoPatologicosObstetricia.Cesareas = model.ObstetriciaAntNoPatologicosCesareas;
                consultaEditada.ConsultaAntNoPatologicosObstetricia.HijosVivos = model.ObstetriciaAntNoPatologicosHijosVivos;
                consultaEditada.ConsultaAntNoPatologicosObstetricia.HijosMuertos = model.ObstetriciaAntNoPatologicosHijosMuertos;
                consultaEditada.ConsultaAntNoPatologicosObstetricia.FechaUltimaRegla = obstetriciaAntNoPatologicosfechaUltimaRegla;
                consultaEditada.ConsultaAntNoPatologicosObstetricia.FechaProbableParto = obstetriciaAntNoPatologicosfechaProbableParto;

                #endregion

                #region GINECOLOGIA - ANT PATOLOGICOS

                if (consultaEditada.ConsultaAntPatologicosGinecologiaId == null)
                {
                    consultaEditada.ConsultaAntPatologicosGinecologia = new ConsultaAntPatologicosGinecologia();
                }
                consultaEditada.ConsultaAntPatologicosGinecologia.Infecciones = model.GinecologiaAntPatologicosInfecciones;
                consultaEditada.ConsultaAntPatologicosGinecologia.Ets = model.GinecologiaAntPatologicosEts;
                consultaEditada.ConsultaAntPatologicosGinecologia.Papanicolau = model.GinecologiaAntPatologicosPapanicolau;
                consultaEditada.ConsultaAntPatologicosGinecologia.Otros = model.GinecologiaAntPatologicosOtros;

                #endregion

                #region GINECOLOGIA - EXAMEN FISICO

                if (consultaEditada.ConsultaExamenFisicoGinecologiaId == null)
                {
                    consultaEditada.ConsultaExamenFisicoGinecologia = new ConsultaExamenFisicoGinecologia();
                }
                consultaEditada.ConsultaExamenFisicoGinecologia.Mamas = model.GinecologiaExamenFisicoMamas;
                consultaEditada.ConsultaExamenFisicoGinecologia.Especuloscopia = model.GinecologiaExamenFisicoEspeculoscopia;
                consultaEditada.ConsultaExamenFisicoGinecologia.TactoVaginal = model.GinecologiaExamenFisicoTactoVaginal;
                consultaEditada.ConsultaExamenFisicoGinecologia.TactoRectal = model.GinecologiaExamenFisicoTactoRectal;
                consultaEditada.ConsultaExamenFisicoGinecologia.VulvaVagina = model.GinecologiaExamenFisicoVulvaVagina;

                #endregion

                #region ESTETICO

                consultaEditada.Estetico_Metabolismo = model.Metabolismo;
                consultaEditada.Estetico_Grasa = model.Grasa;
                consultaEditada.Estetico_Agua = model.Agua;
                consultaEditada.Estetico_IMC = model.IMC;
                consultaEditada.Estetico_Obesidad = model.Obesidad;
                consultaEditada.Estetico_ContornoBrazos = model.ContornoBrazos;
                consultaEditada.Estetico_ContornoBusto = model.ContornoBusto;
                consultaEditada.Estetico_ContornoAbdomen = model.ContornoAbdomen;
                consultaEditada.Estetico_ContornoCadera = model.ContornoCadera;
                consultaEditada.Estetico_ContornoPiernas = model.ContornoPiernas;
                consultaEditada.Estetico_Estatura = model.Estatura;

                #endregion

                #region SOLO PARA MUJERES

                consultaEditada.EstaEmbarazada = model.EstaEmbarazada;
                var numeroSemanasEmbarazo = 0;
                try { numeroSemanasEmbarazo = Convert.ToInt32(model.NumeroSemanasEmbarazo); } catch { numeroSemanasEmbarazo = 0; }
                consultaEditada.NumeroSemanasEmbarazo = numeroSemanasEmbarazo;
                consultaEditada.TomaPildorasAnticonceptivas = model.TomaPildorasAnticonceptivas;
                consultaEditada.EstaAmamantando = model.EstaAmamantando;

                #endregion

                #region BEBIDAS ALCOHOLICAS

                consultaEditada.BebeBebidasAlcoholicas = model.BebeBebidasAlcoholicas;
                consultaEditada.AlcoholUltimas24Horas = model.AlcoholUltimas24Horas;
                consultaEditada.AlcoholSemanal = model.AlcoholSemanal;

                #endregion

                //Dental
                consultaEditada.FechaUltimaRadiografiaDental = model.FechaUltimaRadiografiaDental;

                //Neurologico
                consultaEditada.ExamenNeurologico = model.ExamenNeurologico;

                //Ginecologico
                consultaEditada.ExamenGinecologico = model.ExamenGinecologico;

                #region OBSTETRICIA - EVALUACION OBSTETRICA

                consultaEditada.EvaluacionObstetricaUteroGravio = model.EvaluacionObstetricaUteroGravio;
                consultaEditada.EvaluacionObstetricaAbdomenObstetrico = model.EvaluacionObstetricaAbdomenObstetrico;
                consultaEditada.EvaluacionObstetricaFcf = model.EvaluacionObstetricaFcf;
                consultaEditada.EvaluacionObstetricaAu = model.EvaluacionObstetricaAu;
                consultaEditada.EvaluacionObstetricaBishop = model.EvaluacionObstetricaBishop;
                consultaEditada.EvaluacionObstetricaPresentacionLeopold = model.EvaluacionObstetricaPresentacionLeopold;
                consultaEditada.EvaluacionObstetricaOtras = model.EvaluacionObstetricaOtras;
                consultaEditada.EvaluacionObstetricaActividadUterina = model.EvaluacionObstetricaActividadUterina;
                consultaEditada.EvaluacionObstetricaMovimientoFetalPercetible = model.EvaluacionObstetricaMovimientoFetalPercetible;
                consultaEditada.EvaluacionObstetricaMovimientoFetalEspecifique = model.EvaluacionObstetricaMovimientoFetalEspecifique;
                consultaEditada.EvaluacionObstetricaTactoVaginal = model.EvaluacionObstetricaTactoVaginal;
                consultaEditada.EvaluacionObstetricaD = model.EvaluacionObstetricaD;
                consultaEditada.EvaluacionObstetricaCms = model.EvaluacionObstetricaCms;
                consultaEditada.EvaluacionObstetricaBPorciento = model.EvaluacionObstetricaBPorciento;
                consultaEditada.EvaluacionObstetricaAltitud = model.EvaluacionObstetricaAltitud;
                consultaEditada.EvaluacionObstetricaPosicionCervix = model.EvaluacionObstetricaPosicionCervix;
                consultaEditada.EvaluacionObstetricaMembranasOvulares = model.EvaluacionObstetricaMembranasOvulares;
                consultaEditada.EvaluacionObstetricaLiquidoAmniotico = model.EvaluacionObstetricaLiquidoAmniotico;
                consultaEditada.EvaluacionObstetricaLiquidoAmnioticoEspecifique = model.EvaluacionObstetricaLiquidoAmnioticoEspecifique;
                consultaEditada.EvaluacionObstetricaPelvis = model.EvaluacionObstetricaPelvis;
                consultaEditada.EvaluacionObstetricaConsistencia = model.EvaluacionObstetricaConsistencia;

                #endregion

                #region OBSTETRICIA - ECOGRAFIA OBSTETRICA

                consultaEditada.EcografiaObstetricaFeto = model.EcografiaObstetricaFeto;
                consultaEditada.EcografiaObstetricaEstado = model.EcografiaObstetricaEstado;
                consultaEditada.EcografiaObstetricaSituacion = model.EcografiaObstetricaSituacion;
                consultaEditada.EcografiaObstetricaPresentacion = model.EcografiaObstetricaPresentacion;
                consultaEditada.EcografiaObstetricaPosicion = model.EcografiaObstetricaPosicion;
                consultaEditada.EcografiaObstetricaDorso = model.EcografiaObstetricaDorso;

                #endregion

                #region OBSTETRICIA - BIOMETRIA
                consultaEditada.NumeroBebes = model.NumeroBebes;
                consultaEditada.ObsteBiometriaRlc = model.ObsteBiometriaRlc;
                consultaEditada.ObsteBiometriaSg = model.ObsteBiometriaSg;
                consultaEditada.ObsteBiometriaW = model.ObsteBiometriaW;
                consultaEditada.ObsteBiometriaDbp = model.ObsteBiometriaDbp;
                consultaEditada.ObsteBiometriaHc = model.ObsteBiometriaHc;
                consultaEditada.ObsteBiometriaAc = model.ObsteBiometriaAc;
                consultaEditada.ObsteBiometriaLf = model.ObsteBiometriaLf;
                consultaEditada.ObsteBiometriaEg = model.ObsteBiometriaEg;
                consultaEditada.ObsteBiometriaFcf = model.ObsteBiometriaFcf;
                consultaEditada.ObsteBiometriaPlacenta = model.ObsteBiometriaPlacenta;
                consultaEditada.ObsteBiometriaGrado = model.ObsteBiometriaGrado;
                consultaEditada.ObsteBiometriaIla = model.ObsteBiometriaIla;
                consultaEditada.ObsteBiometriaMalformaciones = model.ObsteBiometriaMalformaciones;
                consultaEditada.ObsteBiometriaPeso = model.ObsteBiometriaPeso;
                consultaEditada.ObsteBiometriaSexo = model.ObsteBiometriaSexo;
                consultaEditada.ObsteBiometriaFechaParto = model.ObsteBiometriaFechaParto;
                consultaEditada.ObsteBiometriaComentario = model.ObsteBiometriaComentario;
                consultaEditada.ObsteBiometriaPresentacion = model.ObsteBiometriaPresentacion;

                // Guardar los datos de las biometrías adicionales
                consultaEditada.ObsteBiometriaRlc2 = model.ObsteBiometriaRlc2;
                consultaEditada.ObsteBiometriaSg2 = model.ObsteBiometriaSg2;
                consultaEditada.ObsteBiometriaDbp2 = model.ObsteBiometriaDbp2;
                consultaEditada.ObsteBiometriaHc2 = model.ObsteBiometriaHc2;
                consultaEditada.ObsteBiometriaAc2 = model.ObsteBiometriaAc2;
                consultaEditada.ObsteBiometriaLf2 = model.ObsteBiometriaLf2;
                consultaEditada.ObsteBiometriaEg2 = model.ObsteBiometriaEg2;
                consultaEditada.ObsteBiometriaFcf2 = model.ObsteBiometriaFcf2;
                consultaEditada.ObsteBiometriaPlacenta2 = model.ObsteBiometriaPlacenta2;
                consultaEditada.ObsteBiometriaGrado2 = model.ObsteBiometriaGrado2;
                consultaEditada.ObsteBiometriaIla2 = model.ObsteBiometriaIla2;
                consultaEditada.ObsteBiometriaMalformaciones2 = model.ObsteBiometriaMalformaciones2;
                consultaEditada.ObsteBiometriaPeso2 = model.ObsteBiometriaPeso2;
                consultaEditada.ObsteBiometriaSexo2 = model.ObsteBiometriaSexo2;
                consultaEditada.ObsteBiometriaFechaParto2 = model.ObsteBiometriaFechaParto2;
                consultaEditada.ObsteBiometriaComentario2 = model.ObsteBiometriaComentario2;
                consultaEditada.ObsteBiometriaPresentacion2 = model.ObsteBiometriaPresentacion2;

                consultaEditada.ObsteBiometriaRlc3 = model.ObsteBiometriaRlc3;
                consultaEditada.ObsteBiometriaSg3 = model.ObsteBiometriaSg3;
                consultaEditada.ObsteBiometriaDbp3 = model.ObsteBiometriaDbp3;
                consultaEditada.ObsteBiometriaHc3 = model.ObsteBiometriaHc3;
                consultaEditada.ObsteBiometriaAc3 = model.ObsteBiometriaAc3;
                consultaEditada.ObsteBiometriaLf3 = model.ObsteBiometriaLf3;
                consultaEditada.ObsteBiometriaEg3 = model.ObsteBiometriaEg3;
                consultaEditada.ObsteBiometriaFcf3 = model.ObsteBiometriaFcf3;
                consultaEditada.ObsteBiometriaPlacenta3 = model.ObsteBiometriaPlacenta3;
                consultaEditada.ObsteBiometriaGrado3 = model.ObsteBiometriaGrado3;
                consultaEditada.ObsteBiometriaIla3 = model.ObsteBiometriaIla3;
                consultaEditada.ObsteBiometriaMalformaciones3 = model.ObsteBiometriaMalformaciones3;
                consultaEditada.ObsteBiometriaPeso3 = model.ObsteBiometriaPeso3;
                consultaEditada.ObsteBiometriaSexo3 = model.ObsteBiometriaSexo3;
                consultaEditada.ObsteBiometriaFechaParto3 = model.ObsteBiometriaFechaParto3;
                consultaEditada.ObsteBiometriaComentario3 = model.ObsteBiometriaComentario3;
                consultaEditada.ObsteBiometriaPresentacion3 = model.ObsteBiometriaPresentacion3;

                consultaEditada.ObsteBiometriaRlc4 = model.ObsteBiometriaRlc4;
                consultaEditada.ObsteBiometriaSg4 = model.ObsteBiometriaSg4;
                consultaEditada.ObsteBiometriaDbp4 = model.ObsteBiometriaDbp4;
                consultaEditada.ObsteBiometriaHc4 = model.ObsteBiometriaHc4;
                consultaEditada.ObsteBiometriaAc4 = model.ObsteBiometriaAc4;
                consultaEditada.ObsteBiometriaLf4 = model.ObsteBiometriaLf4;
                consultaEditada.ObsteBiometriaEg4 = model.ObsteBiometriaEg4;
                consultaEditada.ObsteBiometriaFcf4 = model.ObsteBiometriaFcf4;
                consultaEditada.ObsteBiometriaPlacenta4 = model.ObsteBiometriaPlacenta4;
                consultaEditada.ObsteBiometriaGrado4 = model.ObsteBiometriaGrado4;
                consultaEditada.ObsteBiometriaIla4 = model.ObsteBiometriaIla4;
                consultaEditada.ObsteBiometriaMalformaciones4 = model.ObsteBiometriaMalformaciones4;
                consultaEditada.ObsteBiometriaPeso4 = model.ObsteBiometriaPeso4;
                consultaEditada.ObsteBiometriaSexo4 = model.ObsteBiometriaSexo4;
                consultaEditada.ObsteBiometriaFechaParto4 = model.ObsteBiometriaFechaParto4;
                consultaEditada.ObsteBiometriaComentario4 = model.ObsteBiometriaComentario4;
                consultaEditada.ObsteBiometriaPresentacion4 = model.ObsteBiometriaPresentacion4;

                #endregion

                #region ECOGRAFIA ENDOCAVITARIA
                consultaEditada.EcografiaEndocavitariaUtero = model.EcografiaEndocavitariaUtero;
                consultaEditada.EcografiaEndocavitariaLongitudinal = model.EcografiaEndocavitariaLongitudinal;
                consultaEditada.EcografiaEndocavitariaTransverso = model.EcografiaEndocavitariaTransverso;
                consultaEditada.EcografiaEndocavitariaEndometrio = model.EcografiaEndocavitariaEndometrio;
                consultaEditada.EcografiaEndocavitariaOvarioDerecho = model.EcografiaEndocavitariaOvarioDerecho;
                consultaEditada.EcografiaEndocavitariaOvarioIzquierdo = model.EcografiaEndocavitariaOvarioIzquierdo;
                consultaEditada.EcografiaEndocavitariaFondoSaco = model.EcografiaEndocavitariaFondoSaco;
                consultaEditada.EcografiaEndocavitariaImpresionClinica = model.EcografiaEndocavitariaImpresionClinica;
                consultaEditada.EcografiaEndocavitariaComentario = model.EcografiaEndocavitariaComentario;

                #endregion


                #region REVISION POR SISTEMAS

                // Verifica si la instancia de ConsultaRevisionSistemas existe; si no, crea una nueva
                if (consultaEditada.ConsultaRevisionSistemas == null)
                {
                    consultaEditada.ConsultaRevisionSistemas = new ConsultaRevisionSistemas();
                }

                // Asigna los valores del modelo a las propiedades de ConsultaRevisionSistemas
                consultaEditada.ConsultaRevisionSistemas.AparienciaGeneral = model.RevisionSistemasAparienciaGeneral;
                consultaEditada.ConsultaRevisionSistemas.Abdomen = model.RevisionSistemasAbdomen;
                consultaEditada.ConsultaRevisionSistemas.Torax = model.RevisionSistemasTorax;
                consultaEditada.ConsultaRevisionSistemas.Cuello = model.RevisionSistemasCuello;
                consultaEditada.ConsultaRevisionSistemas.OidosBoca = model.RevisionSistemasOidosBoca;
                consultaEditada.ConsultaRevisionSistemas.Cabeza = model.RevisionSistemasCabeza;
                consultaEditada.ConsultaRevisionSistemas.DorsoYExtremidades = model.RevisionSistemasDorsoYExtremidades;
                consultaEditada.ConsultaRevisionSistemas.Genitales = model.RevisionSistemasGenitales;

                // Nuevos campos
                consultaEditada.ConsultaRevisionSistemas.Neurologico = model.NewRevisionSistemasNeurologico;
                consultaEditada.ConsultaRevisionSistemas.Cardiovascular = model.NewRevisionSistemasCardiovascular;
                consultaEditada.ConsultaRevisionSistemas.Respiratorio = model.NewRevisionSistemasRespiratorio;
                consultaEditada.ConsultaRevisionSistemas.Gastrointestinal = model.NewRevisionSistemasGastrointestinal;
                consultaEditada.ConsultaRevisionSistemas.Musculoesqueletico = model.NewRevisionSistemasMusculoesqueletico;
                consultaEditada.ConsultaRevisionSistemas.PielFanera = model.NewRevisionSistemasPielFanera;
                consultaEditada.ConsultaRevisionSistemas.Genitourinario = model.NewRevisionSistemasGenitourinario;

                #endregion


                #region REVISION POR SISTEMAS - PEDIATRIA

                if (consultaEditada.ConsultaRevisionSistemasPediatriaId == null)
                {
                    consultaEditada.ConsultaRevisionSistemasPediatria = new ConsultaRevisionSistemasPediatria();
                }
                consultaEditada.ConsultaRevisionSistemasPediatria.AparienciaGeneral = model.RevisionSistemasPediatriaAparienciaGeneral;
                consultaEditada.ConsultaRevisionSistemasPediatria.Abdomen = model.RevisionSistemasPediatriaAbdomen;
                consultaEditada.ConsultaRevisionSistemasPediatria.Torax = model.RevisionSistemasPediatriaTorax;
                consultaEditada.ConsultaRevisionSistemasPediatria.Cuello = model.RevisionSistemasPediatriaCuello;
                consultaEditada.ConsultaRevisionSistemasPediatria.OidosBoca = model.RevisionSistemasPediatriaOidosBoca;
                consultaEditada.ConsultaRevisionSistemasPediatria.Cabeza = model.RevisionSistemasPediatriaCabeza;
                consultaEditada.ConsultaRevisionSistemasPediatria.DorsoYExtremidades = model.RevisionSistemasPediatriaDorsoYExtremidades;
                consultaEditada.ConsultaRevisionSistemasPediatria.Genitales = model.RevisionSistemasPediatriaGenitales;


                #endregion

                #region AREA TERAPEUTICA

                consultaEditada.TerapeuticoDatosGenerales = model.TerapeuticoDatosGenerales;
                consultaEditada.TerapeuticoActividadesDiarias = model.TerapeuticoActividadesDiarias;
                consultaEditada.TerapeuticoConQuienVive = model.TerapeuticoConQuienVive;
                consultaEditada.TerapeuticoHabitosAlimenticios = model.TerapeuticoHabitosAlimenticios;
                consultaEditada.TerapeuticoEjercicio = model.TerapeuticoEjercicio;
                consultaEditada.TerapeuticoFinesSemana = model.TerapeuticoFinesSemana;
                consultaEditada.TerapeuticoHistoriaMedica = model.TerapeuticoHistoriaMedica;
                consultaEditada.TerapeuticoHistoriaPeso = model.TerapeuticoHistoriaPeso;
                consultaEditada.notaOperatoria = model.notaOperatoria;//editado notaoperatoria pestaña notaOperatoria RECONSULTA

                #endregion

                #region SERVICIOS

                var servicios = new List<ConsultaServicio>();
                if (model.ServiciosAgregados != null
                 && model.ServiciosAgregados.Count > 0)
                {
                    foreach (var servicioAg in model.ServiciosAgregados)
                    {
                        if (!servicioAg.Pagado)
                        {
                            consultaEditada.EstadoPagoConsultaId = (int)EstadoPagoConsultaEnum.Pendiente;
                        }
                        servicios.Add(new ConsultaServicio
                        {
                            ConsultaId = (int)servicioAg.ConsultaId,
                            ServicioId = (int)servicioAg.ServicioId,
                            NumeroDiente = servicioAg.NumeroDiente,
                            Cantidad = servicioAg.ServicioCantidad,
                            PrecioId = servicioAg.PrecioId,
                            PrecioValor = servicioAg.PrecioValor,
                            PrecioCubiertoSeguro = servicioAg.ServicioValorCubiertoSeguro,
                            PrecioCopago = servicioAg.ServicioValorCopago,
                            Pagado = servicioAg.Pagado
                        });
                    }
                }
                consultaEditada.ConsultasServicios = servicios;

                #endregion

                #region PRESCRIPCIÓN - MEDICAMENTOS OTROS
                if (model.MedicamentosOtros != null && model.MedicamentosOtros.Any())
                {
                    Console.WriteLine("MedicamentosOtros recibidos:");
                    foreach (var m in model.MedicamentosOtros)
                    {
                        Console.WriteLine($"Datos a guardar - {m.Nombre}, {m.Indicaciones}, Cantidad: {m.Cantidad}, Fecha:{m.FechaPrescripcion}, Id: {m.Id}");
                    }

                    // NO reemplazar la colección existente
                    if (consultaEditada.MedicamentosOtros == null)
                        consultaEditada.MedicamentosOtros = new List<MedicamentoOtroConsulta>();

                    foreach (var med in model.MedicamentosOtros)
                    {
                        if (med == null) continue;

                        // Si trae Id > 0 es existente → NO tocar
                        if (med.Id > 0) continue;

                        // Filtra filas vacías para evitar "basura"
                        var nombre = (med.Nombre ?? "").Trim();
                        if (string.IsNullOrWhiteSpace(nombre)) continue;

                        var indic = (med.Indicaciones ?? "").Trim();
                        var cant = med.Cantidad;

                        // Fecha por defecto si no llegó
                        var fecha = DateTime.Now;

                        // Evitar duplicado básico en esta misma consulta (Nombre + Indicaciones + Cantidad)
                        var duplicado = consultaEditada.MedicamentosOtros.Any(x =>
                            string.Equals(x.Nombre, nombre, StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(x.Indicaciones, indic, StringComparison.OrdinalIgnoreCase) &&
                            x.Cantidad == cant
                        );
                        if (duplicado) continue;

                        consultaEditada.MedicamentosOtros.Add(new MedicamentoOtroConsulta
                        {
                            ConsultaId = consultaEditada.Id,
                            Nombre = nombre,
                            Indicaciones = indic,
                            Cantidad = cant,
                            FechaPrescripcion = fecha
                        });
                    }
                }
                #endregion




                #region CARACTERISTICAS DENTALES

                var caracteristicas = new List<ConsultaCaracteristicaDental>();
                if (model.CaracteristicasDientes != null)
                {
                    foreach (var caracteristica in model.CaracteristicasDientes)
                    {
                        caracteristicas.Add(new ConsultaCaracteristicaDental
                        {
                            NumeroDiente = caracteristica.NumeroDiente,
                            Percusiones_VerticalMas = caracteristica.Percusiones_VerticalMas,
                            Percusiones_HorizontalMas = caracteristica.Percusiones_HorizontalMas,
                            Percusiones_VerticalMenos = caracteristica.Percusiones_VerticalMenos,
                            Percusiones_HorizontalMenos = caracteristica.Percusiones_HorizontalMenos,
                            Dolor_Localizado = caracteristica.Dolor_Localizado,
                            Dolor_Fugaz = caracteristica.Dolor_Fugaz,
                            Dolor_Persistente = caracteristica.Dolor_Persistente,
                            Dolor_Referido = caracteristica.Dolor_Referido,
                            Dolor_Espontaneo = caracteristica.Dolor_Espontaneo,
                            Estimulo_Frio = caracteristica.Estimulo_Frio,
                            Estimulo_Calor = caracteristica.Estimulo_Calor,
                            Estimulo_DulceAcido = caracteristica.Estimulo_DulceAcido,
                            Estimulo_Masticacion = caracteristica.Estimulo_Masticacion,
                            Estimulo_Otro = caracteristica.Estimulo_Otro,
                            TermicaFrio_Positiva = caracteristica.TermicaFrio_Positiva,
                            TermicaFrio_Negativa = caracteristica.TermicaFrio_Negativa,
                            TermicaFrio_Localizada = caracteristica.TermicaFrio_Localizada,
                            TermicaFrio_Fugaz = caracteristica.TermicaFrio_Fugaz,
                            TermicaFrio_Incrementa = caracteristica.TermicaFrio_Incrementa,
                            TermicaFrio_Referida = caracteristica.TermicaFrio_Referida,
                            TermicaFrio_Irradiado = caracteristica.TermicaFrio_Irradiado,
                            TermicaFrio_Persistente = caracteristica.TermicaFrio_Persistente,
                            TermicaFrio_Decrece = caracteristica.TermicaFrio_Decrece,
                            TermicaCalor_Positiva = caracteristica.TermicaCalor_Positiva,
                            TermicaCalor_Negativa = caracteristica.TermicaCalor_Negativa,
                            TermicaCalor_Localizada = caracteristica.TermicaCalor_Localizada,
                            TermicaCalor_Fugaz = caracteristica.TermicaCalor_Fugaz,
                            TermicaCalor_Incrementa = caracteristica.TermicaCalor_Incrementa,
                            TermicaCalor_Referida = caracteristica.TermicaCalor_Referida,
                            TermicaCalor_Irradiado = caracteristica.TermicaCalor_Irradiado,
                            TermicaCalor_Persistente = caracteristica.TermicaCalor_Persistente,
                            TermicaCalor_Decrece = caracteristica.TermicaCalor_Decrece,
                            Diagnostico_ManchaBlanca = caracteristica.Diagnostico_ManchaBlanca,
                            Diagnostico_Caries = caracteristica.Diagnostico_Caries,
                            Diagnostico_Traumatismo = caracteristica.Diagnostico_Traumatismo,
                            Diagnostico_Abfraccion = caracteristica.Diagnostico_Abfraccion,
                            Diagnostico_Atricion = caracteristica.Diagnostico_Atricion,
                            Diagnostico_Erosion = caracteristica.Diagnostico_Erosion,
                            Diagnostico_Restauracion = caracteristica.Diagnostico_Restauracion,
                            Diagnostico_Ajustada = caracteristica.Diagnostico_Ajustada,
                            Diagnostico_Desajustada = caracteristica.Diagnostico_Desajustada,
                            Diagnostico_PulpaSana = caracteristica.Diagnostico_PulpaSana,
                            Diagnostico_PulpitisReversible = caracteristica.Diagnostico_PulpitisReversible,
                            Diagnostico_PulpitisIrreversible = caracteristica.Diagnostico_PulpitisIrreversible,
                            Diagnostico_NecrosisPulpar = caracteristica.Diagnostico_NecrosisPulpar
                        });
                    }
                }
                consultaEditada.ConsultasCaracteristicasDentales = caracteristicas;

                #endregion

                #region SEGUIMIENTOS NUTRICIONALES

                if (model.SeguimientosNutricionales != null
                 && model.SeguimientosNutricionales.Count > 0)
                {
                    foreach (var seguimiento in model.SeguimientosNutricionales)
                    {
                        var seguimientoConsulta = new PacienteSeguimientoNutricional
                        {
                            PacienteId = model.PacienteId,
                            Fecha = seguimiento.Fecha,
                            Peso = seguimiento.Peso,
                            IMC = seguimiento.IMC,
                            PGC = seguimiento.PGC,
                            Cuello = seguimiento.Cuello,
                            Busto = seguimiento.Busto,
                            CinturaAbdomen = seguimiento.CinturaAbdomen,
                            Cadera = seguimiento.Cadera,
                            Muslo = seguimiento.Muslo,
                            Brazo = seguimiento.Brazo,
                            Muñeca = seguimiento.Munneca,
                        };
                        if (seguimiento.Id != null)
                        {
                            seguimientoConsulta.Id = (int)seguimiento.Id;
                        }
                        if (seguimientoConsulta.Id == 0)
                        {
                            _pacientesRepository.AddSeguimientoNutricional(seguimientoConsulta);
                        }
                        else
                        {
                            _pacientesRepository.UpdateSeguimientoNutricional(seguimientoConsulta);
                        }
                    }
                }

                #endregion

                #region VACUNAS PACIENTE
                if (model.VacunasPaciente != null)
                {
                    foreach (var vacuna in model.VacunasPaciente)
                    {
                        if (vacuna.VacunaPacienteId == null)
                        {
                            var nuevaVacunaPaciente = new VacunaPaciente
                            {
                                // No asignar el Id aquí, ya que es generado automáticamente por la base de datos
                                PacienteId = (int)model.PacienteId,
                                VacunaId = (int)vacuna.VacunaId,
                                Primera = (bool)vacuna.Primera,
                                FechaPrimera = vacuna.FechaPrimera,
                                Segunda = (bool)vacuna.Segunda,
                                FechaSegunda = vacuna.FechaSegunda,
                                Tercera = (bool)vacuna.Tercera,
                                FechaTercera = vacuna.FechaTercera,
                                PrimerRefuerzo = (bool)vacuna.PrimerRefuerzo,
                                FechaPrimerRefuerzo = vacuna.FechaPrimerRefuerzo,
                                SegundoRefuerzo = (bool)vacuna.SegundoRefuerzo,
                                FechaSegundoRefuerzo = vacuna.FechaSegundoRefuerzo
                            };

                            _pacientesRepository.AddVacuna(nuevaVacunaPaciente);
                        }
                        else
                        {
                            var vacunaPacienteExistente = new VacunaPaciente
                            {
                                Id = (int)vacuna.VacunaPacienteId, // Asignar el Id existente
                                PacienteId = (int)model.PacienteId,
                                VacunaId = (int)vacuna.VacunaId,
                                Primera = (bool)vacuna.Primera,
                                FechaPrimera = vacuna.FechaPrimera,
                                Segunda = (bool)vacuna.Segunda,
                                FechaSegunda = vacuna.FechaSegunda,
                                Tercera = (bool)vacuna.Tercera,
                                FechaTercera = vacuna.FechaTercera,
                                PrimerRefuerzo = (bool)vacuna.PrimerRefuerzo,
                                FechaPrimerRefuerzo = vacuna.FechaPrimerRefuerzo,
                                SegundoRefuerzo = (bool)vacuna.SegundoRefuerzo,
                                FechaSegundoRefuerzo = vacuna.FechaSegundoRefuerzo
                            };

                            _pacientesRepository.UpdateVacuna(vacunaPacienteExistente);
                        }
                    }
                }
                #endregion

                #region ANTECEDENTES FAMILIARES
                if (model.AntecedentesFamiliaresPaciente != null)
                {
                    foreach (var antecedente in model.AntecedentesFamiliaresPaciente)
                    {
                        var antecedenteFamiliar = new PatologiaPaciente
                        {
                            Id = antecedente.Id,
                            DescripcionOtraPatologia = antecedente.DescripcionOtraPatologia ?? "",
                            AbuelaMaterna = antecedente.AbuelaMaterna,
                            AbuelaPaterna = antecedente.AbuelaPaterna,
                            AbueloMaterno = antecedente.AbueloMaterno,
                            AbueloPaterno = antecedente.AbueloPaterno,
                            Hermanos = antecedente.Hermanos,
                            Madre = antecedente.Madre,
                            OtrosMaterno = antecedente.OtrosMaterno,
                            OtrosPaterno = antecedente.OtrosPaterno,
                            Padre = antecedente.Padre
                        };

                        _pacientesRepository.UpdateAntecedenteFamiliar(antecedenteFamiliar);
                    }
                }
                #endregion

                #region CONSULTA - EXAMENES - ARCHIVOS

                if (model.ConsultaExamenArchivos != null
                 && model.ConsultaExamenArchivos.Count > 0)
                {
                    foreach (var archivoexamen in model.ConsultaExamenArchivos)
                    {
                        var nuevoarchivoexamen = new ConsultaExamenArchivo
                        {
                            Id = archivoexamen.Id,
                            ConsultaId = ((int)model.ConsultaId),
                            NombreArchivo = archivoexamen.NombreArchivo,
                            UrlArchivo = archivoexamen.UrlArchivo
                        };

                        if (archivoexamen.Id == 0)
                        {
                            _consultasRepository.AddExamnenArchivo(nuevoarchivoexamen);
                        }
                        else
                        {
                            _consultasRepository.UpdateExamnenArchivo(nuevoarchivoexamen);
                        }
                    }
                }

                #endregion

                #region Información médica
                paciente.MedicaUsaLentesContacto = model.MedicaUsaLentesContacto;
                paciente.MedicaUsaLentesContactoDescripcion = model.MedicaUsaLentesContactoDescripcion;
                paciente.MedicaArticulacionesArtificiales = model.MedicaArticulacionesArtificiales;
                paciente.MedicaArticulacionesArtificialesFecha = model.MedicaArticulacionesArtificialesFecha;
                paciente.MedicaArticulacionesArtificialesComplicaciones = model.MedicaArticulacionesArtificialesComplicaciones;
                paciente.MedicaTomaAlendronato = model.MedicaTomaAlendronato;
                paciente.MedicaTomaAlendronatoFecha = model.MedicaTomaAlendronatoFecha;
                paciente.MedicaTratamientoDolorHuesos = model.MedicaTratamientoDolorHuesos;
                paciente.MedicaTratamientoDolorHuesosFechaInicio = model.MedicaTratamientoDolorHuesosFechaInicio;
                paciente.MedicaTratamientoDolorHuesosDescripcionCaso = model.MedicaTratamientoDolorHuesosDescripcionCaso;
                paciente.MedicaSustanciasReguladorasDrogas = model.MedicaSustanciasReguladorasDrogas;
                paciente.MedicaSustanciasReguladorasDrogasFecha = model.MedicaSustanciasReguladorasDrogasFecha;
                paciente.MedicaUsaTabaco = model.MedicaUsaTabaco;
                paciente.MedicaBebidasAlcoholicas = model.MedicaBebidasAlcoholicas;
                paciente.MedicaBebidasAlcoholicasDescripcion = model.MedicaBebidasAlcoholicasDescripcion;

                #endregion

                #region EXAMENES AGREGADOS DE LABORATORIO - ExamenLabClinicos

                if (model.ExamenesAgregados != null && model.ExamenesAgregados.Count() > 0)
                {
                    consultaEditada.ConsultaExamenesAgregados = new List<ConsultaExamenLabClinico>();
                    foreach (var examen in model.ExamenesAgregados)
                    {

                        if (!examen.Pagado)
                        {
                            consultaEditada.EstadoPagoConsultaId = (int)EstadoPagoConsultaEnum.Pendiente;
                        }
                        var fechaRegistro = DateTime.Now;
                        if (examen.FechaRegistro != null)
                        {
                            fechaRegistro = ((DateTime)examen.FechaRegistro);
                        }
                        consultaEditada.ConsultaExamenesAgregados.Add(new ConsultaExamenLabClinico
                        {
                            FechaRegistro = fechaRegistro,
                            Cantidad = examen.Cantidad,
                            ExamenLabClinicoId = examen.ExamenId,
                            PrecioId = examen.PrecioId,
                            PrecioValor = examen.ValorUnitario,
                            Pagado = examen.Pagado
                        });
                    }
                }



                #endregion


                // // Reemplazar la sección "Agregar examen directamente" con esto:

                // var usuarioSolicita = _userManager.GetUserId(HttpContext.User);

                // // 🔍 BUSCAR SI YA EXISTE UN EXAMEN para este paciente (activo y no eliminado)
                // var examenExistente = _laboratorioRepository.GetExamenByConsulta((int)model.ConsultaId);

                // Examen examenActual;

                // if (examenExistente != null)
                // {
                //     // ✅ USAR EL EXAMEN EXISTENTE
                //     examenActual = examenExistente;
                //     Console.WriteLine($"📋 Usando examen existente ID: {examenActual.Id}");
                // }
                // else
                // {
                //     // ✅ CREAR NUEVO EXAMEN solo si no existe
                //     examenActual = new Examen
                //     {
                //         EstadoExamenId = 1,
                //         ConsultaId = (int)model.ConsultaId,
                //         FechaRealizacion = DateTime.Now,
                //         PacienteId = model.PacienteId,
                //         UsuarioSolicita = usuarioSolicita,
                //         Eliminado = false,
                //         DetalleExamenes = new List<DetalleExamen>()
                //     };
                //     Console.WriteLine("🆕 Creando nuevo examen");
                // }

                // // 🔧 PROCESAR SOLO LOS EXAMENES NO PAGADOS (nuevos)
                // decimal totalPrecio = 0;
                // var nuevosDetalles = new List<DetalleExamen>();

                // foreach (var examenItem in model.ExamenesAgregados.Where(e => !e.Pagado))
                // {
                //     // Verificar si este detalle YA EXISTE en el examen
                //     var detalleExistente = examenActual.DetalleExamenes?
                //         .FirstOrDefault(d => d.ExamenLabClinicoId == examenItem.ExamenId &&
                //                            d.PrecioId == examenItem.PrecioId);

                //     if (detalleExistente != null)
                //     {
                //         Console.WriteLine($"⚠️ DetalleExamen ya existe para ExamenId: {examenItem.ExamenId}");
                //         continue; // Saltar si ya existe
                //     }

                //     var examenLabClinico = _laboratorioRepository.GetExamenLab((int)examenItem.ExamenId, false);
                //     if (examenLabClinico == null)
                //     {
                //         Console.WriteLine($"❌ ExamenLabClinicoId {examenItem.ExamenId} no encontrado.");
                //         continue;
                //     }

                //     var precioExamen = examenLabClinico.ExamenLabClinicosPrecios
                //         .FirstOrDefault(p => p.PrecioId == examenItem.PrecioId);

                //     if (precioExamen == null || precioExamen.PrecioValor <= 0)
                //     {
                //         Console.WriteLine($"❌ PrecioId {examenItem.PrecioId} no válido para el examen {examenItem.ExamenId}.");
                //         continue;
                //     }

                //     // ✅ CREAR NUEVO DETALLE solo para exámenes realmente nuevos
                //     var nuevoDetalle = new DetalleExamen
                //     {
                //         ExamenLabClinicoId = (int)examenItem.ExamenId,
                //         Cantidad = 1,
                //         PrecioId = precioExamen.PrecioId,
                //         PrecioValor = precioExamen.PrecioValor,
                //         Descuento = 0,
                //         Subtotal = precioExamen.PrecioValor,
                //         Total = precioExamen.PrecioValor,
                //         Observacion = ""
                //     };

                //     nuevosDetalles.Add(nuevoDetalle);
                //     totalPrecio += precioExamen.PrecioValor;

                //     Console.WriteLine($"➕ Agregando nuevo detalle para ExamenId: {examenItem.ExamenId}");
                // }

                // // Solo continuar si hay nuevos detalles para agregar
                // if (nuevosDetalles.Any())
                // {
                //     // Agregar los nuevos detalles al examen
                //     if (examenActual.DetalleExamenes == null)
                //         examenActual.DetalleExamenes = new List<DetalleExamen>();

                //     foreach (var detalle in nuevosDetalles)
                //     {
                //         examenActual.DetalleExamenes.Add(detalle);
                //     }

                //     // Guardar o actualizar el examen
                //     if (examenExistente == null)
                //     {
                //         _laboratorioRepository.Add(examenActual); // Nuevo examen
                //         Console.WriteLine("💾 Guardando nuevo examen");
                //     }
                //     else
                //     {
                //         _laboratorioRepository.Update(examenActual); // Actualizar existente
                //         Console.WriteLine("🔄 Actualizando examen existente");
                //     }
                // }
                // else
                // {
                //     Console.WriteLine("ℹ️ No hay nuevos detalles para agregar al examen");
                // }

                // // Marcar como pagados solo los nuevos exámenes procesados
                // foreach (var examen in model.ExamenesAgregados.Where(e => !e.Pagado))
                // {
                //     if (nuevosDetalles.Any(d => d.ExamenLabClinicoId == examen.ExamenId))
                //     {
                //         examen.Pagado = true;
                //         Console.WriteLine($"✅ Marcando como pagado ExamenId: {examen.ExamenId}");
                //     }
                // }

                #region Información dental
                paciente.DentalSangradoCepillar = model.DentalSangradoCepillar;
                paciente.DentalDolorFrio = model.DentalDolorFrio;
                paciente.DentalDolorPresionar = model.DentalDolorPresionar;
                paciente.DentalObjetosAtorados = model.DentalObjetosAtorados;
                paciente.DentalBocaSeca = model.DentalBocaSeca;
                paciente.DentalTratamientoPeriondal = model.DentalTratamientoPeriondal;
                paciente.DentalTratamientoOrtodoncia = model.DentalTratamientoOrtodoncia;
                paciente.DentalProblemasTratamientoDental = model.DentalProblemasTratamientoDental;
                paciente.DentalProblemasTratamientoDentalDescripcion = model.DentalProblemasTratamientoDentalDescripcion;
                paciente.DentalFluoradaAguaDomicilio = model.DentalFluoradaAguaDomicilio;
                paciente.DentalBebeAguaFiltrada = model.DentalBebeAguaFiltrada;
                paciente.DentalDolorOidos = model.DentalDolorOidos;
                paciente.DentalMolestiaRuidoAlto = model.DentalMolestiaRuidoAlto;
                paciente.DentalMolestiaRuidoAltoDescripcion = model.DentalMolestiaRuidoAltoDescripcion;
                paciente.DentalBrumismo = model.DentalBrumismo;
                paciente.DentalLesiones = model.DentalLesiones;
                paciente.DentalLesionesDescripcion = model.DentalLesionesDescripcion;
                paciente.DentalDentaduraPlacas = model.DentalDentaduraPlacas;
                paciente.DentalDentaduraPlacasDescripcion = model.DentalDentaduraPlacasDescripcion;
                paciente.DentalActividadesRecreacion = model.DentalActividadesRecreacion;
                paciente.DentalActividadesRecreacionDescripcion = model.DentalActividadesRecreacionDescripcion;
                paciente.DentalLesionesCabeza = model.DentalLesionesCabeza;
                paciente.DentalLesionesCabezaDescripcion = model.DentalLesionesCabezaDescripcion;

                #endregion

                #region PRESCRIPCION MEDICA - RECETA

                if (model.ElementosPrescripcion != null)
                {
                    // Buscar si ya existe una prescripción para esta consulta
                    var prescripcion = consultaEditada.Prescripciones
                        .FirstOrDefault() ?? new Prescripcion
                        {
                            CitasId = model.CitaId,
                            ConsultaId = consultaEditada.Id,
                        };

                    foreach (var elemento in model.ElementosPrescripcion)
                    {
                        if (!elemento.Eliminado)
                        {

                            if (!elemento.Pagado)
                            {
                                consultaEditada.EstadoPagoConsultaId = (int)EstadoPagoConsultaEnum.Pendiente;
                            }


                            var detalleExistente = prescripcion.DetallePrescripcion
                                .FirstOrDefault(d => d.ProductoId == elemento.ProductoId);

                            if (detalleExistente != null)
                            {
                                detalleExistente.Cantidad = elemento.Cantidad;
                                detalleExistente.ValorCubiertoSeguro = elemento.ValorCubiertoSeguro;
                                detalleExistente.ValorCopago = elemento.ValorCopago;
                                detalleExistente.Indications = elemento.ProductoIndicaciones;
                                detalleExistente.PrecioValor = elemento.PrecioValor ?? detalleExistente.PrecioValor;
                                prescripcion.NextDate = DateTime.Now;
                                continue;
                            }
                            else
                            {


                                var nuevoDetalle = new DetallePrescripcion
                                {
                                    Item = elemento.Item,
                                    Medicine = elemento.ProductoId == null
                                        ? elemento.ProductoNombre
                                        : null,
                                    Cantidad = elemento.Cantidad,
                                    PrecioId = elemento.ProductoPrecioId,
                                    UnidadMedidaVentaId = elemento.UnidadMedidaVentaId,
                                    ProductoId = elemento.ProductoId,
                                    PrecioValor = elemento.PrecioValor ?? 0,
                                    ValorCubiertoSeguro = elemento.ValorCubiertoSeguro,
                                    ValorCopago = elemento.ValorCopago,
                                    Indications = elemento.ProductoIndicaciones,
                                    Pagado = elemento.Pagado,
                                    Color = elemento.Color,
                                    FechaPrescripcion = DateTime.Now,
                                };

                                prescripcion.DetallePrescripcion.Add(nuevoDetalle);
                            }
                        }
                    }


                    if (!consultaEditada.Prescripciones.Contains(prescripcion))
                    {
                        consultaEditada.Prescripciones.Add(prescripcion);
                    }
                }

                #endregion


                #region Agenda automatica de proxima cita

                if (!consultaEditada.ProximaCitaAgendada && model.FechaProximaConsulta != null)
                {
                    var proximaCita = new Citas
                    {
                        PacienteId = model.PacienteId,
                        FechaInicio = Convert.ToDateTime(model.FechaProximaConsulta),
                        FechaFinal = Convert.ToDateTime(model.FechaProximaConsulta),
                        ContadorCitaAgendada = Convert.ToDateTime(model.FechaProximaConsulta),
                        Eliminado = false,
                        SucursalId = consultaEditada.Citas.SucursalId,
                        EspecialidadId = consultaEditada.Citas.EspecialidadId,
                        EmpleadoId = consultaEditada.Citas.EmpleadoId,
                        Finalizada = false,
                        EstadoTurno = "ACTIVO",
                        NivelPrioridadCita = consultaEditada.Citas.NivelPrioridadCita,
                        CitaTipoAtencion = consultaEditada.Citas.CitaTipoAtencion,
                        EstadoCita = "normal",
                        Bloqueada = false
                    };

                    _citasRepository.Add(proximaCita);
                    consultaEditada.ProximaCitaAgendada = true;
                }

                #endregion

                _consultasRepository.Update(consultaEditada);

                var ncita = _citasRepository.GetCita((int)model.CitaId);

                if (!ncita.Finalizada)
                {
                    // Contador de finalización: se setea solo una vez (evita sobrescribir)
                    if (ncita.ContadorCitaFinalizada == null)
                    {
                        ncita.ContadorCitaFinalizada = DateTime.UtcNow;
                        Console.WriteLine($"[FINALIZAR-CITA-EDIT] CitaId={ncita.Id} ContadorCitaFinalizada seteado a {ncita.ContadorCitaFinalizada:O}");
                    }

                    ncita.Finalizada = true;
                    Console.WriteLine($"[FINALIZAR-CITA-EDIT] CitaId={ncita.Id} Finalizada={ncita.Finalizada}");
                }


                ncita.EstadoTurno = model.EstadoTurno;

                _citasRepository.Update(ncita);

                #region OFTALMOLOGÍA
                try
                {
                    bool hayOftEdit =
                        !(string.IsNullOrWhiteSpace(model.Oft_HistoriaEnfermedadActual)
                          && string.IsNullOrWhiteSpace(model.Oft_PacienteMedicos)
                          && string.IsNullOrWhiteSpace(model.Oft_PacienteQuirurgicos)
                          && string.IsNullOrWhiteSpace(model.Oft_PacienteTraumaticos)
                          && string.IsNullOrWhiteSpace(model.Oft_PacienteAlergias)
                          && string.IsNullOrWhiteSpace(model.Oft_PacienteFamiliares)

                          // Datos objetivos
                          && string.IsNullOrWhiteSpace(model.Oft_AgudezaSC_Test)
                          && string.IsNullOrWhiteSpace(model.Oft_AgudezaSC_OD)
                          && string.IsNullOrWhiteSpace(model.Oft_AgudezaSC_OS)
                          && string.IsNullOrWhiteSpace(model.Oft_Contraste_OD)
                          && string.IsNullOrWhiteSpace(model.Oft_Contraste_OS)
                          && string.IsNullOrWhiteSpace(model.Oft_AVCerca_OD)
                          && string.IsNullOrWhiteSpace(model.Oft_AVCerca_OS)
                          && string.IsNullOrWhiteSpace(model.Oft_TestIshihara_OD)
                          && string.IsNullOrWhiteSpace(model.Oft_TestIshihara_OS)
                          && string.IsNullOrWhiteSpace(model.Oft_TestEstereopsis_OD)
                          && string.IsNullOrWhiteSpace(model.Oft_TestEstereopsis_OS)

                          // Lensometría
                          && model.Oft_Lensometria_OD_Esfera == null
                          && model.Oft_Lensometria_OD_Cilindro == null
                          && model.Oft_Lensometria_OD_Eje == null
                          && string.IsNullOrWhiteSpace(model.Oft_Lensometria_OD_Agudeza)
                          && model.Oft_Lensometria_OS_Esfera == null
                          && model.Oft_Lensometria_OS_Cilindro == null
                          && model.Oft_Lensometria_OS_Eje == null
                          && string.IsNullOrWhiteSpace(model.Oft_Lensometria_OS_Agudeza)

                          // Final
                          && model.Oft_Final_OD_Esfera == null
                          && model.Oft_Final_OD_Cilindro == null
                          && model.Oft_Final_OD_Eje == null
                          && string.IsNullOrWhiteSpace(model.Oft_Final_OD_Agudeza)
                          && model.Oft_Final_OS_Esfera == null
                          && model.Oft_Final_OS_Cilindro == null
                          && model.Oft_Final_OS_Eje == null
                          && string.IsNullOrWhiteSpace(model.Oft_Final_OS_Agudeza)
                          && model.Oft_Final_Adicion == null
                          && model.Oft_Final_DIP_mm == null

                          // Retinoscopía
                          && model.Oft_Retino_OD_Esfera == null
                          && model.Oft_Retino_OD_Cilindro == null
                          && model.Oft_Retino_OD_Eje == null
                          && model.Oft_Retino_OS_Esfera == null
                          && model.Oft_Retino_OS_Cilindro == null
                          && model.Oft_Retino_OS_Eje == null

                          // Tipo de lente
                          && string.IsNullOrWhiteSpace(model.Oft_TipoLente)
                          && string.IsNullOrWhiteSpace(model.Oft_LenteMaterialTratamiento)

                          // Inspección / LH / Oftalmoscopía
                          && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_MovExtraoculares_OD)
                          && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_MovExtraoculares_OS)
                          && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_Cejas_OD)
                          && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_Cejas_OS)
                          && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_ParpadosPestanas_OD)
                          && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_ParpadosPestanas_OS)
                          && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_ViaLagrimal_OD)
                          && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_ViaLagrimal_OS)
                          && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_Conjuntiva_OD)
                          && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_Conjuntiva_OS)
                          && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_CorneaEsclera_OD)
                          && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_CorneaEsclera_OS)
                          && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_CamaraAnteriorAngulo_OD)
                          && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_CamaraAnteriorAngulo_OS)
                          && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_IrisPupila_OD)
                          && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_IrisPupila_OS)
                          && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_Cristalino_OD)
                          && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_Cristalino_OS)
                          && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_BUT_OD)
                          && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_BUT_OS)
                          && model.Oft_Inspeccion_PresionIntraocular_OD == null
                          && model.Oft_Inspeccion_PresionIntraocular_OS == null
                          && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_Vitreo_OD)
                          && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_Vitreo_OS)
                          && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_NervioOptico_OD)
                          && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_NervioOptico_OS)
                          && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_Macula_OD)
                          && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_Macula_OS)
                          && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_Retina_OD)
                          && string.IsNullOrWhiteSpace(model.Oft_Inspeccion_Retina_OS)
                         //Impresión clínica y tratamiento
                         && string.IsNullOrWhiteSpace(model.Oft_HistoriaClinicaImpresionClinica)
                         && string.IsNullOrWhiteSpace(model.Oft_HistoriaClinicaComentario)
                        );
                    var oftDb = _oftRepo.GetConsulta((int)model.ConsultaId);
                    Func<decimal?, decimal?> sanearDecimal = valor =>
                    {
                        if (!valor.HasValue) return null;

                        // Si el valor llega anormalmente grande (ej. 125 en vez de 1.25)
                        // y va a romper la columna "numeric" en PostgreSQL (límite común de 99.99)
                        if (valor.Value > 99.99m || valor.Value < -99.99m)
                        {
                            // Restauramos los decimales que el framework omitió dividiendo entre 100
                            return valor.Value / 100m;
                        }

                        return valor;
                    };
                    if (oftDb != null)
                    {
                        // UPDATE
                        // Motivo/Antecedentes
                        oftDb.HistoriaEnfermedadActual = model.Oft_HistoriaEnfermedadActual;
                        oftDb.PacienteMedicos = model.Oft_PacienteMedicos;
                        oftDb.PacienteQuirurgicos = model.Oft_PacienteQuirurgicos;
                        oftDb.PacienteTraumaticos = model.Oft_PacienteTraumaticos;
                        oftDb.PacienteAlergias = model.Oft_PacienteAlergias;
                        oftDb.PacienteFamiliares = model.Oft_PacienteFamiliares;

                        // Objetivos
                        oftDb.AgudezaSC_Test = model.Oft_AgudezaSC_Test;
                        oftDb.AgudezaSC_OD = model.Oft_AgudezaSC_OD;
                        oftDb.AgudezaSC_OS = model.Oft_AgudezaSC_OS;
                        oftDb.Contraste_OD = model.Oft_Contraste_OD;
                        oftDb.Contraste_OS = model.Oft_Contraste_OS;
                        oftDb.AVCerca_OD = model.Oft_AVCerca_OD;
                        oftDb.AVCerca_OS = model.Oft_AVCerca_OS;
                        oftDb.TestIshihara_OD = model.Oft_TestIshihara_OD;
                        oftDb.TestIshihara_OS = model.Oft_TestIshihara_OS;
                        oftDb.TestEstereopsis_OD = model.Oft_TestEstereopsis_OD;
                        oftDb.TestEstereopsis_OS = model.Oft_TestEstereopsis_OS;

                        // Lensometría
                        oftDb.Lensometria_OD_Esfera = model.Oft_Lensometria_OD_Esfera;
                        oftDb.Lensometria_OD_Cilindro = model.Oft_Lensometria_OD_Cilindro;
                        oftDb.Lensometria_OD_Eje = model.Oft_Lensometria_OD_Eje;
                        oftDb.Lensometria_OD_Agudeza = model.Oft_Lensometria_OD_Agudeza;
                        oftDb.Lensometria_OS_Esfera = model.Oft_Lensometria_OS_Esfera;
                        oftDb.Lensometria_OS_Cilindro = model.Oft_Lensometria_OS_Cilindro;
                        oftDb.Lensometria_OS_Eje = model.Oft_Lensometria_OS_Eje;
                        oftDb.Lensometria_OS_Agudeza = model.Oft_Lensometria_OS_Agudeza;

                        // Final
                        // 2. Asignación directa a la base de datos USANDO el model
                        oftDb.Final_OD_Esfera = sanearDecimal(model.Oft_Final_OD_Esfera);
                        oftDb.Final_OD_Cilindro = sanearDecimal(model.Oft_Final_OD_Cilindro);
                        oftDb.Final_OD_Eje = model.Oft_Final_OD_Eje; // Int, no da problema
                        oftDb.Final_OD_Agudeza = model.Oft_Final_OD_Agudeza; // String, no da problema

                        oftDb.Final_OS_Esfera = sanearDecimal(model.Oft_Final_OS_Esfera);
                        oftDb.Final_OS_Cilindro = sanearDecimal(model.Oft_Final_OS_Cilindro);
                        oftDb.Final_OS_Eje = model.Oft_Final_OS_Eje;
                        oftDb.Final_OS_Agudeza = model.Oft_Final_OS_Agudeza;

                        oftDb.Final_Adicion = sanearDecimal(model.Oft_Final_Adicion);
                        oftDb.Final_DIP_mm = sanearDecimal(model.Oft_Final_DIP_mm);

                        // Retinoscopía
                        oftDb.Retino_OD_Esfera = model.Oft_Retino_OD_Esfera;
                        oftDb.Retino_OD_Cilindro = model.Oft_Retino_OD_Cilindro;
                        oftDb.Retino_OD_Eje = model.Oft_Retino_OD_Eje;
                        oftDb.Retino_OS_Esfera = model.Oft_Retino_OS_Esfera;
                        oftDb.Retino_OS_Cilindro = model.Oft_Retino_OS_Cilindro;
                        oftDb.Retino_OS_Eje = model.Oft_Retino_OS_Eje;

                        // Tipo de lente
                        oftDb.TipoLente = model.Oft_TipoLente;
                        oftDb.LenteMaterialTratamiento = model.Oft_LenteMaterialTratamiento;

                        // Inspección / LH / Oftalmoscopía
                        oftDb.Inspeccion_MovExtraoculares_OD = model.Oft_Inspeccion_MovExtraoculares_OD;
                        oftDb.Inspeccion_MovExtraoculares_OS = model.Oft_Inspeccion_MovExtraoculares_OS;
                        oftDb.Inspeccion_Cejas_OD = model.Oft_Inspeccion_Cejas_OD;
                        oftDb.Inspeccion_Cejas_OS = model.Oft_Inspeccion_Cejas_OS;
                        oftDb.Inspeccion_ParpadosPestanas_OD = model.Oft_Inspeccion_ParpadosPestanas_OD;
                        oftDb.Inspeccion_ParpadosPestanas_OS = model.Oft_Inspeccion_ParpadosPestanas_OS;
                        oftDb.Inspeccion_ViaLagrimal_OD = model.Oft_Inspeccion_ViaLagrimal_OD;
                        oftDb.Inspeccion_ViaLagrimal_OS = model.Oft_Inspeccion_ViaLagrimal_OS;
                        oftDb.Inspeccion_Conjuntiva_OD = model.Oft_Inspeccion_Conjuntiva_OD;
                        oftDb.Inspeccion_Conjuntiva_OS = model.Oft_Inspeccion_Conjuntiva_OS;
                        oftDb.Inspeccion_CorneaEsclera_OD = model.Oft_Inspeccion_CorneaEsclera_OD;
                        oftDb.Inspeccion_CorneaEsclera_OS = model.Oft_Inspeccion_CorneaEsclera_OS;
                        oftDb.Inspeccion_CamaraAnteriorAngulo_OD = model.Oft_Inspeccion_CamaraAnteriorAngulo_OD;
                        oftDb.Inspeccion_CamaraAnteriorAngulo_OS = model.Oft_Inspeccion_CamaraAnteriorAngulo_OS;
                        oftDb.Inspeccion_IrisPupila_OD = model.Oft_Inspeccion_IrisPupila_OD;
                        oftDb.Inspeccion_IrisPupila_OS = model.Oft_Inspeccion_IrisPupila_OS;
                        oftDb.Inspeccion_Cristalino_OD = model.Oft_Inspeccion_Cristalino_OD;
                        oftDb.Inspeccion_Cristalino_OS = model.Oft_Inspeccion_Cristalino_OS;
                        oftDb.Inspeccion_BUT_OD = model.Oft_Inspeccion_BUT_OD;
                        oftDb.Inspeccion_BUT_OS = model.Oft_Inspeccion_BUT_OS;
                        oftDb.Inspeccion_PresionIntraocular_OD = model.Oft_Inspeccion_PresionIntraocular_OD;
                        oftDb.Inspeccion_PresionIntraocular_OS = model.Oft_Inspeccion_PresionIntraocular_OS;
                        oftDb.Inspeccion_Vitreo_OD = model.Oft_Inspeccion_Vitreo_OD;
                        oftDb.Inspeccion_Vitreo_OS = model.Oft_Inspeccion_Vitreo_OS;
                        oftDb.Inspeccion_NervioOptico_OD = model.Oft_Inspeccion_NervioOptico_OD;
                        oftDb.Inspeccion_NervioOptico_OS = model.Oft_Inspeccion_NervioOptico_OS;
                        oftDb.Inspeccion_Macula_OD = model.Oft_Inspeccion_Macula_OD;
                        oftDb.Inspeccion_Macula_OS = model.Oft_Inspeccion_Macula_OS;
                        oftDb.Inspeccion_Retina_OD = model.Oft_Inspeccion_Retina_OD;
                        oftDb.Inspeccion_Retina_OS = model.Oft_Inspeccion_Retina_OS;
                        oftDb.HistoriaClinicaImpresionClinica = model.Oft_HistoriaClinicaImpresionClinica;
                        oftDb.HistoriaClinicaComentario = model.Oft_HistoriaClinicaComentario;
                        //oftDb.Fecha = DateTime.Now;

                        _oftRepo.Update(oftDb);
                    }
                    else if (hayOftEdit) // no existía y el user llenó algo -> INSERT
                    {
                        var nuevo = new ConsultasOftalmologia
                        {
                            ConsultaId = (int)model.ConsultaId,
                            PacienteId = (int)model.PacienteId,

                            // Motivo/Antecedentes
                            HistoriaEnfermedadActual = model.Oft_HistoriaEnfermedadActual,
                            PacienteMedicos = model.Oft_PacienteMedicos,
                            PacienteQuirurgicos = model.Oft_PacienteQuirurgicos,
                            PacienteTraumaticos = model.Oft_PacienteTraumaticos,
                            PacienteAlergias = model.Oft_PacienteAlergias,
                            PacienteFamiliares = model.Oft_PacienteFamiliares,

                            // Objetivos
                            AgudezaSC_Test = model.Oft_AgudezaSC_Test,
                            AgudezaSC_OD = model.Oft_AgudezaSC_OD,
                            AgudezaSC_OS = model.Oft_AgudezaSC_OS,
                            Contraste_OD = model.Oft_Contraste_OD,
                            Contraste_OS = model.Oft_Contraste_OS,
                            AVCerca_OD = model.Oft_AVCerca_OD,
                            AVCerca_OS = model.Oft_AVCerca_OS,
                            TestIshihara_OD = model.Oft_TestIshihara_OD,
                            TestIshihara_OS = model.Oft_TestIshihara_OS,
                            TestEstereopsis_OD = model.Oft_TestEstereopsis_OD,
                            TestEstereopsis_OS = model.Oft_TestEstereopsis_OS,

                            // Lensometría
                            Lensometria_OD_Esfera = model.Oft_Lensometria_OD_Esfera,
                            Lensometria_OD_Cilindro = model.Oft_Lensometria_OD_Cilindro,
                            Lensometria_OD_Eje = model.Oft_Lensometria_OD_Eje,
                            Lensometria_OD_Agudeza = model.Oft_Lensometria_OD_Agudeza,
                            Lensometria_OS_Esfera = model.Oft_Lensometria_OS_Esfera,
                            Lensometria_OS_Cilindro = model.Oft_Lensometria_OS_Cilindro,
                            Lensometria_OS_Eje = model.Oft_Lensometria_OS_Eje,
                            Lensometria_OS_Agudeza = model.Oft_Lensometria_OS_Agudeza,

                            // Final
                            Final_OD_Esfera = model.Oft_Final_OD_Esfera,
                            Final_OD_Cilindro = model.Oft_Final_OD_Cilindro,
                            Final_OD_Eje = model.Oft_Final_OD_Eje,
                            Final_OD_Agudeza = model.Oft_Final_OD_Agudeza,
                            Final_OS_Esfera = model.Oft_Final_OS_Esfera,
                            Final_OS_Cilindro = model.Oft_Final_OS_Cilindro,
                            Final_OS_Eje = model.Oft_Final_OS_Eje,
                            Final_OS_Agudeza = model.Oft_Final_OS_Agudeza,
                            Final_Adicion = model.Oft_Final_Adicion,
                            Final_DIP_mm = model.Oft_Final_DIP_mm,

                            // Retinoscopía
                            Retino_OD_Esfera = model.Oft_Retino_OD_Esfera,
                            Retino_OD_Cilindro = model.Oft_Retino_OD_Cilindro,
                            Retino_OD_Eje = model.Oft_Retino_OD_Eje,
                            Retino_OS_Esfera = model.Oft_Retino_OS_Esfera,
                            Retino_OS_Cilindro = model.Oft_Retino_OS_Cilindro,
                            Retino_OS_Eje = model.Oft_Retino_OS_Eje,

                            // Tipo de lente
                            TipoLente = model.Oft_TipoLente,
                            LenteMaterialTratamiento = model.Oft_LenteMaterialTratamiento,

                            // Inspección / LH / Oftalmoscopía
                            Inspeccion_MovExtraoculares_OD = model.Oft_Inspeccion_MovExtraoculares_OD,
                            Inspeccion_MovExtraoculares_OS = model.Oft_Inspeccion_MovExtraoculares_OS,
                            Inspeccion_Cejas_OD = model.Oft_Inspeccion_Cejas_OD,
                            Inspeccion_Cejas_OS = model.Oft_Inspeccion_Cejas_OS,
                            Inspeccion_ParpadosPestanas_OD = model.Oft_Inspeccion_ParpadosPestanas_OD,
                            Inspeccion_ParpadosPestanas_OS = model.Oft_Inspeccion_ParpadosPestanas_OS,
                            Inspeccion_ViaLagrimal_OD = model.Oft_Inspeccion_ViaLagrimal_OD,
                            Inspeccion_ViaLagrimal_OS = model.Oft_Inspeccion_ViaLagrimal_OS,
                            Inspeccion_Conjuntiva_OD = model.Oft_Inspeccion_Conjuntiva_OD,
                            Inspeccion_Conjuntiva_OS = model.Oft_Inspeccion_Conjuntiva_OS,
                            Inspeccion_CorneaEsclera_OD = model.Oft_Inspeccion_CorneaEsclera_OD,
                            Inspeccion_CorneaEsclera_OS = model.Oft_Inspeccion_CorneaEsclera_OS,
                            Inspeccion_CamaraAnteriorAngulo_OD = model.Oft_Inspeccion_CamaraAnteriorAngulo_OD,
                            Inspeccion_CamaraAnteriorAngulo_OS = model.Oft_Inspeccion_CamaraAnteriorAngulo_OS,
                            Inspeccion_IrisPupila_OD = model.Oft_Inspeccion_IrisPupila_OD,
                            Inspeccion_IrisPupila_OS = model.Oft_Inspeccion_IrisPupila_OS,
                            Inspeccion_Cristalino_OD = model.Oft_Inspeccion_Cristalino_OD,
                            Inspeccion_Cristalino_OS = model.Oft_Inspeccion_Cristalino_OS,
                            Inspeccion_BUT_OD = model.Oft_Inspeccion_BUT_OD,
                            Inspeccion_BUT_OS = model.Oft_Inspeccion_BUT_OS,
                            Inspeccion_PresionIntraocular_OD = model.Oft_Inspeccion_PresionIntraocular_OD,
                            Inspeccion_PresionIntraocular_OS = model.Oft_Inspeccion_PresionIntraocular_OS,
                            Inspeccion_Vitreo_OD = model.Oft_Inspeccion_Vitreo_OD,
                            Inspeccion_Vitreo_OS = model.Oft_Inspeccion_Vitreo_OS,
                            Inspeccion_NervioOptico_OD = model.Oft_Inspeccion_NervioOptico_OD,
                            Inspeccion_NervioOptico_OS = model.Oft_Inspeccion_NervioOptico_OS,
                            Inspeccion_Macula_OD = model.Oft_Inspeccion_Macula_OD,
                            Inspeccion_Macula_OS = model.Oft_Inspeccion_Macula_OS,
                            Inspeccion_Retina_OD = model.Oft_Inspeccion_Retina_OD,
                            Inspeccion_Retina_OS = model.Oft_Inspeccion_Retina_OS,
                            HistoriaClinicaImpresionClinica = model.Oft_HistoriaClinicaImpresionClinica,
                            HistoriaClinicaComentario = model.Oft_HistoriaClinicaComentario,
                            Fecha = DateTime.Now
                        };

                        _oftRepo.Add(nuevo);
                    }
                }
                catch (Exception ex)
                {
                    // Si hay un fallo puntual, no se cae el flujo de edición
                }
                #endregion

                #region PODOLOGÍA

                try
                {
                    bool hayPodEdit =
                        !(
                            // 1) Antecedentes
                            (model.Pod_Enfermedades == null || model.Pod_Enfermedades.Length == 0)
                            && string.IsNullOrWhiteSpace(model.Pod_Enfermedades_Otros)
                            && string.IsNullOrWhiteSpace(model.Pod_Medicamentos)
                            && string.IsNullOrWhiteSpace(model.Pod_PresionArterial)

                            // 2) Examen del pie
                            && string.IsNullOrWhiteSpace(model.Pod_Pulso_Pedio)
                            && string.IsNullOrWhiteSpace(model.Pod_Pulso_TibialPosterior)
                            && string.IsNullOrWhiteSpace(model.Pod_Pulso_Popliteo)
                            && string.IsNullOrWhiteSpace(model.Pod_TemperaturaPie)
                            && model.Pod_ProblemasCirculatorios == null
                            && string.IsNullOrWhiteSpace(model.Pod_EstadoPiel)
                            && string.IsNullOrWhiteSpace(model.Pod_ObservacionesExamen)

                            // 3) Tratamiento
                            && (model.Pod_Procedimientos == null || model.Pod_Procedimientos.Length == 0)
                            && string.IsNullOrWhiteSpace(model.Pod_OtrosProcedimientos)
                            && string.IsNullOrWhiteSpace(model.Pod_ObservacionesTratamiento)

                            // 4) Final
                            && string.IsNullOrWhiteSpace(model.Pod_Indicaciones)
                            && model.Pod_PesoKg == null
                            && model.Pod_EstaturaM == null
                            && model.Pod_FechaAtencion == null
                            && string.IsNullOrWhiteSpace(model.Pod_Profesional)
                        );


                    var podDb = _podRepo.GetConsulta((int)model.ConsultaId);

                    if (podDb != null)
                    {
                        // UPDATE
                        // 1) Antecedentes
                        podDb.Enfermedades = model.Pod_Enfermedades ?? Array.Empty<string>();
                        podDb.Enfermedades_Otros = model.Pod_Enfermedades_Otros;
                        podDb.Medicamentos = model.Pod_Medicamentos;
                        podDb.PresionArterial = model.Pod_PresionArterial;

                        // 2) Examen del pie
                        podDb.Pulso_Pedio = model.Pod_Pulso_Pedio;
                        podDb.Pulso_TibialPosterior = model.Pod_Pulso_TibialPosterior;
                        podDb.Pulso_Popliteo = model.Pod_Pulso_Popliteo;
                        podDb.TemperaturaPie = model.Pod_TemperaturaPie;
                        podDb.ProblemasCirculatorios = model.Pod_ProblemasCirculatorios;
                        podDb.EstadoPiel = model.Pod_EstadoPiel;
                        podDb.ObservacionesExamen = model.Pod_ObservacionesExamen;

                        // 3) Tratamiento
                        podDb.Procedimientos = model.Pod_Procedimientos ?? Array.Empty<string>();
                        podDb.OtrosProcedimientos = model.Pod_OtrosProcedimientos;
                        podDb.ObservacionesTratamiento = model.Pod_ObservacionesTratamiento;

                        // 4) Final
                        podDb.Indicaciones = model.Pod_Indicaciones;
                        podDb.PesoKg = model.Pod_PesoKg;
                        podDb.EstaturaM = model.Pod_EstaturaM;
                        podDb.FechaAtencion = model.Pod_FechaAtencion;
                        podDb.Profesional = model.Pod_Profesional;
                        //podDb.Fecha = DateTime.Now;

                        _podRepo.Update(podDb);
                    }
                    else if (hayPodEdit && model.PacienteId.HasValue)
                    {
                        // INSERT
                        var nuevo = new ConsultasPodologia
                        {
                            ConsultaId = (int)model.ConsultaId,
                            PacienteId = (int)model.PacienteId,

                            // 1) Antecedentes
                            Enfermedades = model.Pod_Enfermedades ?? Array.Empty<string>(),
                            Enfermedades_Otros = model.Pod_Enfermedades_Otros,
                            Medicamentos = model.Pod_Medicamentos,
                            PresionArterial = model.Pod_PresionArterial,

                            // 2) Examen del pie
                            Pulso_Pedio = model.Pod_Pulso_Pedio,
                            Pulso_TibialPosterior = model.Pod_Pulso_TibialPosterior,
                            Pulso_Popliteo = model.Pod_Pulso_Popliteo,
                            TemperaturaPie = model.Pod_TemperaturaPie,
                            ProblemasCirculatorios = model.Pod_ProblemasCirculatorios,
                            EstadoPiel = model.Pod_EstadoPiel,
                            ObservacionesExamen = model.Pod_ObservacionesExamen,

                            // 3) Tratamiento
                            Procedimientos = model.Pod_Procedimientos ?? Array.Empty<string>(),
                            OtrosProcedimientos = model.Pod_OtrosProcedimientos,
                            ObservacionesTratamiento = model.Pod_ObservacionesTratamiento,

                            // 4) Final
                            Indicaciones = model.Pod_Indicaciones,
                            PesoKg = model.Pod_PesoKg,
                            EstaturaM = model.Pod_EstaturaM,
                            FechaAtencion = model.Pod_FechaAtencion,
                            Profesional = model.Pod_Profesional,
                            Fecha = DateTime.Now,
                        };

                        _podRepo.Add(nuevo);
                    }
                }
                catch (Exception ex)
                {
                    // Si hay un fallo puntual, no se cae el flujo de edición
                }

                #endregion

                #region HISTORIA CLÍNICA DE ENFERMERÍA
                try
                {
                    bool hayEnfEdit =
                        !(string.IsNullOrWhiteSpace(model.Hce_TipoConsulta)
                          // 2) Motivo
                          && string.IsNullOrWhiteSpace(model.Hce_MotivoConsulta)

                          // 3) Antecedentes (nueva estructura)
                          && string.IsNullOrWhiteSpace(model.Hce_AntecedentesPatologicos)
                          && string.IsNullOrWhiteSpace(model.Hce_AntecedentesQuirurgicos)
                          && string.IsNullOrWhiteSpace(model.Hce_AntecedentesTraumaticos)
                          && string.IsNullOrWhiteSpace(model.Hce_Hospitalizaciones)
                          && string.IsNullOrWhiteSpace(model.Hce_Alergias)
                          && string.IsNullOrWhiteSpace(model.Hce_AntecedentesFamiliares)

                          // 4) Hábitos (texto)
                          && string.IsNullOrWhiteSpace(model.Hce_HabitoAlimentacion)
                          && string.IsNullOrWhiteSpace(model.Hce_ActividadFisica)
                          && string.IsNullOrWhiteSpace(model.Hce_HabitoAlcoholTexto)
                          && string.IsNullOrWhiteSpace(model.Hce_HabitoTabacoTexto)
                          && string.IsNullOrWhiteSpace(model.Hce_OtrosHabitos)

                          // 5) Signos vitales y antropometría
                          && string.IsNullOrWhiteSpace(model.Hce_PresionArterialTxt)
                          && !model.Hce_FC.HasValue
                          && !model.Hce_FR.HasValue
                          && !model.Hce_TemperaturaC.HasValue
                          && !model.Hce_SPO2.HasValue
                          && !model.Hce_PesoKg.HasValue
                          && !model.Hce_TallaM.HasValue
                          && !model.Hce_IMC.HasValue

                          // 6) Exploración por aparatos y sistemas
                          && string.IsNullOrWhiteSpace(model.Hce_CabezaCuello)
                          && string.IsNullOrWhiteSpace(model.Hce_ToraxPulmones)
                          && string.IsNullOrWhiteSpace(model.Hce_Corazon)
                          && string.IsNullOrWhiteSpace(model.Hce_Abdomen)
                          && string.IsNullOrWhiteSpace(model.Hce_Extremidades)
                          && string.IsNullOrWhiteSpace(model.Hce_SistemaNeurologico)
                          && string.IsNullOrWhiteSpace(model.Hce_PielAnexos)

                          // 8) Valoración de enfermería
                          && string.IsNullOrWhiteSpace(model.Hce_ValConcienciaOrientacion)
                          && string.IsNullOrWhiteSpace(model.Hce_ValEstadoNutricional)
                          && string.IsNullOrWhiteSpace(model.Hce_ValEliminacion)
                          && string.IsNullOrWhiteSpace(model.Hce_ValSuenoDescanso)
                          && string.IsNullOrWhiteSpace(model.Hce_ValActividadMovilidad)
                          && string.IsNullOrWhiteSpace(model.Hce_ValAutonomia)

                          // 9) Laboratorios
                          && string.IsNullOrWhiteSpace(model.Hce_Laboratorios)

                          // 10) Diagnóstico de enfermería
                          && string.IsNullOrWhiteSpace(model.Hce_DiagnosticoEnfermeria)

                          // 11) Plan de cuidados / Intervenciones
                          && string.IsNullOrWhiteSpace(model.Hce_AccionesRealizadas)
                          && string.IsNullOrWhiteSpace(model.Hce_MedicamentosAdministrados)
                          && string.IsNullOrWhiteSpace(model.Hce_Tratamiento)

                          // 12) Seguimiento / Evolución / Cita
                          && string.IsNullOrWhiteSpace(model.Hce_Seguimiento)
                        );

                    var enfDb = _enfRepo.GetConsulta((int)model.ConsultaId);

                    if (enfDb != null)
                    {
                        // UPDATE
                        // 1) Tipo de consulta
                        enfDb.TipoConsulta = model.Hce_TipoConsulta;

                        // 2) Motivo
                        enfDb.MotivoConsulta = model.Hce_MotivoConsulta;

                        // 3) Antecedentes
                        enfDb.AntecedentesPatologicos = model.Hce_AntecedentesPatologicos;
                        enfDb.AntecedentesQuirurgicos = model.Hce_AntecedentesQuirurgicos;
                        enfDb.AntecedentesTraumaticos = model.Hce_AntecedentesTraumaticos;
                        enfDb.Hospitalizaciones = model.Hce_Hospitalizaciones;
                        enfDb.Alergias = model.Hce_Alergias;
                        enfDb.AntecedentesFamiliares = model.Hce_AntecedentesFamiliares;

                        // 4) Hábitos
                        enfDb.HabitoAlimentacion = model.Hce_HabitoAlimentacion;
                        enfDb.ActividadFisica = model.Hce_ActividadFisica;
                        enfDb.HabitoAlcoholTexto = model.Hce_HabitoAlcoholTexto;
                        enfDb.HabitoTabacoTexto = model.Hce_HabitoTabacoTexto;
                        enfDb.OtrosHabitos = model.Hce_OtrosHabitos;

                        // 5) Signos vitales y antropometría
                        enfDb.PresionArterialTxt = model.Hce_PresionArterialTxt;
                        enfDb.FC = model.Hce_FC;
                        enfDb.FR = model.Hce_FR;
                        enfDb.TemperaturaC = model.Hce_TemperaturaC;
                        enfDb.SPO2 = model.Hce_SPO2;
                        enfDb.PesoKg = model.Hce_PesoKg;
                        enfDb.TallaM = model.Hce_TallaM;
                        enfDb.IMC = model.Hce_IMC;

                        // 6) Exploración por aparatos y sistemas
                        enfDb.CabezaCuello = model.Hce_CabezaCuello;
                        enfDb.ToraxPulmones = model.Hce_ToraxPulmones;
                        enfDb.Corazon = model.Hce_Corazon;
                        enfDb.Abdomen = model.Hce_Abdomen;
                        enfDb.Extremidades = model.Hce_Extremidades;
                        enfDb.SistemaNeurologico = model.Hce_SistemaNeurologico;
                        enfDb.PielAnexos = model.Hce_PielAnexos;

                        // 8) Valoración de enfermería
                        enfDb.ValConcienciaOrientacion = model.Hce_ValConcienciaOrientacion;
                        enfDb.ValEstadoNutricional = model.Hce_ValEstadoNutricional;
                        enfDb.ValEliminacion = model.Hce_ValEliminacion;
                        enfDb.ValSuenoDescanso = model.Hce_ValSuenoDescanso;
                        enfDb.ValActividadMovilidad = model.Hce_ValActividadMovilidad;
                        enfDb.ValAutonomia = model.Hce_ValAutonomia;

                        // 9) Laboratorios
                        enfDb.Laboratorios = model.Hce_Laboratorios;

                        // 10) Diagnóstico de enfermería
                        enfDb.DiagnosticoEnfermeria = model.Hce_DiagnosticoEnfermeria;

                        // 11) Plan de cuidados / Intervenciones
                        enfDb.AccionesRealizadas = model.Hce_AccionesRealizadas;
                        enfDb.MedicamentosAdministrados = model.Hce_MedicamentosAdministrados;
                        enfDb.Tratamiento = model.Hce_Tratamiento;

                        // 12) Seguimiento
                        enfDb.Seguimiento = model.Hce_Seguimiento;
                        //enfDb.Fecha = DateTime.Now;

                        _enfRepo.Update(enfDb);
                    }
                    else if (hayEnfEdit) // no existía y el user llenó algo -> INSERT
                    {
                        var nuevo = new ConsultasHistoriaClinicaEnfermeria
                        {
                            ConsultaId = (int)model.ConsultaId,
                            PacienteId = (int)model.PacienteId,

                            // 1) Tipo de consulta
                            TipoConsulta = model.Hce_TipoConsulta,

                            // 2) Motivo
                            MotivoConsulta = model.Hce_MotivoConsulta,

                            // 3) Antecedentes
                            AntecedentesPatologicos = model.Hce_AntecedentesPatologicos,
                            AntecedentesQuirurgicos = model.Hce_AntecedentesQuirurgicos,
                            AntecedentesTraumaticos = model.Hce_AntecedentesTraumaticos,
                            Hospitalizaciones = model.Hce_Hospitalizaciones,
                            Alergias = model.Hce_Alergias,
                            AntecedentesFamiliares = model.Hce_AntecedentesFamiliares,

                            // 4) Hábitos
                            HabitoAlimentacion = model.Hce_HabitoAlimentacion,
                            ActividadFisica = model.Hce_ActividadFisica,
                            HabitoAlcoholTexto = model.Hce_HabitoAlcoholTexto,
                            HabitoTabacoTexto = model.Hce_HabitoTabacoTexto,
                            OtrosHabitos = model.Hce_OtrosHabitos,

                            // 5) Signos vitales y antropometría
                            PresionArterialTxt = model.Hce_PresionArterialTxt,
                            FC = model.Hce_FC,
                            FR = model.Hce_FR,
                            TemperaturaC = model.Hce_TemperaturaC,
                            SPO2 = model.Hce_SPO2,
                            PesoKg = model.Hce_PesoKg,
                            TallaM = model.Hce_TallaM,
                            IMC = model.Hce_IMC,

                            // 6) Exploración por aparatos y sistemas
                            CabezaCuello = model.Hce_CabezaCuello,
                            ToraxPulmones = model.Hce_ToraxPulmones,
                            Corazon = model.Hce_Corazon,
                            Abdomen = model.Hce_Abdomen,
                            Extremidades = model.Hce_Extremidades,
                            SistemaNeurologico = model.Hce_SistemaNeurologico,
                            PielAnexos = model.Hce_PielAnexos,

                            // 8) Valoración de enfermería
                            ValConcienciaOrientacion = model.Hce_ValConcienciaOrientacion,
                            ValEstadoNutricional = model.Hce_ValEstadoNutricional,
                            ValEliminacion = model.Hce_ValEliminacion,
                            ValSuenoDescanso = model.Hce_ValSuenoDescanso,
                            ValActividadMovilidad = model.Hce_ValActividadMovilidad,
                            ValAutonomia = model.Hce_ValAutonomia,

                            // 9) Laboratorios
                            Laboratorios = model.Hce_Laboratorios,

                            // 10) Diagnóstico de enfermería
                            DiagnosticoEnfermeria = model.Hce_DiagnosticoEnfermeria,

                            // 11) Plan de cuidados / Intervenciones
                            AccionesRealizadas = model.Hce_AccionesRealizadas,
                            MedicamentosAdministrados = model.Hce_MedicamentosAdministrados,
                            Tratamiento = model.Hce_Tratamiento,
                            // 12) Seguimiento
                            Seguimiento = model.Hce_Seguimiento,

                            Fecha = DateTime.Now,
                        };

                        _enfRepo.Add(nuevo);
                    }
                }
                catch (Exception ex)
                {
                    // Si hay un fallo puntual, no se cae el flujo de edición
                }
                #endregion

                #region VALORACION INICIAL ENFERMERIA
                try
                {
                    bool hayVeEdit =
                        !(
                            // 1) Datos de valoración inicial
                            string.IsNullOrWhiteSpace(model.Ve_Motivo) &&
                            string.IsNullOrWhiteSpace(model.Ve_DiagnosticoMedico) &&
                            string.IsNullOrWhiteSpace(model.Ve_Labs) &&

                            // 2) Medio
                            (model.Ve_Medio == null || model.Ve_Medio.Length == 0) &&

                            // 3) Oxigenación y Circulación
                            (model.Ve_Resp == null || model.Ve_Resp.Length == 0) &&
                            (model.Ve_Circ == null || model.Ve_Circ.Length == 0) &&

                            // 4) Nutrición
                            (model.Ve_Nutricion == null || model.Ve_Nutricion.Length == 0) &&
                            string.IsNullOrWhiteSpace(model.Ve_NutricionObs) &&

                            // 5) Eliminación
                            (model.Ve_Urinario == null || model.Ve_Urinario.Length == 0) &&
                            (model.Ve_Intestinal == null || model.Ve_Intestinal.Length == 0) &&

                            // 6) Movilización / Conciencia
                            (model.Ve_Mov == null || model.Ve_Mov.Length == 0) &&
                            (model.Ve_Conciencia == null || model.Ve_Conciencia.Length == 0) &&

                            // 7) Autocuidado / Reposo
                            (model.Ve_Sueno == null || model.Ve_Sueno.Length == 0) &&
                            string.IsNullOrWhiteSpace(model.Ve_Vestirse) &&
                            string.IsNullOrWhiteSpace(model.Ve_Higiene) &&
                            (model.Ve_Piel == null || model.Ve_Piel.Length == 0) &&
                            string.IsNullOrWhiteSpace(model.Ve_PielUbicacion) &&

                            // 8) Comunicación
                            (model.Ve_Lenguaje == null || model.Ve_Lenguaje.Length == 0) &&
                            (model.Ve_Vision == null || model.Ve_Vision.Length == 0) &&
                            (model.Ve_Oido == null || model.Ve_Oido.Length == 0) &&

                            // 9) Seguridad / Psicosocial
                            (model.Ve_Seg == null || model.Ve_Seg.Length == 0) &&
                            string.IsNullOrWhiteSpace(model.Ve_Religiosos) &&
                            string.IsNullOrWhiteSpace(model.Ve_CreenciasObservaciones) &&
                            string.IsNullOrWhiteSpace(model.Ve_ConoceMotivo) &&
                            string.IsNullOrWhiteSpace(model.Ve_NecesitaInfo) &&

                            // 10) Medicación / Plan
                            string.IsNullOrWhiteSpace(model.Ve_MedicacionActual) &&
                            string.IsNullOrWhiteSpace(model.Ve_PlanTerapeutico)
                        );

                    var veDb = _veRepo.GetConsulta((int)model.ConsultaId);

                    if (veDb != null)
                    {
                        // UPDATE
                        veDb.Motivo = model.Ve_Motivo;
                        veDb.DiagnosticoMedico = model.Ve_DiagnosticoMedico;
                        veDb.Labs = model.Ve_Labs;

                        veDb.Medio = model.Ve_Medio ?? Array.Empty<string>();

                        veDb.Resp = model.Ve_Resp ?? Array.Empty<string>();
                        veDb.Circ = model.Ve_Circ ?? Array.Empty<string>();

                        veDb.Nutricion = model.Ve_Nutricion ?? Array.Empty<string>();
                        veDb.NutricionObs = model.Ve_NutricionObs;

                        veDb.Urinario = model.Ve_Urinario ?? Array.Empty<string>();
                        veDb.Intestinal = model.Ve_Intestinal ?? Array.Empty<string>();

                        veDb.Mov = model.Ve_Mov ?? Array.Empty<string>();
                        veDb.Conciencia = model.Ve_Conciencia ?? Array.Empty<string>();

                        veDb.Sueno = model.Ve_Sueno ?? Array.Empty<string>();
                        veDb.Vestirse = model.Ve_Vestirse;
                        veDb.Higiene = model.Ve_Higiene;
                        veDb.Piel = model.Ve_Piel ?? Array.Empty<string>();
                        veDb.PielUbicacion = model.Ve_PielUbicacion;

                        veDb.Lenguaje = model.Ve_Lenguaje ?? Array.Empty<string>();
                        veDb.Vision = model.Ve_Vision ?? Array.Empty<string>();
                        veDb.Oido = model.Ve_Oido ?? Array.Empty<string>();

                        veDb.Seg = model.Ve_Seg ?? Array.Empty<string>();
                        veDb.Religiosos = model.Ve_Religiosos;
                        veDb.CreenciasObservaciones = model.Ve_CreenciasObservaciones;
                        veDb.ConoceMotivo = model.Ve_ConoceMotivo;
                        veDb.NecesitaInfo = model.Ve_NecesitaInfo;

                        veDb.MedicacionActual = model.Ve_MedicacionActual;
                        veDb.PlanTerapeutico = model.Ve_PlanTerapeutico;
                        //veDb.Fecha = DateTime.Now;

                        _veRepo.Update(veDb);
                    }
                    else if (hayVeEdit && model.PacienteId.HasValue)
                    {
                        // INSERT
                        var nuevo = new ConsultasValoracionInicialEnfermeria
                        {
                            ConsultaId = (int)model.ConsultaId,
                            PacienteId = (int)model.PacienteId,

                            Motivo = model.Ve_Motivo,
                            DiagnosticoMedico = model.Ve_DiagnosticoMedico,
                            Labs = model.Ve_Labs,

                            Medio = model.Ve_Medio ?? Array.Empty<string>(),

                            Resp = model.Ve_Resp ?? Array.Empty<string>(),
                            Circ = model.Ve_Circ ?? Array.Empty<string>(),

                            Nutricion = model.Ve_Nutricion ?? Array.Empty<string>(),
                            NutricionObs = model.Ve_NutricionObs,

                            Urinario = model.Ve_Urinario ?? Array.Empty<string>(),
                            Intestinal = model.Ve_Intestinal ?? Array.Empty<string>(),

                            Mov = model.Ve_Mov ?? Array.Empty<string>(),
                            Conciencia = model.Ve_Conciencia ?? Array.Empty<string>(),

                            Sueno = model.Ve_Sueno ?? Array.Empty<string>(),
                            Vestirse = model.Ve_Vestirse,
                            Higiene = model.Ve_Higiene,
                            Piel = model.Ve_Piel ?? Array.Empty<string>(),
                            PielUbicacion = model.Ve_PielUbicacion,

                            Lenguaje = model.Ve_Lenguaje ?? Array.Empty<string>(),
                            Vision = model.Ve_Vision ?? Array.Empty<string>(),
                            Oido = model.Ve_Oido ?? Array.Empty<string>(),

                            Seg = model.Ve_Seg ?? Array.Empty<string>(),
                            Religiosos = model.Ve_Religiosos,
                            CreenciasObservaciones = model.Ve_CreenciasObservaciones,
                            ConoceMotivo = model.Ve_ConoceMotivo,
                            NecesitaInfo = model.Ve_NecesitaInfo,

                            MedicacionActual = model.Ve_MedicacionActual,
                            PlanTerapeutico = model.Ve_PlanTerapeutico,

                            Fecha = DateTime.Now,
                        };

                        _veRepo.Add(nuevo);
                    }
                }
                catch (Exception ex)
                {
                    // Si hay un fallo puntual, no se cae el flujo de edición
                }

                #endregion

                #region SUEROTERAPIA

                // ==== SUEROTERAPIA (upsert al editar) ====
                // No forzamos que exista; si no hay datos, no creamos nada nuevo.
                Console.WriteLine($"[SUERO][EDITAR] ConsultaId: {model.ConsultaId} | Método EditarConsulta");

                try
                {
                    bool haySueroEdit =
                        !(
                            // 1) Datos de valoración inicial
                            string.IsNullOrWhiteSpace(model.Suero_Motivo) &&
                            string.IsNullOrWhiteSpace(model.Suero_DiagnosticoMedico) &&
                            string.IsNullOrWhiteSpace(model.Suero_Labs) &&

                            // 2) Medio
                            (model.Suero_Medio == null || model.Suero_Medio.Length == 0) &&

                            // 3) Oxigenación y Circulación
                            (model.Suero_Resp == null || model.Suero_Resp.Length == 0) &&
                            (model.Suero_Circ == null || model.Suero_Circ.Length == 0) &&

                            // 4) Nutrición
                            (model.Suero_Nutricion == null || model.Suero_Nutricion.Length == 0) &&
                            string.IsNullOrWhiteSpace(model.Suero_NutricionObs) &&

                            // 5) Plan terapéutico
                            string.IsNullOrWhiteSpace(model.Suero_PlanTerapeutico)
                        );

                    // ====== TRAZAS DETALLADAS DE ENTRADA ======
                    Console.WriteLine("===== [SUERO][EDITAR] TRACE IN =====");
                    Console.WriteLine($"ConsultaId: {model.ConsultaId}");
                    Console.WriteLine($"PacienteId: {(model.PacienteId.HasValue ? model.PacienteId.Value.ToString() : "<null>")}");
                    Console.WriteLine($"haySueroEdit: {haySueroEdit}");

                    // 1) Inicial
                    Console.WriteLine($"Suero_Motivo: '{model.Suero_Motivo}'");
                    Console.WriteLine($"Suero_DiagnosticoMedico: '{model.Suero_DiagnosticoMedico}'");
                    Console.WriteLine($"Suero_Labs: '{model.Suero_Labs}'");

                    // 2) Medio
                    Console.WriteLine($"Suero_Medio: [{string.Join(",", model.Suero_Medio ?? Array.Empty<string>())}]");

                    // 3) Oxigenación y Circulación
                    Console.WriteLine($"Suero_Resp: [{string.Join(",", model.Suero_Resp ?? Array.Empty<string>())}]");
                    Console.WriteLine($"Suero_Circ: [{string.Join(",", model.Suero_Circ ?? Array.Empty<string>())}]");

                    // 4) Nutrición
                    Console.WriteLine($"Suero_Nutricion: [{string.Join(",", model.Suero_Nutricion ?? Array.Empty<string>())}]");
                    Console.WriteLine($"Suero_NutricionObs: '{model.Suero_NutricionObs}'");

                    // 5) Plan
                    Console.WriteLine($"Suero_PlanTerapeutico: '{model.Suero_PlanTerapeutico}'");
                    Console.WriteLine("===== [/SUERO][EDITAR] TRACE IN =====");

                    var sueroDb = _sueroRepo.GetConsulta((int)model.ConsultaId);
                    Console.WriteLine($"[SUERO] Datos existentes en BD: {(sueroDb != null ? "Sí" : "No")}");

                    if (sueroDb != null)
                    {
                        // UPDATE
                        Console.WriteLine("[SUERO] UPDATE existente");

                        sueroDb.PacienteId = model.PacienteId ?? sueroDb.PacienteId;

                        // 1) Inicial
                        sueroDb.Motivo = model.Suero_Motivo;
                        sueroDb.DiagnosticoMedico = model.Suero_DiagnosticoMedico;
                        sueroDb.Labs = model.Suero_Labs;

                        // 2) Medio
                        sueroDb.Medio = model.Suero_Medio ?? Array.Empty<string>();

                        // 3) Oxigenación / Circulación
                        sueroDb.Resp = model.Suero_Resp ?? Array.Empty<string>();
                        sueroDb.Circ = model.Suero_Circ ?? Array.Empty<string>();

                        // 4) Nutrición
                        sueroDb.Nutricion = model.Suero_Nutricion ?? Array.Empty<string>();
                        sueroDb.NutricionObs = model.Suero_NutricionObs;

                        // 5) Plan
                        sueroDb.PlanTerapeutico = model.Suero_PlanTerapeutico;

                        //sueroDb.Fecha = DateTime.Now;

                        _sueroRepo.Update(sueroDb);
                        Console.WriteLine("[SUERO] UPDATE completado");
                    }
                    else if (haySueroEdit && model.PacienteId.HasValue)
                    {
                        // INSERT
                        Console.WriteLine("[SUERO] INSERT nuevo");

                        var nuevo = new ConsultasSueroterapia
                        {
                            ConsultaId = (int)model.ConsultaId,
                            PacienteId = (int)model.PacienteId,

                            // 1) Inicial
                            Motivo = model.Suero_Motivo,
                            DiagnosticoMedico = model.Suero_DiagnosticoMedico,
                            Labs = model.Suero_Labs,

                            // 2) Medio
                            Medio = model.Suero_Medio ?? Array.Empty<string>(),

                            // 3) Oxigenación / Circulación
                            Resp = model.Suero_Resp ?? Array.Empty<string>(),
                            Circ = model.Suero_Circ ?? Array.Empty<string>(),

                            // 4) Nutrición
                            Nutricion = model.Suero_Nutricion ?? Array.Empty<string>(),
                            NutricionObs = model.Suero_NutricionObs,

                            // 5) Plan
                            PlanTerapeutico = model.Suero_PlanTerapeutico,

                            Fecha = DateTime.Now,
                        };

                        _sueroRepo.Add(nuevo);
                        Console.WriteLine("[SUERO] INSERT completado");
                    }
                }
                catch (Exception ex)
                {
                    // Si hay un fallo puntual, no se cae el flujo de edición
                    Console.WriteLine($"[SUERO] ERROR en upsert (se ignora para no romper edición): {ex.Message}");
                }

                #endregion

                TempData["Message"] = "¡La consulta se ha guardado con éxito.!";
                return JsonSerializer.Serialize(new { Exitoso = true, ConsultaId = consultaEditada.Id });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al editar consulta: " + ex.ToString(),
                    Error = ex.ToString()
                });

            }

        }


        [HttpPost]
        public string ConsultarConsultasPaciente(int pacienteId)
        {
            try
            {
                var consultas = _pacientesService.GetHistorialConsultas(pacienteId);
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = consultas
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar historial. " + ex.Message
                });
            }
        }

        //EN PROCESO DE SER ELIMINADO
        //[HttpPost]
        //public string ConsultarPrescripcionesyExamenesConsulta(int consultaId)
        //{
        //    try
        //    {


        //        var serviciosConsulta = new List<VentaUnificadaExamenAgregadoViewModel>();

        //        var prescripcion = _consultasRepository.GetPrescripcionConsulta(consultaId);

        //        if (prescripcion != null)
        //        {
        //            foreach (var elemento in prescripcion.DetallePrescripcion)
        //            {
        //                if (elemento.Item >= 1000)
        //                {

        //                    serviciosConsulta.Add(new VentaUnificadaExamenAgregadoViewModel
        //                    {
        //                        ExamenId = elemento.Item,//correlativo de la tabla
        //                        ExamenCodigo = elemento.Indications, //codigo 
        //                        ExamenNombre = elemento.Medicine, //nombre del examen
        //                                                          //PrecioId = servicio.PrecioId,
        //                                                          //PrecioNombre = servicio.Precio.NombrePrecio,
        //                        ValorUnitario = 1,//elemento.Indications,
        //                        Cantidad = 1,//elemento.Qty,
        //                        ValorSubtotal = 0,// (servicio.PrecioValor ?? 0) * servicio.Cantidad,
        //                        ValorTotal = 1,
        //                        DescuentoPorcentaje = 0,
        //                        DescuentoValor = 0
        //                    });
        //                }
        //            }
        //        }


        //        return JsonSerializer.Serialize(new
        //        {
        //            Exitoso = true,
        //            Resultado = serviciosConsulta
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return JsonSerializer.Serialize(new
        //        {
        //            Exitoso = false,
        //            Mensaje = "Error de servidor al consultar servicios: " + ex.Message
        //        });
        //    }
        //}

        [HttpPost]
        public string ConsultarExamenesAgregadosConsulta(int consultaId)
        {
            try
            {
                var examenesAgregados = new List<ConsultaExamenAgregadoViewModel>();
                var examenesBd = _consultasRepository.GetExamenesAgregadosConsulta(consultaId);
                if (examenesBd != null)
                {
                    foreach (var examen in examenesBd)
                    {
                        var examenLabClinico = examen.ExamenLabClinico ?? new ExamenLabClinico();
                        var precio = examen.Precio ?? new Precio();
                        examenesAgregados.Add(new ConsultaExamenAgregadoViewModel
                        {
                            ExamenId = examen.ExamenLabClinicoId,
                            ExamenCodigo = examenLabClinico.CodigoInterno,
                            ExamenNombre = examenLabClinico.NombreExamen,
                            PrecioId = examen.PrecioId,
                            FechaRegistro = examen.FechaRegistro,
                            Cantidad = examen.Cantidad,
                            PrecioNombre = precio.NombrePrecio,
                            ValorSubtotal = examen.Cantidad * examen.PrecioValor,
                            ValorUnitario = examen.PrecioValor,
                            ValorCopago = examen.PrecioValorCopago,
                            ValorCubiertoSeguro = examen.PrecioValorCubiertoSeguro,
                            Pagado = examen.Pagado
                        });
                    }
                }


                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = examenesAgregados
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar examenes agregados a consulta: " + ex.Message
                });
            }
        }
        public IActionResult CrearExamenFisico(int? consultaId)
        {
            if (consultaId == null) return StatusCode(404);
            var consulta = _consultasRepository.GetConsulta((int)consultaId);
            if (consulta == null) return StatusCode(400);

            var examenFisico = new ExamenFisico()
            {
                Temperatura = "--- Temperatura ---",
                FrecuenciaRespiratoria = "--- FrecuenciaRespiratoria ---",
                FrecuenciaCardiaca = "--- Frecuencia Cardiaca ---",
                SaturacionDeOxigeno = "--- Saturacion De Oxigeno ---",
                PresionArterialBrazoDerecho = "--- Presion Arterial Brazo Derecho ---",
                PresionArterialBrazoIzquierdo = "--- Presion Arterial Brazo Izquierdo ---", /* zzz */
                PresionArterialMedia = "--- Presion Arterial Media ---",
                Observaciones = "--- Observaciones o texto adicional ---",
            };

            consulta.ExamenFisico = examenFisico;
            _consultasRepository.Update(consulta);

            return RedirectToAction("Informacion", new { id = consulta.Id });
        }


        public IActionResult EditarConsultaSignosVitales(int? id)
        {
            if (id == null) return StatusCode(404);

            var consulta = _consultasRepository.GetConsulta((int)id);
            if (consulta == null) return StatusCode(400);

            var model = new SignosVitalesViewModel
            {
                ConsultaId = consulta.Id,
                ExamenFisicoEstadoGeneral = consulta.ExamenFisico?.EstadoGeneral ?? string.Empty,
                ExamenFisicoPeso = consulta.ExamenFisico?.Peso ?? string.Empty,
                ExamenFisicoTalla = consulta.ExamenFisico?.Talla ?? string.Empty,
                ExamenFisicoFrecuenciaCardiaca = consulta.ExamenFisico?.FrecuenciaCardiaca ?? string.Empty,
                ExamenFisicoFrecuenciaRespiratoria = consulta.ExamenFisico?.FrecuenciaRespiratoria ?? string.Empty,
                ExamenFisicoPresionArterial = consulta.ExamenFisico?.PresionArterial ?? string.Empty,
                ExamenFisicoTemperatura = consulta.ExamenFisico?.Temperatura ?? string.Empty,
                ExamenFisicoSaturacionOxigeno = consulta.ExamenFisico?.SaturacionDeOxigeno ?? string.Empty,
                ExamenFisicoGlasgow = consulta.ExamenFisico?.Glasgow ?? string.Empty
            };

            ViewBag.ConsultaId = id;

            return View("_ConsultaFormSignosVitales", model);
        }

        [HttpPost]
        public string EditarConsultaSignosVitales(SignosVitalesViewModel model)
        {
            if (model.ConsultaId == null)
            {
                return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "ConsultaId es nulo." });
            }

            try
            {
                // Obtener la consulta desde la base de datos
                var consultaEditada = _consultasRepository.GetConsulta((int)model.ConsultaId);

                if (consultaEditada == null)
                {
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Consulta no encontrada." });
                }

                if (consultaEditada.ExamenFisico == null)
                {
                    consultaEditada.ExamenFisico = new ExamenFisico();
                }

                consultaEditada.ExamenFisico.EstadoGeneral = model.ExamenFisicoEstadoGeneral;
                consultaEditada.ExamenFisico.Peso = model.ExamenFisicoPeso;
                consultaEditada.ExamenFisico.Talla = model.ExamenFisicoTalla;
                consultaEditada.ExamenFisico.FrecuenciaCardiaca = model.ExamenFisicoFrecuenciaCardiaca;
                consultaEditada.ExamenFisico.FrecuenciaRespiratoria = model.ExamenFisicoFrecuenciaRespiratoria;
                consultaEditada.ExamenFisico.PresionArterial = model.ExamenFisicoPresionArterial;
                consultaEditada.ExamenFisico.Temperatura = model.ExamenFisicoTemperatura;
                consultaEditada.ExamenFisico.SaturacionDeOxigeno = model.ExamenFisicoSaturacionOxigeno;
                consultaEditada.ExamenFisico.Glasgow = model.ExamenFisicoGlasgow;

                // Guardar los cambios en la consulta
                _consultasRepository.Update(consultaEditada);

                return JsonSerializer.Serialize(new { Exitoso = true, ConsultaId = consultaEditada.Id });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Error al editar consulta. " + ex.Message });
            }
        }



        public IActionResult EditarConsultaHistoriaClinicaHospitalizacion(int? id)
        {
            if (id == null) return StatusCode(404);

            var consulta = _consultasRepository.GetConsulta((int)id);
            if (consulta == null) return StatusCode(400);

            var model = new HistoriaClinicaViewModel //Esta es la linea 4847
            {
                ConsultaId = consulta.Id,
                PacienteId = consulta.Citas?.Paciente?.Id,
                HistoriaId = consulta.Historia?.Id ?? 0,
                HistoriaEnfermedadActual = consulta.Historia?.HistoriaEnfermedadActual ?? string.Empty,
                ConsultaMotivo = consulta.ConsultaMotivo ?? string.Empty,
                EstaEmbarazada = consulta.EstaEmbarazada ?? "-",
                NumeroSemanasEmbarazo = consulta.NumeroSemanasEmbarazo ?? 0,
                EstaAmamantando = consulta.EstaAmamantando ?? "-",
                PacienteMedicos = consulta.Citas?.Paciente?.AntecedentesMedicos ?? "-",
                PacienteTraumaticos = consulta.Citas?.Paciente?.AntecedentesTraumaticos ?? "-",
                PacienteAlergias = consulta.Citas?.Paciente?.AntecedentesAlergias ?? "-",
                PacienteVicios = consulta.Citas?.Paciente?.AntecedentesVicios ?? "-",
                PacienteMedicamentos = consulta.Citas?.Paciente?.AntecedentesMedicamentos ?? "-",
                PacienteQuirurgicos = consulta.Citas?.Paciente?.AntecedentesQuirurgicos ?? "-",

                ExamenFisicoEstadoGeneral = consulta.ExamenFisico?.EstadoGeneral ?? string.Empty,
                ExamenFisicoPeso = consulta.ExamenFisico?.Peso ?? string.Empty,
                ExamenFisicoTalla = consulta.ExamenFisico?.Talla ?? string.Empty,
                ExamenFisicoFrecuenciaCardiaca = consulta.ExamenFisico?.FrecuenciaCardiaca ?? string.Empty,
                ExamenFisicoFrecuenciaRespiratoria = consulta.ExamenFisico?.FrecuenciaRespiratoria ?? string.Empty,
                ExamenFisicoPresionArterial = consulta.ExamenFisico?.PresionArterial ?? string.Empty,
                ExamenFisicoTemperatura = consulta.ExamenFisico?.Temperatura ?? string.Empty,
                ExamenFisicoSaturacionOxigeno = consulta.ExamenFisico?.SaturacionDeOxigeno ?? string.Empty,
                ExamenFisicoGlasgow = consulta.ExamenFisico?.Glasgow ?? string.Empty,

                RevisionSistemasAparienciaGeneral = consulta.ConsultaRevisionSistemas?.AparienciaGeneral ?? string.Empty,
                RevisionSistemasCabeza = consulta.ConsultaRevisionSistemas?.Cabeza ?? string.Empty,
                RevisionSistemasCuello = consulta.ConsultaRevisionSistemas?.Cuello ?? string.Empty,
                RevisionSistemasOidosBoca = consulta.ConsultaRevisionSistemas?.OidosBoca ?? string.Empty,
                RevisionSistemasAbdomen = consulta.ConsultaRevisionSistemas?.Abdomen ?? string.Empty,
                RevisionSistemasTorax = consulta.ConsultaRevisionSistemas?.Torax ?? string.Empty,
                RevisionSistemasDorsoYExtremidades = consulta.ConsultaRevisionSistemas?.DorsoYExtremidades ?? string.Empty,
                RevisionSistemasGenitales = consulta.ConsultaRevisionSistemas?.Genitales ?? string.Empty,

                HistoriaClinicaComentario = consulta.Historia?.Comentario ?? string.Empty,
                HistoriaClinicaImpresionClinica = consulta.Historia?.ImpresionClinica ?? string.Empty


            };

            ViewBag.ConsultaId = id;

            return View("_ConsultaFormHistoriaClinicaHospitalizacion", model);
        }
        public IActionResult EditarConsultaHistoriaClinica(int? id)
        {
            if (id == null) return StatusCode(404);

            var consulta = _consultasRepository.GetConsulta((int)id);
            if (consulta == null) return StatusCode(400);

            // Verificaciones y asignaciones por defecto
            if (consulta.Historia == null) consulta.Historia = new Historia();
            if (consulta.ExamenFisico == null) consulta.ExamenFisico = new ExamenFisico();
            if (consulta.ConsultaRevisionSistemas == null) consulta.ConsultaRevisionSistemas = new ConsultaRevisionSistemas();
            if (consulta.Citas?.Paciente != null)
            {
                consulta.Citas.Paciente.AntecedentesMedicos ??= "-";
                consulta.Citas.Paciente.AntecedentesTraumaticos ??= "-";
                consulta.Citas.Paciente.AntecedentesAlergias ??= "-";
                consulta.Citas.Paciente.AntecedentesVicios ??= "-";
                consulta.Citas.Paciente.AntecedentesMedicamentos ??= "-";
                consulta.Citas.Paciente.AntecedentesQuirurgicos ??= "-";
            }

            var model = new HistoriaClinicaViewModel
            {
                ConsultaId = consulta.Id,
                PacienteId = consulta.Citas.Paciente.Id,
                HistoriaId = consulta.Historia?.Id ?? 0,
                HistoriaEnfermedadActual = consulta.Historia?.HistoriaEnfermedadActual ?? string.Empty,
                ConsultaMotivo = consulta.ConsultaMotivo ?? string.Empty,
                EstaEmbarazada = consulta.EstaEmbarazada ?? "-",
                NumeroSemanasEmbarazo = consulta.NumeroSemanasEmbarazo ?? 0,
                EstaAmamantando = consulta.EstaAmamantando ?? "-",
                PacienteMedicos = consulta.Citas.Paciente.AntecedentesMedicos ?? "-",
                PacienteTraumaticos = consulta.Citas.Paciente.AntecedentesTraumaticos ?? "-",
                PacienteAlergias = consulta.Citas.Paciente.AntecedentesAlergias ?? "-",
                PacienteVicios = consulta.Citas.Paciente.AntecedentesVicios ?? "-",
                PacienteMedicamentos = consulta.Citas.Paciente.AntecedentesMedicamentos ?? "-",
                PacienteQuirurgicos = consulta.Citas.Paciente.AntecedentesQuirurgicos ?? "-",

                ExamenFisicoEstadoGeneral = consulta.ExamenFisico?.EstadoGeneral ?? string.Empty,
                ExamenFisicoPeso = consulta.ExamenFisico?.Peso ?? string.Empty,
                ExamenFisicoTalla = consulta.ExamenFisico?.Talla ?? string.Empty,
                ExamenFisicoFrecuenciaCardiaca = consulta.ExamenFisico?.FrecuenciaCardiaca ?? string.Empty,
                ExamenFisicoFrecuenciaRespiratoria = consulta.ExamenFisico?.FrecuenciaRespiratoria ?? string.Empty,
                ExamenFisicoPresionArterial = consulta.ExamenFisico?.PresionArterial ?? string.Empty,
                ExamenFisicoTemperatura = consulta.ExamenFisico?.Temperatura ?? string.Empty,
                ExamenFisicoSaturacionOxigeno = consulta.ExamenFisico?.SaturacionDeOxigeno ?? string.Empty,
                ExamenFisicoGlasgow = consulta.ExamenFisico?.Glasgow ?? string.Empty,

                RevisionSistemasAparienciaGeneral = consulta.ConsultaRevisionSistemas.AparienciaGeneral,
                RevisionSistemasCabeza = consulta.ConsultaRevisionSistemas.Cabeza,
                RevisionSistemasCuello = consulta.ConsultaRevisionSistemas.Cuello,
                RevisionSistemasOidosBoca = consulta.ConsultaRevisionSistemas.OidosBoca,
                RevisionSistemasAbdomen = consulta.ConsultaRevisionSistemas.Abdomen,
                RevisionSistemasTorax = consulta.ConsultaRevisionSistemas.Torax,
                RevisionSistemasDorsoYExtremidades = consulta.ConsultaRevisionSistemas.DorsoYExtremidades,
                RevisionSistemasGenitales = consulta.ConsultaRevisionSistemas.Genitales,

                HistoriaClinicaComentario = consulta.Historia.Comentario,
                HistoriaClinicaImpresionClinica = consulta.Historia.ImpresionClinica
            };

            ViewBag.ConsultaId = id;

            return View("_ConsultaFormHistoriaClinica", model);
        }

        [HttpPost]
        public string EditarConsultaHistoriaClinica(HistoriaClinicaViewModel model)
        {
            if (model.ConsultaId == null)
            {
                return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "ConsultaId es nulo." });
            }

            try
            {
                var consultaEditada = _consultasRepository.GetConsulta((int)model.ConsultaId);

                if (consultaEditada == null)
                {
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Consulta no encontrada." });
                }

                // Crear objetos faltantes
                consultaEditada.Historia ??= new Historia();
                consultaEditada.ExamenFisico ??= new ExamenFisico();
                consultaEditada.ConsultaRevisionSistemas ??= new ConsultaRevisionSistemas();

                var paciente = _pacientesRepository.Get((int)model.PacienteId, false);
                if (paciente == null)
                {
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Paciente no encontrado." });
                }

                #region ANTECEDENTES PATOLOGICOS PACIENTE
                paciente.AntecedentesAlergias = model.PacienteAlergias;
                paciente.AntecedentesMedicamentos = model.PacienteMedicamentos;
                paciente.AntecedentesMedicos = model.PacienteMedicos;
                paciente.AntecedentesTraumaticos = model.PacienteTraumaticos;
                paciente.AntecedentesVicios = model.PacienteVicios;
                paciente.AntecedentesQuirurgicos = model.PacienteQuirurgicos;

                _pacientesRepository.Update(paciente);
                #endregion

                #region ACTUALIZAR HISTORIA CLÍNICA
                consultaEditada.Historia.HistoriaEnfermedadActual = model.HistoriaEnfermedadActual;
                consultaEditada.Historia.Comentario = model.HistoriaClinicaComentario;
                consultaEditada.Historia.ImpresionClinica = model.HistoriaClinicaImpresionClinica;

                consultaEditada.ConsultaMotivo = model.ConsultaMotivo;
                consultaEditada.EstaEmbarazada = model.EstaEmbarazada;
                consultaEditada.NumeroSemanasEmbarazo = model.NumeroSemanasEmbarazo;
                consultaEditada.EstaAmamantando = model.EstaAmamantando;
                #endregion

                #region EXAMEN FISICO
                consultaEditada.ExamenFisico.EstadoGeneral = model.ExamenFisicoEstadoGeneral;
                consultaEditada.ExamenFisico.Peso = model.ExamenFisicoPeso;
                consultaEditada.ExamenFisico.Talla = model.ExamenFisicoTalla;
                consultaEditada.ExamenFisico.FrecuenciaCardiaca = model.ExamenFisicoFrecuenciaCardiaca;
                consultaEditada.ExamenFisico.FrecuenciaRespiratoria = model.ExamenFisicoFrecuenciaRespiratoria;
                consultaEditada.ExamenFisico.PresionArterial = model.ExamenFisicoPresionArterial;
                consultaEditada.ExamenFisico.Temperatura = model.ExamenFisicoTemperatura;
                consultaEditada.ExamenFisico.SaturacionDeOxigeno = model.ExamenFisicoSaturacionOxigeno;
                consultaEditada.ExamenFisico.Glasgow = model.ExamenFisicoGlasgow;
                #endregion

                #region REVISION POR SISTEMAS
                consultaEditada.ConsultaRevisionSistemas.AparienciaGeneral = model.RevisionSistemasAparienciaGeneral;
                consultaEditada.ConsultaRevisionSistemas.Abdomen = model.RevisionSistemasAbdomen;
                consultaEditada.ConsultaRevisionSistemas.Torax = model.RevisionSistemasTorax;
                consultaEditada.ConsultaRevisionSistemas.Cuello = model.RevisionSistemasCuello;
                consultaEditada.ConsultaRevisionSistemas.OidosBoca = model.RevisionSistemasOidosBoca;
                consultaEditada.ConsultaRevisionSistemas.Cabeza = model.RevisionSistemasCabeza;
                consultaEditada.ConsultaRevisionSistemas.DorsoYExtremidades = model.RevisionSistemasDorsoYExtremidades;
                consultaEditada.ConsultaRevisionSistemas.Genitales = model.RevisionSistemasGenitales;
                #endregion


                _consultasRepository.Update(consultaEditada);

                return JsonSerializer.Serialize(new { Exitoso = true, ConsultaId = consultaEditada.Id });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Error al editar consulta. " + ex.Message });
            }
        }

        //[HttpPost]
        //public JsonResult GuardarPrescripcion([FromBody] ViewPrescriptionModel model)
        //{
        //    Prescripcion prescripcion = new Prescripcion()
        //    {
        //        ConsultaId = model.ConsultaId,
        //        NextDate = model.NextDate,
        //    };

        //    _consultasRepository.AddPrescipcion(prescripcion);

        //    foreach (var item in model.Prescriptions)
        //    {
        //        DetallePrescripcion detallePrescripcion = new DetallePrescripcion()
        //        {
        //            Item = item.Item,
        //            Medicine = item.Medicine,
        //            Qty = item.Qty,
        //            Indications = item.Indications,
        //            PrescripcionId = prescripcion.Id
        //        };
        //        _consultasRepository.AddDetallePrescipcion(detallePrescripcion);
        //    }


        //    return Json(prescripcion.Id);
        //    /*
        //    return new ViewAsPdf("Views/Consultas/Prescripcion.cshtml",model){
        //        PageMargins = new Margins(5,5,5,5),
        //        PageOrientation = Orientation.Portrait,
        //        PageSize = Size.Letter,
        //        PageHeight = 150,
        //        PageWidth = 100
        //    };*/
        //}

        //public async Task<IActionResult> Prescripcion(int prescripcionId)
        //{
        //    _consultasRepository.UpdateTablePrescription();
        //    var prescripcion = _consultasRepository.GetPrescripcion(prescripcionId);

        //    ViewPrescriptionModel model = new ViewPrescriptionModel()
        //    {
        //        Nombre = prescripcion.Consulta.Citas.Paciente.Nombre,
        //        Edad = prescripcion.Consulta.Citas.Paciente.Edad == null ? 0 :
        //            (int)prescripcion.Consulta.Citas.Paciente.Edad,
        //        Direccion = prescripcion.Consulta.Citas.Paciente.Direccion,
        //        NextDate = prescripcion.NextDate,
        //        Prescriptions = prescripcion.DetallePrescripcion.ToList()
        //    };

        //    /* ViewPrescriptionModel model = new ViewPrescriptionModel(){
        //        Nombre =" Hever Polanco ",
        //        Edad = 24,
        //        Direccion = "Guatemala",
        //        NextDate = DateTime.Now.AddDays(3),
        //        Prescriptions = prescripcion.DetallePrescripcion
        //    }; */
        //    string footer = "--footer-center \"Próxima Cita: " + model.NextDate.ToString("dd/MM/yyyy") + "  \"" +
        //                                 " --footer-line --footer-font-size \"12\" --footer-spacing 2 --footer-font-name \"calibri light\"";

        //    var options = new ConvertOptions
        //    {
        //        PageOrientation = Wkhtmltopdf.NetCore.Options.Orientation.Portrait,
        //        FooterHtml = "Footer HTML prueba",
        //        PageSize = Wkhtmltopdf.NetCore.Options.Size.Letter
        //    };

        //    _generatePdf.SetConvertOptions(options);

        //    return await _generatePdf.GetPdf("Views/Consultas/Prescripcion.cshtml", model);
        //}
        public IActionResult GenerarVenta(int id)
        {
            return RedirectToAction("NuevaVentaUnificada", "Venta",
                new { isClinica = true, isFarmacia = false, isLaboratorio = false, isEmergencia = false, consultaId = id });
        }

        //Metodo para registrar archivos de examenes
        [HttpPost]
        public JsonResult RegistrarArchivo(int consultaId, string nombreArchivo, string rutaArchivo)
        {
            try
            {
                var consulta = _consultasRepository.GetConsulta(consultaId);
                consulta.ConsultaExamenArchivos.Add(new ConsultaExamenArchivo
                {
                    NombreArchivo = nombreArchivo,
                    UrlArchivo = rutaArchivo
                });
                _consultasRepository.Update(consulta);
                TempData["Message"] = "Se ha subido el archivo " + nombreArchivo;
                return Json(new
                {
                    exitoso = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    exitoso = false,
                    mensaje = "Error al subir archivo. " + ex.Message
                });
            }
        }
        [HttpGet]
        public async Task<JsonResult> BuscarCie10(string codigo, [FromServices] Database.Shared.IRepository.ICie10 cie10Repo)
        {
            var cie10 = await cie10Repo.GetByID(codigo); // Pasa el string directamente
            if (cie10 != null)
            {
                return Json(new { descripcion = cie10.descripcion });
            }
            else
            {
                return Json(new { descripcion = "" });
            }
        }

        [HttpGet]
        public IActionResult RecetaMedicaWord(int consultaId)
        {
            // 1) Obtener la consulta y el paciente
            var consulta = _consultasRepository.GetConsulta(consultaId);
            if (consulta == null) return StatusCode(404);

            var paciente = consulta.Citas?.Paciente;
            var pacienteNombre = paciente?.Nombre ?? string.Empty;
            var fechaEmision = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            // 2) Registro oftalmología
            var oft = _oftRepo.GetConsulta(consultaId); // ya se usa así en otras partes
            if (oft == null)
            {
                // Si no hay registro, devolvemos error "blando" para que el usuario sepa qué pasa
                return StatusCode(400, "No existe información de oftalmología para esta consulta.");
            }

            var tratamiento = oft.HistoriaClinicaComentario ?? "NO HAY TRATAMIENTO REGISTRADO";

            // 2) Ruta de la plantilla
            var templatePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "PlantillasWord",
                "RecetaMedicaOftalmo.docx"
            );

            if (!System.IO.File.Exists(templatePath))
            {
                return StatusCode(500, "No se encontró la plantilla de Word para receta médica.");
            }

            // 3) Copiar la plantilla a memoria y reemplazar marcadores
            byte[] resultado;
            using (var ms = new MemoryStream())
            {
                using (var fs = new FileStream(templatePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    fs.CopyTo(ms);
                }

                using (var wordDoc = WordprocessingDocument.Open(ms, true))
                {
                    string docText;
                    using (var sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                    {
                        docText = sr.ReadToEnd();
                    }

                    docText = docText.Replace("{{PACIENTE_NOMBRE}}", pacienteNombre);
                    docText = docText.Replace("{{FECHA_EMISION}}", fechaEmision);
                    docText = docText.Replace("{{TRATAMIENTO}}", tratamiento);

                    using (var sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                    {
                        sw.Write(docText);
                    }
                }

                resultado = ms.ToArray();
            }

            var fileName = $"Receta_Medica_{consultaId}_{DateTime.Now:yyyyMMddHHmm}.docx";

            return File(
                resultado,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                fileName
            );
        }

        [HttpGet]
        public IActionResult RecetaGafasWord(int consultaId)
        {
            // 1) Consulta y paciente
            var consulta = _consultasRepository.GetConsulta(consultaId);
            if (consulta == null) return StatusCode(404);

            var paciente = consulta.Citas?.Paciente;
            var pacienteNombre = paciente?.Nombre ?? string.Empty;
            var fechaEmision = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            // 2) Registro oftalmología
            var oft = _oftRepo.GetConsulta(consultaId); // ya se usa así en otras partes
            if (oft == null)
            {
                // Si no hay registro, devolvemos error "blando" para que el usuario sepa qué pasa
                return StatusCode(400, "No existe información de oftalmología para esta consulta.");
            }

            // Formateadores seguros
            string Dec(decimal? n) => n.HasValue ? n.Value.ToString("0.00", CultureInfo.InvariantCulture) : string.Empty;
            string Entero(int? n) => n.HasValue ? n.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;

            var odEsfera = Dec(oft.Final_OD_Esfera);
            var odCilindro = Dec(oft.Final_OD_Cilindro);
            var odEje = Entero(oft.Final_OD_Eje);

            var osEsfera = Dec(oft.Final_OS_Esfera);
            var osCilindro = Dec(oft.Final_OS_Cilindro);
            var osEje = Entero(oft.Final_OS_Eje);

            var add = Dec(oft.Final_Adicion);
            var dip = Dec(oft.Final_DIP_mm);
            var material = oft.LenteMaterialTratamiento ?? string.Empty;

            // 3) Ruta plantilla
            var templatePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "PlantillasWord",
                "RecetaGafasOftalmo.docx"
            );

            if (!System.IO.File.Exists(templatePath))
            {
                return StatusCode(500, "No se encontró la plantilla de Word para receta de gafas.");
            }

            // 4) Copiar plantilla a memoria y sustituir marcadores
            byte[] resultado;
            using (var ms = new MemoryStream())
            {
                using (var fs = new FileStream(templatePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    fs.CopyTo(ms);
                }

                using (var wordDoc = WordprocessingDocument.Open(ms, true))
                {
                    string docText;
                    using (var sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                    {
                        docText = sr.ReadToEnd();
                    }

                    docText = docText.Replace("{{PACIENTE_NOMBRE}}", pacienteNombre);
                    docText = docText.Replace("{{FECHA_EMISION}}", fechaEmision);

                    docText = docText.Replace("{{OD_ESFERA}}", odEsfera);
                    docText = docText.Replace("{{OD_CILINDRO}}", odCilindro);
                    docText = docText.Replace("{{OD_EJE}}", odEje);

                    docText = docText.Replace("{{OS_ESFERA}}", osEsfera);
                    docText = docText.Replace("{{OS_CILINDRO}}", osCilindro);
                    docText = docText.Replace("{{OS_EJE}}", osEje);

                    docText = docText.Replace("{{ADIC}}", add);
                    docText = docText.Replace("{{DIP}}", dip);
                    docText = docText.Replace("{{MATERIAL}}", material);

                    using (var sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                    {
                        sw.Write(docText);
                    }
                }

                resultado = ms.ToArray();
            }

            var fileName = $"Receta_Gafas_{consultaId}_{DateTime.Now:yyyyMMddHHmm}.docx";

            return File(
                resultado,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                fileName
            );
        }

        [HttpGet]
        public IActionResult ObtenerContadoresCita(int citaId)
        {
            Console.WriteLine($"[ObtenerContadoresCita] IN -> citaId={citaId}");

            if (citaId <= 0)
            {
                Console.WriteLine($"[ObtenerContadoresCita] BADREQUEST -> citaId inválido: {citaId}");
                return BadRequest("citaId inválido.");
            }

            var cita = _dbContext.Citass
                .AsNoTracking()
                .Where(c => c.Id == citaId)
                .Select(c => new
                {
                    CitaId = c.Id,
                    FechaInicio = c.FechaInicio,
                    FechaFinal = c.FechaFinal,
                    ContadorCitaAgendada = c.ContadorCitaAgendada,
                    ContadorCitaIniciada = c.ContadorCitaIniciada,
                    ContadorCitaFinalizada = c.ContadorCitaFinalizada
                })
                .FirstOrDefault();

            if (cita == null)
            {
                Console.WriteLine($"[ObtenerContadoresCita] NOTFOUND -> Id={citaId}");
                return NotFound($"No existe la cita con Id={citaId}.");
            }

            Console.WriteLine(
                $"[ObtenerContadoresCita] OK -> " +
                $"CitaId={cita.CitaId}, " +
                $"Agendada={cita.ContadorCitaAgendada?.ToString("O") ?? "null"}, " +
                $"Iniciada={cita.ContadorCitaIniciada?.ToString("O") ?? "null"}, " +
                $"Finalizada={cita.ContadorCitaFinalizada?.ToString("O") ?? "null"}"
            );

            return Json(cita);
        }


        [HttpPost]
        public string ObtenerPacientePorConsulta(int consultaId)
        {
            try
            {
                var consulta = _consultasRepository.GetConsulta(consultaId);

                if (consulta == null || consulta.Citas?.Paciente == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "No se encontró el paciente para esta consulta"
                    });
                }

                var paciente = consulta.Citas.Paciente;

                var resultado = new[]
                {
            new
            {
                PacienteId = paciente.Id,
                Nombre = paciente.Nombre,
                Nit = paciente.Nit,
                Direccion = paciente.Direccion,
                //Correo = paciente.Correo,
                FechaNacimiento = paciente.FechaNacimiento?.ToString("yyyy-MM-ddTHH:mm:ss")
            }
        };

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = resultado
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al obtener paciente: " + ex.Message
                });
            }
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                var consulta = _consultasRepository.GetConsulta(id);

                if (consulta == null)
                {
                    return Json(new
                    {
                        exitoso = false,
                        mensaje = "No se encontró la consulta"
                    });
                }

                consulta.Eliminado = true;

                _consultasRepository.Update(consulta);

                TempData["Message"] = "Consulta eliminada correctamente";

                return Json(new
                {
                    exitoso = true,
                    mensaje = "Consulta eliminada correctamente"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    exitoso = false,
                    mensaje = "Error al eliminar la consulta: " + ex.Message
                });
            }
        }
    }
}
