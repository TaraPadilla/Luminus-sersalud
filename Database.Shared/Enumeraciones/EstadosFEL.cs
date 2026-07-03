namespace Database.Shared.Enumeraciones
{
    public enum EstadosFEL
    {
        NoIniciado = 0,
        Pendiente = 1,
        Emitida = 2,
        Error = 3
        // Opcional si luego quieres bloquear concurrencia:
        // Emitiendo = 4
    }
}