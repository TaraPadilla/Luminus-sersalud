using Database.Shared.IRepository;
using Database.Shared.Models;
using System.Collections.Generic;
using System.Linq;

namespace Database.Shared.Data
{
    public class HospitalizacionProductoObservacionRepository : IHospitalizacionProductoObservacion
    {
        private readonly Context _context;

        public HospitalizacionProductoObservacionRepository(Context context)
        {
            _context = context;
        }

        // Añadir una nueva observación
        public HospitalizacionProductoObservacion Add(HospitalizacionProductoObservacion observacion)
        {
            _context.HospitalizacionesProductosObservaciones.Add(observacion);
            _context.SaveChanges();
            return observacion;
        }

        // Modificar una observación existente
        public void Update(HospitalizacionProductoObservacion observacion)
        {
            var existingObservacion = _context.HospitalizacionesProductosObservaciones.Find(observacion.Id);
            if (existingObservacion != null)
            {
                existingObservacion.Observacion = observacion.Observacion;
                existingObservacion.FechaCreacion = observacion.FechaCreacion;
                existingObservacion.UsuarioCreaId = observacion.UsuarioCreaId;
                _context.SaveChanges();
            }
        }

        // Eliminar (lógicamente) una observación
        public void Delete(int id)
        {
            var observacion = _context.HospitalizacionesProductosObservaciones.Find(id);
            if (observacion != null)
            {
                observacion.Eliminado = true;
                _context.SaveChanges();
            }
        }

        // Obtener observaciones por el ID de HospitalizacionProductoAplicacion
        public List<HospitalizacionProductoObservacion> GetByHospitalizacionProductoAplicacionId(int hospitalizacionProductoAplicacionId)
        {
            return _context.HospitalizacionesProductosObservaciones
                .Where(o => o.HospitalizacionProductoAplicacionId == hospitalizacionProductoAplicacionId && !o.Eliminado)
                .ToList();
        }
    }
}
