using Database.Shared.Data;
using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace sistema.Models
{
    public class HospitalizacionInformeGeneralPDFVM:Hospitalizacion
    {
        public string EstablecimientoImagenLogo { get; set; }
        public string EstablecimientoImagenFirma { get; set; }
        public string EstablecimientoDireccion { get; set; }
        public string EstablecimientoTelefono { get; set; }
        public string EstablecimientoCorreoElectronico { get; set; }
    }
}
