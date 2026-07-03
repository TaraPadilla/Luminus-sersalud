namespace farmamest.Models
{
    public class HistorialMedicamentoPdfRow
    {
        public int AplicacionId { get; set; }
        public int? HospitalizacionProductoId { get; set; }
        public bool Aplicado { get; set; }
        public string Origen { get; set; }
        public string Nombre { get; set; }
        public string Indicaciones { get; set; }
        public string Via { get; set; }
        public string Frecuencia { get; set; }
        public decimal CantidadRegistrada { get; set; }
        public decimal CantidadAplicada { get; set; }
        public string Estado { get; set; }
        public string FechaAplicacion { get; set; }
        public string HoraAplicacion { get; set; }
        public string AplicadoPor { get; set; }
        public string FechaRegistro { get; set; }
        public string MotivoDevolucion { get; set; }
    }
}
