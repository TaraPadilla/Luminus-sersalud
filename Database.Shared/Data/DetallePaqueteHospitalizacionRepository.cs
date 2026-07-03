using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Shared.Data
{
    public class DetallePaqueteHospitalizacionRepository : IDetallePaqueteHospitalizacion
    {
        private readonly Context _context = null;

        public DetallePaqueteHospitalizacionRepository(Context context)
        {
            _context = context;
        }

        public DetallePaqueteHospitalizacion GetById(int Id)
        {
            return _context.DetallePaqueteHospitalizacion.Where(x => x.Id == Id).FirstOrDefault();
        }

        // public List<DetallePaqueteHospitalizacion> GetByIdPaqueteHospitalizacion(int id)
        // {
        //     var data = _context.DetallePaqueteHospitalizacion.Where(x => x.PaqueteHospitalizacionId == id)
        //         .Include(x => x.PaqueteHospitalizacion)
        //             .ThenInclude(c => c.DetallePaquetesHospitalizacion)
        //                 .ThenInclude(a => a.Laboratorio)
        //         .Include(x => x.PaqueteHospitalizacion)
        //             .ThenInclude(c => c.DetallePaquetesHospitalizacion)
        //                 .ThenInclude(a => a.Producto)
        //         .Include(x => x.PaqueteHospitalizacion)
        //             .ThenInclude(c => c.DetallePaquetesHospitalizacion)
        //                 .ThenInclude(a => a.Servicio).ToList()
        //         ;
        //     return data;
        // }

        public List<DetallePaqueteHospitalizacion> GetByIdPaqueteHospitalizacion(int id)
        {
            var data = _context.DetallePaqueteHospitalizacion
                .Where(x => x.PaqueteHospitalizacionId == id)
                .Include(x => x.PaqueteHospitalizacion)
                    .ThenInclude(c => c.DetallePaquetesHospitalizacion)
                        .ThenInclude(a => a.Laboratorio)
                .Include(x => x.PaqueteHospitalizacion)
                    .ThenInclude(c => c.DetallePaquetesHospitalizacion)
                        .ThenInclude(a => a.Producto)
                .Include(x => x.PaqueteHospitalizacion)
                    .ThenInclude(c => c.DetallePaquetesHospitalizacion)
                        .ThenInclude(a => a.Servicio)
                .Include(x => x.LaboratorioPrecio)
                .Include(x => x.ProductoInventarioPrecio)
                .ToList();

            return data;
        }

        public void Update(DetallePaqueteHospitalizacion entity)
        {
            _context.DetallePaqueteHospitalizacion.Update(entity);
            _context.SaveChanges();
        }
    }
}
