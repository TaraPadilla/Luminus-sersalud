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
    public class NotaEnfermeria2Service : INotaEnfermeria2Service
    {
        private readonly INotaEnfermeria2 _notaEnfermeria;

        public NotaEnfermeria2Service(INotaEnfermeria2 notaEnfermeria)
        {
            _notaEnfermeria = notaEnfermeria;
        }

        public void AddNotaEnfermeria(NotaEnfermeria2 entity)
        {
            _notaEnfermeria.AddNotaEnfermeria(entity);
        }

        public List<NotaEnfermeria2VM> GetNotaEnfermeriaListByHospitalizacionId(int hospitalicacionId)
        {
            var data = _notaEnfermeria.GetNotaEnfermeriaListByHospitalizacionId(hospitalicacionId);

            var result = data
                .Select(nota => new NotaEnfermeria2VM
                {
                    Id = nota.Id,
                    Evolucion = nota.Evolucion,
                    Sintomas = nota.Sintomas,
                    Diagnostico = nota.Diagnostico,
                    FechaRegistro = nota.FechaRegistro.ToString("yyyy-MM-dd HH:mm:ss"), // Formato de fecha con hora
                    HospitalizacionId = nota.HospitalizacionId,
                    UserId = nota.UserId,
                    Profesional = nota.User?.Persona != null ? $"{nota.User.Persona.NombreYApellidos}" : "-",
                    TurnoEnfermeriaId = nota.TurnoEnfermeriaId
                }).ToList();

            return result;
        }
    }
}
