using System;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Database.Shared.IRepository;
using Database.Shared.Data;
using Microsoft.EntityFrameworkCore.Internal;
using Database.Shared.Dto;
using Database.Shared.SqlDataSeed;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Database.Shared
{
    public partial class Context : IdentityDbContext<IdentityUser>
    {
        public string ConnectionString { get; set; }
        public Context(DbContextOptions<Context> options) : base(options)
        {
            // repo = new EmpleadoRepository(this);
            // this.Database.EnsureCreated();
            // this.Database.EnsureCreated();
        }
        public DbSet<Ambiente> Ambientes { get; set; }
        public DbSet<ConfiguracionSistema> ConfiguracionesSistema { get; set; }
        public DbSet<OrdenMedica> OrdenMedica { get; set; }
        public DbSet<OrdenesMedicas> OrdenesMedicas { get; set; }
        public DbSet<WebAuthnCredential> WebAuthnCredentials { get; set; }

        public DbSet<HospitalizacionCambioHabitacion> HospitalizacionCambiosHabitacion { get; set; }

        public DbSet<MovimientoProducto> MovimientosProducto { get; set; }

        public DbSet<MovimientoProductoNacional> MovimientosProductoNacional { get; set; }

        public DbSet<TipoMovimientoProducto> TipoMovimientoProducto { get; set; }
        public DbSet<AlergiaRara> AlergiaRaras { get; set; }
        public DbSet<AlergiaRaraPaciente> AlergiaRaraPacientes { get; set; }
        public DbSet<AccionPaciente> AccionesPaciente { get; set; }
        public DbSet<CategoriaHabitacion> CategoriasHabitaciones { get; set; }
        public DbSet<CategoriaHabitacionTarifa> CategoriaHabitacionTarifas { get; set; }
        public DbSet<Habitacion> Habitaciones { get; set; }

        public DbSet<EstadoHabitacion> EstadosHabitacion { get; set; }
        public DbSet<PaqueteHospitalizacion> PaquetesHospitalizacion { get; set; }
        public DbSet<Hospitalizacion> Hospitalizaciones { get; set; }
        public DbSet<HospitalizacionServicio> HospitalizacionesServicios { get; set; }
        public DbSet<HospitalizacionProducto> HospitalizacionesProductos { get; set; }
        public DbSet<HospitalizacionProductoAplicacion> HospitalizacionesProductosAplicaciones { get; set; }
        public DbSet<HospitalizacionProductoObservacion> HospitalizacionesProductosObservaciones { get; set; }
        public DbSet<HospitalizacionExamen> HospitalizacionesExamenes { get; set; }
        public DbSet<HospitalizacionPaqueteHospitalizacion> HospitalizacionesPaquetesHospitalizacion { get; set; }
        public DbSet<HospitalizacionRecetaDetalle> HospitalizacionRecetaDetalle { get; set; }
        public DbSet<ExamenFisicoHosp> ExamenesFisicosHosp { get; set; }
        public DbSet<ExamenFisicoHospDato> ExamenesFisicosHospDatos { get; set; }
        public DbSet<DatoExamenFisicoHosp> DatosExamenFisicoHosp { get; set; }
        public DbSet<PresupuestoDental> PresupuestosDentales { get; set; }
        public DbSet<PresupuestoDentalDetalle> PresupuestosDentalesDetalles { get; set; }


        public DbSet<HospitalizacionInsumoDirecto> HospitalizacionInsumosDirectos { get; set; }
        public DbSet<HospitalizacionInsumoDirectoAplicacion> HospitalizacionInsumosDirectosAplicaciones { get; set; }

        public DbSet<Paciente> Pacientes { get; set; }

        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Municipio> Municipios { get; set; }
        public DbSet<PacienteHistorial> PacientesHistorial { get; set; }
        public DbSet<PacienteArchivo> PacienteArchivos { get; set; }
        public DbSet<EstadoPaciente> EstadosPaciente { get; set; }
        public DbSet<TipoPaciente> TiposPacientes { get; set; }
        public DbSet<PacienteSeguimientoNutricional> PacientesSeguimientosNutricionales { get; set; }
        public DbSet<PacienteRangoSaludable> PacientesRangosSaludables { get; set; }
        public DbSet<PacienteResultadoExamenLaboratorio> PacientesResultadosExamenesLaboratorio { get; set; }
        public DbSet<PacienteAntecedentePersonal> PacientesAntecedentesPersonales { get; set; }
        public DbSet<PacienteApnp> PacienteApnp { get; set; }
        public DbSet<PacientePediatricoApnp> PacientePediatricoApnp { get; set; }
        public DbSet<AntecedentePersonal> AntecedentesPersonales { get; set; }
        public DbSet<PacientePreguntaRegistro> PacientesPreguntasRegistro { get; set; }
        public DbSet<PreguntaRegistro> PreguntasRegistro { get; set; }
        public DbSet<SeguroEpss> SegurosEpss { get; set; }
        public DbSet<CuentaPorCobrar> CuentasPorCobrar { get; set; }
        public DbSet<DetalleCuentaPorCobrar> DetallesCuentaPorCobrar { get; set; }
        public DbSet<TipoPatologia> TipoPatologias { get; set; }
        public DbSet<PatologiaPaciente> PatologiasPacientes { get; set; }
        public DbSet<Clientes> Clientes { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Bodega> Bodegas { get; set; }
        public DbSet<ProductoInventario> ProductosInventario { get; set; }
        public DbSet<ProductoInventarioPrecio> ProductosInventarioPrecios { get; set; }
        public DbSet<UnidadMedidaCompra> UnidadesMedidaCompra { get; set; }
        public DbSet<UnidadMedidaVenta> UnidadesMedidaVenta { get; set; }
        public DbSet<ProductoEquivalencia> ProductosEquivalencias { get; set; }
        public DbSet<ViaAdministracions> ViaAdministracions { get; set; }
        public DbSet<Viadmin> viadmins { get; set; }
        public DbSet<TipoProducto> TipoProductos { get; set; }
        public DbSet<PresentacionProducto> PresentacionProductos { get; set; }
        public DbSet<GrupoTProducto> GrupoTProductos { get; set; }
        public DbSet<LaboratorioProducto> LaboratorioProductos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Grupo> Grupos { get; set; }
        public DbSet<VentaPerdida> VentasPerdidas { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetalleVentas { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<TipoGastoProrrateo> TiposGastosProrrateo { get; set; }
        public DbSet<CategoriaCompra> CategoriasCompras { get; set; }
        public DbSet<VisitadorMedico> VisitadorMedico { get; set; }
        public DbSet<MedicamentoOtroConsulta> MedicamentosOtrosConsulta { get; set; }

        #region DbSET Ordenes de compra
        public DbSet<OrdenCompra> OrdenesCompra { get; set; }
        public DbSet<DetalleOrdenCompra> DetalleOrdenesCompra { get; set; }
        public DbSet<DetalleOrdenCompraUbicacion> DetalleOrdenesCompraUbicaciones { get; set; }
        public DbSet<DetalleOrdenCompraUbicacionPrecio> DetalleOrdenesCompraUbicacionesPrecios { get; set; }
        #endregion
        #region DbSET Compras
        public DbSet<Compra> Compras { get; set; }
        public DbSet<CompraTipoDocumento> CompraTiposDocumento { get; set; }
        public DbSet<TipoCompra> TipoCompra { get; set; }
        public DbSet<DetalleCompra> DetalleCompras { get; set; }
        public DbSet<DetalleCompraUnidadVentaPrecio> DetalleComprasUnidadesVentaPrecio { get; set; }
        public DbSet<DetalleCompraUbicacion> DetalleCompraUbicaciones { get; set; }
        public DbSet<DetalleCompraUbicacionPrecio> DetalleCompraUbicacionesPrecios { get; set; }
        #endregion
        public DbSet<Recepcion> Recepciones { get; set; }
        public DbSet<EstadoRecepcion> EstadoRecepciones { get; set; }
        public DbSet<Precio> Precios { get; set; }
        public DbSet<CategoriaServicio> CategoriasServicios { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<ServicioInsumo> ServiciosInsumos { get; set; }
        public DbSet<ServicioPrecio> ServiciosPrecios { get; set; }
        //public DbSet<VentaServicio> VentaServicios { get; set; }
        //public DbSet<DetalleServicio> DetalleServicios { get; set; }
        public DbSet<Banco> Bancos { get; set; }
        public DbSet<Seguro> Seguros { get; set; }
        public DbSet<Envio> Envios { get; set; }
        public DbSet<EstadosEnvio> EstadosEnvio { get; set; }
        public DbSet<DetalleEnvio> DetalleEnvios { get; set; }
        public DbSet<Ruta> Rutas { get; set; }
        public DbSet<DetalleCaja> DetalleCajas { get; set; }
        public DbSet<Caja> Cajas { get; set; }
        public DbSet<Gasto> Gastos { get; set; }
        public DbSet<CategoriaGasto> CategoriasGastos { get; set; }
        public DbSet<Pagos> Pagos { get; set; }
        public DbSet<FormaPago> FormaPagos { get; set; }
        public DbSet<Moneda> Monedas { get; set; }
        public DbSet<User> Usuarios { get; set; }
        public DbSet<Cotizacion> Cotizaciones { get; set; }
        public DbSet<CotizacionesPreOrden> CotizacionesPreOrden { get; set; }

        public DbSet<Ambulancia> Ambulancias { get; set; }
        public DbSet<DetalleCotizacion> DetalleCotizaciones { get; set; }
        public DbSet<Emergencia> Emergencias { get; set; }
        public DbSet<EmergenciaDetalle> EmergenciaDetalles { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<Citas> Citass { get; set; }
        public DbSet<CitasServicio> CitasServicios { get; set; }
        public DbSet<CalendarioFechaBloqueada> CalendarioFechasBloqueadas { get; set; }
        public DbSet<Consulta> Consultas { get; set; }
        public DbSet<ConsultaServicio> ConsultasServicios { get; set; }
        public DbSet<ConsultaExamenLabClinico> ConsultasExamenLabClinicos { get; set; }
        public DbSet<Archivo> Archivos { get; set; }
        public DbSet<ConsultaExamenArchivo> ConsultaExamenArchivo { get; set; }
        public DbSet<ConsultaCaracteristicaDental> ConsultasCaracteristicasDentales { get; set; }
        public DbSet<ConsultaRevisionSistemas> ConsultaRevisionSistemas { get; set; }
        public DbSet<ConsultaRevisionSistemasPediatria> ConsultaRevisionSistemasPediatria { get; set; }
        public DbSet<EstadoPagoConsulta> EstadoPagoConsultas { get; set; }
        public DbSet<Prescripcion> Prescripciones { get; set; }
        public DbSet<DetallePrescripcion> DetallePrescripcion { get; set; }
        public DbSet<Historia> Historia { get; set; }
        public DbSet<HistoriaPediatria> HistoriaPediatria { get; set; }
        public DbSet<ExamenFisico> ExamenFisico { get; set; }
        public DbSet<ExamenFisicoPediatria> ExamenFisicoPediatria { get; set; }
        public DbSet<ConsultaExamenFisicoGinecologia> ConsultaExamenFisicoGinecologia { get; set; }
        public DbSet<ConsultaAntPatologicosGinecologia> ConsultaAntPatologicosGinecologia { get; set; }
        public DbSet<ConsultaAntNoPatologicosGinecologia> ConsultaAntNoPatologicosGinecologia { get; set; }
        public DbSet<ConsultaAntNoPatologicosObstetricia> ConsultaAntNoPatologicosObstetricia { get; set; }


        public DbSet<TipoEspecialidad> TipoEspecialidad { get; set; }
        public DbSet<Especialidad> Especialidad { get; set; }
        public DbSet<Sexo> Sexo { get; set; }
        public DbSet<TrasladosProductos> TrasladosProductos { get; set; }
        public DbSet<EstadoTraslados> EstadoTraslados { get; set; }
        public DbSet<DetalleTrasladoProductos> DetalleTrasladoProductos { get; set; }
        public DbSet<TipoBodega> TipoBodegas { get; set; }
        //public DbSet<CajaClinica> CajaClinicas { get; set; }
        //public DbSet<DetalleCajaClinica> DetalleCajaClinicas { get; set; }

        // ==========================================================
        // Requisición (nuevo proceso)
        // ==========================================================
        public DbSet<Requision> Requision { get; set; }
        public DbSet<RequisionDetalle> RequisionDetalle { get; set; }
        public DbSet<RequisionHistorial> RequisionHistorial { get; set; }

        public DbSet<DepartamentoOrg> DepartamentosOrg { get; set; }
        public DbSet<UnidadOrg> UnidadesOrg { get; set; }
        public DbSet<SeccionOrg> SeccionesOrg { get; set; }


        public DbSet<DevolucionNacional> DevolucionNacional { get; set; }
        public DbSet<DevolucionNacionalDetalle> DevolucionNacionalDetalle { get; set; }


        // laboratorio clinico

        public DbSet<CategoriaLabClinico> CategoriaLabClinicos { get; set; }
        public DbSet<CategoriaGeneralLabClinico> CategoriaGeneralLabClinico { get; set; }

        public DbSet<ExamenLabClinico> ExamenLabClinicos { get; set; }
        public DbSet<ExamenLabClinicoPrecio> ExamenLabClinicosPrecios { get; set; }

        public DbSet<ExamenLabClinicoPregunta> ExamenLabClinicosPreguntas { get; set; }
        public DbSet<DatosExamenesLabClinico> DatosExamenesLabClinicos { get; set; }
        public DbSet<DatoTipoExamenLabClinico> DatoTipoExamenLabClinico { get; set; }//Alamcena los tipos de taso del examen
        public DbSet<DetalleExamen> DetalleExamenes { get; set; }
        public DbSet<Examen> Examenes { get; set; }
        //public DbSet<VentasLab> VentasLabs { get; set; }
        public DbSet<EstadoExamen> EstadoExamenes { get; set; }
        //public DbSet<DetalleCajaLab> DetalleCajaLab { get; set; }
        //public DbSet<CajaLab> CajaLab { get; set; }
        public DbSet<Medicos> Medicos { get; set; }
        public DbSet<Clinica> Clinicas { get; set; }
        public DbSet<Sucursal> Sucursales { get; set; }
        public DbSet<SucursalServicio> SucursalesServicios { get; set; }
        public DbSet<Resultados> Resultados { get; set; }
        public DbSet<Vacuna> Vacunas { get; set; }
        public DbSet<VacunaPaciente> VacunasPacientes { get; set; }
        public DbSet<PersonaSeguro> PersonasSeguro { get; set; }
        //Personas
        public DbSet<TipoRedSocial> TiposRedSocial { get; set; }
        public DbSet<Persona> Personas { get; set; }
        public DbSet<TipificacionComunicacion> TipificacionesComunicacion { get; set; }
        public DbSet<FaseTratamiento> FasesTratamiento { get; set; }
        public DbSet<PacienteFaseTratamiento> PacientesFasesTratamiento { get; set; }
        public DbSet<Grabacion> Grabaciones { get; set; }
        // public DbSet<Role> Roles { get; set; }
        // public DbSet<IdentityUser> IdentityUsers {get;set;}
        //Repositorio de archivos
        public DbSet<RepositorioCarpeta> RepositorioCarpetas { get; set; }
        public DbSet<RepositorioArchivo> RepositorioArchivos { get; set; }
        public DbSet<PacientesInformacionExtra> PacientesInformacionExtra { get; set; }
        public DbSet<ExamenLabClinicoInsumo> ExamenLabClinicoInsumo { get; set; }
        public DbSet<CitasExamenes> CitasExamenes { get; set; }

        //Receta
        public DbSet<Receta> Recetas { get; set; }
        public DbSet<CategoriaReceta> CategoriasRecetas { get; set; }
        public DbSet<RecetaMenu> RecetasMenu { get; set; }
        public DbSet<RecetaMenuRelacion> RecetasMenuRelacion { get; set; }

        public DbSet<HospitalizacionReceta> HospitalizacionesReceta { get; set; }
        public DbSet<BancoTrazabilidad> BancoTrazabilidad { get; set; }

        //Cuentas
        public DbSet<Cuentas> Cuentas { get; set; }
        public DbSet<CuentasTrazabilidad> CuentasTrazabilidad { get; set; }
        public DbSet<TipoCuenta> TipoCuenta { get; set; }
        public DbSet<CategoriasCuentaContable> CategoriasCuentaContables { get; set; }
        public DbSet<CuentaContable> CuentaContable { get; set; }
        public DbSet<CuentaContableTrazabilidad> CuentaContableTrazabilidad { get; set; }
        public DbSet<Nomenclatura> Nomenclaturas { get; set; }
        public DbSet<CuentaContableNomenclatura> CuentaContableNomenclaturas { get; set; }
        public DbSet<DetallePaqueteHospitalizacion> DetallePaqueteHospitalizacion { get; set; }
        public DbSet<HospitalizacionDetallePaqueteHospitalizacion> HospitalizacionDetallePaqueteHospitalizacion { get; set; }

        //Auditoria
        public DbSet<Auditoria> Auditoria { get; set; }
        public DbSet<AuditoriaProducto> AuditoriaProducto { get; set; }
        public DbSet<AuditoriaNuevoSP> AuditoriaNuevoSP { get; set; }
        public DbSet<VentaVentaUnificadaProductoExistenteSP> VentaVentaUnificadaProductoExistenteSP { get; set; }
        public DbSet<ExamenLabClinicosSP> ExamenLabClinicosSP { get; set; }
        public DbSet<TipoCompraProveedor> TipoCompraProveedor { get; set; }
        public DbSet<ControlGlucometria2> ControlGlucometria2 { get; set; }
        public DbSet<DetalleControlGlucometria2> DetalleControlGlucometria2 { get; set; }
        public DbSet<NotaEnfermeria2> NotaEnfermeria2 { get; set; }
        public DbSet<TurnoEnfermeria> TurnoEnfermeria { get; set; }
        public DbSet<NotaEvolucion> NotaEvolucion { get; set; }

        public DbSet<NotaOperatoria> NotaOperatoria { get; set; }

        public DbSet<IngestaExcreta2> IngestaExcreta2 { get; set; }
        //public DbSet<HospitalizacionRecetaDetalle> HospitalizacionRecetaDetalle { get; set; }
        public DbSet<InfoIngesta> InfoIngesta { get; set; }
        public DbSet<NotaMedica2> NotaMedica2 { get; set; }
        public DbSet<HospitalizacionUsuarioAcceso> HospitalizacionUsuarioAcceso { get; set; }
        public DbSet<ConsentimientoHospi> ConsentimientoHospi { get; set; }

        #region DtoSPs

        public DbSet<DtoSpInventarioProductos> DtoSpInventarioProductos { get; set; }

        public DbSet<DtoSpGetServicios> DtoSpGetServicios { get; set; }
        public DbSet<DtoSpGetVentasProductoAnnio> DtoSpGetVentasProductoAnnio { get; set; }
        public DbSet<DtoSpGetComprasProducto> DtoSpGetComprasProducto { get; set; }
        public DbSet<DtoSpGetProductosMasVendidos> DtoSpGetProductosMasVendidos { get; set; }
        public DbSet<DtoSpGetProductosMenosVendidos> DtoSpGetProductosMenosVendidos { get; set; }

        #endregion

        public DbSet<SolicitudMedicamento> SolicitudMedicamento { get; set; }

        public DbSet<SolicitudMedicamentoNacional> SolicitudMedicamentoNacional { get; set; }

        public DbSet<DevolucionMedicamento> DevolucionMedicamento { get; set; }
        public DbSet<HospitalizacionExamenPdf> HospitalizacionExamenPdf { get; set; }

        public DbSet<ListaChequeo> ListasChequeo { get; set; }

        public DbSet<CuestionarioPreAnestesico> CuestionariosPreAnestesicos { get; set; }

        public DbSet<KitIngreso> KitsIngreso { get; set; }
        public DbSet<KitIngresoDetalle> KitsIngresoDetalles { get; set; }

        public DbSet<HospitalizacionKitConsumo> HospitalizacionKitConsumo { get; set; }

        public DbSet<Cie10> Cie10 { get; set; }

        #region  Oftalmologia, podologi, HistoriaClinicaEnfermeria
        // Agregar este DbSet
        public DbSet<ConsultasOftalmologia> ConsultasOftalmologia { get; set; }

        public DbSet<ConsultasPodologia> ConsultasPodologia { get; set; }

        public DbSet<ConsultasHistoriaClinicaEnfermeria> ConsultasHistoriaClinicaEnfermeria { get; set; }

        #endregion

        #region ValoracionInicialEnfermeria, Sueroterapia
        public DbSet<ConsultasValoracionInicialEnfermeria> ConsultasValoracionInicialEnfermeria { get; set; }

        public DbSet<ConsultasSueroterapia> ConsultasSueroterapia { get; set; }
        #endregion 

        #region WebAuthn
        public DbSet<WebAuthnRegistrationToken> WebAuthnRegistrationTokens { get; set; }
        #endregion

        public DbSet<DocumentosHospitalizacion> DocumentosHospitalizacion { get; set; }

        public DbSet<MedicamentoNoControlado> MedicamentosNoControlado { get; set; }

        public DbSet<Notificacion> Notificaciones { get; set; }

        #region HospitalizacionHonorarios
        public DbSet<HospitalizacionHonorario> HospitalizacionHonorarios { get; set; }
        #endregion

        public DbSet<HistorialImportacionExcel> HistorialImportacionExcel { get; set; }

        public DbSet<HospitalizacionGastoAdministrativo> HospitalizacionGastosAdministrativos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            base.OnModelCreating(modelBuilder);


            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc), // Guardar como UTC
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc).ToLocalTime() // Leer como hora del servidor
            );

            var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc).ToLocalTime() : v
            );

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        modelBuilder.Entity(entityType.ClrType)
                            .Property(property.Name)
                            .HasConversion(dateTimeConverter);
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        modelBuilder.Entity(entityType.ClrType)
                            .Property(property.Name)
                            .HasConversion(nullableDateTimeConverter);
                    }
                }
            }


            modelBuilder.Entity<Notificacion>(entity =>
            {
                entity.ToTable("Notificaciones");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityByDefaultColumn();
                entity.Property(e => e.Titulo).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Mensaje).HasMaxLength(500).IsRequired();
                entity.Property(e => e.FechaCreacion).IsRequired();
                entity.Property(e => e.Leida).HasDefaultValue(false);
            });


            // Tablas reales en PostgreSQL (según tu BD)
            modelBuilder.Entity<Receta>().ToTable("Receta");
            modelBuilder.Entity<RecetaMenu>().ToTable("RecetaMenu");
            modelBuilder.Entity<RecetaMenuRelacion>().ToTable("RecetaMenuRelacion");
            modelBuilder.Entity<CategoriaReceta>().ToTable("CategoriasRecetas");

            // PK compuesta para join table
            modelBuilder.Entity<RecetaMenuRelacion>()
                .HasKey(x => new { x.RecetaId, x.MenuId });

            modelBuilder.Entity<OrdenesMedicas>()
                .Property(e => e.Examenes)
                .HasColumnType("text[]"); // Mapeo para PostgreSQL
            modelBuilder.Entity<OrdenesMedicas>()
                .Property(e => e.Dietas)
                .HasColumnType("text[]"); // Mapeo para PostgreSQL

            modelBuilder.Entity<HospitalizacionHonorario>().ToTable("HospitalizacionHonorarios");
            modelBuilder.Entity<ConsentimientoHospi>().ToTable("ConsentimientoHospi");
            modelBuilder.Entity<Ambiente>().ToTable("Ambientes");
            modelBuilder.Entity<HospitalizacionUsuarioAcceso>().ToTable("HospitalizacionUsuarioAcceso");
            //modelBuilder.Entity<HospitalizacionRecetaDetalle>().ToTable("HospitalizacionRecetaDetalle");
            modelBuilder.Entity<NotaMedica2>().ToTable("NotaMedica2");
            modelBuilder.Entity<ControlGlucometria2>().ToTable("ControlGlucometria2");
            modelBuilder.Entity<NotaEnfermeria2>().ToTable("NotaEnfermeria2");
            modelBuilder.Entity<TurnoEnfermeria>().ToTable("TurnoEnfermeria");
            modelBuilder.Entity<NotaEvolucion>().ToTable("NotaEvolucion");
            modelBuilder.Entity<NotaOperatoria>().ToTable("NotaOperatoria");
            modelBuilder.Entity<IngestaExcreta2>().ToTable("IngestaExcreta2");
            modelBuilder.Entity<InfoIngesta>().ToTable("InfoIngesta");
            modelBuilder.Entity<ConfiguracionSistema>().ToTable("ConfiguracionesSistema");
            modelBuilder.Entity<AccionPaciente>().ToTable("AccionesPaciente");
            modelBuilder.Entity<CategoriaHabitacion>().ToTable("CategoriasHabitaciones");
            modelBuilder.Entity<CategoriaHabitacionTarifa>().ToTable("CategoriaHabitacionTarifas");
            modelBuilder.Entity<Habitacion>().ToTable("Habitaciones");
            modelBuilder.Entity<EstadoHabitacion>().ToTable("EstadosHabitacion");
            modelBuilder.Entity<PaqueteHospitalizacion>().ToTable("PaquetesHospitalizacion");
            modelBuilder.Entity<DetallePaqueteHospitalizacion>().ToTable("DetallePaquetesHospitalizacion");
            modelBuilder.Entity<Hospitalizacion>().ToTable("Hospitalizaciones");
            modelBuilder.Entity<HospitalizacionServicio>().ToTable("HospitalizacionesServicios");
            modelBuilder.Entity<HospitalizacionProducto>().ToTable("HospitalizacionesProductos");
            modelBuilder.Entity<HospitalizacionProductoAplicacion>().ToTable("HospitalizacionesProductosAplicaciones");
            modelBuilder.Entity<HospitalizacionProductoObservacion>().ToTable("HospitalizacionesProductosObservaciones");
            modelBuilder.Entity<HospitalizacionExamen>().ToTable("HospitalizacionesExamenes");
            modelBuilder.Entity<HospitalizacionPaqueteHospitalizacion>().ToTable("HospitalizacionesPaquetesHospitalizacion");
            modelBuilder.Entity<ExamenFisicoHosp>().ToTable("ExamenesFisicosHosp");
            modelBuilder.Entity<ExamenFisicoHospDato>().ToTable("ExamenesFisicosHospDatos");
            modelBuilder.Entity<DatoExamenFisicoHosp>().ToTable("DatosExamenFisicoHosp");
            modelBuilder.Entity<PresupuestoDental>().ToTable("PresupuestosDentales");
            modelBuilder.Entity<PresupuestoDentalDetalle>().ToTable("PresupuestosDentalesDetalles");
            modelBuilder.Entity<Paciente>().ToTable("Pacientes");
            modelBuilder.Entity<PacienteHistorial>().ToTable("PacientesHistorial");
            modelBuilder.Entity<PacienteArchivo>().ToTable("PacientesArchivos");
            modelBuilder.Entity<EstadoPaciente>().ToTable("EstadosPaciente");
            modelBuilder.Entity<TipoPaciente>().ToTable("TiposPacientes");
            modelBuilder.Entity<PacienteSeguimientoNutricional>().ToTable("PacientesSeguimientosNutricionales");
            modelBuilder.Entity<PacienteRangoSaludable>().ToTable("PacientesRangosSaludables");
            modelBuilder.Entity<PacienteResultadoExamenLaboratorio>().ToTable("PacientesResultadosExamenesLaboratorio");
            modelBuilder.Entity<PacienteAntecedentePersonal>().ToTable("PacientesAntecedentesPersonales");
            modelBuilder.Entity<AntecedentePersonal>().ToTable("AntecedentesPersonales");
            modelBuilder.Entity<PacientePreguntaRegistro>().ToTable("PacientesPreguntasRegistro");
            modelBuilder.Entity<PreguntaRegistro>().ToTable("PreguntasRegistro");
            modelBuilder.Entity<SeguroEpss>().ToTable("SegurosEpss");
            modelBuilder.Entity<CuentaPorCobrar>().ToTable("CuentasPorCobrar");
            modelBuilder.Entity<DetalleCuentaPorCobrar>().ToTable("DetallesCuentaPorCobrar");
            modelBuilder.Entity<TipoPatologia>().ToTable("TipoPatologias");
            modelBuilder.Entity<Producto>().ToTable("Productos");
            modelBuilder.Entity<Bodega>().ToTable("Bodegas");
            modelBuilder.Entity<ProductoInventario>().ToTable("ProductosInventario");
            modelBuilder.Entity<ProductoInventarioPrecio>().ToTable("ProductosInventarioPrecios");
            modelBuilder.Entity<UnidadMedidaCompra>().ToTable("UnidadesMedidaCompra");
            modelBuilder.Entity<UnidadMedidaVenta>().ToTable("UnidadesMedidaVenta");
            modelBuilder.Entity<ProductoEquivalencia>().ToTable("ProductosEquivalencias");
            modelBuilder.Entity<ViaAdministracions>().ToTable("ViaAdministracion");
            modelBuilder.Entity<Viadmin>().ToTable("Viadmin");
            modelBuilder.Entity<TipoProducto>().ToTable("TipoProductos");
            modelBuilder.Entity<PresentacionProducto>().ToTable("PresentacionProductos");
            modelBuilder.Entity<LaboratorioProducto>().ToTable("LaboratorioProductos");
            modelBuilder.Entity<Categoria>().ToTable("Categorias");
            modelBuilder.Entity<Marca>().ToTable("Marcas");
            modelBuilder.Entity<Venta>().ToTable("Ventas");
            modelBuilder.Entity<DetalleVenta>().ToTable("DetalleVentas");
            modelBuilder.Entity<Proveedor>().ToTable("Proveedores");
            modelBuilder.Entity<Empleado>().ToTable("Empleados");
            modelBuilder.Entity<OrdenCompra>().ToTable("OrdenesCompra");
            modelBuilder.Entity<DetalleOrdenCompra>().ToTable("DetalleOrdenesCompra");
            modelBuilder.Entity<DetalleOrdenCompraUbicacion>().ToTable("DetalleOrdenesCompraUbicaciones");
            modelBuilder.Entity<DetalleOrdenCompraUbicacionPrecio>().ToTable("DetalleOrdenesCompraUbicacionesPrecios");
            modelBuilder.Entity<TipoGastoProrrateo>().ToTable("TiposGastosProrrateo");
            modelBuilder.Entity<CategoriaCompra>().ToTable("CategoriasCompras");
            modelBuilder.Entity<Compra>().ToTable("Compras");
            modelBuilder.Entity<CompraTipoDocumento>().ToTable("CompraTiposDocumento");
            modelBuilder.Entity<DetalleCompra>().ToTable("DetalleCompras");
            modelBuilder.Entity<DetalleCompraUnidadVentaPrecio>().ToTable("DetalleComprasUnidadesVentaPrecio");
            modelBuilder.Entity<DetalleCompraUbicacion>().ToTable("DetalleCompraUbicaciones");
            modelBuilder.Entity<DetalleCompraUbicacionPrecio>().ToTable("DetalleCompraUbicacionesPrecios");
            modelBuilder.Entity<Recepcion>().ToTable("Recepciones");
            modelBuilder.Entity<EstadoRecepcion>().ToTable("EstadoRecepciones");
            modelBuilder.Entity<Precio>().ToTable("Precios");
            modelBuilder.Entity<CategoriaServicio>().ToTable("CategoriasServicios");
            modelBuilder.Entity<Servicio>().ToTable("Servicios");
            modelBuilder.Entity<Sucursal>().ToTable("Sucursales");
            modelBuilder.Entity<SucursalServicio>().ToTable("SucursalesServicios");
            modelBuilder.Entity<ServicioInsumo>().ToTable("ServiciosInsumos");
            modelBuilder.Entity<ServicioPrecio>().ToTable("ServiciosPrecios");

            modelBuilder.Entity<ListaChequeo>().ToTable("ListasChequeo");
            modelBuilder.Entity<CuestionarioPreAnestesico>().ToTable("CuestionariosPreAnestesicos");


            //modelBuilder.Entity<VentaServicio>().ToTable("VentaServicios");
            //modelBuilder.Entity<DetalleServicio>().ToTable("DetalleServicios");
            modelBuilder.Entity<Banco>().ToTable("Bancos");
            modelBuilder.Entity<Seguro>().ToTable("Seguros");
            modelBuilder.Entity<Envio>().ToTable("Envios");
            modelBuilder.Entity<EstadosEnvio>().ToTable("EstadosEnvio");
            modelBuilder.Entity<DetalleEnvio>().ToTable("DetalleEnvios");
            modelBuilder.Entity<Ruta>().ToTable("Rutas");
            modelBuilder.Entity<Caja>().ToTable("Cajas");
            modelBuilder.Entity<DetalleCaja>().ToTable("DetalleCaja");
            modelBuilder.Entity<Gasto>().ToTable("Gastos");
            modelBuilder.Entity<CategoriaGasto>().ToTable("CategoriasGastos");
            modelBuilder.Entity<Pagos>().ToTable("Pagos");
            modelBuilder.Entity<FormaPago>().ToTable("FormaPagos");
            modelBuilder.Entity<Moneda>().ToTable("Monedas");
            modelBuilder.Entity<Cotizacion>().ToTable("Cotizaciones");
            modelBuilder.Entity<DetalleCotizacion>().ToTable("DetalleCotizaciones");
            modelBuilder.Entity<Emergencia>().ToTable("Emergencias");
            modelBuilder.Entity<EmergenciaDetalle>().ToTable("EmergenciaDetalles");
            modelBuilder.Entity<Cita>().ToTable("Citas");
            modelBuilder.Entity<Citas>().ToTable("Citass");
            modelBuilder.Entity<CitasServicio>().ToTable("CitasServicios");
            modelBuilder.Entity<CalendarioFechaBloqueada>().ToTable("CalendarioFechasBloqueadas");
            modelBuilder.Entity<TipoEspecialidad>().ToTable("TipoEspecialidad");
            modelBuilder.Entity<Especialidad>().ToTable("Especialidad");
            modelBuilder.Entity<Sexo>().ToTable("Sexo");
            modelBuilder.Entity<TrasladosProductos>().ToTable("TrasladosProductos");
            modelBuilder.Entity<EstadoTraslados>().ToTable("EstadoTraslados");
            modelBuilder.Entity<DetalleTrasladoProductos>().ToTable("DetalleTraslado");
            modelBuilder.Entity<TipoBodega>().ToTable("TipoBodega");
            modelBuilder.Entity<GrupoTProducto>().ToTable("GrupoTProductos");
            modelBuilder.Entity<Grupo>().ToTable("Grupos");
            modelBuilder.Entity<Vacuna>().ToTable("Vacunas");
            modelBuilder.Entity<VacunaPaciente>().ToTable("VacunasPacientes");
            modelBuilder.Entity<PersonaSeguro>().ToTable("PersonasSeguro");
            modelBuilder.Entity<TipoRedSocial>().ToTable("TiposRedSocial");
            modelBuilder.Entity<Persona>().ToTable("Personas");
            modelBuilder.Entity<TipificacionComunicacion>().ToTable("TipificacionesComunicacion");
            modelBuilder.Entity<Consulta>().ToTable("Consultas");
            modelBuilder.Entity<ConsultaServicio>().ToTable("ConsultasServicios");
            modelBuilder.Entity<ConsultaCaracteristicaDental>().ToTable("ConsultasCaracteristicasDentales");
            modelBuilder.Entity<Prescripcion>().ToTable("Prescripcion");
            modelBuilder.Entity<DetallePrescripcion>().ToTable("DetallePrescripcion");
            modelBuilder.Entity<FaseTratamiento>().ToTable("FasesTratamiento");
            modelBuilder.Entity<PacienteFaseTratamiento>().ToTable("PacientesFasesTratamiento");
            modelBuilder.Entity<Grabacion>().ToTable("Grabaciones");
            modelBuilder.Entity<ExamenLabClinico>().ToTable("ExamenLabClinicos");
            modelBuilder.Entity<ExamenLabClinicoPrecio>().ToTable("ExamenLabClinicosPrecios");
            //laboratorioClinico
            modelBuilder.Entity<CategoriaGeneralLabClinico>().ToTable("CategoriaGeneralLabClinico");


            //Repositorio de archivos
            modelBuilder.Entity<RepositorioCarpeta>().ToTable("RepositorioCarpetas");
            modelBuilder.Entity<RepositorioArchivo>().ToTable("RepositorioArchivos");

            //Receta
            modelBuilder.Entity<Receta>().ToTable("Receta");
            modelBuilder.Entity<HospitalizacionReceta>().ToTable("HospitalizacionesReceta");
            //Banco
            modelBuilder.Entity<BancoTrazabilidad>().ToTable("BancoTrazabilidad");

            //cuenta
            modelBuilder.Entity<Cuentas>().ToTable("Cuentas");
            modelBuilder.Entity<TipoCuenta>().ToTable("TipoCuenta");
            modelBuilder.Entity<CuentasTrazabilidad>().ToTable("CuentasTrazabilidad");
            modelBuilder.Entity<DetallePaqueteHospitalizacion>().ToTable("DetallePaqueteHospitalizacion");
            modelBuilder.Entity<HospitalizacionDetallePaqueteHospitalizacion>().ToTable("HospitalizacionDetallePaqueteHospitalizacion");

            //Auditoria
            modelBuilder.Entity<Auditoria>().ToTable("Auditoria");
            modelBuilder.Entity<AuditoriaProducto>().ToTable("AuditoriaProducto");
            modelBuilder.Entity<TipoCompraProveedor>().ToTable("TipoCompraProveedor");


            modelBuilder.Entity<SolicitudMedicamento>().ToTable("SolicitudMedicamento");
            modelBuilder.Entity<SolicitudMedicamentoNacional>().ToTable("SolicitudMedicamentoNacional");
            modelBuilder.Entity<DevolucionMedicamento>().ToTable("DevolucionMedicamento");
            modelBuilder.Entity<HospitalizacionExamenPdf>().ToTable("HospitalizacionExamenPdf");

            //oftalmologia
            modelBuilder.Entity<ConsultasOftalmologia>().ToTable("ConsultasOftalmologia");
            // Configurar la relación uno a uno entre Consulta y ConsultasOftalmologia
            modelBuilder.Entity<ConsultasOftalmologia>()
            .HasOne<Consulta>()
            .WithOne()
            .HasForeignKey<ConsultasOftalmologia>(x => x.ConsultaId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ConsultasOftalmologia>()
                .HasIndex(x => x.ConsultaId)
                .IsUnique();

            // Configuración alternativa para relación uno a muchos
            /*  modelBuilder.Entity<ConsultasOftalmologia>()
             .HasOne<Consulta>()
             .WithMany()
             .HasForeignKey(x => x.ConsultaId)
             .OnDelete(DeleteBehavior.Cascade);

             modelBuilder.Entity<ConsultasOftalmologia>()
                 .HasIndex(x => x.ConsultaId); // no único */



            modelBuilder.Entity<ContactoEmergencia>(entity =>
            {
                entity.ToTable("ContactosEmergencia");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Telefono).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Parentesco).IsRequired().HasMaxLength(100);

                entity.HasOne(e => e.ConsentimientoHospi)
                    .WithMany(c => c.ContactosEmergencia)
                    .HasForeignKey(e => e.ConsentimientoHospiId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ======================
            // PODOLOGÍA
            // ======================
            modelBuilder.Entity<ConsultasPodologia>().ToTable("ConsultasPodologia");

            // Relación 1:1 con Consulta (UNIQUE por ConsultaId)
            modelBuilder.Entity<ConsultasPodologia>()
                .HasOne<Consulta>()
                .WithOne()
                .HasForeignKey<ConsultasPodologia>(x => x.ConsultaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ConsultasPodologia>()
                .HasIndex(x => x.ConsultaId)
                .IsUnique();

            // Índice en PacienteId (tu script lo crea)
            modelBuilder.Entity<ConsultasPodologia>()
                .HasIndex(x => x.PacienteId)
                .HasDatabaseName("idx_pod_paciente");

            // ------ Tipos de columna para alinear con el script ------
            var pod = modelBuilder.Entity<ConsultasPodologia>();

            // Arrays: TEXT[]
            pod.Property(p => p.Enfermedades)
               .HasColumnType("text[]");
            pod.Property(p => p.Procedimientos)
               .HasColumnType("text[]");

            // Numéricos
            pod.Property(p => p.PesoKg)
               .HasColumnType("numeric(5,1)");
            pod.Property(p => p.EstaturaM)
               .HasColumnType("numeric(4,2)");

            // Fecha DATE
            pod.Property(p => p.FechaAtencion)
               .HasColumnType("date");

            // Boolean
            pod.Property(p => p.ProblemasCirculatorios)
               .HasColumnType("boolean");

            // (Opcional) Defaults en BD para evitar NULL en arrays:
            // pod.Property(p => p.Enfermedades)
            //    .HasDefaultValueSql("'{}'::text[]");
            // pod.Property(p => p.Procedimientos)
            //    .HasDefaultValueSql("'{}'::text[]");

            /// ======================
            // Historia Clínica de Enfermería
            // ======================
            modelBuilder.Entity<ConsultasHistoriaClinicaEnfermeria>()
                .ToTable("ConsultasHistoriaClinicaEnfermeria");

            // Relación 1:1 con Consulta (UNIQUE por ConsultaId)
            modelBuilder.Entity<ConsultasHistoriaClinicaEnfermeria>()
                .HasOne<Consulta>()
                .WithOne()
                .HasForeignKey<ConsultasHistoriaClinicaEnfermeria>(x => x.ConsultaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ConsultasHistoriaClinicaEnfermeria>()
                .HasIndex(x => x.ConsultaId)
                .IsUnique();

            // Índice en PacienteId
            modelBuilder.Entity<ConsultasHistoriaClinicaEnfermeria>()
                .HasIndex(x => x.PacienteId)
                .HasDatabaseName("idx_enf_paciente");




            modelBuilder.Entity<HospitalizacionInsumoDirecto>(entity =>
{
    entity.ToTable("HospitalizacionInsumosDirectos"); 
    entity.HasKey(e => e.Id);

    entity.Property(e => e.Id).HasColumnName("Id");
    entity.Property(e => e.HospitalizacionId).HasColumnName("HospitalizacionId");
    entity.Property(e => e.ProductoId).HasColumnName("ProductoId");
    entity.Property(e => e.PrecioValor).HasColumnName("PrecioValor").HasPrecision(18, 2);
    entity.Property(e => e.Eliminado).HasColumnName("Eliminado").HasDefaultValue(false);
    entity.Property(e => e.FechaCreacion).HasColumnName("FechaCreacion").HasDefaultValueSql("CURRENT_TIMESTAMP");

    entity.HasMany(d => d.Aplicaciones)
          .WithOne(p => p.HospitalizacionInsumoDirecto)
          .HasForeignKey(d => d.HospitalizacionInsumoDirectoId)
          .HasConstraintName("FK_InsumosDirectosAplicaciones_Insumo");
});

            modelBuilder.Entity<HospitalizacionInsumoDirectoAplicacion>(entity =>
            {
                entity.ToTable("HospitalizacionInsumosDirectosAplicaciones");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.HospitalizacionInsumoDirectoId).HasColumnName("HospitalizacionInsumoDirectoId");
                entity.Property(e => e.Aplicado).HasColumnName("Aplicado").HasDefaultValue(false);
            });


            /// ======================
            // Valoración Inicial de Enfermería
            // ======================
            modelBuilder.Entity<ConsultasValoracionInicialEnfermeria>()
                .ToTable("ConsultasValoracionInicialEnfermeria");
            // Relación 1:1 con Consulta (UNIQUE por ConsultaId)
            modelBuilder.Entity<ConsultasValoracionInicialEnfermeria>()
                .HasOne<Consulta>()
                .WithOne()
                .HasForeignKey<ConsultasValoracionInicialEnfermeria>(x => x.ConsultaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ConsultasValoracionInicialEnfermeria>()
                .HasIndex(x => x.ConsultaId)
                .IsUnique();

            // Índice en PacienteId (igual que en Podología/HCE)
            modelBuilder.Entity<ConsultasValoracionInicialEnfermeria>()
                .HasIndex(x => x.PacienteId)
                .HasDatabaseName("idx_ve_paciente");

            // Tipos de columna TEXT[] para todos los arreglos (mismo criterio que Podología)
            var ve = modelBuilder.Entity<ConsultasValoracionInicialEnfermeria>();
            ve.Property(p => p.Medio).HasColumnType("text[]");
            ve.Property(p => p.Resp).HasColumnType("text[]");
            ve.Property(p => p.Circ).HasColumnType("text[]");
            ve.Property(p => p.Nutricion).HasColumnType("text[]");
            ve.Property(p => p.Urinario).HasColumnType("text[]");
            ve.Property(p => p.Intestinal).HasColumnType("text[]");
            ve.Property(p => p.Mov).HasColumnType("text[]");
            ve.Property(p => p.Conciencia).HasColumnType("text[]");
            ve.Property(p => p.Sueno).HasColumnType("text[]");
            ve.Property(p => p.Piel).HasColumnType("text[]");
            ve.Property(p => p.Lenguaje).HasColumnType("text[]");
            ve.Property(p => p.Vision).HasColumnType("text[]");
            ve.Property(p => p.Oido).HasColumnType("text[]");
            ve.Property(p => p.Seg).HasColumnType("text[]");

            /// ======================
            // Sueroterapia
            // ======================
            // ======================
            // Sueroterapia
            // ======================
            modelBuilder.Entity<ConsultasSueroterapia>()
                .ToTable("ConsultasSueroterapia");

            // Relación 1:1 con Consulta usando la navegación 'Consulta' y la FK existente 'ConsultaId'
            modelBuilder.Entity<ConsultasSueroterapia>()
                .HasOne(s => s.Consulta)                 // <-- usar la navegación
                .WithOne()
                .HasForeignKey<ConsultasSueroterapia>(s => s.ConsultaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índice único por ConsultaId (1:1)
            modelBuilder.Entity<ConsultasSueroterapia>()
                .HasIndex(s => s.ConsultaId)
                .IsUnique();

            // Relación con Paciente (1:N desde Paciente, sin sombra)
            modelBuilder.Entity<ConsultasSueroterapia>()
                .HasOne(s => s.Paciente)
                .WithMany()
                .HasForeignKey(s => s.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Tipos array para PostgreSQL
            var suero = modelBuilder.Entity<ConsultasSueroterapia>();
            suero.Property(p => p.Medio).HasColumnType("text[]");
            suero.Property(p => p.Resp).HasColumnType("text[]");
            suero.Property(p => p.Circ).HasColumnType("text[]");
            suero.Property(p => p.Nutricion).HasColumnType("text[]");




            #region Estados de paciente
            modelBuilder.Entity<EstadoPaciente>().HasData(new EstadoPaciente
            {
                Id = 1,
                NombreEstado = "Activo"
            });
            modelBuilder.Entity<EstadoPaciente>().HasData(new EstadoPaciente
            {
                Id = 2,
                NombreEstado = "Inactivo"
            });
            #endregion

            #region Tipos de red social
            modelBuilder.Entity<TipoRedSocial>().HasData(new TipoRedSocial
            {
                Id = 1,
                NombreRedSocial = "Facebook"
            });
            modelBuilder.Entity<TipoRedSocial>().HasData(new TipoRedSocial
            {
                Id = 2,
                NombreRedSocial = "Instagram"
            });
            modelBuilder.Entity<TipoRedSocial>().HasData(new TipoRedSocial
            {
                Id = 3,
                NombreRedSocial = "Tiktok"
            });
            modelBuilder.Entity<TipoRedSocial>().HasData(new TipoRedSocial
            {
                Id = 4,
                NombreRedSocial = "WhatsApp"
            });
            #endregion

            #region Tipos de pacientes
            modelBuilder.Entity<TipoPaciente>().HasData(new TipoPaciente
            {
                Id = 1,
                NombreTipo = "Nuevo"
            });
            modelBuilder.Entity<TipoPaciente>().HasData(new TipoPaciente
            {
                Id = 2,
                NombreTipo = "Retomante"
            });
            #endregion

            #region Acciones de paciente
            modelBuilder.Entity<AccionPaciente>().HasData(new AccionPaciente
            {
                Id = 1,
                NombreAccion = "Registro"
            });
            modelBuilder.Entity<AccionPaciente>().HasData(new AccionPaciente
            {
                Id = 2,
                NombreAccion = "Retiro"
            });
            #endregion

            #region Formas de pago
            modelBuilder.Entity<FormaPago>().HasData(new FormaPago
            {
                Id = 1,
                NombreFormaPago = "Efectivo",
            });
            modelBuilder.Entity<FormaPago>().HasData(new FormaPago
            {
                Id = 2,
                NombreFormaPago = "Tarjeta de Débito",
            });
            modelBuilder.Entity<FormaPago>().HasData(new FormaPago
            {
                Id = 3,
                NombreFormaPago = "Tarjeta de Crédito",
            });
            modelBuilder.Entity<FormaPago>().HasData(new FormaPago
            {
                Id = 4,
                NombreFormaPago = "Cheques",
            });
            modelBuilder.Entity<FormaPago>().HasData(new FormaPago
            {
                Id = 5,
                NombreFormaPago = "Transferencia",
            });
            modelBuilder.Entity<FormaPago>().HasData(new FormaPago
            {
                Id = 6,
                NombreFormaPago = "Visa Link",
            });
            modelBuilder.Entity<FormaPago>().HasData(new FormaPago
            {
                Id = 7,
                NombreFormaPago = "Visa Net",
            });
            modelBuilder.Entity<FormaPago>().HasData(new FormaPago
            {
                Id = 8,
                NombreFormaPago = "Credito",
            });
            #endregion

            #region Monedas
            modelBuilder.Entity<Moneda>().HasData(new Moneda
            {
                Id = 1,
                NombreMoneda = "Dólar"
            });
            modelBuilder.Entity<Moneda>().HasData(new Moneda
            {
                Id = 2,
                NombreMoneda = "Quetzal"
            });
            #endregion

            #region Especialidades
            modelBuilder.Entity<Especialidad>().HasData(new Especialidad
            {
                Id = 1,
                NombreEspecialidad = "Medicina general",
            });

            modelBuilder.Entity<Especialidad>().HasData(new Especialidad
            {
                Id = 2,
                NombreEspecialidad = "Medicina estética",
            });

            modelBuilder.Entity<Especialidad>().HasData(new Especialidad
            {
                Id = 3,
                NombreEspecialidad = "Ginecología y Obstetricia",
            });

            modelBuilder.Entity<Especialidad>().HasData(new Especialidad
            {
                Id = 4,
                NombreEspecialidad = "Pediatría",
            });

            modelBuilder.Entity<Especialidad>().HasData(new Especialidad
            {
                Id = 5,
                NombreEspecialidad = "Cirugía",
            });

            modelBuilder.Entity<Especialidad>().HasData(new Especialidad
            {
                Id = 6,
                NombreEspecialidad = "Traumatología",
            });

            modelBuilder.Entity<Especialidad>().HasData(new Especialidad
            {
                Id = 7,
                NombreEspecialidad = "Medicina interna",
            });


            modelBuilder.Entity<Especialidad>().HasData(new Especialidad
            {
                Id = 8,
                NombreEspecialidad = "Laboratorio clínico",
            });

            modelBuilder.Entity<Especialidad>().HasData(new Especialidad
            {
                Id = 9,
                NombreEspecialidad = "Cardiología",
            });

            modelBuilder.Entity<Especialidad>().HasData(new Especialidad
            {
                Id = 10,
                NombreEspecialidad = "Nutrición",
            });
            #endregion

            #region Sexo
            modelBuilder.Entity<Sexo>().HasData(new Sexo
            {
                Id = 1,
                DescripcionSexo = "Masculino",
            });

            modelBuilder.Entity<Sexo>().HasData(new Sexo
            {
                Id = 2,
                DescripcionSexo = "Femenino",
            });

            modelBuilder.Entity<Sexo>().HasData(new Sexo
            {
                Id = 3,
                DescripcionSexo = "Otro",
            });
            #endregion

            #region Tipificaciones de comunicacion
            modelBuilder.Entity<TipificacionComunicacion>().HasData(new TipificacionComunicacion
            {
                Id = 1,
                NombreTipificacion = "Contactado"
            });
            modelBuilder.Entity<TipificacionComunicacion>().HasData(new TipificacionComunicacion
            {
                Id = 2,
                NombreTipificacion = "Recontactado"
            });
            modelBuilder.Entity<TipificacionComunicacion>().HasData(new TipificacionComunicacion
            {
                Id = 3,
                NombreTipificacion = "Nuevo ingreso"
            });
            #endregion

            #region Compra tipo de documento

            modelBuilder.Entity<CompraTipoDocumento>().HasData(new CompraTipoDocumento
            {
                Id = 1,
                NombreTipoDocumento = "Orden de compra"
            });
            modelBuilder.Entity<CompraTipoDocumento>().HasData(new CompraTipoDocumento
            {
                Id = 2,
                NombreTipoDocumento = "Compra"
            });

            #endregion

            #region Tipos de patologias - Antecedentes familiares
            modelBuilder.Entity<TipoPatologia>().HasData(new TipoPatologia
            {
                Id = 1,
                Tipo = "Diabetes",
                VerInputDescripcion = false
            });
            modelBuilder.Entity<TipoPatologia>().HasData(new TipoPatologia
            {
                Id = 2,
                Tipo = "Cardiopatías",
                VerInputDescripcion = false
            });
            modelBuilder.Entity<TipoPatologia>().HasData(new TipoPatologia
            {
                Id = 3,
                Tipo = "Neoplasias",
                VerInputDescripcion = false
            });
            modelBuilder.Entity<TipoPatologia>().HasData(new TipoPatologia
            {
                Id = 4,
                Tipo = "Epilepsia",
                VerInputDescripcion = false
            });
            modelBuilder.Entity<TipoPatologia>().HasData(new TipoPatologia
            {
                Id = 5,
                Tipo = "Malformaciones",
                VerInputDescripcion = false
            });
            modelBuilder.Entity<TipoPatologia>().HasData(new TipoPatologia
            {
                Id = 6,
                Tipo = "SIDA",
                VerInputDescripcion = false
            });
            modelBuilder.Entity<TipoPatologia>().HasData(new TipoPatologia
            {
                Id = 7,
                Tipo = "Enfermedades renales",
                VerInputDescripcion = false
            });
            modelBuilder.Entity<TipoPatologia>().HasData(new TipoPatologia
            {
                Id = 8,
                Tipo = "Hepatitis",
                VerInputDescripcion = false
            });
            modelBuilder.Entity<TipoPatologia>().HasData(new TipoPatologia
            {
                Id = 9,
                Tipo = "Artritis",
                VerInputDescripcion = false
            });
            modelBuilder.Entity<TipoPatologia>().HasData(new TipoPatologia
            {
                Id = 10,
                Tipo = "Otra",
                VerInputDescripcion = true
            });
            modelBuilder.Entity<TipoPatologia>().HasData(new TipoPatologia
            {
                Id = 11,
                Tipo = "Aparentemente sano",
                VerInputDescripcion = false
            });
            #endregion

            #region Fases de tratamiento
            modelBuilder.Entity<FaseTratamiento>().HasData(new FaseTratamiento
            {
                Id = 1,
                NombreFase = "Adelgazamiento",
                Valor = 2650
            });
            modelBuilder.Entity<FaseTratamiento>().HasData(new FaseTratamiento
            {
                Id = 2,
                NombreFase = "Mantenimiento 1",
                Valor = 1600
            });
            modelBuilder.Entity<FaseTratamiento>().HasData(new FaseTratamiento
            {
                Id = 3,
                NombreFase = "Mantenimiento 2",
                Valor = 1600
            });
            modelBuilder.Entity<FaseTratamiento>().HasData(new FaseTratamiento
            {
                Id = 4,
                NombreFase = "Mantenimiento 3",
                Valor = 1600
            });
            #endregion

            #region Bancos
            modelBuilder.Entity<Banco>().HasData(new Banco
            {
                Id = 1,
                Nombre = "BANCO AGROMERCANTIL DE GUATEMALA S.A.",
            });

            modelBuilder.Entity<Banco>().HasData(new Banco
            {
                Id = 2,
                Nombre = "VIVIBANCO S.A.",
            });

            modelBuilder.Entity<Banco>().HasData(new Banco
            {
                Id = 3,
                Nombre = "BANCO G&T CONTINENTAL S.A.",
            });

            modelBuilder.Entity<Banco>().HasData(new Banco
            {
                Id = 4,
                Nombre = "BANCO DE AMERICA CENTRAL S.A.",
            });

            modelBuilder.Entity<Banco>().HasData(new Banco
            {
                Id = 5,
                Nombre = "BANCO FICOHSA GUATEMALA S.A.",
            });

            modelBuilder.Entity<Banco>().HasData(new Banco
            {
                Id = 6,
                Nombre = "BANCO INTERNACIONAL S.A.",
            });

            modelBuilder.Entity<Banco>().HasData(new Banco
            {
                Id = 7,
                Nombre = "BANCO DE DESARROLLO RURAL S.A.",
            });

            modelBuilder.Entity<Banco>().HasData(new Banco
            {
                Id = 8,
                Nombre = "BANCO INDUSTRIAL S.A.",
            });

            modelBuilder.Entity<Banco>().HasData(new Banco
            {
                Id = 9,
                Nombre = "BANCO DE CREDITO S.A.",
            });

            modelBuilder.Entity<Banco>().HasData(new Banco
            {
                Id = 10,
                Nombre = "BANCO PROMERICA S.A.",
            });

            modelBuilder.Entity<Banco>().HasData(new Banco
            {
                Id = 11,
                Nombre = "CITIBANK N.A SUCURSAL GUATEMALA",
            });

            modelBuilder.Entity<Banco>().HasData(new Banco
            {
                Id = 12,
                Nombre = "EL CREDITO HIPOTECARIO NACIONAL DE GUATEMALA",
            });


            modelBuilder.Entity<Banco>().HasData(new Banco
            {
                Id = 13,
                Nombre = "BANCO DE LOS TRABAJADORES",
            });


            modelBuilder.Entity<Banco>().HasData(new Banco
            {
                Id = 14,
                Nombre = "BANCO INMOBILIARIO S.A.",
            });
            #endregion

            #region Estados de habitacion
            modelBuilder.Entity<EstadoHabitacion>().HasData(new EstadoHabitacion
            {
                Id = 1,
                NombreEstado = "Disponible",
            });
            modelBuilder.Entity<EstadoHabitacion>().HasData(new EstadoHabitacion
            {
                Id = 2,
                NombreEstado = "Ocupada",
            });
            modelBuilder.Entity<EstadoHabitacion>().HasData(new EstadoHabitacion
            {
                Id = 3,
                NombreEstado = "En limpieza",
            });
            #endregion|

            #region Estados traslados
            modelBuilder.Entity<EstadoTraslados>().HasData(new EstadoTraslados
            {
                Id = 1,
                DescripcionEstado = "Aceptado",
            });

            modelBuilder.Entity<EstadoTraslados>().HasData(new EstadoTraslados
            {
                Id = 2,
                DescripcionEstado = "Denegado",
            });

            modelBuilder.Entity<EstadoTraslados>().HasData(new EstadoTraslados
            {
                Id = 3,
                DescripcionEstado = "Con problema",
            });

            modelBuilder.Entity<EstadoTraslados>().HasData(new EstadoTraslados
            {
                Id = 4,
                DescripcionEstado = "Faltantes",
            });

            modelBuilder.Entity<EstadoTraslados>().HasData(new EstadoTraslados
            {
                Id = 5,
                DescripcionEstado = "En tránsito",
            });
            #endregion

            #region Datos examen fisico hospitalizacion
            modelBuilder.Entity<DatoExamenFisicoHosp>().HasData(new DatoExamenFisicoHosp
            {
                Id = 1,
                NombreDato = "Presion arterial brazo derecho"
            });
            modelBuilder.Entity<DatoExamenFisicoHosp>().HasData(new DatoExamenFisicoHosp
            {
                Id = 2,
                NombreDato = "Presion arterial brazo izquierdo"
            });
            modelBuilder.Entity<DatoExamenFisicoHosp>().HasData(new DatoExamenFisicoHosp
            {
                Id = 3,
                NombreDato = "Temperatura"
            });
            modelBuilder.Entity<DatoExamenFisicoHosp>().HasData(new DatoExamenFisicoHosp
            {
                Id = 4,
                NombreDato = "Frecuencia cardiaca"
            });
            modelBuilder.Entity<DatoExamenFisicoHosp>().HasData(new DatoExamenFisicoHosp
            {
                Id = 5,
                NombreDato = "Frecuencia respiratoria"
            });
            modelBuilder.Entity<DatoExamenFisicoHosp>().HasData(new DatoExamenFisicoHosp
            {
                Id = 6,
                NombreDato = "Saturacion de oxigeno"
            });
            modelBuilder.Entity<DatoExamenFisicoHosp>().HasData(new DatoExamenFisicoHosp
            {
                Id = 7,
                NombreDato = "Glucosa"
            });
            modelBuilder.Entity<DatoExamenFisicoHosp>().HasData(new DatoExamenFisicoHosp
            {
                Id = 11,
                NombreDato = "Presion arterial media"
            });
            #endregion

            #region Ambientes
            modelBuilder.Entity<Ambiente>().HasData(new Ambiente
            {
                Id = 1,
                NombreAmbiente = "Farmacia",
            });

            modelBuilder.Entity<Ambiente>().HasData(new Ambiente
            {
                Id = 2,
                NombreAmbiente = "Clinica",
            });

            modelBuilder.Entity<Ambiente>().HasData(new Ambiente
            {
                Id = 3,
                NombreAmbiente = "Hospital",
            });
            modelBuilder.Entity<Ambiente>().HasData(new Ambiente
            {
                Id = 4,
                NombreAmbiente = "Laboratorio",
            });
            modelBuilder.Entity<Ambiente>().HasData(new Ambiente
            {
                Id = 5,
                NombreAmbiente = "Bodega",
            });
            modelBuilder.Entity<Ambiente>().HasData(new Ambiente
            {
                Id = 6,
                NombreAmbiente = "Global",
            });
            #endregion

            #region Tipos Bodega
            modelBuilder.Entity<TipoBodega>().HasData(new TipoBodega
            {
                Id = 1,
                DescripcionBodega = "FARMACIA",
            });

            modelBuilder.Entity<TipoBodega>().HasData(new TipoBodega
            {
                Id = 2,
                DescripcionBodega = "CLINICA",
            });

            modelBuilder.Entity<TipoBodega>().HasData(new TipoBodega
            {
                Id = 3,
                DescripcionBodega = "BODEGA",
            });

            modelBuilder.Entity<TipoBodega>().HasData(new TipoBodega
            {
                Id = 4,
                DescripcionBodega = "LABORATORIO",
            });
            modelBuilder.Entity<TipoBodega>().HasData(new TipoBodega
            {
                Id = 6,
                DescripcionBodega = "GLOBAL",
            });

            #endregion

            #region TipoProducto
            modelBuilder.Entity<TipoProducto>().HasData(new TipoProducto
            {
                Id = 1,
                NombreTipoProducto = "Medicamentos",
            });

            modelBuilder.Entity<TipoProducto>().HasData(new TipoProducto
            {
                Id = 2,
                NombreTipoProducto = "Insumos Médicos",
            });

            modelBuilder.Entity<TipoProducto>().HasData(new TipoProducto
            {
                Id = 3,
                NombreTipoProducto = "Equipos Médicos",
            });

            modelBuilder.Entity<TipoProducto>().HasData(new TipoProducto
            {
                Id = 4,
                NombreTipoProducto = "Muestras Médicas",
            });
            #endregion

            #region Precios
            modelBuilder.Entity<Precio>().HasData(new Precio
            {
                Id = 1,
                NombrePrecio = "Normal",
                Eliminado = false
            });
            modelBuilder.Entity<Precio>().HasData(new Precio
            {
                Id = 2,
                NombrePrecio = "VIP",
                Eliminado = false
            });
            #endregion

            #region Tipos de compra
            modelBuilder.Entity<TipoCompra>().HasData(new TipoCompra
            {
                Id = 1,
                Tipo = "CONTADO",
            });

            modelBuilder.Entity<TipoCompra>().HasData(new TipoCompra
            {
                Id = 2,
                Tipo = "CREDITO",
            });
            #endregion

            #region Estado de recepcion
            modelBuilder.Entity<EstadoRecepcion>().HasData(new EstadoRecepcion
            {
                Id = 1,
                Estado = "No ingresado",
            });

            modelBuilder.Entity<EstadoRecepcion>().HasData(new EstadoRecepcion
            {
                Id = 2,
                Estado = "Ingresado",
            });
            #endregion

            #region Estado de pago de consulta
            modelBuilder.Entity<EstadoPagoConsulta>().HasData(new EstadoPagoConsulta
            {
                Id = 1,
                Estado = "Pagado",
            });

            modelBuilder.Entity<EstadoPagoConsulta>().HasData(new EstadoPagoConsulta
            {
                Id = 2,
                Estado = "Pendiente",
            });

            modelBuilder.Entity<EstadoPagoConsulta>().HasData(new EstadoPagoConsulta
            {
                Id = 3,
                Estado = "Cortesía",
            });
            #endregion

            #region Estados examen
            modelBuilder.Entity<EstadoExamen>().HasData(new EstadoExamen
            {
                Id = 1,
                Nombre = "Solicitados",
            });

            modelBuilder.Entity<EstadoExamen>().HasData(new EstadoExamen
            {
                Id = 2,
                Nombre = "En proceso",
            });

            modelBuilder.Entity<EstadoExamen>().HasData(new EstadoExamen
            {
                Id = 3,
                Nombre = "Cancelados",
            });

            modelBuilder.Entity<EstadoExamen>().HasData(new EstadoExamen
            {
                Id = 4,
                Nombre = "Revisión",
            });

            modelBuilder.Entity<EstadoExamen>().HasData(new EstadoExamen
            {
                Id = 5,
                Nombre = "Finalizados",
            });
            #endregion

            #region Sucursal
            modelBuilder.Entity<Sucursal>().HasData(new Sucursal
            {
                Id = 1,
                NombreSucursal = "La Gomera",
            });

            modelBuilder.Entity<Sucursal>().HasData(new Sucursal
            {
                Id = 2,
                NombreSucursal = "Zicapate",
            });

            modelBuilder.Entity<Sucursal>().HasData(new Sucursal
            {
                Id = 3,
                NombreSucursal = "Puerto de San José",
            });

            modelBuilder.Entity<Sucursal>().HasData(new Sucursal
            {
                Id = 4,
                NombreSucursal = "San Pedro Yepocapa",
            });

            modelBuilder.Entity<Sucursal>().HasData(new Sucursal
            {
                Id = 5,
                NombreSucursal = "Siquinalá",
            });
            #endregion

            #region Usuarios default
            var hasher = new PasswordHasher<User>();

            //Usuario administrador
            var usuarioAdministradorDefault = new User
            {
                Id = "dc6916f0-8d11-43a3-b143-37ce429bee33",
                UserName = "admin@redowl.com",
                NormalizedUserName = "ADMIN@REDOWL.COM",
                PasswordHash = hasher.HashPassword(null, "redowl@123"),
                Email = "admin@redowl.com",
                NormalizedEmail = "ADMIN@REDOWL.COM",
                LockoutEnabled = true
            };
            modelBuilder.Entity<User>().HasData(usuarioAdministradorDefault);

            //Usuario desarrollador
            var usuarioDesarrolladorDefault = new User
            {
                Id = "b69a6775-b481-4fc5-93b8-4d63dbe1d9cb",
                UserName = "dev@redowl.com",
                NormalizedUserName = "DEV@REDOWL.COM",
                PasswordHash = hasher.HashPassword(null, "devredowl@123"),
                Email = "dev@redowl.com",
                NormalizedEmail = "DEV@REDOWL.COM",
                LockoutEnabled = true
            };
            modelBuilder.Entity<User>().HasData(usuarioDesarrolladorDefault);

            #endregion

            #region Roles
            var objDataSeedRoles = new DataSeedRoleList();
            var listaRoles = objDataSeedRoles.RoleList;
            if (listaRoles != null)
            {
                foreach (var rol in listaRoles)
                {
                    modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole
                    {
                        Id = rol.Id,
                        Name = rol.Name,
                        NormalizedName = rol.NormalizedName
                    });
                }
            }

            #endregion

            #region Relacion Usuarios roles default

            //Relacion administrador
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = objDataSeedRoles.rol_Administrador.Id,
                    UserId = usuarioAdministradorDefault.Id
                }
            );

            //Relacion desarrollador
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = objDataSeedRoles.rol_Desarrollador.Id,
                    UserId = usuarioDesarrolladorDefault.Id
                }
            );

            #endregion

            #region Antecedentes personales

            modelBuilder.Entity<AntecedentePersonal>().HasData(new AntecedentePersonal
            {
                Id = 1,
                NombreAntecedente = "Varicela"
            });
            modelBuilder.Entity<AntecedentePersonal>().HasData(new AntecedentePersonal
            {
                Id = 2,
                NombreAntecedente = "Rubeola"
            });
            modelBuilder.Entity<AntecedentePersonal>().HasData(new AntecedentePersonal
            {
                Id = 3,
                NombreAntecedente = "Sarampión"
            });
            modelBuilder.Entity<AntecedentePersonal>().HasData(new AntecedentePersonal
            {
                Id = 4,
                NombreAntecedente = "Parotiditis"
            });
            modelBuilder.Entity<AntecedentePersonal>().HasData(new AntecedentePersonal
            {
                Id = 5,
                NombreAntecedente = "Tosferina"
            });
            modelBuilder.Entity<AntecedentePersonal>().HasData(new AntecedentePersonal
            {
                Id = 6,
                NombreAntecedente = "Escarlatina"
            });
            modelBuilder.Entity<AntecedentePersonal>().HasData(new AntecedentePersonal
            {
                Id = 7,
                NombreAntecedente = "Parasitosis"
            });
            modelBuilder.Entity<AntecedentePersonal>().HasData(new AntecedentePersonal
            {
                Id = 8,
                NombreAntecedente = "Hepatitis"
            });
            modelBuilder.Entity<AntecedentePersonal>().HasData(new AntecedentePersonal
            {
                Id = 9,
                NombreAntecedente = "SIDA"
            });
            modelBuilder.Entity<AntecedentePersonal>().HasData(new AntecedentePersonal
            {
                Id = 10,
                NombreAntecedente = "VIH"
            });
            modelBuilder.Entity<AntecedentePersonal>().HasData(new AntecedentePersonal
            {
                Id = 11,
                NombreAntecedente = "Asma"
            });
            modelBuilder.Entity<AntecedentePersonal>().HasData(new AntecedentePersonal
            {
                Id = 12,
                NombreAntecedente = "Disfunciones endocrinas"
            });

            #endregion

            #region Vacuna

            modelBuilder.Entity<Vacuna>().HasData(new Vacuna
            {
                Id = 1,
                Nombre = "Hexavalente"
            });
            modelBuilder.Entity<Vacuna>().HasData(new Vacuna
            {
                Id = 2,
                Nombre = "Rotavirus"
            });
            modelBuilder.Entity<Vacuna>().HasData(new Vacuna
            {
                Id = 3,
                Nombre = "Neumococo"
            });
            modelBuilder.Entity<Vacuna>().HasData(new Vacuna
            {
                Id = 4,
                Nombre = "MMR"
            });
            modelBuilder.Entity<Vacuna>().HasData(new Vacuna
            {
                Id = 5,
                Nombre = "Varicela"
            });
            modelBuilder.Entity<Vacuna>().HasData(new Vacuna
            {
                Id = 6,
                Nombre = "Hepatitis A"
            });
            modelBuilder.Entity<Vacuna>().HasData(new Vacuna
            {
                Id = 7,
                Nombre = "Influenza"
            });
            modelBuilder.Entity<Vacuna>().HasData(new Vacuna
            {
                Id = 8,
                Nombre = "Papiloma Virus"
            });
            modelBuilder.Entity<Vacuna>().HasData(new Vacuna
            {
                Id = 9,
                Nombre = "BCG"
            });
            modelBuilder.Entity<Vacuna>().HasData(new Vacuna
            {
                Id = 10,
                Nombre = "Hepatitis B"
            });
            modelBuilder.Entity<Vacuna>().HasData(new Vacuna
            {
                Id = 11,
                Nombre = "COVID-19"
            });

            #endregion

            #region Tipo Cuenta
            modelBuilder.Entity<TipoCuenta>().HasData(new TipoCuenta
            {
                Id = 1,
                Nombre = "Monetaria"
            });
            modelBuilder.Entity<TipoCuenta>().HasData(new TipoCuenta
            {
                Id = 2,
                Nombre = "Ahorro"
            });

            modelBuilder.Entity<TipoCuenta>().HasData(new TipoCuenta
            {
                Id = 3,
                Nombre = "Inversión"
            });



            #endregion

            #region Nomenclatura
            modelBuilder.Entity<Nomenclatura>().HasData(new Nomenclatura
            {
                Id = 1,
                Nombre = "Ingreso"
            });
            modelBuilder.Entity<Nomenclatura>().HasData(new Nomenclatura
            {
                Id = 2,
                Nombre = "Egreso"
            });

            #endregion

            #region Tipo de movimiento producto

            modelBuilder.Entity<TipoMovimientoProducto>().HasData(new TipoMovimientoProducto
            {
                Id = 1,
                Nombre = "Entrada"
            });
            modelBuilder.Entity<TipoMovimientoProducto>().HasData(new TipoMovimientoProducto
            {
                Id = 2,
                Nombre = "Salida"
            });
            modelBuilder.Entity<TipoMovimientoProducto>().HasData(new TipoMovimientoProducto
            {
                Id = 3,
                Nombre = "Salida - Venta"
            });
            modelBuilder.Entity<TipoMovimientoProducto>().HasData(new TipoMovimientoProducto
            {
                Id = 4,
                Nombre = "Entrada - Compra"
            });

            #endregion

            #region Requisicion
            // ==========================================================
            // Requisición (nuevo proceso)
            // ==========================================================
            modelBuilder.Entity<Requision>().ToTable("Requision");
            modelBuilder.Entity<RequisionDetalle>().ToTable("RequisionDetalle");
            modelBuilder.Entity<RequisionHistorial>().ToTable("RequisionHistorial");
            #endregion


            #region DevolucionNacional
            modelBuilder.Entity<DevolucionNacional>(entity =>
            {
                entity.ToTable("DevolucionNacional");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityByDefaultColumn();
            });

            modelBuilder.Entity<DevolucionNacionalDetalle>(entity =>
            {
                entity.ToTable("DevolucionNacionalDetalle");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityByDefaultColumn();
            });

            modelBuilder.Entity<DevolucionNacional>()
                .HasMany(e => e.Detalles)
                .WithOne(d => d.DevolucionNacional)
                .HasForeignKey(d => d.DevolucionNacionalId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion


            modelBuilder.Entity<DocumentosHospitalizacion>(entity =>
            {
                entity.ToTable("DocumentosHospitalizacion");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NombreArchivo).HasMaxLength(255).IsRequired();
                entity.Property(e => e.RutaArchivo).HasMaxLength(500).IsRequired();
                entity.Property(e => e.Extension).HasMaxLength(10);

                entity.HasOne(e => e.Hospitalizacion)
                    .WithMany()
                    .HasForeignKey(e => e.HospitalizacionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Paciente)
                    .WithMany()
                    .HasForeignKey(e => e.PacienteId)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<MedicamentoNoControlado>().ToTable("MedicamentoNoControlado");


            base.OnModelCreating(modelBuilder);
        }
        public override int SaveChanges()
        {
            ConvertDateTimesToUtc();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ConvertDateTimesToUtc();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        private void ConvertDateTimesToUtc()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                foreach (var property in entry.Properties)
                {
                    if (property.CurrentValue is DateTime dateTimeValue)
                    {
                        if (dateTimeValue.Kind == DateTimeKind.Unspecified)
                        {
                            property.CurrentValue = DateTime.SpecifyKind(dateTimeValue, DateTimeKind.Utc);
                        }
                        else if (dateTimeValue.Kind == DateTimeKind.Local)
                        {
                            property.CurrentValue = dateTimeValue.ToUniversalTime();
                        }
                    }
                }
            }
        }

    }

}
