using System.Collections.Generic;

namespace farmamest.Models
{
    public class DocumentoEmbebidoVm
    {
        public string Nombre { get; set; }
        public string UrlOriginal { get; set; }
        public bool ArchivoExiste { get; set; }
        public List<string> PaginasBase64 { get; set; } = new();
    }
}
