using Database.Shared.Data;
using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Models;
using farmamest.Service.IService;
using sistema.Models;
using sistema.Service.IService;
using System.Collections.Generic;
using System.Linq;

namespace farmamest.Service
{
    public class ConsultasService : IConsultasService
    {
        private readonly IConsultas _consultasRepository;

        public ConsultasService(IConsultas consultasRepository)
        {
            _consultasRepository = consultasRepository;
        }
        public void DeleteArchivoConsulta(int archivoId)
        {
            _consultasRepository.DeleteArchivoConsulta(archivoId);
        }
        public List<ArchivoVM> GetArchivosConsulta(int consultaId)
        {
            var listaArchivos = new List<ArchivoVM>();
            var archivos = _consultasRepository.GetArchivos(consultaId);
            if (archivos != null)
            {
                foreach (var archivo in archivos)
                {
                    listaArchivos.Add(new ArchivoVM
                    {
                        Id = archivo.Id,
                        Fecha = archivo.FechaCarga.ToString("dd MMM yy"),
                        Nombre = archivo.NombreArchivo,
                        UrlArchivo = archivo.UrlArchivo
                    });
                }
            }
            return listaArchivos;
        }
        public List<ArchivoVM> GetArchivosConsultaNew(int pacienteId)
        {
            var listaArchivos = new List<ArchivoVM>();
            var archivos = _consultasRepository.GetArchivosNew(pacienteId);
            if (archivos != null)
            {
                foreach (var archivo in archivos)
                {
                    listaArchivos.Add(new ArchivoVM
                    {
                        Id = archivo.Id,
                        Fecha = archivo.FechaCarga.ToString("dd MMM yy"),
                        Nombre = archivo.NombreArchivo,
                        UrlArchivo = archivo.UrlArchivo
                    });
                }
            }
            return listaArchivos;
        }
    }
}
