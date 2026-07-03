using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Database.Shared.Models
{
    public class ConsultaCaracteristicaDental
    {
        public int Id { get; set; }
        public int? ConsultaId { get; set; }
        public Consulta Consulta { get; set; }
        public int? NumeroDiente { get; set; }
        public bool Percusiones_VerticalMas { get; set; }
        public bool Percusiones_HorizontalMas { get; set; }
        public bool Percusiones_VerticalMenos { get; set; }
        public bool Percusiones_HorizontalMenos { get; set; }
        public bool Dolor_Localizado { get; set; }
        public bool Dolor_Fugaz { get; set; }
        public bool Dolor_Persistente { get; set; }
        public bool Dolor_Referido { get; set; }
        public bool Dolor_Espontaneo { get; set; }
        public bool Estimulo_Frio { get; set; }
        public bool Estimulo_Calor { get; set; }
        public bool Estimulo_DulceAcido { get; set; }
        public bool Estimulo_Masticacion { get; set; }
        public bool Estimulo_Otro { get; set; }
        public bool TermicaFrio_Positiva { get; set; }
        public bool TermicaFrio_Negativa { get; set; }
        public bool TermicaFrio_Localizada { get; set; }
        public bool TermicaFrio_Fugaz { get; set; }
        public bool TermicaFrio_Incrementa { get; set; }
        public bool TermicaFrio_Referida { get; set; }
        public bool TermicaFrio_Irradiado { get; set; }
        public bool TermicaFrio_Persistente { get; set; }
        public bool TermicaFrio_Decrece { get; set; }
        public bool TermicaCalor_Positiva { get; set; }
        public bool TermicaCalor_Negativa { get; set; }
        public bool TermicaCalor_Localizada { get; set; }
        public bool TermicaCalor_Fugaz { get; set; }
        public bool TermicaCalor_Incrementa { get; set; }
        public bool TermicaCalor_Referida { get; set; }
        public bool TermicaCalor_Irradiado { get; set; }
        public bool TermicaCalor_Persistente { get; set; }
        public bool TermicaCalor_Decrece { get; set; }
        public bool Diagnostico_ManchaBlanca { get; set; }
        public bool Diagnostico_Caries { get; set; }
        public bool Diagnostico_Traumatismo { get; set; }
        public bool Diagnostico_Abfraccion { get; set; }
        public bool Diagnostico_Atricion { get; set; }
        public bool Diagnostico_Erosion { get; set; }
        public bool Diagnostico_Restauracion { get; set; }
        public bool Diagnostico_Ajustada { get; set; }
        public bool Diagnostico_Desajustada { get; set; }
        public bool Diagnostico_PulpaSana { get; set; }
        public bool Diagnostico_PulpitisReversible { get; set; }
        public bool Diagnostico_PulpitisIrreversible { get; set; }
        public bool Diagnostico_NecrosisPulpar { get; set; }
    }
}