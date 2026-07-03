using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Database.Shared.Models
{
    public class Sucursal
    {
        #region Configuraciones desarrollador
        public bool FarmaciaHabilitada { get; set; }
        public bool ClinicaHabilitada { get; set; }
        public bool BodegaHabilitada { get; set; }
        public bool LaboratorioHabilitado { get; set; }
        #endregion

        public Sucursal()
        {
            Citas = new List<Citas>();
            SucursalServicios = new List<SucursalServicio>();
            Compras = new List<Compra>();
            Empleados = new List<Empleado>();
            //CajasLab = new List<CajaLab>();
            //CajasClinica = new List<CajaClinica>();
            Cajas = new List<Caja>();
            Bodegas = new List<Bodega>();
        }
        public int Id { get; set; }
        public string NombreSucursal { get; set; }
        public string Direccion { get; set; }
        public string Horario { get; set; }
        public bool Eliminado { get; set; } = false;
        public ICollection<Citas> Citas { get; set; }
        public ICollection<SucursalServicio> SucursalServicios { get; set; }
        public ICollection<Compra> Compras { get; set; }
        public ICollection<Empleado> Empleados { get; set; }
        //public ICollection<CajaLab> CajasLab { get; set; }
        //public ICollection<CajaClinica> CajasClinica { get; set; }
        public ICollection<Caja> Cajas { get; set; }
        public ICollection<Bodega> Bodegas { get; set; }

    }
}