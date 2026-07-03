using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace Database.Shared.Models
{
    public class ExamenFisicoPediatria
    {
        public int Id { get; set; }
        public string Temperatura { get; set; }
        public string FrecuenciaRespiratoria { get; set; }
        public string FrecuenciaCardiaca { get; set; }
        public string SaturacionDeOxigeno { get; set; }
        public string PresionArterialBrazoDerecho { get; set; }
        public string PresionArterialBrazoIzquierdo { get; set; }
        public string Observaciones { get; set; }
        public string Peso { get; set; }
        public string Glasgow { get; set; }
        public string Talla { get; set; }
        public string IMC { get; set; }
        public string Glucosa { get; set; }
        public string TensionArterialLpm { get; set; }
        public string TensionArterialMmhg { get; set; }
        //Opciones Cabeza:
        //ictericia-conjuntival,secrecion-conjuntival,
        //candidiasis-oral, eritema-oral,musositis,otros
        public string Cabeza { get; set; }
        public string CabezaOtros { get; set; }
        //Opciones Cuello:
        //ingurgitacion-yugular,adenopatias-palpables,
        //tiroides-palpable,otros
        public string Cuello { get; set; }
        public string CuelloOtros { get; set; }
        //Opciones Cardiaco:
        //arritmia,galope,taquicardia,soplo
        //otros
        public string Cardiaco { get; set; }
        public string CardiacoSoplo { get; set; }
        public string CardiacoOtros { get; set; }
        //Opciones Respiratorio
        //ruidos-disminuidos-d,estertores-finos-d,
        //estertores-gruesos-d,sibilancias-d,otros
        public string Respiratorio { get; set; }
        public string RespiratorioOtros { get; set; }
        //Opciones Abdomen:
        //masa-palpable,ascitis-leve-a-moderada,ascitis-a-tension,
        //rigidez-involuntaria,otros
        public string Abdomen { get; set; }
        public string AbdomenOtros { get; set; }
        //Opciones GU
        //edema-escrotal-vulvar,edema-perineal
        //otros
        public string Gu { get; set; }
        public string GuOtros { get; set; }
        //Opciones espalda:
        //dolor-persusion-apofisis-espinosas,giordano-d
        public string Espalda { get; set; }
        public string EspaldaDolorPersusionApofisis { get; set; }
        //Opciones Extremidades
        //edema-ms-ts,edema-ms-ps,fuerza-nl-excepto,
        //sensibilidad-nl-excepto,rem-nl-excepto
        public string Extremidades { get; set; }
        public string ExtremidadesFuerzaNlExcepto { get; set; }
        public string ExtremidadesSensibilidadNlExcepto { get; set; }
        public string ExtremidadesRemNlExcepto { get; set; }
        public string EstadoGeneral { get; set; }
        public string PresionArterial { get; set; }
        public string PediatricoPesoTalla { get; set; }
        public string PediatricoPesoEdad { get; set; }
        public string PediatricoTallaEdad { get; set; }
    }
}