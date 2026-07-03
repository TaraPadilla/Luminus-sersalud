namespace sistema.Models
{

    public class RecetaAplicacionViewModel
    {
        public int IdHospitalizacionReceta { get; set; }
        public int IdReceta { get; set; }
        public string NombreReceta { get; set; }
        public string Ingredientes { get; set; }
        public string Cantidad { get; set; }
        public string Indicaciones { get; set; }
        public bool Aplicado { get; set; }
        public string FechaHoraAplicacion { get; set; }
        public string PersonaAplica { get; set; }
        public string PersonaCrea { get; set; }

    }
}
