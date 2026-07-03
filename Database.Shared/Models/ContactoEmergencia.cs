namespace Database.Shared.Models
{
    public class ContactoEmergencia
    {
        public int Id { get; set; }
        public int ConsentimientoHospiId { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Parentesco { get; set; }

        // Navegación
        public virtual ConsentimientoHospi ConsentimientoHospi { get; set; }
    }
}