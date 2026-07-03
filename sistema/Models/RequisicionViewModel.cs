using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using Database.Shared.IRepository;
using Database.Shared.Models;

namespace sistema.Models
{
    public class RequisicionViewModel
    {
        // =========================
        // CORE (REQUISICIÓN)
        // =========================
        public int? RequisicionId { get; set; }

        public int? BodegaOrigenId { get; set; }
        public int? BodegaDestinoId { get; set; }

        public List<TrasladoProductoViewModel> Productos { get; set; } = new();

        public string Observaciones { get; set; }
        public RequisicionEstado EstadoRequisicion { get; set; } = RequisicionEstado.Borrador;

        public SelectList ListaBodegasOrigen { get; set; }
        public SelectList ListaBodegasDestino { get; set; }

        // =========================
        // ENCABEZADO REQUISICIÓN
        // =========================
        public string Direccion { get; set; }              // maxlength 35 en vista (hospital)
        public string Departamento { get; set; }           // maxlength 60
        public string UnidadSeccion { get; set; }          // maxlength 60
        public string Otros { get; set; }                  // maxlength 60

        public int? NumeroRequisicion { get; set; }        // numérico (6) - validación se define luego
        public int? NumeroOrden { get; set; }              // numérico (4) - validación se define luego
        public DateTime? FechaSolicitud { get; set; }      // date

        // =========================
        // FIRMAS / SELLOS (SOLO TEXTO POR AHORA)
        // =========================
        public string NombreSolicitante { get; set; }              // maxlength 60
        public string FirmaSolicitanteBase64 { get; set; }         // base64 string (opcional, no se muestra si es null o vacío)
        public string NombreJefaturaCoordinador { get; set; }      // maxlength 60
        public string NombreGerenteAdministrativo { get; set; }    // maxlength 60
        public string NombreEncargadoAlmacen { get; set; }         // maxlength 60
        public string NombreReceptorFinal { get; set; }            // maxlength 60

        public string JefaturaNombre { get; set; }
        public string GerenciaNombre { get; set; }


        public string NombreBodega { get; set; }                  // maxlength 60

        public string VOBOJefatura { get; set; }
        public string AutorizacionGerencia { get; set; }

        public string AutorizacionAlmacen { get; set; }

        public string FirmaSolicitante { get; set; }

        public string EntregadoNombre { get; set; }


        // =========================
        // EXISTENTE
        // =========================
        public void Init(IBodega bodegaRepository, bool rolFarmacia)
        {
            var bodegas = bodegaRepository.GetList();

            var bodegasOrigen = bodegas
                .Where(b => b.Id == 40 || b.Id == 41 || b.Id == 42 || b.Id == 4)
                .ToList();

            var bodegasDestino = bodegas;

            if (rolFarmacia)
            {
                // El rol farmacia filtrará cualquier bodega de la lista anterior que se llame exactamente "Bodega"
                bodegasOrigen = bodegasOrigen.Where(a => a.NombreBodega != "Bodega").ToList();
            }

            this.BodegaOrigenId = 40;

            ListaBodegasOrigen = new SelectList(bodegasOrigen, "Id", "BodegaSucursalText");
            ListaBodegasDestino = new SelectList(bodegasDestino, "Id", "BodegaSucursalText");
        }
    }
}
