using Database.Shared.Models;
using Database.Shared.IRepository;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Paginacion;


namespace Database.Shared.Data
{
    public class ProveedorRepository : IProveedor
    {

        private readonly Context _context = null;

        public ProveedorRepository(Context context)
        {
            _context = context;
        }

        public void Add(Proveedor proveedor, bool saveChanges = true)
        {
            _context.Proveedores.Add(proveedor);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        public Proveedor Get(int id, bool includeRelatedEntities = true)
        {
            return _context.Proveedores
                .Include(x => x.TipoCompraProveedor)
               .Where(a => a.Id == id)
               .SingleOrDefault();


        }

        public Proveedor GetByNombre(string nombre, bool includeRelatedEntities = true)
        {
            return _context.Proveedores
                .Include(x => x.TipoCompraProveedor)
               .Where(a => a.Nombre == nombre)
               .SingleOrDefault();


        }


        public List<Proveedor> GetList() => _context.Proveedores.Where(x=>x.Eliminado==false).ToList();
    
        public List<Banco> ListarBancos() => _context.Bancos.ToList();

        public PaginacionList<Proveedor> PaginacionProveedores(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var proveedor = _context.Proveedores.AsQueryable();


            // para hacer la busqueda, el IsNullOrEmpty verifica que no este vacia la cadena entrante
            if(!string.IsNullOrEmpty(searchString))
            {
                proveedor = proveedor.Where(s => s.Nombre.Contains(searchString));
            }

            // esto que hice no es buena practica, lo ideal seria hacer un patron por estrategia
            // visitar : https://refactoring.guru/es/design-patterns/strategy
            // asi como lo tengo funciona pero no es tan tan tan recomendado
            // quizas mas adelante lo mejoremos con un patron de estrategia.

            switch(sortOrder)
            {
                case "Nombre_desc":
                proveedor = proveedor.OrderByDescending(s => s.Nombre);
                break;

                default:
                proveedor = proveedor.OrderBy(s => s.Nombre);
                break;
            }

            return PaginacionList<Proveedor>.CreateAsyncc(proveedor.Where(x=>x.Eliminado==false), pageNumber ?? 1, pageSize);
        }

        public Proveedor GetProveedorPorNombre(string nombre)
        {
            return _context.Proveedores.Where(a => a.Nombre == nombre).SingleOrDefault();
        }

        public void Update(Proveedor model, bool saveChanges = true)
        {

            _context.Entry(model).State = EntityState.Modified;

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

    }
}