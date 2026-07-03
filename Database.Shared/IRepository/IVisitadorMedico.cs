using Database.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.IRepository
{
    public interface IVisitadorMedico
    {
        /// <summary>
        /// Agrega un nuevo visitador médico al sistema.
        /// </summary>
        /// <param name="entity">Entidad del visitador médico.</param>
        void AddVisitadorMedico(VisitadorMedico entity);

        /// <summary>
        /// Obtiene la lista de todos los visitadores médicos.
        /// </summary>
        /// <returns>Lista de visitadores médicos.</returns>
        List<VisitadorMedico> GetAllVisitadorMedico();

        /// <summary>
        /// Obtiene un visitador médico por su identificador único.
        /// </summary>
        /// <param name="id">ID del visitador médico.</param>
        /// <returns>Entidad del visitador médico.</returns>
        VisitadorMedico GetVisitadorMedicoById(int id);

        /// <summary>
        /// Actualiza los datos de un visitador médico.
        /// </summary>
        /// <param name="entity">Entidad actualizada del visitador médico.</param>
        void UpdateVisitadorMedico(VisitadorMedico entity);

        /// <summary>
        /// Elimina un visitador médico por su identificador único.
        /// </summary>
        /// <param name="id">ID del visitador médico a eliminar.</param>
        void DeleteVisitadorMedico(int id);
    }
}
