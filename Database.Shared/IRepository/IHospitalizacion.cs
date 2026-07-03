using Database.Shared.Models;
using System.Collections.Generic;
using Database.Shared.Paginacion;

namespace Database.Shared.IRepository
{
    public interface IHospitalizacion
    {
        public Hospitalizacion Add(Hospitalizacion hospitalizacion, bool saveChanges = true);
        public Hospitalizacion Get(
            int hospitalizacionId,
            bool includeMedicamentos = true,
            bool includeServicios = true,
            bool includeExamenes = true,
            bool includePaquetes = true,
            bool includeConsultas = true,
            bool includeOrdenesMedicas = true
            );
        public List<NotaMedica2> GetNotasMedicasByHospitalizacion(int hospitalizacionId);
        public OrdenesMedicas GetOrdenMedica(int ordenMedicaId);
        public ServicioPrecio GetServicioPrecioById(int id);
        public void AddServicio(HospitalizacionServicio hospitalizacionServicio);
        public HospitalizacionServicio GetHospitalizacionServicio(int hospitalizacionServicioId);
        public HospitalizacionProducto AddMedicamento(HospitalizacionProducto hospitalizacionProducto);
        public HospitalizacionProducto GetHospitalizacionMedicamento(int hospitalizacionMedicamentoId);
        public void AddProductoAplicacion
            (HospitalizacionProductoAplicacion hospitalizacionProductoAplicacion);
        public void AddExamen(HospitalizacionExamen hospitalizacionExamen);
        public HospitalizacionExamen GetHospitalizacionExamen(int hospitalizacionExamenId);
        public List<HospitalizacionServicio> GetServicios(int hospitalizacionId);
        public List<HospitalizacionProducto> GetProductos(int hospitalizacionId);
        public List<HospitalizacionProductoAplicacion> GetProductosAplicacion(int hospitalizacionId);
        public HospitalizacionProductoAplicacion GetProductoAplicacion(int hospitalizacionProductoAplicacionId);
        public HospitalizacionServicio GetServicioHospitalizacion(int hospitalizacionServicioId);
        public void UpdateProductoAplicacion(HospitalizacionProductoAplicacion hospitalizacionProductoAplicacion);
        public void Update(HospitalizacionServicio hospitalizacionServicio);
        public List<HospitalizacionExamen> GetExamenes(int hospitalizacionId);
        public List<DatoExamenFisicoHosp> GetDatosExamenFisicoHosp();
        public void AddExamenFisicoHosp(ExamenFisicoHosp examen);
        public List<ExamenFisicoHosp> GetExamenesFisicosHosp(int hospitalizacionId);
        public void Update(Hospitalizacion hospitalizacion);
        public void Update(HospitalizacionProducto hospitalizacionProducto);
        public void Update(HospitalizacionExamen hospitalizacionExamen);
        public void Update(HospitalizacionPaqueteHospitalizacion hospitalizacionPaqueteHospitalizacion);
        public void Update(HospitalizacionReceta hospReceta);
        public PaqueteHospitalizacion AddPaqueteHospitalizacion(PaqueteHospitalizacion paquete, bool saveChanges = true);
        public PaqueteHospitalizacion GetPaqueteHospitalizacion(int paqueteId);
        public List<PaqueteHospitalizacion> GetPaquetesHospitalizacion();
        public void AddHospitalizacionPaqueteHospitalizacion(HospitalizacionPaqueteHospitalizacion paquete);
        public HospitalizacionPaqueteHospitalizacion GetHospitalizacionPaqueteHospitalizacion(int hospitalizacionPaqueteHospitalizacionId);
        public List<HospitalizacionPaqueteHospitalizacion> GetPaquetesAgregados(int hospitalizacionId);
        public void UpdatePaqueteHospitalizacion(PaqueteHospitalizacion paquete);
        public void DeletePaqueteHospitalizacion(int paqueteId);
        public void SaveChanges();
        List<Hospitalizacion> GetHospitalizaciones();
        public void AddHospitalizacionReceta(HospitalizacionReceta entity);
        public List<HospitalizacionReceta> GetHospitalizacionRecetaByIdHospitalizacion(int hospitalizacionId);
        public DetalleCuentaPorCobrar GetDetalleCuenta(int hospitalizacionId);
        public void DeleteHospitalizacionReceta(int IdHospitalizacionReceta);
        public HospitalizacionReceta GetHospitalizacionRecetaById(int IdHospitalizacionReceta);

        public void UpdateHospitalicacionReceta(HospitalizacionReceta receta);
        public List<HospitalizacionPaqueteHospitalizacion> GetHospitalizacionPaqueteByIdHospitalizacion(int HospitalizacionId);
        public HospitalizacionPaqueteHospitalizacion GetHospitalizacionPaqueteHospitalizacionById(int Id);
        NotaMedica2 GetNotaMedica2(int notaMedicaId);
        public void UpdateHospitalizacionPaqueteHospitalizacion(HospitalizacionPaqueteHospitalizacion entity);
        public HospitalizacionRecetaDetalle GetHospitalizacionRecetaDetalleById(int Id);
        public void UpdateHospitalicacionRecetaDetalle(HospitalizacionRecetaDetalle receta);

        public List<HospitalizacionRecetaDetalle> GetHospitalizacionRecetaDetalleByIdHospitalizacion(int hospitalizacionId);
        Hospitalizacion GetHospitalizacionById(int hospitalizacionId);
        public void AddCambioHabitacion(HospitalizacionCambioHabitacion cambio);
        public List<HospitalizacionCambioHabitacion> GetCambiosHabitacion(int hospitalizacionId);
        public HospitalizacionCambioHabitacion GetCambioHabitacion(int cambioHabitacionId);
        public void DeleteCambioHabitacion(int cambioHabitacionId);
        public void UpdateCambioHabitacion(HospitalizacionCambioHabitacion cambio);

        NotaOperatoria GetNotaOperatoria(int notaOperatoriaId);
        IEnumerable<NotaOperatoria> GetNotasOperatoriasByHospitalizacion(int hospitalizacionId);

        public List<HospitalizacionGastoAdministrativo> GetGastosAdministrativos(int hospitalizacionId);

    }
}