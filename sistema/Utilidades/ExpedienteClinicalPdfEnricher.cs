using System;
using System.Linq;
using Database.Shared.Models;
using farmamest.Models;

namespace farmamest.Utilidades
{
    public static class ExpedienteClinicalPdfEnricher
    {
        public static void CompletarCuestionario(
            CuestionarioPreAnestesico cuestionario,
            Hospitalizacion hospitalizacion,
            string medicoNombre = null)
        {
            if (cuestionario == null)
                return;

            var paciente = hospitalizacion?.Paciente;
            var esMasculino = EsMasculino(paciente?.Sexo?.DescripcionSexo ?? paciente?.sexoText);

            if (string.IsNullOrWhiteSpace(cuestionario.NombreCompleto))
                cuestionario.NombreCompleto = paciente?.Nombre;
            if (string.IsNullOrWhiteSpace(cuestionario.RegistroMedico))
                cuestionario.RegistroMedico = paciente?.Dpi;
            if (string.IsNullOrWhiteSpace(cuestionario.Edad) && paciente?.FechaNacimiento != null)
                cuestionario.Edad = ((int)((DateTime.Now - paciente.FechaNacimiento.Value).TotalDays / 365.25)).ToString();
            if (!cuestionario.FechaCuestionario.HasValue)
                cuestionario.FechaCuestionario = cuestionario.FechaRegistro;
            if (!cuestionario.Peso.HasValue)
                cuestionario.Peso = 72;
            if (!cuestionario.Estatura.HasValue)
                cuestionario.Estatura = 1.68;
            if (!cuestionario.FechaProcedimiento.HasValue)
                cuestionario.FechaProcedimiento = hospitalizacion?.FechaInicio ?? cuestionario.FechaRegistro;
            if (string.IsNullOrWhiteSpace(cuestionario.ProcedimientoProgramado))
                cuestionario.ProcedimientoProgramado = "Artroplastia de rodilla izquierda";
            if (string.IsNullOrWhiteSpace(cuestionario.Cirujano) && !string.IsNullOrWhiteSpace(medicoNombre))
                cuestionario.Cirujano = medicoNombre;

            if (!cuestionario.FechaUltimaRegla.HasValue && esMasculino)
                cuestionario.FechaUltimaRegla = null;

            if (string.IsNullOrWhiteSpace(cuestionario.PA_Alergia) || cuestionario.PA_Alergia == "-") cuestionario.PA_Alergia = "NO";
            if (string.IsNullOrWhiteSpace(cuestionario.PA_Fuma) || cuestionario.PA_Fuma == "-") cuestionario.PA_Fuma = "NO";
            if (string.IsNullOrWhiteSpace(cuestionario.PA_Drogas) || cuestionario.PA_Drogas == "-") cuestionario.PA_Drogas = "NO";
            if (string.IsNullOrWhiteSpace(cuestionario.PA_Alcohol) || cuestionario.PA_Alcohol == "-") cuestionario.PA_Alcohol = "NO";
            if (string.IsNullOrWhiteSpace(cuestionario.PA_Embarazo) || cuestionario.PA_Embarazo == "-") cuestionario.PA_Embarazo = esMasculino ? "NO" : "N/S";
            if (string.IsNullOrWhiteSpace(cuestionario.PA_Transfusion) || cuestionario.PA_Transfusion == "-") cuestionario.PA_Transfusion = "NO";
            if (string.IsNullOrWhiteSpace(cuestionario.PA_Asma) || cuestionario.PA_Asma == "-") cuestionario.PA_Asma = "NO";
            if (string.IsNullOrWhiteSpace(cuestionario.PA_Pulmones) || cuestionario.PA_Pulmones == "-") cuestionario.PA_Pulmones = "NO";
            if (string.IsNullOrWhiteSpace(cuestionario.PA_Corazon) || cuestionario.PA_Corazon == "-") cuestionario.PA_Corazon = "NO";
            if (string.IsNullOrWhiteSpace(cuestionario.PA_AtaqueCardiaco) || cuestionario.PA_AtaqueCardiaco == "-") cuestionario.PA_AtaqueCardiaco = "NO";
            if (string.IsNullOrWhiteSpace(cuestionario.PA_Angina) || cuestionario.PA_Angina == "-") cuestionario.PA_Angina = "NO";
            if (string.IsNullOrWhiteSpace(cuestionario.PA_Soplo) || cuestionario.PA_Soplo == "-") cuestionario.PA_Soplo = "NO";
            if (string.IsNullOrWhiteSpace(cuestionario.PA_Presion) || cuestionario.PA_Presion == "-") cuestionario.PA_Presion = "NO";
            if (string.IsNullOrWhiteSpace(cuestionario.PA_Higado) || cuestionario.PA_Higado == "-") cuestionario.PA_Higado = "NO";
            if (string.IsNullOrWhiteSpace(cuestionario.PA_Rinones) || cuestionario.PA_Rinones == "-") cuestionario.PA_Rinones = "NO";
            if (string.IsNullOrWhiteSpace(cuestionario.PA_Diabetes) || cuestionario.PA_Diabetes == "-") cuestionario.PA_Diabetes = "NO";
            if (string.IsNullOrWhiteSpace(cuestionario.PA_Epilepsia) || cuestionario.PA_Epilepsia == "-") cuestionario.PA_Epilepsia = "NO";
            if (string.IsNullOrWhiteSpace(cuestionario.PA_Derrame) || cuestionario.PA_Derrame == "-") cuestionario.PA_Derrame = "NO";
            if (string.IsNullOrWhiteSpace(cuestionario.PA_Tiroides) || cuestionario.PA_Tiroides == "-") cuestionario.PA_Tiroides = "NO";
            if (string.IsNullOrWhiteSpace(cuestionario.PA_Anestesico) || cuestionario.PA_Anestesico == "-") cuestionario.PA_Anestesico = "NO";
            if (string.IsNullOrWhiteSpace(cuestionario.PA_AceptaTransfusion) || cuestionario.PA_AceptaTransfusion == "-") cuestionario.PA_AceptaTransfusion = "SI";

            if (string.IsNullOrWhiteSpace(cuestionario.AI_Medicamentos))
                cuestionario.AI_Medicamentos = "Losartan 50 mg c/24h";
        }

