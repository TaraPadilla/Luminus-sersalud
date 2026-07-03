using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class ConsultasOftalmologia
    {
        public long Id { get; set; }
        public int ConsultaId { get; set; }

        public int PacienteId { get; set; }

        // Motivo / Antecedentes
        public string HistoriaEnfermedadActual { get; set; }
        public string PacienteMedicos { get; set; }
        public string PacienteQuirurgicos { get; set; }
        public string PacienteTraumaticos { get; set; }
        public string PacienteAlergias { get; set; }
        public string PacienteFamiliares { get; set; }

        // Datos objetivos
        // AV sin corrección
        public string AgudezaSC_Test { get; set; }   // enum en BD → string en modelo
        public string AgudezaSC_OD { get; set; }     // enum en BD → string en modelo
        public string AgudezaSC_OS { get; set; }     // enum en BD → string en modelo

        // Sensibilidad de contraste
        public string Contraste_OD { get; set; }
        public string Contraste_OS { get; set; }

        // AV cerca sin corrección
        public string AVCerca_OD { get; set; }       // enum en BD → string en modelo
        public string AVCerca_OS { get; set; }       // enum en BD → string en modelo

        // Tests especiales
        public string TestIshihara_OD { get; set; }
        public string TestIshihara_OS { get; set; }
        public string TestEstereopsis_OD { get; set; }
        public string TestEstereopsis_OS { get; set; }

        // Óptica - Lensometría (Histórico)
        public decimal? Lensometria_OD_Esfera { get; set; }
        public decimal? Lensometria_OD_Cilindro { get; set; }
        public int? Lensometria_OD_Eje { get; set; }
        public string Lensometria_OD_Agudeza { get; set; }

        public decimal? Lensometria_OS_Esfera { get; set; }
        public decimal? Lensometria_OS_Cilindro { get; set; }
        public int? Lensometria_OS_Eje { get; set; }
        public string Lensometria_OS_Agudeza { get; set; }

        // Óptica - Final
        public decimal? Final_OD_Esfera { get; set; }
        public decimal? Final_OD_Cilindro { get; set; }
        public int? Final_OD_Eje { get; set; }
        public string Final_OD_Agudeza { get; set; }

        public decimal? Final_OS_Esfera { get; set; }
        public decimal? Final_OS_Cilindro { get; set; }
        public int? Final_OS_Eje { get; set; }
        public string Final_OS_Agudeza { get; set; }

        public decimal? Final_Adicion { get; set; }
        public decimal? Final_DIP_mm { get; set; }

        // Óptica - Retinoscopía
        public decimal? Retino_OD_Esfera { get; set; }
        public decimal? Retino_OD_Cilindro { get; set; }
        public int? Retino_OD_Eje { get; set; }

        public decimal? Retino_OS_Esfera { get; set; }
        public decimal? Retino_OS_Cilindro { get; set; }
        public int? Retino_OS_Eje { get; set; }

        // Tipo de lente / Material
        public string TipoLente { get; set; }                 // enum en BD → string en modelo
        public string LenteMaterialTratamiento { get; set; }

        // Inspección / LH / Oftalmoscopía
        public string Inspeccion_MovExtraoculares_OD { get; set; } // enum en BD → string
        public string Inspeccion_MovExtraoculares_OS { get; set; }
        public string Inspeccion_Cejas_OD { get; set; }
        public string Inspeccion_Cejas_OS { get; set; }
        public string Inspeccion_ParpadosPestanas_OD { get; set; }
        public string Inspeccion_ParpadosPestanas_OS { get; set; }
        public string Inspeccion_ViaLagrimal_OD { get; set; }
        public string Inspeccion_ViaLagrimal_OS { get; set; }

        // Segmento anterior
        public string Inspeccion_Conjuntiva_OD { get; set; }
        public string Inspeccion_Conjuntiva_OS { get; set; }
        public string Inspeccion_CorneaEsclera_OD { get; set; }
        public string Inspeccion_CorneaEsclera_OS { get; set; }
        public string Inspeccion_CamaraAnteriorAngulo_OD { get; set; }
        public string Inspeccion_CamaraAnteriorAngulo_OS { get; set; }
        public string Inspeccion_IrisPupila_OD { get; set; }
        public string Inspeccion_IrisPupila_OS { get; set; }
        public string Inspeccion_Cristalino_OD { get; set; }
        public string Inspeccion_Cristalino_OS { get; set; }
        public string Inspeccion_BUT_OD { get; set; }
        public string Inspeccion_BUT_OS { get; set; }
        public decimal? Inspeccion_PresionIntraocular_OD { get; set; }
        public decimal? Inspeccion_PresionIntraocular_OS { get; set; }

        // Segmento posterior
        public string Inspeccion_Vitreo_OD { get; set; }
        public string Inspeccion_Vitreo_OS { get; set; }
        public string Inspeccion_NervioOptico_OD { get; set; }
        public string Inspeccion_NervioOptico_OS { get; set; }
        public string Inspeccion_Macula_OD { get; set; }
        public string Inspeccion_Macula_OS { get; set; }
        public string Inspeccion_Retina_OD { get; set; }
        public string Inspeccion_Retina_OS { get; set; }

        // Impresión clínica / Tratamiento
        public string HistoriaClinicaImpresionClinica { get; set; }
        public string HistoriaClinicaComentario { get; set; }

        //Fecha
        public DateTime Fecha { get; set; }
}
};










