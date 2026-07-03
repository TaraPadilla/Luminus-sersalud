using Database.Shared.Models;
using Database.Shared.Paginacion;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.AspNetCore.Identity;
using sistema.Models;
using System.Collections.Generic;

namespace sistema.Service.IService
{
    public interface IConsultasService
    {
        void DeleteArchivoConsulta(int archivoId);
        List<ArchivoVM> GetArchivosConsulta(int consultaId);
        List<ArchivoVM> GetArchivosConsultaNew(int pacienteId);

    }
}
