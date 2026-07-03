using sistema.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Win32;
using Microsoft.AspNetCore.Hosting;
using sistema.Service.IService;
using Database.Shared.Models;
using System.Text.Json;
using Database.Shared.IRepository;

namespace sistema.Controllers
{
    public class FilesController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IFilesService _filesService;
        private readonly IArchivos _archivosRepository;

        public FilesController(IWebHostEnvironment env, IFilesService filesService, IArchivos archivosRepository)
        {
            _env = env;
            _filesService = filesService;
            _archivosRepository = archivosRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SubirArchivo(string base64Archivo, string extension)
        {
            try
            {
                if (base64Archivo.Trim().Length != 0)
                {
                    //Dar nombre al archivo
                    var ticks = DateTime.Now.Ticks;
                    var fecha = DateTime.Now.ToString("yyyyMMdd");
                    var nombreArchivo = fecha + ticks + extension;

                    //Directorio de archivo
                    var directorio = _env.ContentRootPath.ToString() +
                        "/wwwroot/documentos/";
                    if (!Directory.Exists(directorio))
                    {
                        Directory.CreateDirectory(directorio);
                    }


                    var byteArchivo = Convert.FromBase64String(base64Archivo.Split(',')[1]);
                    using (var streamFile = new FileStream(directorio + nombreArchivo,
                        FileMode.OpenOrCreate))
                    {
                        streamFile.Write(byteArchivo, 0, byteArchivo.Length);
                        streamFile.Flush();
                    }

                    //Ruta de registro en BD
                    var rutaArchivo = "../documentos/" + nombreArchivo;
                    return Json(new { exitoso = true, url = rutaArchivo });
                }
                return Json(new
                {
                    exitoso = false,
                    mensaje = "No hay archivos para subir"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    exitoso = false,
                    mensaje = "Error al subir archivo. " + ex.Message
                });
            }
        }
        [HttpPost]
        public async Task<string> UploadFile(IFormFile file, int? consultaId = null)
        {
            try
            {
                var fecha = DateTime.Now;
                var resultado = await _filesService.UploadFile(file, "archivos_consultas");
                if (resultado.Exitoso)
                {
                    var archivo = new Archivo
                    {
                        ConsultaId = consultaId,
                        UrlArchivo = resultado.UrlArchivo,
                        FechaCarga = fecha,
                        NombreArchivo = resultado.NombreArchivo
                    };
                    _archivosRepository.Add(archivo);
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al subir archivo: " + ex.Message
                });
            }
        }
    }
}
