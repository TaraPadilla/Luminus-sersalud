namespace sistema.Models
{
    public class MunicipiosViewModel
    {
        public int? Id { get; set; }
        public string NombreMunicipio { get; set; }

        public int? DepartamentoId { get; set; }

        public string MunicipioNombreMostrar
        {
            get
            {
                return $"{Id} - {NombreMunicipio}";
            }

        }

       
    }
}
