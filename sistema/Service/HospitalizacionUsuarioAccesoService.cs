using Database.Shared.IRepository;
using Database.Shared.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using farmamest.Models;
using farmamest.Service.IService;
using System.Collections.Generic;
using System.Linq;

namespace farmamest.Service
{
    public class HospitalizacionUsuarioAccesoService : IHospitalizacionUsuarioAccesoService
    {
        private readonly IHospitalizacionUsuarioAcceso _hospitalizacionUsuarioAccesoRepository;
        private readonly IUser _userRepository;

        public HospitalizacionUsuarioAccesoService(IHospitalizacionUsuarioAcceso hospitalizacionUsuarioAccesoRepository, IUser userRepository)
        {
            _hospitalizacionUsuarioAccesoRepository = hospitalizacionUsuarioAccesoRepository;
            _userRepository = userRepository;
        }

        public void Add(HospitalizacionUsuarioAcceso entity)
        {
            entity.Eliminado = false;
            _hospitalizacionUsuarioAccesoRepository.Add(entity);
        }
        public void UpdateUsuarioAcceso(HospitalizacionUsuarioAccesoViewModels model)
        {
            var usuarioAcceso = _hospitalizacionUsuarioAccesoRepository.GetById(model.Id);
            if (usuarioAcceso != null)
            {
                usuarioAcceso.AutorizacionTabEnfermeria = model.AutTabEnfermeria;
                usuarioAcceso.AutorizacionTabActualizarEstadia = model.AutTabActualizarEstadia;
                usuarioAcceso.AutorizacionTabNotaMedica = model.AutTabNotaMedica;
                usuarioAcceso.AutorizacionTabNotaEnfermeria = model.AutTabNotaEnfermeria;
                usuarioAcceso.AutorizacionTabNotaEvolucion = model.AutTabNotaEvolucion;
                usuarioAcceso.AutorizacionTabSignosVitales = model.AutTabSignosVitales;
                usuarioAcceso.AutorizacionTabDietas = model.AutTabDietas;
                usuarioAcceso.AutorizacionTabPagos = model.AutTabPagos;
                usuarioAcceso.AutorizacionTabIncretaExcreta = model.AutTabIncretaExcreta;
                usuarioAcceso.AutorizacionTabControlGlucometria = model.AutTabControlGlucometria;

                _hospitalizacionUsuarioAccesoRepository.Update(usuarioAcceso);
            }
        }

        public void Delete(int id)
        {
            var entity = _hospitalizacionUsuarioAccesoRepository.GetById(id);
            entity.Eliminado = true;
            _hospitalizacionUsuarioAccesoRepository.Delete(entity);
        }

        public List<HospitalizacionUsuarioAccesoViewModels> GetAllByHospitalizacionId(int hospitalizacionId)
        {
            var data = _hospitalizacionUsuarioAccesoRepository.GetHospitalizacionUsuarioAccesosByIdHospitalizacion(hospitalizacionId);

            return data.Select(x => new HospitalizacionUsuarioAccesoViewModels
            {
                Id = x.Id,
                UserId = x.UserId,
                HospitalizacionId = x.HospitalizacionId,
                UserNombre = x.User.Persona.NombreYApellidos,
                UserEmail = x.User.UserName,
                AutTabEnfermeria = x.AutorizacionTabEnfermeria,
                AutTabControlGlucometria = x.AutorizacionTabControlGlucometria,
                AutTabNotaEnfermeria = x.AutorizacionTabNotaEnfermeria,
                AutTabNotaEvolucion = x.AutorizacionTabNotaEvolucion,
                AutTabIncretaExcreta = x.AutorizacionTabIncretaExcreta,
                AutTabPagos = x.AutorizacionTabPagos,
                AutTabSignosVitales = x.AutorizacionTabSignosVitales,
                AutTabNotaMedica = x.AutorizacionTabNotaMedica,
                AutTabDietas = x.AutorizacionTabDietas,
                AutTabActualizarEstadia = x.AutorizacionTabActualizarEstadia
            }).ToList();
        }

        //public bool ValidarVisualizacionHospitalizacionUsuarioAcceso(int hospitalizacionId, string usuarioId)
        //{

        //    var validate = _userRepository.ValidateRolAdmin(usuarioId);
        //    if (validate)
        //    {
        //        return true;
        //    }
        //    var data = GetAllByHospitalizacionId(hospitalizacionId);

        //    return data.Any(ua => ua.UserId == usuarioId);
        //}
    }
}
