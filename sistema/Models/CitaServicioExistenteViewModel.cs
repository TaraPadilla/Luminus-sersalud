using Database.Shared.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace sistema.Models
{
    public class CitaServicioExistenteViewModel
    {
        public int ServicioId { get; set; }
        public string ServicioNombre { get; set; }
        public string ServicioCodigo { get; set; }
        public int ServicioDuracionHoras { get; set; }
        public int ServicioDuracionMinutos { get; set; }

		public string ServicioNombreMostrar
		{
			get
			{
				return $"{ServicioCodigo} - {ServicioNombre}";
			}
		}
		public string ServicioDuracionText
        {
            get
            {
                return $"{ServicioDuracionHoras}h{ServicioDuracionMinutos}m";
            }
        }
    }
}
