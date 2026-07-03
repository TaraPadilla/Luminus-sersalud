namespace Database.Shared.Enumeraciones
{
    public enum EstadoRecepcionEnum
    {
        NoIngresado = 1,
        Ingresado = 2
    }

    public enum EstadoExamenEnum
    {
        Solictiado = 1,
        EnProceso = 2,
        Cancelado = 3,
        Revision = 4,
        Finalizado = 5
    }
    public enum FormaPagoEnum
    {
        Efectivo = 1,
        TarjetaVisa = 2,
        TarjetaMastercard = 3,
        Cheques = 4,
        Transferencia = 5,
        VisaLink = 6,
        VisaNet = 7,
        Seguro = 8
    }
    public enum MonedasEnum
    {
        Dolar = 1,
        Quetzal = 2
    }
    public enum AccionPacienteEnum
    {
        Registro = 1,
        Retiro = 2
    }
    public enum EstadoPacienteEnum
    {
        Activo = 1,
        Inactivo = 2
    }
    public enum EstadoPagoConsultaEnum
    {
        Pagado = 1,
        Pendiente = 2,
        Cortesia = 3,
    }
    public enum TipoPacienteEnum
    {
        Nuevo = 1,
        Retomante = 2
    }
    public enum TipoMovimientoProductoEnum
    {
        Entrada = 1,
        Salida = 2,
        SalidaVenta = 3,
        EntradaCompra = 4
    }
    public enum TipificacionComunicacionEnum
    {
        Contactado = 1,
        Recontactado = 2,
        NuevoIngreso = 3
    }
    public enum TipoProductoEnum
    {
        Medicamentos = 1,
        InsumosMedicos = 2,
        EquiposMedicos = 3,
        EquiposQuirurgicos = 4,
        Suministros = 5
    }
    public enum TipoBodegaEnum
    {
        Farmacia = 1,
        Clinica = 2,
        Bodega = 3,
        Laboratorio = 4,
        Hospital = 5,
        Global = 6
    }
    public enum FaseTratamientoEnum
    {
        Adelgazamiento = 1,
        Mantenimiento1 = 2,
        Mantenimiento2 = 3,
        Mantenimiento3 = 4
    }
    public enum ValorMembresiaEnum
    {
        Membresia = 800
    }
    public enum EstadoHabitacionEnum
    {
        Disponible = 1,
        Ocupada = 2,
        EnLimpieza = 3
    }
    public enum CompraTipoDocumentoEnum
    {
        OrdenCompra = 1,
        Compra = 2
    }
    public enum EstadoTrasladoEnum
    {
        Aceptado = 1,
        Denegado = 2,
        ConProblema = 3,
        Faltantes = 4,
        EnTransito = 5
    }
    public enum AmbienteEnum
    {
        Farmacia = 1,
        Clinica = 2,
        Hospital = 3,
        Laboratorio = 4,
        Bodega = 5,
        Global = 6
    }

    public enum PosicionPacienteTab
    {
        DatosPaciente = 1,
        AntFamiliares = 2,
        AntPatologicos = 3,
        MotivoConsultaHistoriaEnfermedad = 4,
        PadecimientoActual = 5,
        ExamenFisico = 6,
        GabinetesPrevios = 7,
        Angologia = 8,
        PoliticasPago = 9,
        DeclaracionConsentimiento = 10,
        DatosPago = 11,
    }
    public enum TipoCompraEnum
    {
        Contado = 1,
        Credito = 2
    }
}
