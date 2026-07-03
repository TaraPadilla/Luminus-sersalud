using Database.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System;
using Database.Shared.IRepository;
using farmamest.Service.IService;

namespace farmamest.Service
{
    public class VisitadorMedicoService : IVisitadorMedicoService
    {
        private readonly IVisitadorMedico _visitadorMedico;

        public VisitadorMedicoService(IVisitadorMedico visitadorMedico)
        {
            _visitadorMedico = visitadorMedico;
        }

        /// <summary>
        /// Agrega un nuevo visitador médico al sistema.
        /// </summary>
        /// <param name="entity">Entidad del visitador médico.</param>
        public void AddVisitadorMedico(VisitadorMedico entity)
        {
            _visitadorMedico.AddVisitadorMedico(entity);
        }

        /// <summary>
        /// Obtiene todos los visitadores médicos registrados.
        /// </summary>
        /// <returns>Lista de visitadores médicos.</returns>
        public List<VisitadorMedico> GetAllVisitadorMedico()
        {
            return _visitadorMedico.GetAllVisitadorMedico();
        }

        /// <summary>
        /// Obtiene un visitador médico por su identificador único.
        /// </summary>
        /// <param name="id">ID del visitador médico.</param>
        /// <returns>Entidad del visitador médico.</returns>
        public VisitadorMedico GetVisitadorMedicoById(int id)
        {
            return _visitadorMedico.GetVisitadorMedicoById(id);
        }

        /// <summary>
        /// Actualiza los datos de un visitador médico.
        /// </summary>
        /// <param name="entity">Entidad actualizada del visitador médico.</param>
        public void UpdateVisitadorMedico(VisitadorMedico entity)
        {
            _visitadorMedico.UpdateVisitadorMedico(entity);
        }

        /// <summary>
        /// Elimina un visitador médico por su identificador único.
        /// </summary>
        /// <param name="id">ID del visitador médico a eliminar.</param>
        public void DeleteVisitadorMedico(int id)
        {
            _visitadorMedico.DeleteVisitadorMedico(id);
        }
    }
}
