using Database.Shared.Data;
using Database.Shared.IRepository;
using Database.Shared.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Presentation;
using farmamest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using sistema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace farmamest.Controllers
{
    public class CuentaContableController : Controller
    {
        private readonly ICuentaContable _cuentaContableRepository;
        private readonly UserManager<User> _userManager;
        private readonly IBanco _bancoRepository;
        private readonly INomenclatura _nomenclaturaRepository;
        private readonly ICuentas _cuentasRepository;
        private readonly ICategoriaCuentaContable _categoriaCuentaContableRepository;

        public CuentaContableController(ICuentaContable cuentaContableRepository, UserManager<User> userManager, 
            IBanco bancoRepository, INomenclatura nomenclaturaRepository, ICuentas cuentasRepository, 
            ICategoriaCuentaContable categoriaCuentaContableRepository)
        {
            _cuentaContableRepository = cuentaContableRepository;
            _userManager = userManager;
            _bancoRepository = bancoRepository;
            _nomenclaturaRepository = nomenclaturaRepository;
            _cuentasRepository = cuentasRepository;
            _categoriaCuentaContableRepository = categoriaCuentaContableRepository;
        }

        public IActionResult Nuevo()
        {
            //Lista desplegable categoria

            var listcategoria = _categoriaCuentaContableRepository.GetAll();

            ViewBag.Categoria = new SelectList(listcategoria, "Id", "Nombre");
            //Lista desplegable nomenclatura

            var listNomenclatura = _nomenclaturaRepository.GetAll();

            ViewBag.Nomenclatura = new SelectList(listNomenclatura, "Id", "Nombre");

            var model = new CuentaContableViewModel();
            return View(model);
        }
        [HttpPost]
        public string Nuevo(CuentaContableViewModel model)
        {
            try
            {
                var nomeclaturas = new List<CuentaContableNomenclatura>();
                if (model.NomenclaturaId == 0)
                {
                    nomeclaturas.Add(new CuentaContableNomenclatura
                    {
                        NomenclaturaId = 1,
                        CuentaContableId = model.Id,   
                    });
                    nomeclaturas.Add(new CuentaContableNomenclatura
                    {
                        NomenclaturaId = 2,
                        CuentaContableId = model.Id,
                    });

                }
                else
                {
                    nomeclaturas.Add(new CuentaContableNomenclatura
                    {
                        NomenclaturaId = model.NomenclaturaId,
                        CuentaContableId = model.Id,
                    });
                }

                int? cuentaId = model.CuentaId;

                var cuenta = new CuentaContable
                {
                    Id = model.Id,
                    BancoId = model.BancoId,
                    CategoriaCuentaId = model.CategoriaCuentaId,
                    CuentaId = model.CuentaId == 0 ? null : cuentaId,
                    Eliminado = false,
                    Especificaciones = model.Especificaciones,
                    NombreCuenta = model.NombreCuenta,
                    Nomenclatura = nomeclaturas
                };

                _cuentaContableRepository.Add(cuenta);

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

        [HttpPost]
        public string ConsultarBancos()
        {
            try
            {
                var data = _bancoRepository.GetAll();
                List<BancoViewModel> bancos = new List<BancoViewModel>();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        bancos.Add(new BancoViewModel
                        {
                            Id = item.Id,
                            Nombre  = item.Nombre,
                            Direccion = item.Direccion,
                        });
                    }
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = bancos
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar los bancos. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarCuentasBanco(int Id)
        {
            try
            {
                var data = (_cuentasRepository.GetAll()).Where(x => x.BancoId == Id).ToList();
                List<CuentasViewModel> cuentas = new List<CuentasViewModel>();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        cuentas.Add(new CuentasViewModel
                        {
                            CuentaId = item.Id,
                            Nombre  = item.NombreCuenta,
                            NumeroCuenta = item.NumeroCuenta,
                            BancoId = item.BancoId,
                            TipoCuentaId = item.TipoCuentaId
                        });
                    }
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = cuentas
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar los bancos. " + ex.Message
                });
            }
        }

        public IActionResult Lista()
        {
            var data = _cuentaContableRepository.GetAll();
            return View(data);
        }

        public IActionResult Modificar(int CuentaId)
        {
            var data = _cuentaContableRepository.GetById(CuentaId);

            var model = new CuentaContableViewModel
            {
                Id = data.Id,
                BancoId = data.BancoId,
                CategoriaCuentaId = data.CategoriaCuentaId,
                CuentaId = data.Cuenta == null ? 0 : (int)data.CuentaId,
                Especificaciones = data.Especificaciones,
                NombreCuenta = data.NombreCuenta,
                NomenclaturaId = 0
            };
            //Lista desplegable categoria

            var listcategoria = _categoriaCuentaContableRepository.GetAll();

            ViewBag.Categoria = new SelectList(listcategoria, "Id", "Nombre");
            //Lista desplegable nomenclatura

            var listNomenclatura = _nomenclaturaRepository.GetAll();

            ViewBag.Nomenclatura = new SelectList(listNomenclatura, "Id", "Nombre");
            return View(model);
        }
        [HttpPost]
        public string ModificarCuenta(CuentaContableViewModel model)
        {
            try
            {
                var nomeclaturas = new List<CuentaContableNomenclatura>();

                if (model.NomenclaturaId == 0)
                {
                    nomeclaturas.Add(new CuentaContableNomenclatura
                    {
                        NomenclaturaId = 1,
                        CuentaContableId = model.Id,
                    });
                    nomeclaturas.Add(new CuentaContableNomenclatura
                    {
                        NomenclaturaId = 2,
                        CuentaContableId = model.Id,
                    });

                }
                else
                {
                    nomeclaturas.Add(new CuentaContableNomenclatura
                    {
                        NomenclaturaId = model.NomenclaturaId,
                        CuentaContableId = model.Id,
                    });
                }

                int? cuentaId = model.CuentaId;


                var data = _cuentaContableRepository.GetById(model.Id);
                data.NombreCuenta = model.NombreCuenta;
                data.CategoriaCuentaId = model.CategoriaCuentaId;
                data.Especificaciones = model.Especificaciones;
                data.BancoId = model.BancoId;
                data.CuentaId = cuentaId == 0 ? null : cuentaId;
                data.Nomenclatura = nomeclaturas;

                _cuentaContableRepository.Update(data);

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
                _cuentaContableRepository.Delete((int)cuentaId, HttpContext.User);
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
        [HttpPost]
        public string ConsultarCuentas()
        {
            try
            {
                var data = _cuentaContableRepository.GetAll();
                List<CuentaContableViewModel> cuentas = new List<CuentaContableViewModel>();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        int? cuentaId = item.Cuenta == null ? 0 : item.CuentaId;
                        cuentas.Add(new CuentaContableViewModel
                        {
                            Id = item.Id,
                            CuentaId = (int)cuentaId,
                            NombreCuenta = item.NombreCuenta,
                            Especificaciones = item.NombreCuenta,
                            BancoId = item.BancoId,
                            NomenclaturaId = 0,
                            CategoriaCuentaId = item.CategoriaCuentaId,

                        });
                    }
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = cuentas
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar las cuentas. " + ex.Message
                });
            }
        }
    }
}
