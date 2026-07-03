using Database.Shared.Data;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Mvc;
using sistema.Models;
using System.Collections.Generic;
using System;
using farmamest.Models;
using System.Text.Json;

namespace farmamest.Controllers
{
    public class FormasPagoController : Controller
    {
        private readonly IFormasPago _formasPagoRepository;
        public FormasPagoController(IFormasPago formasPagoRepository)
        {
            _formasPagoRepository = formasPagoRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ConsultarFormasPago()
        {
            try
            {
                var listaFormaPago = new List<FormasPagoViewModel>();
                var data = _formasPagoRepository.GetAll();

                if (data != null)
                {
                    foreach (var formaPago in data)
                    {
                        listaFormaPago.Add(new FormasPagoViewModel
                        {
                            Id = formaPago.Id,
                            NombreFormaPago = formaPago.NombreFormaPago
                        });
                    }
                }

                // Retornamos la estructura que espera tu JavaScript
                return Json(new
                {
                    Exitoso = true,
                    Resultado = listaFormaPago
                });
            }
            catch (Exception ex)
            {
                // Retornamos el manejo de error estructurado
                return Json(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar formas de pago existentes: " + ex.Message
                });
            }
        }
    }
}
