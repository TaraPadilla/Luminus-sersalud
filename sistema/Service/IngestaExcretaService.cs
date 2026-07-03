using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Models;
using farmamest.Service.IService;
using System.Collections.Generic;
using System.Linq;

namespace farmamest.Service
{
    public class IngestaExcretaService : IIngestaExcretaService
    {
        private readonly IIngestaExcreta _ingestaExcretaRepository;

        public IngestaExcretaService(IIngestaExcreta ingestaExcretaRepository)
        {
            _ingestaExcretaRepository = ingestaExcretaRepository;
        }

        public void Add(IngestaExcreta2 entity)
        {
            _ingestaExcretaRepository.Add(entity);
        }

        public IngestaExcreta2 GetById(int id)
        {
            return _ingestaExcretaRepository.Get(id);
        }

        public void Update(IngestaExcreta2 entity)
        {
            _ingestaExcretaRepository.Update(entity);
        }

        public List<IngestaExcretaViewModel> GetListByHospitalizacionId(int hospitalicacionId)
        {
            var data = _ingestaExcretaRepository.GetListByHospitalizacionId(hospitalicacionId);

            var result = data.Select(nota => new IngestaExcretaViewModel
            {
                Id = nota.Id,
                CuantasHoras = nota.CuantasHoras,
                Enfermeria = nota.User?.Persona?.NombreYApellidos ?? "-",
                Excreta = nota.Excreta,
                Orina = nota.Orina,
                Heces = nota.Heces,
                Vomito = nota.Vomito,
                Sudoracion = nota.Sudoracion,
                Drenajes = nota.Drenajes,
                OtrosLiquidos = nota.OtrosLiquidos,
                FechaRegistro = nota.FechaRegistro.ToString("yyyy-MM-dd HH:mm:ss"),
                HospitalizacionId = hospitalicacionId,
                IngestaIV = nota.IngestaIV,
                IngestaIV2 = nota.IngestaIV2,
                IngestaIV3 = nota.IngestaIV3,
                IngestaIV4 = nota.IngestaIV4,
                IngestaIV5 = nota.IngestaIV5,
                IngestaIV6 = nota.IngestaIV6,
                IngestaPO = nota.IngestaPO,
                TotalIngesta = nota.TotalIngesta,
                UserId = nota.UserId,
                
                Autorizado = nota.Autorizado,
                UsuarioAutoriza = nota.UsuarioAutoriza,
                FechaAutorizacion = nota.FechaAutorizacion?.ToString("yyyy-MM-dd HH:mm:ss")
            }).ToList();

            return result;
        }
    }
}