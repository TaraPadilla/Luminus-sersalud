using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class Persona
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int? SexoId { get; set; }
        public Sexo Sexo { get; set; }
        public int? TipificacionComunicacionId { get; set; }
        public TipificacionComunicacion TipificacionComunicacion { get; set; }
        public string Telefono { get; set; }
        public int? TipoRedSocialId { get; set; }
        public TipoRedSocial TipoRedSocial { get; set; }
        public string Email { get; set; }
        public DateTime? FechaContacto { get; set; }
        public bool? Paciente { get; set; }
        public bool? TomaServicio { get; set; }
        public string MotivoNoTomarServicio { get; set; }
        public bool Eliminada { get; set; }
    }
}