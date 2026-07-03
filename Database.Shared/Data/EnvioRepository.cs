using Database.Shared.IRepository;
using Database.Shared.Models;
using Database.Shared.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Paginacion;
using System;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Database.Shared.Data
{
    public class EnvioRepository : IEnvio
    {

        private readonly Context _context = null;
        //  private readonly UserManager<IdentityUser> _userManager;
        public EnvioRepository(Context context)
        {
            _context = context;
        }

        public void Add(DetalleEnvio detalle, bool saveChanges = true)
        {
            _context.Add(detalle);
            if (saveChanges)
            {

                _context.SaveChanges();
            }
        }

        public void AddPago(Pagos pago, bool saveChanges = true)
        {
            _context.Add(pago);
            if (saveChanges)
            {

                _context.SaveChanges();
            }
        }

        public List<Envio> GetList() => _context.Envios
        .OrderByDescending(a => a.Id)
        .ToList();

         public List<FormaPago> GetListPagos() => _context.FormaPagos
        .OrderBy(a => a.Id)
            .Where(a=>!a.Eliminada)
        .ToList();

         public List<Envio> GetListadoFecha(DateTime inicio, DateTime final)=> _context.Envios.Include(s=>s.EstadosEnvio).Include(a => a.DetalleEnvios)
         .ThenInclude(a => a.Producto).Where(a => a.FechaEnvio<=final && a.FechaEnvio >=inicio).ToList();

        public List<DetalleEnvio> GetDetalle(int id, bool includeRelatedEntities = true)
        {

            var detalle = _context.DetalleEnvios.AsQueryable();

            

            return detalle.Where(x => x.Envio.Id == id).ToList();


        }

    

        public PaginacionList<Envio> PaginacionEnvios(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var envio = _context.Envios.AsQueryable();


            // para hacer la busqueda, el IsNullOrEmpty verifica que no este vacia la cadena entrante
            if(!string.IsNullOrEmpty(searchString))
            {
                envio = envio.Where(s => s.Id.ToString().Contains(searchString));
            }

            // esto que hice no es buena practica, lo ideal seria hacer un patron por estrategia
            // visitar : https://refactoring.guru/es/design-patterns/strategy
            // asi como lo tengo funciona pero no es tan tan tan recomendado
            // quizas mas adelante lo mejoremos con un patron de estrategia.

            switch(sortOrder)
            {
        
                default:
                envio = envio.OrderByDescending(s => s.FechaEnvio);
                break;
            }

            return PaginacionList<Envio>.CreateAsyncc(envio.Include(s=>s.EstadosEnvio).Include(s=>s.Ruta).Include(a=>a.User).ThenInclude(a => a.Persona), pageNumber ?? 1, pageSize);
        }

        public PaginacionList<Envio> PaginacionEnviosLiquidados(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var envio = _context.Envios.AsQueryable();


            // para hacer la busqueda, el IsNullOrEmpty verifica que no este vacia la cadena entrante
            if(!string.IsNullOrEmpty(searchString))
            {
                envio = envio.Where(s => s.Id.ToString().Contains(searchString));
            }

            // esto que hice no es buena practica, lo ideal seria hacer un patron por estrategia
            // visitar : https://refactoring.guru/es/design-patterns/strategy
            // asi como lo tengo funciona pero no es tan tan tan recomendado
            // quizas mas adelante lo mejoremos con un patron de estrategia.

            switch(sortOrder)
            {
        
                default:
                envio = envio.OrderByDescending(s => s.FechaEnvio);
                break;
            }

            return PaginacionList<Envio>.CreateAsyncc(envio.Include(s=>s.EstadosEnvio).Include(s=>s.Ruta).Include(a=>a.User).ThenInclude(a => a.Persona).Where(x=>x.EstadosEnvioId==3), pageNumber ?? 1, pageSize);
        }

        public PaginacionList<Envio> PaginacionEnviosPedidos(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var envio = _context.Envios.AsQueryable();


            // para hacer la busqueda, el IsNullOrEmpty verifica que no este vacia la cadena entrante
            if(!string.IsNullOrEmpty(searchString))
            {
                envio = envio.Where(s => s.Id.ToString().Contains(searchString));
            }

            // esto que hice no es buena practica, lo ideal seria hacer un patron por estrategia
            // visitar : https://refactoring.guru/es/design-patterns/strategy
            // asi como lo tengo funciona pero no es tan tan tan recomendado
            // quizas mas adelante lo mejoremos con un patron de estrategia.

            switch(sortOrder)
            {
        
                default:
                envio = envio.OrderByDescending(s => s.FechaEnvio);
                break;
            }

            return PaginacionList<Envio>.CreateAsyncc(envio.Include(s=>s.EstadosEnvio).Include(s=>s.Ruta).Include(a=>a.User).ThenInclude(a => a.Persona).Where(x=>x.EstadosEnvioId==1), pageNumber ?? 1, pageSize);
        }

        public PaginacionList<Envio> PaginacionEnviosRechazados(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var envio = _context.Envios.AsQueryable();


            // para hacer la busqueda, el IsNullOrEmpty verifica que no este vacia la cadena entrante
            if(!string.IsNullOrEmpty(searchString))
            {
                envio = envio.Where(s => s.Id.ToString().Contains(searchString));
            }

            // esto que hice no es buena practica, lo ideal seria hacer un patron por estrategia
            // visitar : https://refactoring.guru/es/design-patterns/strategy
            // asi como lo tengo funciona pero no es tan tan tan recomendado
            // quizas mas adelante lo mejoremos con un patron de estrategia.

            switch(sortOrder)
            {
        
                default:
                envio = envio.OrderByDescending(s => s.FechaEnvio);
                break;
            }

            return PaginacionList<Envio>.CreateAsyncc(envio.Include(s=>s.EstadosEnvio).Include(s=>s.Ruta).Include(a=>a.User).ThenInclude(a => a.Persona).Where(x=>x.EstadosEnvioId==4), pageNumber ?? 1, pageSize);
        }
        public PaginacionList<Envio> PaginacionEnviosEnRuta(string sortOrder, string searchString, int? pageNumber, int pageSize, string id)
        {
            var envio = _context.Envios.AsQueryable();


            // para hacer la busqueda, el IsNullOrEmpty verifica que no este vacia la cadena entrante
            if(!string.IsNullOrEmpty(searchString))
            {
                envio = envio.Where(s => s.Id.ToString().Contains(searchString));
            }

            // esto que hice no es buena practica, lo ideal seria hacer un patron por estrategia
            // visitar : https://refactoring.guru/es/design-patterns/strategy
            // asi como lo tengo funciona pero no es tan tan tan recomendado
            // quizas mas adelante lo mejoremos con un patron de estrategia.

            switch(sortOrder)
            {
        
                default:
                envio = envio.OrderByDescending(s => s.FechaEnvio);
                break;
            }

             if(id == ""){
                 return PaginacionList<Envio>.CreateAsyncc(envio.Include(s=>s.EstadosEnvio).Include(s=>s.Ruta).Include(a=>a.User).ThenInclude(a => a.Persona).Where(x=>x.EstadosEnvioId==2), pageNumber ?? 1, pageSize);

            }
            else
            {
                 return PaginacionList<Envio>.CreateAsyncc(envio.Include(s=>s.EstadosEnvio).Include(s=>s.Ruta).Include(a=>a.User).ThenInclude(a => a.Persona).Where(x=>x.EstadosEnvioId==2).Where(x=>x.User.Id==id), pageNumber ?? 1, pageSize);
               

            }

           
        }

        public PaginacionList<Envio> PaginacionMisPedidos(string sortOrder, string searchString, int? pageNumber, int pageSize,string id)
        {
            var envio = _context.Envios.AsQueryable();


            // para hacer la busqueda, el IsNullOrEmpty verifica que no este vacia la cadena entrante
            if(!string.IsNullOrEmpty(searchString))
            {
                envio = envio.Where(s => s.Id.ToString().Contains(searchString));
            }

            // esto que hice no es buena practica, lo ideal seria hacer un patron por estrategia
            // visitar : https://refactoring.guru/es/design-patterns/strategy
            // asi como lo tengo funciona pero no es tan tan tan recomendado
            // quizas mas adelante lo mejoremos con un patron de estrategia.

            switch(sortOrder)
            {
        
                default:
                envio = envio.OrderByDescending(s => s.FechaEnvio);
                break;
            }

            return PaginacionList<Envio>.CreateAsyncc(envio.Include(s=>s.EstadosEnvio).Include(s=>s.Ruta).Include(a=>a.User).ThenInclude(a => a.Persona).Where(x=>x.EstadosEnvioId==1).Where(x=>x.User.Id==id), pageNumber ?? 1, pageSize);
        }

         public PaginacionList<Envio> PaginacionMisPedidosEntregados(string sortOrder, string searchString, int? pageNumber, int pageSize,string id)
        {
            var envio = _context.Envios.AsQueryable();


            // para hacer la busqueda, el IsNullOrEmpty verifica que no este vacia la cadena entrante
            if(!string.IsNullOrEmpty(searchString))
            {
                envio = envio.Where(s => s.Id.ToString().Contains(searchString));
            }

            // esto que hice no es buena practica, lo ideal seria hacer un patron por estrategia
            // visitar : https://refactoring.guru/es/design-patterns/strategy
            // asi como lo tengo funciona pero no es tan tan tan recomendado
            // quizas mas adelante lo mejoremos con un patron de estrategia.

            switch(sortOrder)
            {
        
                default:
                envio = envio.OrderByDescending(s => s.FechaEnvio);
                break;
            }

            if(id == ""){
                return PaginacionList<Envio>.CreateAsyncc(envio.Include(s=>s.EstadosEnvio).Include(s=>s.Ruta).Include(a=>a.User).ThenInclude(a => a.Persona).Where(x=>x.EstadosEnvioId==1002), pageNumber ?? 1, pageSize);

            }
            else
            {
                return PaginacionList<Envio>.CreateAsyncc(envio.Include(s=>s.EstadosEnvio).Include(s=>s.Ruta).Include(a=>a.User).ThenInclude(a => a.Persona).Where(x=>x.EstadosEnvioId==1002).Where(x=>x.User.Id==id), pageNumber ?? 1, pageSize);

            }

            
        }

        // private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(_context.Users);

        public void saveChanges()
        {

            _context.SaveChanges();
        }

        public Envio Get(int id, bool includeRelatedEntities = true)
        {
            var envio = _context.Envios.AsQueryable();

            if (includeRelatedEntities)
            {
                envio = envio
                .Include(a => a.DetalleEnvios).Include(a=> a.Ruta).Include(a => a.DetalleEnvios).ThenInclude(a=>a.Producto);
            }

            return envio.Where(a => a.Id == id).SingleOrDefault();
        }

        public void Delete(int id, bool savechanges = true)
        {
            var set = _context.Set<DetalleEnvio>();
            var entity = set.Find(id);
            set.Remove(entity);

            if (true)
            {
                _context.SaveChanges();

            }

        }

        public void Update(Envio model, bool saveChanges = true)
        {

            _context.Entry(model).State = EntityState.Modified;

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }


        



    }

}



