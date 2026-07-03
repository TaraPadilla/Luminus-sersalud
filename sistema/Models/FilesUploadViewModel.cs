using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace sistema.Models
{
    public class FilesUploadViewModel
    {
        public List<IFormFile> Archivos { get; set; }
        public string Directorio { get; set; }
    }
}
