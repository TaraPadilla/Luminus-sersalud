using Database.Shared.Data;
using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Models;
using Microsoft.AspNetCore.Mvc;
using sistema.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace farmamest.Controllers
{
    public class BancoController : Controller
    {
        private readonly IBanco _bancoRepository;

        public BancoController(IBanco bancoRepository)
        {
            _bancoRepository = bancoRepository;
        }

        public IActionResult Nuevo()
        {
            var model = new BancoViewModel();
            return View(model);
        }
        [HttpPost]
        public string Nuevo(BancoViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model?.Nombre))
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "El nombre del banco es obligatorio."
                });
            }

            try
            {
                var banco = new Banco
                {
                    Nombre = model.Nombre,
                    Direccion = model.Direccion,
                    Proveedores = null

                };

                _bancoRepository.AddBanco(banco);
                TempData["Message"] = "El banco se ha creado con éxito!";

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
                    Mensaje = "Error al registrar el banco. " + ex.Message
                });
            }
        }
        public IActionResult Lista()
        {
            var bancos = (_bancoRepository.GetAll());
            return View(bancos);
        }

        public IActionResult Modificar(int bancoId)
        {
            var banco = _bancoRepository.GetBanco(bancoId);
            var model = new BancoViewModel
            {
                Id = banco.Id,
                Nombre = banco.Nombre,
                Direccion = banco.Direccion
            };
            return View(model);
        }
        [HttpPost]
        public string ModificarBanco(BancoViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model?.Nombre))
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "El nombre del banco es obligatorio."
                });
            }

            try
            {
                var banco = _bancoRepository.GetBanco(model.Id);
                banco.Nombre = model.Nombre;
                banco.Direccion = model.Direccion;

                _bancoRepository.UpdateBanco(banco);

                TempData["Message"] = "El banco se ha modificado con éxito";

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
                    Mensaje = "Error al modificar el banco. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string EliminarBanco(int? bancoId)
        {
            try
            {
                _bancoRepository.DeleteBanco((int)bancoId, HttpContext.User);

                TempData["Message"] = "El banco se ha eliminado con éxito";

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
                    Mensaje = "Error al eliminar el banco. " + ex.Message
                });
            }
        }
    }
}
