using Database.Shared.Data;
using Microsoft.AspNetCore.Mvc;
using sistema.Models;
using System.Collections.Generic;
using System;
using Database.Shared.IRepository;
using System.Text.Json;

namespace farmamest.Controllers
{
    public class LaboratorioProductoController : Controller
    {
        private readonly ILaboratorioProducto _laboratorioProducto;

        public LaboratorioProductoController(ILaboratorioProducto laboratorioProducto)
        {
            _laboratorioProducto = laboratorioProducto;
        }

        [HttpPost]
        public string ConsultarLaboratorioProductoExistentes()
        {
            try
            {
                var laboratorioExsitente = new List<LaboratorioProductoViewModel>();
                var laboratorioBd = _laboratorioProducto.GetAll();
                if (laboratorioBd != null)
                {
                    foreach (var lab in laboratorioBd)
                    {
                        laboratorioExsitente.Add(new LaboratorioProductoViewModel
                        {
                            LaboratorioProducto = lab,
                            NombreLaboratorio = lab.NombreLaboratorioProducto
                        });
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = laboratorioExsitente
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar laboratorio producto: " + ex.Message
                });
            }
        }
    }
}
