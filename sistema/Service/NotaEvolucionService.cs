using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Models;
using farmamest.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;

namespace farmamest.Service
{
    public class NotaEvolucionService : INotaEvolucionService
    {
        private readonly INotaEvolucion _notaEvolucion;

        public NotaEvolucionService(INotaEvolucion notaEvolucion)
        {
            _notaEvolucion = notaEvolucion;
        }

        public void AddNotaEvolucion(NotaEvolucion entity)
        {
            _notaEvolucion.AddNotaEvolucion(entity);
        }

        public List<NotaEvolucionVM> GetNotaEvolucionListByHospitalizacionId(int hospitalicacionId)
        {
            var data = _notaEvolucion.GetNotaEvolucionListByHospitalizacionId(hospitalicacionId);

            var result = data.Select(nota => new NotaEvolucionVM
            {
                Id = nota.Id,
                Evolucion = nota.Evolucion,
                Sintomas = nota.Sintomas,
                Diagnostico = nota.Diagnostico,
                FechaRegistro = nota.FechaRegistro.ToString("yyyy-MM-dd HH:mm:ss"), // Formato de fecha con hora
                HospitalizacionId = nota.HospitalizacionId,
                UserId = nota.UserId,
                Profesional = nota.User?.Persona != null ? $"{nota.User.Persona.NombreYApellidos}" : "-"
            }).ToList();

            return result;
        }
    }
}
