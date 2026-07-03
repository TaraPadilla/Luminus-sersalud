namespace sistema.Models
{
    public class CuestionarioPreAnestesicoViewModel
    {
        public int Id { get; set; }
        public int? HospitalizacionId { get; set; }
        public string pacienteRegistro { get; set; }
        public string pacienteNombre { get; set; }
        public string pacienteEdad { get; set; }
        public string fechaCuestionario { get; set; }
        public decimal? peso { get; set; }
        public decimal? estatura { get; set; }
        public string fechaUltimaRegla { get; set; }
        public string fechaProcedimiento { get; set; }
        public string procedimientoProgramado { get; set; }
        public string cirujano { get; set; }
        public string pa_alergia { get; set; }
        public string pa_alergiaCual { get; set; }
        public string pa_fuma { get; set; }
        public string pa_fumaCuanto { get; set; }
        public string pa_drogas { get; set; }
        public string pa_drogasCuales { get; set; }
        public string pa_alcohol { get; set; }
        public string pa_alcoholCuanto { get; set; }
        public string pa_embarazo { get; set; }
        public string pa_transfusion { get; set; }
        public string pa_asma { get; set; }
        public string pa_pulmones { get; set; }
        public string pa_corazon { get; set; }
        public string pa_ataqueCardiaco { get; set; }
        public string pa_angina { get; set; }
        public string pa_soplo { get; set; }
        public string pa_presion { get; set; }
        public string pa_higado { get; set; }
        public string pa_rinones { get; set; }
        public string pa_diabetes { get; set; }
        public string pa_epilepsia { get; set; }
        public string pa_derrame { get; set; }
        public string pa_tiroides { get; set; }
        public string pa_anestesico { get; set; }
        public string pa_aceptaTransfusion { get; set; }
        public string ai_medicamentos { get; set; }
        public string ai_actividad { get; set; }
        public string ai_actividadDetalle { get; set; }
        public string ai_operacionesPrevias { get; set; }
        public string ai_comentarios { get; set; }

        public int pacienteId { get; set; }
        public int habitacionId { get; set; }
        public int hospitalizacionId { get; set; }

    }
}
