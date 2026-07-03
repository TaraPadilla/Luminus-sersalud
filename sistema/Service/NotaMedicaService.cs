using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Models;
using farmamest.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace farmamest.Service
{
    public class NotaMedica2Service : INotaMedica2Service
    {
        private readonly INotaMedica2 _notaMedicaRepository;

        public NotaMedica2Service(INotaMedica2 notaMedicaRepository)
        {
            _notaMedicaRepository = notaMedicaRepository;
        }

        public void Add(NotaMedica2 entity)
        {
            _notaMedicaRepository.Add(entity);
        }

        public List<NotaMedica2ViewModel> GetListByIdHospitalizacion(int idHospitalizacion)
        {
            var data = _notaMedicaRepository
                .GetAllByIdHospitalizacion(idHospitalizacion)
                .OrderBy(x => x.FechaRegistro);

            var result = data.Select(x =>
            {

                return new NotaMedica2ViewModel
                {
                    Id = x.Id,
                    Diagnostico = x.Diagnostico,
                    FechaRegistro = x.FechaRegistro.ToString("yyyy-MM-dd HH:mm:ss"),
                    HistoriaProblema = x.HistoriaProblema,
                    Sintomas = x.Sintomas,
                    Profesional = (x.Profesional != null && x.Profesional.Persona != null)
                                  ? x.Profesional.Persona.NombreYApellidos
                                  : "-",
                    Autorizado = x.Autorizado,
                    UsuarioAutoriza = x.UsuarioAutoriza,
                    FechaAutorizacion = x.FechaAutorizacion?.ToString("yyyy-MM-dd HH:mm:ss")
                };
            }).ToList();

            return result;
        }


        public NotaMedica2 GetById(int id)
        {
            return _notaMedicaRepository.GetById(id);
        }

        public void Update(NotaMedica2 entity)
        {
            _notaMedicaRepository.Update(entity);
        }

    }
}