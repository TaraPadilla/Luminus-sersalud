using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using Database.Shared.IRepository;
using Database.Shared.Models;

namespace sistema.Models
{
    public class DevolucionViewModel
    {

        public int? RequisicionId { get; set; }

        public int? BodegaOrigenId { get; set; }
        public int? BodegaDestinoId { get; set; }

        public List<TrasladoProductoViewModel> Productos { get; set; } = new();

        public string Observaciones { get; set; }
        public RequisicionEstado EstadoRequisicion { get; set; } = RequisicionEstado.Borrador;

        public SelectList ListaBodegasOrigen { get; set; }
        public SelectList ListaBodegasDestino { get; set; }

        // =========================
        // ENCABEZADO
        // =========================
        public string Direccion { get; set; }
        public string Departamento { get; set; }
        public string UnidadSeccion { get; set; }
        public string Otros { get; set; }
        public int? NumeroDevolucion { get; set; }
        public DateTime? FechaSolicitud { get; set; }

        // =========================
        // FIRMAS / NOMBRES  
        // =========================
        public string NombreSolicitante { get; set; }
        public string FirmaSolicitanteBase64 { get; set; }
        public string NombreJefaturaCoordinador { get; set; }
        public string NombreGerenteAdministrativo { get; set; }
        public string NombreEncargadoAlmacen { get; set; }
        public string NombreReceptorFinal { get; set; }
        public string JefaturaNombre { get; set; }
        public string GerenciaNombre { get; set; }
        public string NombreBodega { get; set; }
        public string VOBOJefatura { get; set; }
        public string AutorizacionGerencia { get; set; }
        public string AutorizacionAlmacen { get; set; }
        public string FirmaSolicitante { get; set; }
        public string EntregadoNombre { get; set; }

        public int? ProveedorId { get; set; }

        public int? NumeroOficio  { get; set; }


        // =========================
        // INIT  
        // =========================
        public void Init(IBodega bodegaRepository, bool rolFarmacia)
        {
            var bodegas = bodegaRepository.GetList();
            var bodegasOrigen = bodegas;
            var bodegasDestino = bodegas;

            if (rolFarmacia)
                bodegasOrigen = bodegasOrigen.Where(a => a.NombreBodega != "Bodega").ToList();

            ListaBodegasOrigen = new SelectList(bodegasOrigen, "Id", "BodegaSucursalText", BodegaOrigenId);
            ListaBodegasDestino = new SelectList(bodegasDestino, "Id", "BodegaSucursalText", BodegaDestinoId);
        }
    }
}