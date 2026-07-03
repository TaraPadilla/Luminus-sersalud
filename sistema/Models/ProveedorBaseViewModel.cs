using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;

namespace sistema.Models
{
    public class ProveedorBaseViewModel
    {
        public Proveedor Proveedor { get; set; } = new Proveedor();

        public List<Banco> Listabancos = new List<Banco>();

        public IFormFile imagen { get; set; }
        public SelectList Listbancos { get; set; }

        public bool Modificar { get; set; }
        public SelectList TipoCompras { get; set; }
        public int SelectedTipoCompraId { get; set; }
        public string ProveedorNombre { get; set; }
        public int? ProveedorPoliticasDevolucion { get; set; }
        public int? ProveedorDiasCredito { get; set; }
        public void Init(IProveedor proveedorRepository)
        {
            Listbancos = new SelectList(proveedorRepository.ListarBancos(), "Id", "Nombre");
        }
        public int Id
        {
            get { return Proveedor.Id; }
            set { Proveedor.Id = value; }
        }
    }
}