        public static void CompletarListaChequeo(
            ListaChequeo lista,
            Hospitalizacion hospitalizacion,
            string medicoNombre = null)
        {
            if (lista == null)
                return;

            var paciente = hospitalizacion?.Paciente;
            var nombreParts = (paciente?.Nombre ?? "").Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            var primerNombre = nombreParts.Length > 0 ? nombreParts[0] : "Paciente";
            var restoNombre = nombreParts.Length > 1 ? nombreParts[1] : "";

            if (string.IsNullOrWhiteSpace(lista.MedicoTratante) && !string.IsNullOrWhiteSpace(medicoNombre))
                lista.MedicoTratante = medicoNombre;
            if (string.IsNullOrWhiteSpace(lista.NombrePaciente))
                lista.NombrePaciente = primerNombre;
            if (string.IsNullOrWhiteSpace(lista.ApellidoPaciente))
                lista.ApellidoPaciente = restoNombre;
            if (!lista.FechaNacimiento.HasValue)
                lista.FechaNacimiento = paciente?.FechaNacimiento;
            if (!lista.CI_FechaNacConfirma.HasValue)
                lista.CI_FechaNacConfirma = paciente?.FechaNacimiento ?? lista.FechaNacimiento;
            if (!lista.CP_FechaNacCirujano.HasValue)
                lista.CP_FechaNacCirujano = paciente?.FechaNacimiento ?? lista.FechaNacimiento;

            if (string.IsNullOrWhiteSpace(lista.CI_NombreConfirma) || lista.CI_NombreConfirma == "SI")
            {
                lista.CI_NombreConfirma = primerNombre;
                lista.CI_ApellidoConfirma = restoNombre;
            }

            if (string.IsNullOrWhiteSpace(lista.CI_Operacion) || lista.CI_Operacion == "SI")
            {
                var procedimiento = hospitalizacion?.Consultas?.FirstOrDefault()?.ConsultaMotivo
                    ?? "Artroplastia de rodilla izquierda";
                lista.CI_Operacion = procedimiento;
            }

            if (string.IsNullOrWhiteSpace(lista.CP_NombrePacienteCirujano) || lista.CP_NombrePacienteCirujano == "SI")
            {
                lista.CP_NombrePacienteCirujano = primerNombre;
                lista.CP_ApellidoPacienteCirujano = restoNombre;
            }

            if (string.IsNullOrWhiteSpace(lista.CP_NombreCirugia) || lista.CP_NombreCirugia == "SI")
                lista.CP_NombreCirugia = lista.CI_Operacion ?? "Artroplastia de rodilla izquierda";

            if (string.IsNullOrWhiteSpace(lista.CI_Consentimiento) || lista.CI_Consentimiento == "-") lista.CI_Consentimiento = "SI";
            if (string.IsNullOrWhiteSpace(lista.CI_LadoOperar) || lista.CI_LadoOperar == "-") lista.CI_LadoOperar = "Izquierda";
            if (string.IsNullOrWhiteSpace(lista.CI_SitioMarcado) || lista.CI_SitioMarcado == "-") lista.CI_SitioMarcado = "SI";
            if (string.IsNullOrWhiteSpace(lista.CI_Alergia) || lista.CI_Alergia == "-") lista.CI_Alergia = "NO";
            if (string.IsNullOrWhiteSpace(lista.CI_EvalPreanestesica) || lista.CI_EvalPreanestesica == "-") lista.CI_EvalPreanestesica = "SI";
            if (string.IsNullOrWhiteSpace(lista.CI_AccesoIV) || lista.CI_AccesoIV == "-") lista.CI_AccesoIV = "SI";
            if (string.IsNullOrWhiteSpace(lista.CI_EquipoAnestesia) || lista.CI_EquipoAnestesia == "-") lista.CI_EquipoAnestesia = "SI";
            if (string.IsNullOrWhiteSpace(lista.CI_Medicamentos) || lista.CI_Medicamentos == "-") lista.CI_Medicamentos = "SI";
            if (string.IsNullOrWhiteSpace(lista.CI_Oximetro) || lista.CI_Oximetro == "-") lista.CI_Oximetro = "SI";
            if (string.IsNullOrWhiteSpace(lista.CI_EquipoAspiracion) || lista.CI_EquipoAspiracion == "-") lista.CI_EquipoAspiracion = "SI";
            if (string.IsNullOrWhiteSpace(lista.CI_ViaAerea) || lista.CI_ViaAerea == "-") lista.CI_ViaAerea = "SI";
            if (string.IsNullOrWhiteSpace(lista.CP_Presentacion) || lista.CP_Presentacion == "-") lista.CP_Presentacion = "SI";
            if (string.IsNullOrWhiteSpace(lista.CP_EventosCriticos) || lista.CP_EventosCriticos == "-") lista.CP_EventosCriticos = "NO";
            if (string.IsNullOrWhiteSpace(lista.CP_Esterilidad) || lista.CP_Esterilidad == "-") lista.CP_Esterilidad = "SI";
            if (string.IsNullOrWhiteSpace(lista.CP_EventosCriticosAnest) || lista.CP_EventosCriticosAnest == "-") lista.CP_EventosCriticosAnest = "NO";
            if (string.IsNullOrWhiteSpace(lista.CP_ProfilaxisAntibiotica) || lista.CP_ProfilaxisAntibiotica == "-") lista.CP_ProfilaxisAntibiotica = "SI";
            if (string.IsNullOrWhiteSpace(lista.CP_Tromboprofilaxis) || lista.CP_Tromboprofilaxis == "-") lista.CP_Tromboprofilaxis = "SI";
            if (string.IsNullOrWhiteSpace(lista.CP_ManejoDolor) || lista.CP_ManejoDolor == "-") lista.CP_ManejoDolor = "SI";
            if (string.IsNullOrWhiteSpace(lista.CS_NombreOperacion) || lista.CS_NombreOperacion == "-") lista.CS_NombreOperacion = "SI";
            if (string.IsNullOrWhiteSpace(lista.CS_RecuentoCompleto) || lista.CS_RecuentoCompleto == "-") lista.CS_RecuentoCompleto = "SI";
            if (string.IsNullOrWhiteSpace(lista.CS_RepasoPostOp) || lista.CS_RepasoPostOp == "-") lista.CS_RepasoPostOp = "SI";
            if (string.IsNullOrWhiteSpace(lista.CS_Traslado) || lista.CS_Traslado == "-") lista.CS_Traslado = "SI";
            if (string.IsNullOrWhiteSpace(lista.CS_Complicaciones) || lista.CS_Complicaciones == "-") lista.CS_Complicaciones = "NO";
        }

