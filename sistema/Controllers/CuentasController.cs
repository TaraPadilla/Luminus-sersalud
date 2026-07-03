using Database.Shared.Data;
using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using sistema.Models;
using System;
using System.Linq;
using System.Text.Json;

namespace farmamest.Controllers
{
    public class CuentasController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ICuentas _cuentasRepository;
        private readonly IBanco _bancoRepository;
        private readonly ITipoCuenta _tipoCuentaRepository;

        public CuentasController(UserManager<User> userManager, ICuentas cuentasRepository, IBanco bancoRepository, ITipoCuenta tipoCuentaRepository)
        {
            _userManager = userManager;
            _cuentasRepository = cuentasRepository;
            _bancoRepository = bancoRepository;
            _tipoCuentaRepository = tipoCuentaRepository;
        }

        public IActionResult Nuevo()
        {
            //Lista desplegable tipo cuenta
            
            var listTipoCuenta = _tipoCuentaRepository.GetAll();

            ViewBag.TipoCuenta = new SelectList(listTipoCuenta, "Id", "Nombre");

            //Lista desplegable cuentas

            var listBanco = _bancoRepository.GetAll();

            ViewBag.Banco = new SelectList(listBanco, "Id", "Nombre");


            var model = new CuentasViewModel();
            return View(model);
        }
        [HttpPost]
        public string Nuevo(CuentasViewModel model)
        {
            try
            {
                var cuentas = new Cuentas
                {
                    NombreCuenta = model.Nombre,
                    BancoId = model.BancoId,
                    Eliminado = false,
                    NumeroCuenta = model.NumeroCuenta,
                    TipoCuentaId = model.TipoCuentaId,

                };

                _cuentasRepository.Add(cuentas);
                TempData["Message"] = "La cuenta se ha creado con éxito!";

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
                    Mensaje = "Error al registrar la cuenta. " + ex.Message
                });
            }
        }
        public IActionResult Lista()
        {
            var data = _cuentasRepository.GetAll();

            return View(data);
        }
        public IActionResult Modificar(int cuentaId)
        {
            //Lista desplegable tipo cuenta

            var listTipoCuenta = _tipoCuentaRepository.GetAll();

            ViewBag.TipoCuenta = new SelectList(listTipoCuenta, "Id", "Nombre");

            //Lista desplegable cuentas

            var listBanco = _bancoRepository.GetAll();

            ViewBag.Banco = new SelectList(listBanco, "Id", "Nombre");

            var cuenta = _cuentasRepository.GetById(cuentaId);
            var model = new CuentasViewModel
            {
                CuentaId = cuenta.Id,
                Nombre = cuenta.NombreCuenta,
                NumeroCuenta = cuenta.NumeroCuenta,
                BancoId = cuenta.BancoId,
                TipoCuentaId = cuenta.TipoCuentaId
            };
            return View(model);
        }
        [HttpPost]
        public string ModificarCuenta(CuentasViewModel model)
        {
            try
            {
                var cuentas = _cuentasRepository.GetById(model.CuentaId);
                cuentas.NombreCuenta = model.Nombre;
                cuentas.NumeroCuenta = model.NumeroCuenta;
                cuentas.BancoId = model.BancoId;
                cuentas.TipoCuentaId = model.TipoCuentaId;

                _cuentasRepository.Update(cuentas);
                TempData["Message"] = "La cuenta se ha modificado con éxito";

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
                    Mensaje = "Error al modificar la cuenta. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string EliminarCuenta(int? cuentaId)
        {
            try
            {
                _cuentasRepository.Delete((int)cuentaId, HttpContext.User);
                TempData["Message"] = "La cuenta se ha eliminado con éxito";

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
                    Mensaje = "Error al eliminar la cuenta. " + ex.Message
                });
            }
        }
    }
}
