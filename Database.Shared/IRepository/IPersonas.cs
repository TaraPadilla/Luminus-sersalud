using Database.Shared.Models;
using System.Collections.Generic;
using Database.Shared.Paginacion;

namespace Database.Shared.IRepository
{
    public interface IPersonas
    {
        List<Sexo> GetSexosList();
        Persona Add(Persona model);
        Persona Get(int pacienteId);
        void Update(Persona persona);
        List<Persona> GetPersonas();
        IList<TipificacionComunicacion> GetTipificacionesComunicacion();
        IList<TipoRedSocial> GetTiposRedSocial();
    }
}