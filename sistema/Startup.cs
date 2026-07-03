using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Http.Features;
using System.Net;
using Database.Shared;
using Microsoft.EntityFrameworkCore;
using Database.Shared.IRepository;
using Database.Shared.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Rotativa.AspNetCore;
using Database.Shared.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Web;
using Wkhtmltopdf.NetCore;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using sistema.Service;
using System.Text.Json.Serialization;
using System.Collections.Concurrent;
using farmamest.Service.IService;
using farmamest.Service;
using sistema.Service.IService;
using sistema.UtilidadesEmailWp.Services;
using sistema.UtilidadesEmailWp.Services.IService;
using farmamest.UtilidadesEmailWp;
using Microsoft.Extensions.Options;
using Sistema.Services.WebAuthn;
using Database.Shared.Repository;
using Hangfire;
using Hangfire.PostgreSql;

namespace farmamest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();

            services.Configure<FelSettings>(Configuration.GetSection("Fel"));

            services.AddHttpClient("Fel", (sp, client) =>
            {
                var fel = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<FelSettings>>().Value;

                if (string.IsNullOrWhiteSpace(fel.BaseUrl))
                    throw new InvalidOperationException("Falta configurar Fel:BaseUrl");

                client.BaseAddress = new Uri(fel.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(fel.TimeoutSeconds <= 0 ? 30 : fel.TimeoutSeconds);
            });

            services.AddFido2(options =>
          {
              var fido2Section = Configuration.GetSection("Fido2");
              options.ServerDomain = fido2Section["ServerDomain"];
              options.ServerName = fido2Section["ServerName"];
              options.Origins = new HashSet<string>(fido2Section.GetSection("Origins").Get<string[]>());
              options.TimestampDriftTolerance = fido2Section.GetValue<int>("TimestampDriftTolerance");
          });


            // services.AddFido2(options =>
            // {
            //     options.ServerDomain = "hospitalsancristobalgt.com";
            //     options.ServerName = "Hospital San Cristobal"; // (puede ser el nombre que quieras)
            //     options.Origins = new HashSet<string> { "https://hospitalsancristobalgt.com/" };
            //     options.TimestampDriftTolerance = 300000;
            // });

            // services.AddFido2(options =>
            // {
            //     options.ServerDomain = "localhost";
            //     options.ServerName = "AVM Software";
            //     options.Origins = new HashSet<string> { "http://localhost:5000" };
            //     options.TimestampDriftTolerance = 300000;
            // });


            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder =>
                    {
                        builder.WithOrigins("https://middleware-staging.dev-land.space")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            services.AddHttpContextAccessor();
            // services.AddControllersWithViews();
            services.AddRazorPages();
            // services.Configure<ForwardedHeadersOptions>(options =>
            // {
            //     options.KnownProxies.Add(IPAddress.Parse("10.0.0.100"));
            // });

            services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<Context>();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(2880);
            });

            services.AddControllersWithViews();

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                options.JsonSerializerOptions.MaxDepth = 64;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true; // ← agrega esto


            });
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            //services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            //services.AddDbContext<Context>(options => options.UseSqlServer(Configuration.GetConnectionString("farmaowl")));
            services.AddDbContext<Context>(options => options.UseNpgsql(Configuration.GetConnectionString("farmaowl")));
            services.AddScoped<Database.Shared.IRepository.IConfiguracionSistema, Database.Shared.Data.ConfiguracionSistemaRepository>();
            services.AddScoped<Database.Shared.IRepository.IDespegablesProducto, Database.Shared.Data.CategoriaRepository>();
            services.AddScoped<Database.Shared.IRepository.IBodega, Database.Shared.Data.BodegaRepository>();
            services.AddScoped<Database.Shared.IRepository.IAmbiente, Database.Shared.Data.AmbienteRepository>();
            services.AddScoped<Database.Shared.IRepository.IProducto, Database.Shared.Data.ProductosRepository>();
            services.AddScoped<Database.Shared.IRepository.IProductoEquivalencia, Database.Shared.Data.ProductoEquivalenciaRepository>();
            services.AddScoped<Database.Shared.IRepository.IEmpleado, Database.Shared.Data.EmpleadoRepository>();
            services.AddScoped<Database.Shared.IRepository.ICliente, Database.Shared.Data.ClienteRepository>();
            services.AddScoped<Database.Shared.IRepository.IProveedor, Database.Shared.Data.ProveedorRepository>();
            services.AddScoped<Database.Shared.IRepository.IVenta, Database.Shared.Data.VentaRepository>();
            services.AddScoped<Database.Shared.IRepository.ICompra, Database.Shared.Data.CompraRepository>();
            services.AddScoped<Database.Shared.IRepository.IServicio, Database.Shared.Data.ServicioRepository>();
            //services.AddScoped<Database.Shared.IRepository.IVentaServicio, Database.Shared.Data.VentaServicioRepository>();
            services.AddScoped<Database.Shared.IRepository.IEnvio, Database.Shared.Data.EnvioRepository>();
            services.AddScoped<Database.Shared.IRepository.IRuta, Database.Shared.Data.RutaRepository>();
            services.AddScoped<Database.Shared.IRepository.ICaja, Database.Shared.Data.CajaRepository>();
            services.AddScoped<Database.Shared.IRepository.IGasto, Database.Shared.Data.GastoRepository>();
            services.AddScoped<Database.Shared.IRepository.IVisitadorMedico, Database.Shared.Data.VisitadorMedicoRepository>();
            services.AddScoped<Database.Shared.IRepository.ICategoriaGasto, Database.Shared.Data.CategoriaGastoRepository>();
            services.AddScoped<Database.Shared.IRepository.IUser, Database.Shared.Data.UserRepository>();
            services.AddScoped<Database.Shared.IRepository.ICotizacion, Database.Shared.Data.CotizacionRepository>();
            services.AddScoped<Database.Shared.IRepository.ICitas, Database.Shared.Data.CitasRepository>();
            services.AddScoped<Database.Shared.IRepository.IChart, Database.Shared.Data.ChartRepository>();
            services.AddScoped<Database.Shared.IRepository.ITraslados, Database.Shared.Data.TrasladosRepository>();
            //services.AddScoped<Database.Shared.IRepository.ICajaClinica, Database.Shared.Data.CajaClinicaRepository>();
            services.AddScoped<Database.Shared.IRepository.IConsultas, Database.Shared.Data.ConsultasRepository>();
            services.AddScoped<Database.Shared.IRepository.IPacientes, Database.Shared.Data.PacientesRepository>();
            services.AddScoped<Database.Shared.IRepository.IHospitalizacion, Database.Shared.Data.HospitalizacionRepository>();
            services.AddScoped<Database.Shared.IRepository.IHospitalizacionProductoObservacion, Database.Shared.Data.HospitalizacionProductoObservacionRepository>();
            services.AddScoped<Database.Shared.IRepository.IHabitacion, Database.Shared.Data.HabitacionRepository>();
            services.AddScoped<Database.Shared.IRepository.IPersonas, Database.Shared.Data.PersonasRepository>();
            services.AddScoped<Database.Shared.IRepository.ICuentasPorCobrar, Database.Shared.Data.CuentasPorCobrarRepository>();
            services.AddScoped<Database.Shared.IRepository.IPrecios, Database.Shared.Data.PreciosRepository>();
            services.AddScoped<Database.Shared.IRepository.IGrabaciones, Database.Shared.Data.GrabacionesRepository>();
            services.AddScoped<Database.Shared.IRepository.ILaboratorioClinico, Database.Shared.Data.LaboratorioClinico>();
            services.AddScoped<Database.Shared.IRepository.ISucursal, Database.Shared.Data.SucursalRepository>();
            services.AddScoped<Database.Shared.IRepository.IPaquetesHospitalizacion, Database.Shared.Data.PaquetesHospitalizacionRepository>();
            services.AddScoped<Database.Shared.IRepository.IVacunas, Database.Shared.Data.VacunasRepository>();
            services.AddScoped<Database.Shared.IRepository.IAlergiaRaraPacientes, Database.Shared.Data.AlergiaRaraPacientesRepository>();
            services.AddScoped<Database.Shared.IRepository.IArchivos, Database.Shared.Data.ArchivosRepository>();
            services.AddScoped<Database.Shared.IRepository.IGrafica, Database.Shared.Data.GraficaRepository>();
            services.AddScoped<Database.Shared.IRepository.IRecetas, Database.Shared.Data.RecetaRepository>();
            services.AddScoped<Database.Shared.IRepository.IFormasPago, Database.Shared.Data.FormasPagoRepository>();
            services.AddScoped<Database.Shared.IRepository.IEstadoExamen, Database.Shared.Data.EstadoExamenRepository>();
            services.AddScoped<Database.Shared.IRepository.ILaboratorioProducto, Database.Shared.Data.LaboratorioProductoRepository>();
            services.AddScoped<Database.Shared.IRepository.IBanco, Database.Shared.Data.BancoRepository>();
            services.AddScoped<Database.Shared.IRepository.ISeguro, Database.Shared.Data.SeguroRepository>();
            services.AddScoped<Database.Shared.IRepository.ICategoriaCuentaContable, Database.Shared.Data.CategoriaCuentaContableRepository>();
            services.AddScoped<Database.Shared.IRepository.ICuentaContable, Database.Shared.Data.CuentaContableRepository>();
            services.AddScoped<Database.Shared.IRepository.ICuentas, CuentasRepository>();
            services.AddScoped<Database.Shared.IRepository.ITipoCuenta, TipoCuentaRepository>();
            services.AddScoped<Database.Shared.IRepository.INomenclatura, NomenclaturaRepository>();
            services.AddScoped<Database.Shared.IRepository.IDetallePaqueteHospitalizacion, DetallePaqueteHospitalizacionRepository>();
            services.AddScoped<IHospitalizacionDetallePaqueteHospitalizacion, HospitalizacionDetallePaqueteHospitalizacionRepository>();
            services.AddScoped<IAuditoria, AuditoriaRepository>();
            services.AddScoped<ITipoProducto, TipoProductoRepository>();
            services.AddScoped<ITipoCompra, TipoCompraRepository>();
            services.AddScoped<ITurnoEnfermeria, TurnoEnfermeriaRepository>();
            services.AddScoped<INotaEnfermeria2, NotaEnfermeria2Repository>();
            services.AddScoped<INotaEvolucion, NotaEvolucionRepository>();
            services.AddScoped<INotaOperatoria, NotaOperatoriaRepository>();
            services.AddScoped<IEmergencias, EmergenciasRepository>();
            services.AddScoped<IInfoIngesta, InfoIngestaRepository>();
            services.AddScoped<IConsentimientoHospi, ConsentimientoHospiRepository>();
            services.AddScoped<ISolicitudMedicamentoRepository, SolicitudMedicamentoRepository>();
            services.AddScoped<ISolicitudMedicamentoNacionalRepository, SolicitudMedicamentoNacionalRepository>();
            services.AddScoped<IDevolucionMedicamentoRepository, DevolucionMedicamentoRepository>();
            services.AddScoped<IHospitalizacionExamenPdf, HospitalizacionExamenPdfRepository>();
            services.AddScoped<IRequision, RequisionRepository>();
            services.AddScoped<IDevolucionNacional, DevolucionNacionalRepository>();

            services.AddScoped<IListaChequeo, ListaChequeoRepository>();
            services.AddScoped<IListaChequeoService, ListaChequeoService>();

            services.AddScoped<ICuestionarioPreAnestesico, CuestionarioPreAnestesicoRepository>();
            services.AddScoped<ICuestionarioPreAnestesicoService, CuestionarioPreAnestesicoService>();


            services.AddScoped<IKitIngreso, KitIngresoRepository>();
            services.AddScoped<IKitIngresoService, KitIngresoService>();

            # region Services
            services.AddScoped<ICalculadorCuentaService, CalculadorCuentaService>();

            services.AddScoped<IConsentimientoHospiService, ConsentimientoHospiService>();
            services.AddScoped<sistema.Service.IService.ICajaService, CajaService>();
            services.AddScoped<sistema.Service.IService.IEmergenciaService, EmergenciaService>();
            services.AddScoped<IProductosService, ProductosService>();
            services.AddScoped<IConsultasService, ConsultasService>();
            services.AddScoped<IPacientesService, PacientesService>();
            services.AddScoped<IHospitalizacionUsuarioAcceso, HospitalizacionUsuarioAccesoRepository>();
            services.AddScoped<IHospitalizacionUsuarioAccesoService, HospitalizacionUsuarioAccesoService>();
            services.AddScoped<ITurnoEnfermeriaService, TurnoEnfermeriaService>();
            services.AddScoped<INotaEnfermeria2Service, NotaEnfermeria2Service>();
            services.AddScoped<INotaEvolucionService, NotaEvolucionService>();
            services.AddScoped<INotaOperatoriaService, NotaOperatoriaService>();
            services.AddScoped<IControlGlucometria2, ControlGlucometria2Repository>();
            services.AddScoped<IControlGlucometria2Service, ControlGlucometria2Service>();
            services.AddScoped<IIngestaExcreta, IngestaExcretaRepository>();
            services.AddScoped<IInfoIngestaService, InfoIngestaService>();
            services.AddScoped<IIngestaExcretaService, IngestaExcretaService>();
            services.AddScoped<IEspecialidad, EspecialidadRepository>();
            services.AddScoped<INotaMedica2, NotaMedica2Repository>();
            services.AddScoped<INotaMedica2Service, NotaMedica2Service>();
            services.AddScoped<ICategoriaGeneralLabClinico, CategoriaGeneralLabClinicoRepository>();
            services.AddScoped<ICategoriaGeneralLabClinicoService, CategoriaGeneralLabClinicoService>();
            services.AddScoped<IHospitalizacionService, HospitalizacionService>();
            services.AddScoped<IHospitalizacionProductoObservacionService, HospitalizacionProductoObservacionService>();
            services.AddScoped<IHospitalizacionUsuarioAccesoService, HospitalizacionUsuarioAccesoService>();
            services.AddScoped<IServiciosService, ServiciosService>();
            services.AddScoped<IComprasService, ComprasService>();
            services.AddScoped<ICuentasPorCobrarService, CuentasPorCobrarService>();
            services.AddScoped<IFilesService, FilesService>();
            services.AddScoped<IGraficasService, GraficasService>();
            services.AddScoped<ISolicitudMedicamentoService, SolicitudMedicamentoService>();
            services.AddScoped<ISolicitudMedicamentoNacionalService, SolicitudMedicamentoNacionalService>();
            services.AddScoped<IDevolucionMedicamentoService, DevolucionMedicamentoService>();
            services.AddScoped<IHospitalizacionExamenPdfService, HospitalizacionExamenPdfService>();
            #endregion

            #region Configuracion WhatsApp e Email
            services.Configure<WhatsAppSettings>(Configuration.GetSection("WhatsAppSettings"));
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IWhatsAppService, WhatsAppService>();
            #endregion

            services.AddScoped<IHomeService, HomeService>();
            services.AddScoped<Database.Shared.IRepository.ICie10, Database.Shared.Data.Cie10Repository>();

            #region Oftalmologia, podologia, HistoriaClinicaEnfermeria, ValoracionInicialEnfermeria, Sueroterapia
            services.AddScoped<IConsultasOftalmologia, ConsultasOftalmologiaRepository>();
            services.AddScoped<IConsultasPodologia, ConsultasPodologiaRepository>();
            services.AddScoped<IConsultasHistoriaClinicaEnfermeria, ConsultasHistoriaClinicaEnfermeriaRepository>();
            services.AddScoped<IConsultasValoracionInicialEnfermeria, ConsultasValoracionInicialEnfermeriaRepository>();
            services.AddScoped<IConsultasSueroterapia, ConsultasSueroterapiaRepository>();
            #endregion


            services.AddDistributedMemoryCache();

            services.AddScoped<IWebAuthnStore, EfWebAuthnStore>();
            services.AddScoped<IWebAuthnService, WebAuthnService>();

            services.AddScoped<IHospitalizacionDocumentoRepository, HospitalizacionDocumentoRepository>();

            services.AddScoped<IMedicamentoNoControladoRepository, MedicamentoNoControladoRepository>();
            services.AddScoped<IMedicamentoNotificacionService, MedicamentoNotificacionService>();


            services.AddControllers().AddJsonOptions(opts =>
                opts.JsonSerializerOptions.DefaultIgnoreCondition =
                    System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull);




            services.Configure<IdentityOptions>(options =>
              {
                  // Password settings.
                  options.Password.RequireDigit = false;
                  options.Password.RequireLowercase = false;
                  options.Password.RequireNonAlphanumeric = false;
                  options.Password.RequireUppercase = false;
                  options.Password.RequiredLength = 6;
                  options.Password.RequiredUniqueChars = 1;

                  // Lockout settings.
                  options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(60);
                  //options.Lockout.MaxFailedAccessAttempts = 5;
                  options.Lockout.AllowedForNewUsers = true;

                  // Default SignIn settings.
                  options.SignIn.RequireConfirmedEmail = false;
                  options.SignIn.RequireConfirmedPhoneNumber = false;

                  // User settings.
                  options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                  options.User.RequireUniqueEmail = true;

              });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                // options.Cookie.Name = Configuration["CookieName"];
                options.Cookie.Name = "YourAppCookieName";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(100000);

                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;
            });

            services.Configure<FormOptions>(x => x.ValueCountLimit = 10000);


            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                // options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // services.AddMvc(config =>
            // {
            //     var policy = new AuthorizationPolicyBuilder()
            //                     .RequireAuthenticatedUser()
            //                     .Build();
            //     config.Filters.Add(new AuthorizeFilter(policy));
            // });

            services.Configure<PasswordHasherOptions>(option =>
            {
                option.IterationCount = 12000;
            });

            services.AddWkhtmltopdf();
            // services.AddMvc().SetC services.AddWkhtmltopdf();ompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);
            //services.AddWkhtmltopdf("Rotativa");
            services.AddControllersWithViews().AddRazorRuntimeCompilation();


            services.AddControllersWithViews()
            .AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            var connectionString = Configuration.GetConnectionString("farmaowl");
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Connection string 'farmaowl' not found.");

            services.AddHangfire(config => config.UsePostgreSqlStorage(connectionString));
            services.AddHangfireServer();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Context context)
        {
            //context.Database.Migrate();


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // app.UseForwardedHeaders(new ForwardedHeadersOptions
            // {
            //     ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            // });


            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders =
                ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto |
                ForwardedHeaders.XForwardedHost,
                ForwardLimit = null
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            //cookie policy
            //app.UseCookiePolicy();
            app.UseCors("AllowSpecificOrigin");

            app.UseRouting();
            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseDefaultFiles();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            //Rotativa.AspNetCore.RotativaConfiguration.Setup(env.WebRootPath, "Rotativa/Linux");
            Rotativa.AspNetCore.RotativaConfiguration.Setup(env.WebRootPath, "Rotativa/Windows");

        }
    }
}
