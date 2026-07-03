using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Database.Shared.Data
{
    public class NotaOperatoriaRepository : INotaOperatoria
    {
        private readonly Context _context;

        public NotaOperatoriaRepository(Context context)
        {
            _context = context;
        }

        public void AddNotaOperatoria(NotaOperatoria entity)
        {
            _context.NotaOperatoria.Add(entity);
            _context.SaveChanges();
        }

        public List<NotaOperatoria> GetNotaOperatoriaListByHospitalizacionId(int hospitalizacionId)
        {
            return _context.NotaOperatoria
                .Include(x => x.User)
                    .ThenInclude(x => x.Persona)
                .Where(x => x.HospitalizacionId == hospitalizacionId)
                .OrderByDescending(x => x.FechaRegistro)
                .ToList();
        }

        public void GuardarFirmaNotaOperatoria(int notaId, string firmaRuta)
        {
            var nota = _context.NotaOperatoria.Find(notaId);
            if (nota == null)
                throw new Exception($"No se encontró la nota operatoria con Id {notaId}.");

            nota.FirmaRuta = firmaRuta;
            nota.FechaFirma = DateTime.Now;
            _context.SaveChanges();
        }

        public void ActualizarNotaOperatoria(NotaOperatoria entity)
        {
            var nota = _context.NotaOperatoria.Find(entity.Id);
            if (nota == null)
                throw new Exception($"No se encontró la nota operatoria con Id {entity.Id}.");

            nota.FechaOperacion = entity.FechaOperacion;
            nota.HoraComenzo = entity.HoraComenzo;
            nota.HoraTermino = entity.HoraTermino;
            nota.Cirujano = entity.Cirujano;
            nota.PrimerAyudante = entity.PrimerAyudante;
            nota.SegundoAyudante = entity.SegundoAyudante;
            nota.Anestesista = entity.Anestesista;
            nota.Instrumentista = entity.Instrumentista;
            nota.Circulante = entity.Circulante;
            nota.DiagnosticoPreOperatorio = entity.DiagnosticoPreOperatorio;
            nota.DiagnosticoPostOperatorio = entity.DiagnosticoPostOperatorio;
            nota.OperacionEfectuada = entity.OperacionEfectuada;
            nota.HallazgosTransOperatorios = entity.HallazgosTransOperatorios;
            nota.Diagnostico = entity.Diagnostico;
            _context.SaveChanges();
        }
    }
}