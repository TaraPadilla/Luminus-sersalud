using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using sistema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Wkhtmltopdf.NetCore;

namespace sisrest.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class SucursalesController : Controller
    {
        private readonly ISucursal _sucursalRepository = null;
        private readonly IAmbiente _ambientesRepository = null;

        public SucursalesController(
            ISucursal sucursalRepository,
            IAmbiente ambientesRepository
            )
        {
            _sucursalRepository = sucursalRepository;
            _ambientesRepository = ambientesRepository;
        }

        public IActionResult Index()
        {
            var model = _sucursalRepository.GetList();

            return View(model);
        }

        public IActionResult Nueva()
        {
            return View();
        }

        [HttpPost]
        public string Nueva(SucursalViewModel model)
        {
            try
            {
                var sucursal = new Sucursal
                {
                    NombreSucursal = model.Nombre,
                    Direccion = model.Direccion,
                    Horario = model.Horario,
                    Eliminado = false
                };

                //Crear bodegas nuevas
                var ambientes = _ambientesRepository.GetList();
                if (ambientes != null)
                {
                    foreach (var ambiente in ambientes)
                    {
                        //Por cada uno de los ambientes existentes se crea una bodega
                        //Ambeintes son Hospital, Clinica, Farmacia, etc.
                        sucursal.Bodegas.Add(new Bodega
                        {
                            AmbienteId = ambiente.Id,
                            NombreBodega = ambiente.NombreAmbiente + " Sucursal " + sucursal.NombreSucursal
                        });
                    }
                }

                _sucursalRepository.Add(sucursal);


                TempData["Message"] = "Sucursal registrada";

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
                    Mensaje = "Error de sevridor al registrar sucursal. " + ex.Message
                });
            }
        }
        public IActionResult Modificar(int id)
        {
            var sucursal = _sucursalRepository.Get(id);
            if (sucursal == null)
            {
                TempData["Message"] = "Registro no existente";
                return RedirectToAction("Index");
            }

            var model = new SucursalViewModel
            {
                SucursalId = sucursal.Id,
                Nombre = sucursal.NombreSucursal,
                Direccion = sucursal.Direccion,
                Horario = sucursal.Horario
            };

            return View(model);
        }

        [HttpPost]
        public string Modificar(SucursalViewModel model)
        {
            try
            {
                var sucursal = _sucursalRepository.Get((int)model.SucursalId);

                sucursal.NombreSucursal = model.Nombre;
                sucursal.Direccion = model.Direccion;
                sucursal.Horario = model.Horario;
                _sucursalRepository.Update(sucursal);
                TempData["Message"] = "Sucursal modificada";

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
                    Mensaje = "Error de sevridor al modificar sucursal. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string Eliminar(int sucursalId)
        {
            try
            {
                _sucursalRepository.Delete(sucursalId);

                TempData["Message"] = "Sucursal eliminada";

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
                    Mensaje = "Error de servidor al eliminar sucursal. " + ex.Message
                });
            }
        }
    }

}