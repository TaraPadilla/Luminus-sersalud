using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.DataBindings
{
    public class DtoFilesUploadFile
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; }
        public string UrlArchivo { get; set; }
        public string NombreArchivo { get; set; }
    }
}
