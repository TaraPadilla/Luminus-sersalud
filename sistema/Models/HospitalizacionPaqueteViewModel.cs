using farmamest.Models;
using System.Collections.Generic;

namespace sistema.Models
{
    public class HospitalizacionPaqueteViewModel
    {
        public string Fecha { get; set; }
        public int? Id { get; set; }
        public string CodigoInterno { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public List<HospitalizacionPaqueteProductoAgregadoViewModel> Productos { get; set; }
        public List<HospitalizacionPaqueteServicioAgregadoViewModel> Servicios { get; set; }
        public List<HospitalizacionPaqueteLaboratorioAgregadoViewModel> Laboratorios { get; set; }
        public decimal PrecioPaquete { get; set; }
        public decimal PrecioCosto { get; set; }
    }
}
