

namespace farmamest.Models
{
    public class CuentaContableViewModel
    {
        public int Id { get; set; }
        public string NombreCuenta { get; set; }
        public string Especificaciones { get; set; }
        public int BancoId { get; set; }
        public int CuentaId { get; set; }
        public int NomenclaturaId { get; set; }
        public int CategoriaCuentaId { get; set; }

    }
}
