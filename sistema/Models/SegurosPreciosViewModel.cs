
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace sistema.Models
{
    public class SegurosPreciosViewModel
    {
        // public DateTime FechaEdicion { get; set; }
        public SelectList SegurosSelectListItem { get; set; }

        public int SeguroId { get; set; }

        public int TipoPrecio { get; set; }
        public int TipoItem { get; set; }
        public void Init(ISeguro seguroRepository)
        {

            SegurosSelectListItem = new SelectList(seguroRepository.GetList(), "Id", "Nombre");
        }

    }

}