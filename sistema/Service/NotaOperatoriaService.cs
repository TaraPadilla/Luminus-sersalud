using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Models;
using farmamest.Service.IService;
using System.Collections.Generic;

namespace farmamest.Service
{
    public class NotaOperatoriaService : INotaOperatoriaService
    {
        private readonly INotaOperatoria _notaOperatoriaRepository;

        public NotaOperatoriaService(INotaOperatoria notaOperatoriaRepository)
        {
            _notaOperatoriaRepository = notaOperatoriaRepository;
        }

        public void Add(NotaOperatoria entity)
        {
            _notaOperatoriaRepository.AddNotaOperatoria(entity);
        }

        public void GuardarFirma(int notaId, string firmaRuta)
        {
            _notaOperatoriaRepository.GuardarFirmaNotaOperatoria(notaId, firmaRuta);
        }

        public void ActualizarNotaOperatoria(NotaOperatoria entity)
        {
            _notaOperatoriaRepository.ActualizarNotaOperatoria(entity);
        }

        // Metodos de compatibilidad (delegan a los anteriores)
        public void AddNotaOperatoria(NotaOperatoria entity) => Add(entity);
        public List<NotaOperatoria> GetNotaOperatoriaListByHospitalizacionId(int id)
            => _notaOperatoriaRepository.GetNotaOperatoriaListByHospitalizacionId(id);
        public void GuardarFirmaNotaOperatoria(int notaId, string firmaRuta)
            => GuardarFirma(notaId, firmaRuta);

        public List<NotaOperatoriaVM> GetByHospitalizacionId(int hospitalizacionId)
        {
            var notas = _notaOperatoriaRepository.GetNotaOperatoriaListByHospitalizacionId(hospitalizacionId);
            var resultado = new List<NotaOperatoriaVM>();

            foreach (var nota in notas)
            {
                resultado.Add(new NotaOperatoriaVM
                {
                    Id = nota.Id,
                    Diagnostico = nota.Diagnostico,
                    FechaRegistro = nota.FechaRegistro.ToString("dd/MM/yyyy hh:mm tt"),
                    HospitalizacionId = nota.HospitalizacionId,
                    UserId = nota.UserId,
                    Profesional = (nota.User != null && nota.User.Persona != null)
                ? $"{nota.User.Persona.Nombre} {nota.User.Persona.Apellido}"
                : (nota.UserId ?? "Usuario no identificado"),

                    // Datos de la operación
                    FechaOperacion = nota.FechaOperacion.HasValue
                                                    ? nota.FechaOperacion.Value.ToString("dd/MM/yyyy")
                                                    : null,
                    HoraComenzo = nota.HoraComenzo,
                    HoraTermino = nota.HoraTermino,

                    // Personal de quirófano
                    Cirujano = nota.Cirujano,
                    PrimerAyudante = nota.PrimerAyudante,
                    SegundoAyudante = nota.SegundoAyudante,
                    Anestesista = nota.Anestesista,
                    Instrumentista = nota.Instrumentista,
                    Circulante = nota.Circulante,

                    // Diagnósticos y hallazgos
                    DiagnosticoPreOperatorio = nota.DiagnosticoPreOperatorio,
                    DiagnosticoPostOperatorio = nota.DiagnosticoPostOperatorio,
                    OperacionEfectuada = nota.OperacionEfectuada,
                    HallazgosTransOperatorios = nota.HallazgosTransOperatorios,

                    // Firma
                    FirmaRuta = nota.FirmaRuta,
                    FechaFirma = nota.FechaFirma.HasValue
                                     ? nota.FechaFirma.Value.ToString("dd/MM/yyyy hh:mm tt")
                                     : null,
                });
            }

            return resultado;
        }
    }
}