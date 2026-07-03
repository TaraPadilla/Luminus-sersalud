using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Database.Shared.Models;
using farmamest.Helpers;
using farmamest.Service.IService;

namespace farmamest.Controllers
{
    public class CuestionarioPreAnestesicoController : Controller
    {
        private readonly ICuestionarioPreAnestesicoService _service;
        private readonly UserManager<User> _userManager;

        private static readonly JsonSerializerOptions _jsonOpts = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        private static readonly JsonSerializerOptions _jsonInputOpts = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        public CuestionarioPreAnestesicoController(
            ICuestionarioPreAnestesicoService service,
            UserManager<User> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        private static DateTime? NormalizarFechaCalendario(DateTime? fecha)
        {
            if (!fecha.HasValue) return null;
            return DateTime.SpecifyKind(fecha.Value.Date, DateTimeKind.Unspecified);
        }

        private static string FormatearFechaCalendario(DateTime? fecha)
        {
            return fecha.HasValue ? fecha.Value.ToString("yyyy-MM-dd") : null;
        }

        [HttpPost]
        public async Task<string> AgregarCuestionario()
        {
            try
            {
                var model = await HttpRequestJsonHelper.LeerCuerpoJsonAsync<CuestionarioPreAnestesicoInputVM>(
                    Request, this, _jsonInputOpts);
                if (model == null)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Datos inválidos." }, _jsonOpts);

                if (model.HospitalizacionId == 0)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "HospitalizacionId inválido." }, _jsonOpts);

                var nuevo = new CuestionarioPreAnestesico
                {
                    HospitalizacionId = model.HospitalizacionId,
                    FechaRegistro = DateTime.Now,
                    UserId = _userManager.GetUserId(HttpContext.User),

                    // Datos del paciente
                    NombreCompleto = model.NombreCompleto,
                    RegistroMedico = model.RegistroMedico,
                    Edad = model.Edad,
                    FechaCuestionario = NormalizarFechaCalendario(model.FechaCuestionario),
                    Peso = model.Peso,
                    Estatura = model.Estatura,
                    FechaUltimaRegla = NormalizarFechaCalendario(model.FechaUltimaRegla),
                    FechaProcedimiento = NormalizarFechaCalendario(model.FechaProcedimiento),
                    ProcedimientoProgramado = model.ProcedimientoProgramado,
                    Cirujano = model.Cirujano,

                    // Antecedentes
                    PA_Alergia = model.PA_Alergia,
                    PA_AlergiaCual = model.PA_AlergiaCual,
                    PA_Fuma = model.PA_Fuma,
                    PA_FumaCuanto = model.PA_FumaCuanto,
                    PA_Drogas = model.PA_Drogas,
                    PA_DrogasCuales = model.PA_DrogasCuales,
                    PA_Alcohol = model.PA_Alcohol,
                    PA_AlcoholCuanto = model.PA_AlcoholCuanto,
                    PA_Embarazo = model.PA_Embarazo,
                    PA_EmbarazoCual = model.PA_EmbarazoCual,
                    PA_Transfusion = model.PA_Transfusion,
                    PA_TransfusionCual = model.PA_TransfusionCual,
                    PA_Asma = model.PA_Asma,
                    PA_AsmaCual = model.PA_AsmaCual,
                    PA_Pulmones = model.PA_Pulmones,
                    PA_PulmonesCual = model.PA_PulmonesCual,
                    PA_Corazon = model.PA_Corazon,
                    PA_CorazonCual = model.PA_CorazonCual,
                    PA_AtaqueCardiaco = model.PA_AtaqueCardiaco,
                    PA_AtaqueCardiacoCual = model.PA_AtaqueCardiacoCual,
                    PA_Angina = model.PA_Angina,
                    PA_AnginaCual = model.PA_AnginaCual,
                    PA_Soplo = model.PA_Soplo,
                    PA_SoploCual = model.PA_SoploCual,
                    PA_Presion = model.PA_Presion,
                    PA_PresionCual = model.PA_PresionCual,
                    PA_Higado = model.PA_Higado,
                    PA_HigadoCual = model.PA_HigadoCual,
                    PA_Rinones = model.PA_Rinones,
                    PA_RinonesCual = model.PA_RinonesCual,
                    PA_Diabetes = model.PA_Diabetes,
                    PA_DiabetesCual = model.PA_DiabetesCual,
                    PA_Epilepsia = model.PA_Epilepsia,
                    PA_EpilepsiaCual = model.PA_EpilepsiaCual,
                    PA_Derrame = model.PA_Derrame,
                    PA_DerrameCual = model.PA_DerrameCual,
                    PA_Tiroides = model.PA_Tiroides,
                    PA_TiroidesCual = model.PA_TiroidesCual,
                    PA_Anestesico = model.PA_Anestesico,
                    PA_AnestesicoCual = model.PA_AnestesicoCual,
                    PA_AceptaTransfusion = model.PA_AceptaTransfusion,
                    PA_AceptaTransfusionCual = model.PA_AceptaTransfusionCual,

                    // Información adicional
                    AI_Medicamentos = model.AI_Medicamentos,
                    AI_Actividad = model.AI_Actividad,
                    AI_ActividadDetalle = model.AI_ActividadDetalle,
                    AI_OperacionesPrevias = model.AI_OperacionesPrevias,
                    AI_Comentarios = model.AI_Comentarios,
                };

                _service.Add(nuevo);

                return JsonSerializer.Serialize(new { exitoso = true, resultado = true }, _jsonOpts);
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    exitoso = false,
                    resultado = "Error: " + ex.Message + " | " + ex.InnerException?.Message
                }, _jsonOpts);
            }
        }

        [HttpPost]
        public string ObtenerCuestionarios(int idHospitalizacion)
        {
            try
            {
                var result = _service.GetByHospitalizacionId(idHospitalizacion)
                    .Select(c => new
                    {
                        c.Id,
                        c.HospitalizacionId,
                        FechaRegistro = c.FechaRegistro.ToString("yyyy-MM-dd HH:mm:ss"),
                        FechaCuestionario = FormatearFechaCalendario(c.FechaCuestionario),
                        FechaProcedimiento = FormatearFechaCalendario(c.FechaProcedimiento),
                        FechaUltimaRegla = FormatearFechaCalendario(c.FechaUltimaRegla),
                        c.NombreCompleto,
                        c.RegistroMedico,
                        c.Edad,
                        c.Peso,
                        c.Estatura,
                        c.ProcedimientoProgramado,
                        c.Cirujano,
                        c.PA_Alergia,
                        c.PA_AlergiaCual,
                        c.PA_Fuma,
                        c.PA_FumaCuanto,
                        c.PA_Drogas,
                        c.PA_DrogasCuales,
                        c.PA_Alcohol,
                        c.PA_AlcoholCuanto,
                        c.PA_Embarazo,
                        c.PA_EmbarazoCual,
                        c.PA_Transfusion,
                        c.PA_TransfusionCual,
                        c.PA_Asma,
                        c.PA_AsmaCual,
                        c.PA_Pulmones,
                        c.PA_PulmonesCual,
                        c.PA_Corazon,
                        c.PA_CorazonCual,
                        c.PA_AtaqueCardiaco,
                        c.PA_AtaqueCardiacoCual,
                        c.PA_Angina,
                        c.PA_AnginaCual,
                        c.PA_Soplo,
                        c.PA_SoploCual,
                        c.PA_Presion,
                        c.PA_PresionCual,
                        c.PA_Higado,
                        c.PA_HigadoCual,
                        c.PA_Rinones,
                        c.PA_RinonesCual,
                        c.PA_Diabetes,
                        c.PA_DiabetesCual,
                        c.PA_Epilepsia,
                        c.PA_EpilepsiaCual,
                        c.PA_Derrame,
                        c.PA_DerrameCual,
                        c.PA_Tiroides,
                        c.PA_TiroidesCual,
                        c.PA_Anestesico,
                        c.PA_AnestesicoCual,
                        c.PA_AceptaTransfusion,
                        c.PA_AceptaTransfusionCual,
                        c.AI_Medicamentos,
                        c.AI_Actividad,
                        c.AI_ActividadDetalle,
                        c.AI_OperacionesPrevias,
                        c.AI_Comentarios
                    })
                    .ToList();
                return JsonSerializer.Serialize(new { exitoso = true, resultado = result }, _jsonOpts);
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    exitoso = false,
                    mensaje = "Error al obtener cuestionarios: " + ex.Message
                }, _jsonOpts);
            }
        }

        [HttpPost]
        public async Task<string> ActualizarCuestionario()
        {
            try
            {
                var model = await HttpRequestJsonHelper.LeerCuerpoJsonAsync<CuestionarioPreAnestesicoInputVM>(
                    Request, this, _jsonInputOpts);
                if (model == null || model.Id == 0)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Id inválido." }, _jsonOpts);

                var actualizado = new CuestionarioPreAnestesico
                {
                    Id = model.Id,
                    HospitalizacionId = model.HospitalizacionId,

                    // Datos del paciente
                    NombreCompleto = model.NombreCompleto,
                    RegistroMedico = model.RegistroMedico,
                    Edad = model.Edad,
                    FechaCuestionario = NormalizarFechaCalendario(model.FechaCuestionario),
                    Peso = model.Peso,
                    Estatura = model.Estatura,
                    FechaUltimaRegla = NormalizarFechaCalendario(model.FechaUltimaRegla),
                    FechaProcedimiento = NormalizarFechaCalendario(model.FechaProcedimiento),
                    ProcedimientoProgramado = model.ProcedimientoProgramado,
                    Cirujano = model.Cirujano,

                    // Antecedentes
                    PA_Alergia = model.PA_Alergia,
                    PA_AlergiaCual = model.PA_AlergiaCual,
                    PA_Fuma = model.PA_Fuma,
                    PA_FumaCuanto = model.PA_FumaCuanto,
                    PA_Drogas = model.PA_Drogas,
                    PA_DrogasCuales = model.PA_DrogasCuales,
                    PA_Alcohol = model.PA_Alcohol,
                    PA_AlcoholCuanto = model.PA_AlcoholCuanto,
                    PA_Embarazo = model.PA_Embarazo,
                    PA_Transfusion = model.PA_Transfusion,
                    PA_TransfusionCual = model.PA_TransfusionCual,
                    PA_Asma = model.PA_Asma,
                    PA_AsmaCual = model.PA_AsmaCual,
                    PA_Pulmones = model.PA_Pulmones,
                    PA_PulmonesCual = model.PA_PulmonesCual,
                    PA_Corazon = model.PA_Corazon,
                    PA_CorazonCual = model.PA_CorazonCual,
                    PA_AtaqueCardiaco = model.PA_AtaqueCardiaco,
                    PA_AtaqueCardiacoCual = model.PA_AtaqueCardiacoCual,
                    PA_Angina = model.PA_Angina,
                    PA_AnginaCual = model.PA_AnginaCual,
                    PA_Soplo = model.PA_Soplo,
                    PA_SoploCual = model.PA_SoploCual,
                    PA_Presion = model.PA_Presion,
                    PA_PresionCual = model.PA_PresionCual,
                    PA_Higado = model.PA_Higado,
                    PA_HigadoCual = model.PA_HigadoCual,
                    PA_Rinones = model.PA_Rinones,
                    PA_RinonesCual = model.PA_RinonesCual,
                    PA_Diabetes = model.PA_Diabetes,
                    PA_DiabetesCual = model.PA_DiabetesCual,
                    PA_Epilepsia = model.PA_Epilepsia,
                    PA_EpilepsiaCual = model.PA_EpilepsiaCual,
                    PA_Derrame = model.PA_Derrame,
                    PA_DerrameCual = model.PA_DerrameCual,
                    PA_Tiroides = model.PA_Tiroides,
                    PA_TiroidesCual = model.PA_TiroidesCual,
                    PA_Anestesico = model.PA_Anestesico,
                    PA_AnestesicoCual = model.PA_AnestesicoCual,
                    PA_AceptaTransfusion = model.PA_AceptaTransfusion,
                    PA_AceptaTransfusionCual = model.PA_AceptaTransfusionCual,

                    // Información adicional
                    AI_Medicamentos = model.AI_Medicamentos,
                    AI_Actividad = model.AI_Actividad,
                    AI_ActividadDetalle = model.AI_ActividadDetalle,
                    AI_OperacionesPrevias = model.AI_OperacionesPrevias,
                    AI_Comentarios = model.AI_Comentarios,
                };

                _service.Actualizar(actualizado);

                return JsonSerializer.Serialize(new { exitoso = true }, _jsonOpts);
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    exitoso = false,
                    resultado = "Error: " + ex.Message + " | " + ex.InnerException?.Message
                }, _jsonOpts);
            }
        }
    }

    // ── INPUT VM ──────────────────────────────────────────────────────────────
    public class CuestionarioPreAnestesicoInputVM
    {
        public int Id { get; set; } // 0 = nuevo, >0 = editar
        public int HospitalizacionId { get; set; }

        // Datos del paciente
        public string NombreCompleto { get; set; }
        public string RegistroMedico { get; set; }
        public string Edad { get; set; }
        public DateTime? FechaCuestionario { get; set; }
        public double? Peso { get; set; }
        public double? Estatura { get; set; }
        public DateTime? FechaUltimaRegla { get; set; }
        public DateTime? FechaProcedimiento { get; set; }
        public string ProcedimientoProgramado { get; set; }
        public string Cirujano { get; set; }

        // Antecedentes — izquierda
        public string PA_Alergia { get; set; }
        public string PA_AlergiaCual { get; set; }
        public string PA_Fuma { get; set; }
        public string PA_FumaCuanto { get; set; }
        public string PA_Drogas { get; set; }
        public string PA_DrogasCuales { get; set; }
        public string PA_Alcohol { get; set; }
        public string PA_AlcoholCuanto { get; set; }
        public string PA_Embarazo { get; set; }

        public string PA_EmbarazoCual { get; set; }

        public string PA_Transfusion { get; set; }
        public string PA_TransfusionCual { get; set; }
        public string PA_Asma { get; set; }
        public string PA_AsmaCual { get; set; }
        public string PA_Pulmones { get; set; }
        public string PA_PulmonesCual { get; set; }
        public string PA_Corazon { get; set; }
        public string PA_CorazonCual { get; set; }

        // Antecedentes — derecha
        public string PA_AtaqueCardiaco { get; set; }
        public string PA_AtaqueCardiacoCual { get; set; }
        public string PA_Angina { get; set; }
        public string PA_AnginaCual { get; set; }
        public string PA_Soplo { get; set; }
        public string PA_SoploCual { get; set; }
        public string PA_Presion { get; set; }
        public string PA_PresionCual { get; set; }
        public string PA_Higado { get; set; }
        public string PA_HigadoCual { get; set; }
        public string PA_Rinones { get; set; }
        public string PA_RinonesCual { get; set; }
        public string PA_Diabetes { get; set; }
        public string PA_DiabetesCual { get; set; }
        public string PA_Epilepsia { get; set; }
        public string PA_EpilepsiaCual { get; set; }
        public string PA_Derrame { get; set; }
        public string PA_DerrameCual { get; set; }
        public string PA_Tiroides { get; set; }
        public string PA_TiroidesCual { get; set; }
        public string PA_Anestesico { get; set; }
        public string PA_AnestesicoCual { get; set; }
        public string PA_AceptaTransfusion { get; set; }
        public string PA_AceptaTransfusionCual { get; set; }

        // Información adicional
        public string AI_Medicamentos { get; set; }
        public string AI_Actividad { get; set; }
        public string AI_ActividadDetalle { get; set; }
        public string AI_OperacionesPrevias { get; set; }
        public string AI_Comentarios { get; set; }
    }
}