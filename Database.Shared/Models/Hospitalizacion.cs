using Database.Shared.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class Hospitalizacion
    {
        public Hospitalizacion()
        {
            HospitalizacionesServicios = new List<HospitalizacionServicio>();
            HospitalizacionesProductos = new List<HospitalizacionProducto>();
            HospitalizacionesExamenes = new List<HospitalizacionExamen>();
            ExamenesFisicosHosp = new List<ExamenFisicoHosp>();
            DetallesCuentaPorCobrar = new List<DetalleCuentaPorCobrar>();
            HospitalizacionesPaquetesHospitalizacion = new List<HospitalizacionPaqueteHospitalizacion>();
            HospitalizacionUsuariosAcceso = new List<HospitalizacionUsuarioAcceso>();
            Consultas = new List<Consulta>();
        }
        public List<OrdenesMedicas> OrdenesMedicas { get; set; } = new List<OrdenesMedicas>();

        public int Id { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int HabitacionId { get; set; }
        public Habitacion Habitacion { get; set; }
        public int CategoriaHabitacionTarifaId { get; set; }
        public CategoriaHabitacionTarifa CategoriaHabitacionTarifa { get; set; }
        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; }
        public bool Finalizada { get; set; }
        public DateTime? FechaHoraFinalizada { get; set; }
        public bool Pagada { get; set; }
        public DateTime? FechaHoraPago { get; set; }
        public string Observaciones { get; set; }
        public bool Eliminada { get; set; }
        public int? EspecialidadId { get; set; }
        public Especialidad Especialidad { get; set; }
        public string UrlArchivoConsentimiento { get; set; }
        public ICollection<HospitalizacionUsuarioAcceso> HospitalizacionUsuariosAcceso { get; set; }
        public ICollection<HospitalizacionServicio> HospitalizacionesServicios { get; set; }
        public ICollection<HospitalizacionProducto> HospitalizacionesProductos { get; set; }
        public ICollection<HospitalizacionExamen> HospitalizacionesExamenes { get; set; }
        public ICollection<ExamenFisicoHosp> ExamenesFisicosHosp { get; set; }
        public ICollection<DetalleCuentaPorCobrar> DetallesCuentaPorCobrar { get; set; }
        public ICollection<HospitalizacionPaqueteHospitalizacion> HospitalizacionesPaquetesHospitalizacion { get; set; }
        public ICollection<Consulta> Consultas { get; set; }
        public int NochesInt
        {
            get
            {
                int noches = 0;
                if (FechaInicio != null && FechaFin != null)
                {
                    var fechaInicio = new DateTime(
                            FechaInicio.Year,
                            FechaInicio.Month,
                            FechaInicio.Day
                            );
                    var fechaFin = new DateTime(
                        FechaFin.Year,
                        FechaFin.Month,
                        FechaFin.Day
                        );
                    if (fechaInicio != fechaFin)
                    {
                        noches = (fechaFin - fechaInicio).Days;
                    }
                    else
                    {
                        noches = 1;
                    }
                }
                return noches;
            }
        }
        public decimal ValorNocheDecimal
        {
            get
            {
                if (CategoriaHabitacionTarifa != null)
                {
                    return CategoriaHabitacionTarifa.ValorTarifa;
                }
                else
                {
                    return 0;
                }
            }
        }
        public decimal ValorHabitacionDecimal
        {
            get
            {
                return ValorNocheDecimal * Convert.ToDecimal(NochesInt);
            }
        }
        public decimal ValorExamenesDecimal
        {
            get
            {
                decimal valorExamenes = 0;
                if (HospitalizacionesExamenes != null)
                {
                    foreach (var examen in HospitalizacionesExamenes)
                    {
                        if (examen.Examen != null && examen.Examen.DetalleExamenes != null)
                        {
                            foreach (var detalleExamen in examen.Examen.DetalleExamenes)
                            {
                                valorExamenes += detalleExamen.PrecioValor;
                            }
                        }
                    }
                }
                return valorExamenes;
            }
        }
        public decimal ValorServiciosDecimal
        {
            get
            {
                decimal valorServicios = 0;
                if (HospitalizacionesServicios != null)
                {
                    foreach (var servicio in HospitalizacionesServicios)
                    {
                        valorServicios += servicio.Precio * servicio.Cantidad;
                    }
                }
                return valorServicios;
            }
        }
        public decimal ValorMedicamentosDecimal
        {
            get
            {
                decimal valorMedicamentos = 0;
                if (HospitalizacionesProductos != null)
                {
                    foreach (var medicamento in HospitalizacionesProductos)
                    {
                        valorMedicamentos += medicamento.PrecioValor * medicamento.Cantidad;
                    }
                }
                return valorMedicamentos;
            }
        }
    }
}
