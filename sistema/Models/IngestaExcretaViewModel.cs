using Database.Shared.Models;

namespace farmamest.Models
{
    public class IngestaExcretaViewModel
    {
        public int Id { get; set; }
        public string FechaRegistro { get; set; }
        public string IngestaIV { get; set; }
        public string IngestaIV2 { get; set; }
        public string IngestaIV3 { get; set; }
        public string IngestaIV4 { get; set; }
        public string IngestaIV5 { get; set; }
        public string IngestaIV6 { get; set; }
        public string IngestaPO { get; set; }
        public string TotalIngesta { get; set; }
        public string Excreta { get; set; }
        public string Orina { get; set; }
        public string Heces { get; set; }
        public string Vomito { get; set; }
        public string Sudoracion { get; set; }
        public string Drenajes { get; set; }
        public string OtrosLiquidos { get; set; }
        public string CuantasHoras { get; set; }
        public string UserId { get; set; }
        /// <summary>
        /// Este es el nombre de la enfemera, la persona que realiza el registro
        /// </summary>
        public string Enfermeria { get; set; }
        public int HospitalizacionId { get; set; }

        public bool Autorizado { get; set; }
        public string UsuarioAutoriza { get; set; }
        public string FechaAutorizacion { get; set; }
    }
}
