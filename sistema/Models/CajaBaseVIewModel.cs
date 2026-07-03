using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Database.Shared.Paginacion;
using System.ComponentModel.DataAnnotations;

namespace sistema.Models
{
    public class CajaBaseViewModel
    {
        public CajaBaseViewModel()
        {
            Caja.ResponsableApertura = new User();
            Caja.ResponsableCierre = new User();
        }
        public decimal CajaMontoApertura { get; set; }
        public string CajaNombrePersonalizado { get; set; }
        public Caja Caja { get; set; } = new Caja(); //En proceso de eliminarse
        public IList<Caja> ListaCajas { get; set; }

        //AperturarAmbienteId determina el tipo de bodega
        //Farmacia, Clinica, Bodega
        //De esta manera se maneja en una sola vista todas las cajas y el desarrollo se hace mas eficiente
        public int AperturarAmbienteId { get; set; }
        public int? AperturarSucursalId { get; set; }
        public SelectList ListaSucursales { get; set; }

        public void Init(ICaja _cajaRepository, ISucursal _sucursalRepository)
        {
            ListaCajas = _cajaRepository.ListarCajas();
            ListaSucursales = new SelectList(_sucursalRepository.GetList(), "Id", "NombreSucursal");
        }

        public int Id
        {
            get { return Caja.Id; }
            set { Caja.Id = value; }
        }
    }
}