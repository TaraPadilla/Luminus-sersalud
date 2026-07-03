using Database.Shared.DataBindings;
using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Models;
using farmamest.Service.IService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using sistema.Service.IService;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace sistema.Service
{
    public class FilesService : IFilesService
    {
        private readonly IWebHostEnvironment _env;
        public FilesService(IWebHostEnvironment env)
        {
            _env = env;
        }
        public async Task<DtoFilesUploadFile> UploadFile(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                return new DtoFilesUploadFile
                {
                    Exitoso = false,
                    Mensaje = "No hay ningun archivo seleccionado"
                };

            var path = Path.Combine(_env.ContentRootPath.ToString(), "wwwroot", folder, file.FileName);

            using (var stream = new FileStream(path, FileMode.OpenOrCreate))
            {
                await file.CopyToAsync(stream);
            }

            return new DtoFilesUploadFile
            {
                Exitoso = true,
                Mensaje = "Archivo subido",
                UrlArchivo = $"/{folder}/" + file.FileName,
                NombreArchivo = file.FileName
            };
        }
    }
}