namespace sistema.Models
{
    public class HospitalizacionServicioViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Subtotal { get; set; }
        public bool Aplicado { get; set; }
        public bool AgregadoConsulta { get; set; }
        public string FechaHoraAplicacion { get; set; }
        public string PersonaAplica { get; set; }
        public string PersonaCrea { get; set; }

        public string AplicadoText
        {
            get
            {
                return Aplicado ? "Si" : "No";
            }
        }
    }
}
