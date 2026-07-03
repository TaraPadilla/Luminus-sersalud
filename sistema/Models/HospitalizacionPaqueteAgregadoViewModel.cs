using Database.Shared.Models;
using System.Collections.Generic;

namespace sistema.Models
{
    public class HospitalizacionPaqueteAgregadoViewModel
    {
        public int? Id { get; set; }
        public int PaqueteId { get; set; }
        public string FechaHora { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public List<HospitalizacionPaqueteDetallePaqueteServicioViewModel> Servicios { get; set; }
        public List<HospitalizacionPaqueteDetallePaqueteProductoViewModel> Productos { get; set; }
        public List<HospitalizacionPaqueteDetallePaqueteLaboratorioViewModel> Laboratorios { get; set; }
        public decimal Precio { get; set; }

    }
    public class HospitalizacionPaqueteDetallePaqueteServicioViewModel
    {
        public int? Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Precio { get; set; }
    }
    public class HospitalizacionPaqueteDetallePaqueteProductoViewModel
    {
        public int? Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Precio { get; set; }
    }
    public class HospitalizacionPaqueteDetallePaqueteLaboratorioViewModel
    {
        public int? Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Precio { get; set; }
    }
}
