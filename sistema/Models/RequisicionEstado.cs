namespace sistema.Models
{
    public enum RequisicionEstado
    {
        Borrador = 1,
        EnviadoAJefatura = 2,

        VistoBuenoJefatura = 3,
        VistoBuenoGerencia = 4,

        TrasladadoABodega = 5,

        EntregadoAKardex = 6,


        // VistoBuenoGerencia = 4,
        // EntregadoAKardex = 5,
        // DespachoAsignado = 6,
        // EnPreparacionBodega = 7,
        // EntregadoInsumos = 8,
        // DescargadoKardex = 9,
        // Finalizado = 10,
        // Rechazado = 99
    }
}
