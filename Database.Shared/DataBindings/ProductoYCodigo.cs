using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace Database.Shared.DataBindings
{
    public class ProductoYCodigo 
    {
        public string ProductoYCodigoDeBarras {get;set;}
        public string CodigoReferencia {get;set;}

    }
}