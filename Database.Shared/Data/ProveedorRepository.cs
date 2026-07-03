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
            var existing = _context.Proveedores
                .Include(x => x.TipoCompraProveedor)
                .FirstOrDefault(x => x.Id == model.Id);

            if (existing == null)
                return;

            existing.Nombre = model.Nombre;
            existing.Direccion = model.Direccion;
            existing.Correo = model.Correo;
            existing.Giro = model.Giro;
            existing.Telefono_1 = model.Telefono_1;
            existing.Telefono_2 = model.Telefono_2;
            existing.Celular_1 = model.Celular_1;
            existing.Celular_2 = model.Celular_2;
            existing.Nit = model.Nit;
            existing.CuentaBancaria = model.CuentaBancaria;
            existing.BancoId = model.BancoId;
            existing.Observaciones = model.Observaciones;
            existing.TipoProveedor = model.TipoProveedor;
            existing.FrecuenciaEntrega = model.FrecuenciaEntrega;
            existing.DiasEntrega = model.DiasEntrega;
            existing.DiasCredito = model.DiasCredito;
            existing.PoliticasDevolucion = model.PoliticasDevolucion;
            existing.PoliticasDevolucionVencimiento = model.PoliticasDevolucionVencimiento;

            if (model.TipoCompraProveedor != null)
            {
                _context.TipoCompraProveedor.RemoveRange(existing.TipoCompraProveedor);
                foreach (var tipoCompra in model.TipoCompraProveedor)
                {
                    existing.TipoCompraProveedor.Add(new TipoCompraProveedor
                    {
                        ProveedorId = existing.Id,
                        TipoCompraId = tipoCompra.TipoCompraId
                    });
                }
            }

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

    }
}