        public static void EnriquecerConsentimientoRadiologia(
            ConsentimientoHospiVM consentimiento,
            Hospitalizacion hospitalizacion,
            CuestionarioPreAnestesico cuestionario = null)
        {
            if (consentimiento == null)
                return;

            var consulta = hospitalizacion?.Consultas?.FirstOrDefault();
            var sexo = consentimiento.Genero
                ?? consulta?.Citas?.Paciente?.Sexo?.DescripcionSexo
                ?? consulta?.Citas?.Paciente?.sexoText;
            var esMasculino = EsMasculino(sexo);

            consentimiento.RadiacionEmbarazada = NormalizarSiNo(
                consulta?.EstaEmbarazada ?? cuestionario?.PA_Embarazo,
                esMasculino ? "NO" : "N/S");
            consentimiento.RadiacionAmamantando = NormalizarSiNo(
                consulta?.EstaAmamantando,
                esMasculino ? "NO" : "N/S");

            if (string.IsNullOrWhiteSpace(consentimiento.RadiacionFUM))
            {
                if (esMasculino)
                    consentimiento.RadiacionFUM = "N/A";
                else if (cuestionario?.FechaUltimaRegla != null)
                    consentimiento.RadiacionFUM = cuestionario.FechaUltimaRegla.Value.ToString("dd/MM/yyyy");
            }

            if (!string.IsNullOrWhiteSpace(consentimiento.DPIResponsable)
                && string.Equals(consentimiento.DPIResponsable, consentimiento.DPI, StringComparison.OrdinalIgnoreCase)
                && (esMasculino || EsAdulto(consentimiento.Edad)))
            {
                consentimiento.DPIResponsable = null;
                consentimiento.NombreResponsable = null;
            }
        }

        private static bool EsMasculino(string sexo)
        {
            if (string.IsNullOrWhiteSpace(sexo))
                return false;

            return sexo.Contains("Masculino", StringComparison.OrdinalIgnoreCase)
                || string.Equals(sexo, "M", StringComparison.OrdinalIgnoreCase)
                || sexo.StartsWith("Masc", StringComparison.OrdinalIgnoreCase);
        }

        private static bool EsAdulto(string edadTexto)
        {
            if (string.IsNullOrWhiteSpace(edadTexto))
                return true;

            return int.TryParse(edadTexto.Trim(), out var edad) && edad >= 18;
        }

        private static string NormalizarSiNo(string valor, string fallback)
        {
            if (!string.IsNullOrWhiteSpace(valor) && valor != "-")
            {
                var v = valor.Trim();
                if (v.Equals("SI", StringComparison.OrdinalIgnoreCase)
                    || v.Equals("SÍ", StringComparison.OrdinalIgnoreCase)
                    || v.Equals("YES", StringComparison.OrdinalIgnoreCase))
                    return "SI";
                if (v.Equals("NO", StringComparison.OrdinalIgnoreCase))
                    return "NO";
                return v;
            }

            return fallback ?? "N/S";
        }
    }
}
