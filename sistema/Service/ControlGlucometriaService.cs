using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Models;
using farmamest.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace farmamest.Service
{
    public class ControlGlucometria2Service : IControlGlucometria2Service
    {
        private readonly IControlGlucometria2 _controlGlucometria2Repository;

        public ControlGlucometria2Service(IControlGlucometria2 controlGlucometria2Repository)
        {
            _controlGlucometria2Repository = controlGlucometria2Repository;
        }

        public void Add(ControlGlucometria2 entity)
        {
            _controlGlucometria2Repository.Add(entity);
        }

        public List<DetalleControlGlucometria2ViewModel> GetByHospitalizacionId(int hospitalizacionId)
        {
            var data = _controlGlucometria2Repository.GetDetalleControlGlucometria2ByHospitalizacionId(hospitalizacionId);

            return data.Select(x => new DetalleControlGlucometria2ViewModel
            {
                Id = x.Id,
                Dosis = 1,
                FechaHora = x.ControlGlucometria2.FechaHora.ToString("yyyy-MM-dd HH:mm:ss"),
                Firma = x.ControlGlucometria2.Firma == null ? "-" : x.ControlGlucometria2.Firma,
                GMT = x.ControlGlucometria2.GMT,
                HospitalizacionId = x.ControlGlucometria2.HospitalizacionId,
                Insulina = x.ControlGlucometria2.Insulina,
                Medicamento = x.ControlGlucometria2.Medicamento,
                Unidades = x.ControlGlucometria2.Unidades,
                NombrePersonaAplica = x.User?.Persona == null ? "-" : x.User.Persona.NombreYApellidos,
                NombreProfesional = x.Profesional?.Persona == null ? "-" : x.Profesional.Persona.NombreYApellidos,
                PersonaAplicaId = x.UserId == null ? "-" : x.UserId,
                ProfesionalId = x.ProfesionalId,
                FechaHoraAplicacion = x.FechaAplicacion?.ToString("yyyy-MM-dd HH:mm:ss"),
                Aplicado = x.Aplicado,

                // Nuevos campos
                Autorizado = x.ControlGlucometria2.Autorizado,
                UsuarioAutoriza = x.ControlGlucometria2.UsuarioAutoriza,
                FechaAutorizacion = x.ControlGlucometria2.FechaAutorizacion?.ToString("yyyy-MM-dd HH:mm:ss")
            }).ToList();
        }

        public void AplicacionDetalleControlGlucometria2ById(int id, string personaAplica)
        {
            var data = _controlGlucometria2Repository.GetDetalleControlGlucometria2ById(id);

            data.UserId = personaAplica;
            data.Aplicado = true;
            data.FechaAplicacion = DateTime.Now;

            _controlGlucometria2Repository.Update(data);
        }


        public ControlGlucometria2 GetById(int id)
        {
            return _controlGlucometria2Repository.GetById(id);
        }

        public void Update(ControlGlucometria2 entity)
        {
            _controlGlucometria2Repository.Update(entity);
        }

    }
}
