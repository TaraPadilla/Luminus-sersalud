using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace sistema.Models
{
    public class TrasladoViewModel
    {
        public int? TrasladoId { get; set; }

        public int? BodegaOrigenId { get; set; }
        public int? BodegaDestinoId { get; set; }
        public List<TrasladoProductoViewModel> Productos { get; set; }
        public string Observaciones { get; set; }
        public string EstadoTraslado { get; set; }
        public SelectList ListaBodegasOrigen { get; set; }
        public SelectList ListaBodegasDestino { get; set; }
        public void Init(IBodega bodegaRepository, bool rolFarmacia)
        {
            var bodegas = bodegaRepository.GetList();
            var bodegasOrigen = bodegas;
            var bodegasDestino = bodegas;
            if (rolFarmacia)
            {
                //El rol farmacia no puede ver en las bodegas de origen
                //la bodega con el nombre BODEGA
                bodegasOrigen = bodegasOrigen.Where(a => a.NombreBodega != "Bodega").ToList();
            }
            ListaBodegasOrigen = new SelectList(bodegasOrigen, "Id", "BodegaSucursalText");
            ListaBodegasDestino = new SelectList(bodegasDestino, "Id", "BodegaSucursalText");
        }
    }
}