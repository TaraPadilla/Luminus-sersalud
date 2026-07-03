namespace sistema.Models
{
    public class HospitalizacionRecetaViewModel
    {
        public int Id { get; set; }
        public int HospitalizacionId { get; set; }
        public string Cantidad { get; set; }
        public string Indicaciones { get; set; }
    }
}
